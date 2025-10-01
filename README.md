# ContractFirst API Template

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker)](https://www.docker.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

一个基于 .NET 8 的现代化契约优先 API 开发模板，采用 Clean Architecture 和模块化设计，提供企业级 API 开发的最佳实践。

## ✨ 核心特性

### 🏗️ 架构设计
- **契约优先开发**: 基于接口和契约的 API 设计模式
- **Clean Architecture**: 清晰的层次分离和依赖规则
- **模块化设计**: 可插拔的引擎系统，便于扩展和维护
- **多场景支持**: 支持 Web API、测试、集成测试等多种场景

### 🔧 技术栈
- **运行时**: .NET 8.0
- **Web框架**: ASP.NET Core
- **依赖注入**: Autofac (支持属性注入)
- **数据访问**: Entity Framework Core + MongoDB
- **消息处理**: Mediator.Net (CQRS 模式)
- **对象映射**: AutoMapper
- **API文档**: Swagger/OpenAPI 3.0
- **认证授权**: JWT Bearer Token
- **日志系统**: Serilog + Seq
- **测试框架**: xUnit + NSubstitute

### 🚀 开箱即用
- **完整的基础设施**: JWT 认证、CORS、健康检查、异常处理
- **多数据库支持**: MySQL (EF Core) + MongoDB
- **容器化支持**: Docker + Docker Compose
- **CI/CD 集成**: GitLab CI 配置
- **代码质量**: 自定义代码分析器 + 单元测试

## 📁 项目结构

```
ContractFirst.Api.Template/
├── src/
│   ├── ContractFirst.Api/                 # Web API 主项目
│   │   ├── Controllers/                   # API 控制器
│   │   ├── FilterAndMiddlewares/          # 过滤器和中间件
│   │   └── Program.cs                     # 应用入口 (极简配置)
│   ├── ContractFirst.Api.Engines/         # 引擎系统 (核心)
│   │   ├── Bases/                         # 引擎基础接口
│   │   ├── MediatorEngines/               # 中介者配置
│   │   ├── SwaggerEngines/                # API 文档配置
│   │   ├── EfCoreEngines/                 # EF Core 配置
│   │   ├── MongoDbEngines/                # MongoDB 配置
│   │   └── ConventionEngines/             # 约定配置
│   ├── ContractFirst.Api.Infrastructure/  # 基础设施层
│   │   ├── DataPersistence/               # 数据持久化
│   │   ├── JwtFunction/                   # JWT 认证
│   │   ├── CorsFunction/                  # CORS 配置
│   │   └── SeqLog/                        # 日志配置
│   ├── ContractFirst.Api.Primary/         # 核心契约层
│   │   ├── Contracts/                     # API 契约接口
│   │   └── Bases/                         # 基础类和扩展
│   ├── ContractFirst.Api.Realization/     # 业务实现层
│   │   ├── Handlers/                      # Mediator 处理器
│   │   ├── Currents/                      # 当前上下文
│   │   └── Bases/                         # 业务基础类
│   ├── ContractFirst.Api.DbMigration/     # 数据库迁移工具
│   └── tests/                             # 测试项目
├── src.analyzers/                         # 代码分析器
└── 配置文件
    ├── Dockerfile                         # Docker 配置
    ├── .gitlab-ci.yml                     # CI/CD 配置
    └── *.ps1                              # PowerShell 脚本
```

## 🚀 快速开始

### 环境要求
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (可选)
- 数据库: MySQL 8.0+ 或 MongoDB 6.0+

### 1. 克隆项目
```bash
git clone https://github.com/huangkemeng/ContractFirst.Api.Template.git
cd ContractFirst.Api.Template
```

### 2. 配置数据库
#### MySQL 配置
编辑 `src/ContractFirst.Api.Infrastructure/DataPersistence/EfCore/db-setting.json`:
```json
{
  "DbSetting": {
    "ConnectionStrings": {
      "WebApi": "Server=localhost;Database=contractfirst;Uid=root;Pwd=your_password;",
      "IntegrationTest": "Server=localhost;Database=contractfirst_test;User Id=sa;Password=Password;"
    }
  }
}
```

#### MongoDB 配置
编辑 `src/ContractFirst.Api.Infrastructure/DataPersistence/MongoDb/mongodb-setting.json`:
```json
{
  "MongoDbSetting": {
    "Servers": [
      {
        "Host": "localhost",
        "Port": 27017
      }
    ],
    "DatabaseNames": {
      "WebApi": "contractfirst",
      "IntegrationTest": "contractfirst_test"
    }
  }
}
```

### 3. 运行数据库迁移
```bash
cd src/ContractFirst.Api.DbMigration
./run-migration.ps1
```

### 4. 启动应用
```bash
cd src/ContractFirst.Api
dotnet run
```

应用将在以下地址启动：
- **HTTPS**: https://localhost:7247
- **HTTP**: http://localhost:5050
- **Swagger UI**: https://localhost:7247/swagger

## 🐳 Docker 部署

### 使用 Docker Compose (推荐)
```bash
# 启动所有服务 (应用 + 数据库)
docker-compose up -d

# 查看服务状态
docker-compose ps

# 停止服务
docker-compose down
```

### 手动构建
```bash
# 构建镜像
docker build -t contractfirst-api .

# 运行容器
docker run -d \
  -p 8080:80 \
  -p 8081:443 \
  --name contractfirst-api \
  contractfirst-api
```

## ⚙️ 配置说明

### JWT 配置
编辑 `src/ContractFirst.Api.Infrastructure/JwtFunction/jwt-setting.json`:
```json
{
  "JwtSetting": {
    "Issuer": "ContractFirst.Api",
    "SignKey": "your-256-bit-secret-key-here",
    "Audience": "ContractFirst.Api.Users",
    "LongExpiresInMinutes": 43200,  // 30天
    "ShortExpiresInMinutes": 1440   // 1天
  }
}
```

### CORS 配置
编辑 `src/ContractFirst.Api.Infrastructure/CorsFunction/cors-setting.json`:
```json
{
  "CorsSetting": {
    "AllowedOrigins": [
      "https://localhost:3000",
      "https://yourapp.com"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowedHeaders": ["*"]
  }
}
```

### 日志配置
编辑 `src/ContractFirst.Api.Infrastructure/SeqLog/seq-setting.json`:
```json
{
  "SeqSetting": {
    "ServerUrl": "http://localhost:5341",
    "ApiKey": "your-seq-api-key",
    "LogLevel": "Information"
  }
}
```

## 🧪 测试

### 运行单元测试
```bash
cd src.tests/ContractFirst.Api.UnitTests
dotnet test
```

### 运行集成测试
```bash
cd src.tests/ContractFirst.Api.IntegrationTests
dotnet test
```

### 测试覆盖率
```bash
# 安装覆盖率工具
dotnet tool install -g dotnet-reportgenerator-globaltool

# 生成覆盖率报告
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:*/TestResults/*/coverage.cobertura.xml -targetdir:coveragereport -reporttypes:Html
```

## 🔍 引擎系统

项目采用创新的引擎系统来管理应用生命周期：

### Builder Engines (构建时引擎)
- **RegisterSwagger**: Swagger/OpenAPI 配置
- **RegisterMediator**: Mediator 模式配置
- **RegisterDbSet**: EF Core 上下文配置
- **RegisterMongoDb**: MongoDB 客户端配置

### App Engines (运行时引擎)
- **UseSwagger**: Swagger UI 中间件
- **UseMigrations**: 数据库迁移
- **ConfigureConvention**: MVC 约定配置

### 自定义引擎
通过实现 `IBuilderEngine` 或 `IAppEngine` 接口，可以轻松添加自定义引擎。

## 📊 代码质量

### 代码分析器
项目包含自定义 Roslyn 分析器：
- **ContractNamingAnalyzer**: 确保契约接口命名规范
- 自动在构建过程中执行

### 代码规范
- 使用 `Nullable` 引用类型
- 遵循 C# 编码规范
- 包含 XML 文档注释

## 🚀 部署选项

### 1. Docker 部署
```bash
docker build -t contractfirst-api .
docker run -d -p 8080:80 contractfirst-api
```

### 2. GitLab CI/CD
项目包含完整的 GitLab CI 配置：
- 自动构建和测试
- Docker 镜像构建和推送
- 部署到目标环境

### 3. 传统部署
```bash
# 发布应用
dotnet publish -c Release -o ./publish

# 运行发布版本
cd ./publish
dotnet ContractFirst.Api.dll
```

## 🤝 贡献指南

我们欢迎社区贡献！请遵循以下步骤：

1. **Fork 项目**
2. **创建功能分支** (`git checkout -b feature/AmazingFeature`)
3. **提交更改** (`git commit -m 'Add some AmazingFeature'`)
4. **推送到分支** (`git push origin feature/AmazingFeature`)
5. **创建 Pull Request**

### 开发规范
- 遵循现有的代码风格
- 添加适当的单元测试
- 更新相关文档
- 确保所有测试通过

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 📞 支持与联系

- **项目维护者**: [huangkemeng](https://github.com/huangkemeng)
- **项目主页**: [https://github.com/huangkemeng/ContractFirst.Api.Template](https://github.com/huangkemeng/ContractFirst.Api.Template)
- **问题反馈**: [GitHub Issues](https://github.com/huangkemeng/ContractFirst.Api.Template/issues)

## 🙏 致谢

感谢所有为这个项目做出贡献的开发者和社区成员。特别感谢以下开源项目：

- [ASP.NET Core](https://github.com/dotnet/aspnetcore)
- [Entity Framework Core](https://github.com/dotnet/efcore)
- [AutoMapper](https://github.com/AutoMapper/AutoMapper)
- [Mediator.Net](https://github.com/mayuanyang/Mediator.Net)
- [Serilog](https://github.com/serilog/serilog)

---

**⭐ 如果这个项目对您有帮助，请给我们一个 Star！**
