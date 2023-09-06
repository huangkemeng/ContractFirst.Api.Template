using System.Text;
using Autofac;
using CollectionSystem.Api.Engines.Bases;
using CollectionSystem.Api.Infrastructure.EfCore;
using CollectionSystem.Api.Infrastructure.MongoDb;
using Mediator.Net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CollectionSystem.Api.IntegrationTests;

[Collection("Sequential")]
public class TestBase : IClassFixture<SequentialCollectionFixture>, IAsyncDisposable, IDisposable
{
    public TestBase()
    {
        Build();
    }

    protected IContainer TestServices { get; private set; }
    protected IMediator TestMediator { get; private set; }
    protected SqlDbContext TestDbContext { get; private set; }

    protected void Build(Action<ContainerBuilder>? builderAction = null)
    {
        var builder = new ContainerBuilder();
        TestServices = builder.TestBuildWithEngines(builderAction);
        TestMediator = TestServices.Resolve<IMediator>();
        TestDbContext = TestServices.Resolve<SqlDbContext>();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }

    private async Task<SqlDbContext?> CheckDbConnect()
    {
        var dbContext = TestServices.Resolve<SqlDbContext>();
        // 为了区分mock的和非mock的
        if (dbContext.GetType().FullName!.Contains(nameof(CollectionSystem)))
        {
            var connectString = dbContext.Database.GetConnectionString();
            if (!string.IsNullOrWhiteSpace(connectString))
            {
                if (await dbContext.Database.CanConnectAsync())
                {
                    return dbContext;
                }

                var sqlConnectionStringBuilder = new SqlConnectionStringBuilder(connectString);
                var createDbSql = GetCreateDbSql(sqlConnectionStringBuilder);
                sqlConnectionStringBuilder.InitialCatalog = "master";
                await using var connection = new SqlConnection(sqlConnectionStringBuilder.ToString());
                connection.Open();
                await using var command = new SqlCommand(createDbSql, connection);
                await command.ExecuteNonQueryAsync();
                await dbContext.Database.OpenConnectionAsync();
                if (await dbContext.Database.CanConnectAsync())
                {
                    return dbContext;
                }
            }
        }

        return null;
    }

    private string GetCreateDbSql(SqlConnectionStringBuilder sqlConnectionStringBuilder)
    {
        return
            $@"IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = '{sqlConnectionStringBuilder.InitialCatalog}')
BEGIN
    CREATE DATABASE {sqlConnectionStringBuilder.InitialCatalog};
END";
    }

    protected async Task StartupInfrastructure()
    {
        await StartupSqlDb();
    }

    private async Task StartupSqlDb()
    {
        var db = await CheckDbConnect();
        if (db is not null)
        {
            var migrations = db.Database.GetMigrations();
            if (migrations.Any())
            {
                await db.Database.MigrateAsync();
            }
        }
    }

    protected async Task CleanupInfrastructure()
    {
        await CleanDbTables();
        await CleanMongoDbCollections();
    }

    private async Task CleanMongoDbCollections()
    {
        var mongoDatabase = CheckMongoDbConnection();
        if (mongoDatabase != null)
        {
            var collectionNames = await (await mongoDatabase.ListCollectionNamesAsync()).ToListAsync();
            foreach (var collectionName in collectionNames)
            {
                var collection = mongoDatabase.GetCollection<BsonDocument>(collectionName);
                var filter = Builders<BsonDocument>.Filter.Empty;
                await collection.DeleteManyAsync(filter);
            }
        }
    }

    private IMongoDatabase? CheckMongoDbConnection()
    {
        var mongoDbContext = TestServices.Resolve<MongoDbContext>();

        if (mongoDbContext is { Database: not null })
        {
            if (mongoDbContext.GetType().FullName!.Contains(nameof(CollectionSystem)))
            {
                return mongoDbContext.Database;
            }
        }

        return null;
    }

    private async Task CleanDbTables()
    {
        var db = await CheckDbConnect();
        if (db is not null)
        {
            var tableTypes = db.Model.GetEntityTypes();
            var sqlBuilder = new StringBuilder();
            foreach (var tableType in tableTypes)
            {
                sqlBuilder.AppendLine($"DELETE FROM {tableType.GetTableName()};");
            }

            await db.Database.ExecuteSqlRawAsync(sqlBuilder.ToString());
        }
    }

    public void Dispose()
    {
        TestServices.Dispose();
        TestMediator.Dispose();
        TestDbContext.Dispose();
    }
}