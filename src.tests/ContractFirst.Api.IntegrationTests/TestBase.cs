using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using ContractFirst.Api.Engines.Bases;
using ContractFirst.Api.Infrastructure.EfCore;
using ContractFirst.Api.Infrastructure.MongoDb;
using Mediator.Net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ContractFirst.Api.IntegrationTests;

[Collection("Sequential")]
public class TestBase : IClassFixture<SequentialCollectionFixture>, IAsyncDisposable, IDisposable
{
    public TestBase()
    {
        Build();
    }

    protected ILifetimeScope TestLifetimeScope { get; private set; }
    protected IMediator TestMediator { get; private set; }
    protected ApplicationDbContext TestDbContext { get; private set; }

    protected void Build(Action<ContainerBuilder>? builderAction = null)
    {
        if (TestEnvironmentCache.LifetimeScope == null)
        {
            var builder = new ContainerBuilder();
            TestEnvironmentCache.LifetimeScope = builder.TestBuildWithEngines(builderAction);
        }

        TestLifetimeScope = builderAction != null
            ? TestEnvironmentCache.LifetimeScope.BeginLifetimeScope(builderAction)
            : TestEnvironmentCache.LifetimeScope;
        TestMediator = TestLifetimeScope.Resolve<IMediator>();
        TestDbContext = TestLifetimeScope.Resolve<ApplicationDbContext>();
    }

    public ValueTask DisposeAsync()
    {
        Dispose();
        return default;
    }

    private async Task<ApplicationDbContext?> CheckDbConnect()
    {
        var dbContext = TestLifetimeScope.Resolve<ApplicationDbContext>();
        // 为了区分mock的和非mock的
        if (dbContext.GetType().FullName!.Contains("ContractFirst.Api"))
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
        if (!TestEnvironmentCache.IsInfrastructureStarted)
        {
            await StartupSqlDb();
            TestEnvironmentCache.IsInfrastructureStarted = true;
        }
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
        var mongoDbContext = TestLifetimeScope.Resolve<MongoDbContext>();

        if (mongoDbContext is { Database: not null })
        {
            if (mongoDbContext.GetType().FullName!.Contains("ContractFirst.Api"))
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
        TestLifetimeScope.Dispose();
        TestMediator.Dispose();
        TestDbContext.Dispose();
    }
}