# ContractFirstApi.Template 開發框架

## NuGet 包地址

```
https://www.nuget.org/packages/ContractFirstApi.Template/
```

## 背景

按照傳統的前後端協作方式，一般是後端將接口開發好後，通過 Swagger UI 的方式將接口給到前端對接。但是需要強調的是，大多數情況下，後端的業務邏輯複雜，接口開發完成並交付給前端對接的速度可能會有點慢。

基於這樣的一種場景，我總結出了**契約優先（ContractFirst）** 的開發方式，並且做成了基於 C# 開發的 VS 模板。基於這樣的一套模板以及接下來我將要介紹的開發方式，開發者將可以大大提高前後端的並行開發的效率。此外，除了契約優先這樣一種開發理念之外，本套模板還有諸多優勢。

## 快速上手

### 安裝模板

```bash
dotnet new install ContractFirstApi.Template::0.1.2
```

### 創建項目

```bash
dotnet new ContractFirst --name <your project name>
```

## 項目結構

一個剛創建的項目將包含下面的內容：

```
<your project name>/
├── <your project name>/          # WebAPI 層
├── <your project name>.DbMigration/    # 數據遷移層
├── <your project name>.Engines/        # AOP 引擎層
├── <your project name>.Infrastructure/ # 基礎設施層
├── <your project name>.Primary/        # 基本層（關鍵層）
├── <your project name>.Realization/    # 實現層
├── <your project name>.CodeAnalyzer/   # 代碼分析器層（可選）
├── <your project name>.CodeAnalyzer.Tests/ # 代碼分析器測試層（可選）
└── <your project name>.CodeGenerator/  # 代碼生成器層（可選）
```

### 各項目功能說明

- **無後綴（WebAPI 層）**：包含 Controller 以及 Startup 的內容，想要看到 Swagger 頁面，應將該項目設置為啟動項目並啟動
- **.DbMigration**：數據遷移層，使用 EFCore Tool 進行數據庫遷移的腳本以及用來指定數據庫上下文的 DbMigrationFactory
- **.Engines**：AOP 引擎層，所有的 AOP 都在這層進行編寫，是整個項目能夠自動運轉起來的發動機
- **.Infrastructure**：基礎設施層，數據庫，阿里雲等等常見的基礎設施服務，將在這層編寫
- **.Primary**：基本層，也可以稱作關鍵層。該層沒有任何具體的實現，只包含了一些契約接口的定義以及實體的定義
- **.Realization**：實現層。是對基本層中定義的契約的實現
- **.CodeAnalyzer**：代碼分析器層。用於約束項目的代碼風格，使項目的代碼更加規範化（可選）
- **.CodeAnalyzer.Tests**：代碼分析器的測試層，用於測試我們編寫的分析器和代碼修復器（可選）
- **.CodeGenerator**：代碼生成器層，用於動態生成項目代碼（可選）

## 使用教程

### 什麼是 AOP 引擎？

AOP 引擎是使用 AOP 的思路，集中將有關系的內容關聯起來，將需要注入的資源發現並注入等等一切自動化的實現。

**示例**：基礎設施層中，EfCore 有配置文件 `setting.json` 和對應的 `setting.cs` 類，而 `setting.cs` 的值是自動從 `setting.json` 中來的，這兩個文件能自動關聯，就是因為在 AOP 引擎層實現的一個 `SettingEngine`。

### 使用 AOP 引擎

#### 內置引擎及其功能

- **ConventionEngines**：常規配置引擎
  - 跨域配置
  - JWT 的配置
  - 全局異常配置
  - 全局時區處理配置
  - 自動屬性注入配置等

- **EfCoreEngines**：EfCore 配置引擎
  - DbContext 的注入
  - DbSet<T> 的注入
  - EfCore 事件（增刪改）的發現和注入
  - Run Migrations 等

- **MediatorEngines**：Mediator 的注入
  - 自動將實現和契約綁定
  - EfCorePipe 和 ValidatorPipe 的注入
  - 當契約還沒有對應實現時，自動返回 FakerResponse 的實現

- **ScanServiceEngines**：掃描程序集中是否有類有特性 AsType，當有時，自動注入對應的接口

- **SettingEngines**：掃描基礎設施類中是否有 ISetting 的實現類，當有時，自動賦值並注入

- **SwaggerEngines**：Swagger 的相關配置

#### 啟用引擎功能

在 WebAPI 層的 `Program.cs` 類中使用下面來啟動引擎並開啟對應的功能：

```csharp
var app = builder
    .BuildWithEngines(options =>
    {
        options.EnableSwagger = true;
        options.EnableFakerRealization = true;
        options.EnableAutoResolve = true;
        options.EnableEfCore = false;
        options.EnableGlobalExceptionFilter = false;
        options.EnableTimezoneHandler = false;
        options.EnableValidator = false;
        options.EnableCors = false;
        options.EnableJwt = false;
    });
```

### 實現一個自定義 AOP 引擎

引擎根據其執行的期間不同，分成兩種引擎：`IBuilderEngine` 和 `IAppEngine`。

#### BuilderEngine 示例

```csharp
public class RegisterSqlDbContext : IBuilderEngine
{
    private readonly ContainerBuilder containerBuilder;

    public RegisterSqlDbContext(ContainerBuilder containerBuilder)
    {
        this.containerBuilder = containerBuilder;
    }
    
    public void Run()
    {
        containerBuilder.RegisterType<SqlDbContext>()
            .AsSelf()
            .As<DbContext>()
            .InstancePerLifetimeScope();
    }
}
```

或者：

```csharp
public class RegisterSqlDbContext : IBuilderEngine
{
    private readonly IServiceCollection serviceCollection;

    public RegisterSqlDbContext(IServiceCollection serviceCollection)
    {
        this.serviceCollection = serviceCollection;
    }
    
    public void Run()
    {
        serviceCollection.AddDbContext<SqlDbContext>();
    }
}
```

#### AppEngine 示例

```csharp
public class UseEfMigrations : IAppEngine
{
    private readonly SqlDbContext dbContext;

    public UseEfMigrations(SqlDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void Run()
    {
        var migrations = dbContext.Database.GetMigrations();
        if (migrations.Any())
        {
            dbContext.Database.Migrate();
        }
    }
}
```

## 契約優先 (ContractFirst)

### 什麼是契約優先？

契約優先就是字面意思，當開發者創建好項目之後，便可以開始編寫契約而不需要關注任何實現，引擎會自動生成一個實現，直到真的找到了基於該契約的真正實現。

### 使用契約優先

在 WebAPI 層的 `BuildWithEngines` 方法中，允許引擎生成一個假的契約實現：

```csharp
options.EnableFakerRealization = true;
```

#### 創建 Get 契約

在 `.Primary` 項目的 Contracts 文件夾下，新建一個繼承 `IRequestContract<,>` 的接口：

```csharp
public interface IGetUserInfoContract : IRequestContract<GetUserInfoRequest, GetUserInfoResponse>
{
}

public class GetUserInfoRequest : IRequest
{
    public Guid UserId { get; set; }
}

public class GetUserInfoResponse : IResponse
{
    public Guid UserId { get; set; }
    public string UserName { get; set; }
    public int Age { get; set; }
    public bool Enable { get; set; }
}
```

#### 創建 Controller

在 WebAPI 層的 Controllers 文件夾下創建繼承自 `BaseController` 的控制器：

```csharp
public class UserController : BaseController
{
    [HttpGet]
    [ProducesResponseType(typeof(GetUserInfoResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserInfo([FromQuery] GetUserInfoRequest request,
        CancellationToken cancellationToken)
    {
        var response = await Mediator.RequestAsync<GetUserInfoRequest, GetUserInfoResponse>(request, cancellationToken);
        return Ok(response);
    }
}
```

`BaseController` 已經自動注入了 `IMediator`：

```csharp
[ApiController]
[Route("api/[controller]")]
public class BaseController : ControllerBase
{
    [AutoResolve] 
    public IMediator Mediator { get; set; }
}
```

#### 創建 Post/Put/Patch/Delete 契約

使用 `ICommandContract<>` 或 `ICommandContract<,>`：

```csharp
public interface IAddUserContract : ICommandContract<AddUserCommand>
{
}

public class AddUserCommand : ICommand
{
    public string UserName { get; set; }
    public int Age { get; set; }
    public bool Enable { get; set; }
}
```

### 實現契約

在 `.Realization` 項目中創建契約的具體實現：

```csharp
public class GetUserInfoHandler : IGetUserInfoContract
{
    public Task<GetUserInfoResponse> Handle(IReceiveContext<GetUserInfoRequest> context,
        CancellationToken cancellationToken)
    {
        return Task.FromResult(new GetUserInfoResponse
        {
            UserId = Guid.NewGuid(),
            Age = 20,
            Enable = true,
            UserName = "real name"
        });
    }

    public void Validator(ContractValidator<GetUserInfoRequest> validator)
    {
        validator.RuleFor(e => e.UserId)
            .NotEmpty();
    }

    public Task TestAsync()
    {
        throw new NotImplementedException();
    }
}
```

## 模板的世界觀 (Why design like that?)

### 一個契約應該包含什麼？

在模板的設計者眼中，契約的實現應該至少具備完整的業務邏輯代碼和對參數的驗證。

#### 契約接口定義

```csharp
public interface IRequestContract<TRequest, TResponse> : IContract<TRequest>, IRequestHandler<TRequest, TResponse>
    where TRequest : class, IRequest where TResponse : class, IResponse
{
}

public interface IContract<T> where T : IMessage
{
    Task TestAsync();
    void Validator(ContractValidator<T> validator);
}
```

這意味著 `IRequestContract` 的實現必須包含三個方法：

```csharp
Task<TResponse> Handle(IReceiveContext<T> context, CancellationToken cancellationToken);
Task TestAsync();
void Validator(ContractValidator<T> validator);
```

#### Validator 示例

```csharp
public void Validator(ContractValidator<GetUserInfoRequest> validator)
{
    validator.RuleFor(e => e.UserId)
             .NotEmpty();
}
```

### 一個實體應該包含什麼？

所有的實體都應該繼承自 `IEntityPrimary`，也就是至少包含下面兩個字段：

```csharp
public class User : IEntityPrimary
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
}
```

#### 實體類型

- **`IEfDbEntity`**：如果一個實體後續會使用 EfCore 進行操作，請實現該接口
- **`IExtendedEntity`**：擴展表的實體請實現該接口
- **`IMainEntity`**：主表（相對於擴展表）的實體請基於本接口
- **`IMultipleSystem`**：多個系統共享的實體請實現該接口
- **`IRelationEntity`**：多對多關系表的實體請基於本接口
- **`IUpdatableEntity`**：需要記錄更新日期和更新人的實體請基於該接口

### 什麼是 IEfDbEntity？

#### EfCore 實體示例

```csharp
public class User : IEntityPrimary, IEfDbEntity<User>
{
    public User()
    {
        this.InitPropertyValues();
    }
    
    public Guid Id { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public string Name { get; set; }

    public static void ConfigureEntityMapping(EntityTypeBuilder<User> builder)
    {
        builder.AutoConfigure();
        builder.Property(e => e.Name)
            .HasColumnName("Username")
            .HasMaxLength(50);
    }
}
```

#### 使用 DbSet 代替 DbContext

```csharp
public class GetUserInfoHandler : IGetUserInfoContract
{
    private readonly DbSet<User> userSet;

    public GetUserInfoHandler(DbSet<User> userSet)
    {
        this.userSet = userSet;
    }

    public async Task<GetUserInfoResponse> Handle(IReceiveContext<GetUserInfoRequest> context,
        CancellationToken cancellationToken)
    {
        var user = await userSet.FirstOrDefaultAsync(e => e.Name == "Matt.H", cancellationToken);
        return new GetUserInfoResponse
        {
            UserId = user?.Id,
            Age = 20,
            Enable = user is not null,
            UserName = user?.Name
        };
    }
}
```

### 什麼是 IEntityEvent？

EfCore 實體事件分為：
- **`IMultipleRunEntityEvent`**：多次運行的實體事件
- **`ISingleRunEntityEvent`**：單次運行的實體事件

#### 創建實體事件

在 `.Primary` 項目中創建事件接口：

```csharp
public interface IMainEntityAddedEvent : IMultipleRunEntityEvent
{
}
```

#### 綁定實體事件

```csharp
public class User : IMainEntity, IEfDbEntity<User>, IHasEntityEvent<User>
{
    // ... 其他屬性

    public static void ConfigureEntityEvent(EntityEventBuilder<User> builder)
    {
        builder.Entity()
            .HasAddedEvent<IMainEntityAddedEvent>();
        
        // 或者綁定屬性事件
        builder.Property(e => e.Name)
            .HasUpdatedEvent<IUserUpdatedEvent>();
    }
}
```

#### 實現實體事件

```csharp
[AsType(LifetimeEnum.Scope, typeof(IMainEntityAddedEvent))]
public class MainEntityAddedEvent : IMainEntityAddedEvent
{
    public Task Handle(EntityEntry entry, CancellationToken cancellationToken)
    {
        if (entry.Entity is IMainEntity mainEntity)
        {
            // todo: mainEntity.CreatedBy = userId;
        }
        return Task.CompletedTask;
    }
}
```

### 自動注入是怎麼做的？

#### 使用 AsType 特性

```csharp
[AsType(LifetimeEnum.Scope, typeof(ICurrent<User>))]
public class CurrentUser : ICurrent<User>
{
    public Task<User?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<Guid?> GetCurrentUserIdAsync()
    {
        throw new NotImplementedException();
    }
}
```

#### ICurrent 實現示例

```csharp
[AsType(LifetimeEnum.Scope, typeof(ICurrent<Staff>))]
public class CurrentStaff : ICurrent<Staff>
{
    private readonly DbSet<Staff> staffDb;
    private readonly JwtService jwtService;
    private readonly IHttpContextAccessor httpContextAccessor;

    public CurrentStaff(
        DbSet<Staff> staffDb,
        JwtService jwtService,
        IHttpContextAccessor httpContextAccessor)
    {
        this.staffDb = staffDb;
        this.jwtService = jwtService;
        this.httpContextAccessor = httpContextAccessor;
    }

    public async Task<Staff?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        var id = await GetCurrentUserIdAsync();
        if (id != null)
        {
            return await staffDb.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }
        return null;
    }
    
    public async Task<Guid?> GetCurrentUserIdAsync()
    {
        var currentAuthHeader = JwtService.GetAuthentication(httpContextAccessor);
        if (currentAuthHeader != null)
        {
            var token = currentAuthHeader.Parameter;
            if (!string.IsNullOrWhiteSpace(token))
            {
                var validateResult = await jwtService.ValidateTokenAsync(token, false);
                if (validateResult.IsValid)
                {
                    var staffId = validateResult.ClaimsIdentity.FindFirst(JwtRegisteredClaimNames.Sid)?.Value;
                    if (staffId != null && Guid.TryParse(staffId, out var id))
                    {
                        return id;
                    }
                }
            }
        }
        return null;
    }
}
```

### DbUp 是怎麼做的？

模板中使用的是 EfCore 的 Migration，在項目啟動的時候，運行所有未運行的 Migrations：

```csharp
var dbContext = migrateScope.Resolve<SqlDbContext>();
var connectString = dbContext.Database.GetConnectionString();
if (!string.IsNullOrWhiteSpace(connectString))
{
    var migrations = dbContext.Database.GetMigrations();
    if (migrations.Any() && dbContext.Database.CanConnect())
    {
        dbContext.Database.Migrate();
    }
}
```
