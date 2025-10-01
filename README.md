# ContractFirst API Template

一个基于 .NET 8 的契约优先 API 开发模板，采用现代架构模式和最佳实践。

## 🚀 特性

- **契约优先设计**: 基于接口和契约的API设计模式
- **模块化架构**: 清晰的层次分离和模块化设计
- **多数据源支持**: 支持 Entity Framework Core 和 MongoDB
- **现代化技术栈**: 使用 .NET 8、Mediator、AutoMapper、Autofac 等
- **完整的基础设施**: 包含 JWT 认证、CORS、日志记录等基础设施
- **容器化支持**: 提供 Docker 配置和 CI/CD 流水线
- **代码质量**: 包含代码分析器和单元测试

## 🏗️ 架构概览

项目采用分层架构设计：

```
ContractFirst.Api.Template/
├── src/
│   ├── ContractFirst.Api/              # Web API 主项目
│   ├── ContractFirst.Api.Engines/      # 引擎模块 (依赖注入、中间件等)
│   ├── ContractFirst.Api.Infrastructure/ # 基础设施 (数据持久化、认证等)
│   ├── ContractFirst.Api.Primary/      # 核心契约和接口定义
│   ├── ContractFirst.Api.Realization/  # 业务逻辑实现
│   ├── ContractFirst.Api.DbMigration/  # 数据库迁移工具
│   └── tests/                          # 测试项目
├── src.analyzers/                      # 代码分析器
└── 配置文件 (Dockerfile, CI/CD 等)
```

## 📋 技术栈

- **框架**: .NET 8.0
- **Web框架**: ASP.NET Core
- **依赖注入**: Autofac
- **ORM**: Entity Framework Core, MongoDB Driver
- **中介者模式**: Mediator.Net
- **对象映射**: AutoMapper
- **认证授权**: JWT Bearer
- **日志记录**: Serilog + Seq
- **测试框架**: xUnit
- **容器化**: Docker
- **CI/CD**: GitLab CI

## 🚀 快速开始

### 前提条件

- .NET 8.0 SDK
- Docker (可选，用于容器化部署)
- 数据库 (MySQL/MongoDB)

### 安装和运行

1. **克隆项目**
   ```bash
   git clone https://github.com/huangkemeng/ContractFirst.Api.Template.git
   cd ContractFirst.Api.Template
   ```

2. **配置数据库连接**
   编辑 `src/ContractFirst.Api.Infrastructure/DataPersistence/EfCore/db-setting.json` 和 MongoDB 配置文件。

3. **运行数据库迁移**
   ```bash
   cd src/ContractFirst.Api.DbMigration
   dotnet run
   ```

4. **启动应用**
   ```bash
   cd src/ContractFirst.Api
   dotnet run
   ```

5. **访问 API 文档**
   打开浏览器访问 `https://localhost:7001/swagger`

### Docker 运行

```bash
# 构建镜像
docker build -t contractfirst-api .

# 运行容器
docker run -p 8080:80 contractfirst-api
```

## 📁 项目结构详解

### ContractFirst.Api
- **Program.cs**: 应用程序入口点，使用引擎模式构建应用
- **Controllers/**: API 控制器，继承自 WebBaseController
- **FilterAndMiddlewares/**: 过滤器和中间件

### ContractFirst.Api.Engines
引擎系统负责应用的生命周期管理：
- **BuilderEngine**: 构建时引擎
- **AppEngine**: 应用运行时引擎
- **MediatorEngines**: 中介者模式配置
- **SwaggerEngines**: API 文档配置
- **EfCoreEngines**: Entity Framework 配置
- **MongoDbEngines**: MongoDB 配置

### ContractFirst.Api.Infrastructure
基础设施层提供：
- **数据持久化**: EF Core 和 MongoDB 支持
- **认证授权**: JWT 认证服务
- **CORS 配置**: 跨域资源共享
- **日志记录**: Serilog 集成 Seq
- **设置管理**: 统一的配置管理

### ContractFirst.Api.Primary
核心契约和接口定义：
- **Contracts/**: API 契约接口
- **Bases/**: 基础类和扩展方法

### ContractFirst.Api.Realization
业务逻辑实现层：
- **Handlers/**: Mediator 请求处理器
- **Currents/**: 当前上下文管理
- **Bases/**: 业务基础类和映射器

## 🔧 配置说明

### 数据库配置
项目支持多种数据库配置：

**Entity Framework Core** (`db-setting.json`):
```json
{
  "ConnectionString": "Server=localhost;Database=ContractFirstDb;Uid=root;Pwd=password;",
  "Provider": "MySql"
}
```

**MongoDB** (`mongodb-setting.json`):
```json
{
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "ContractFirstDb"
}
```

### JWT 配置
```json
{
  "Secret": "your-secret-key",
  "Issuer": "ContractFirst.Api",
  "Audience": "ContractFirst.Api.Users",
  "ExpireMinutes": 60
}
```

## 🧪 测试

项目包含完整的测试套件：

```bash
# 运行单元测试
cd src.tests/ContractFirst.Api.UnitTests
dotnet test

# 运行集成测试
cd src.tests/ContractFirst.Api.IntegrationTests
dotnet test
```

## 📊 代码质量

项目包含自定义代码分析器：
- **ContractNamingAnalyzer**: 确保契约命名规范
- 集成到构建过程中自动执行

## 🚀 部署

### 使用 Docker
```bash
docker build -t contractfirst-api .
docker run -d -p 8080:80 --name contractfirst-api contractfirst-api
```

### 使用 GitLab CI
项目包含完整的 GitLab CI 配置，支持：
- 自动构建和测试
- Docker 镜像构建和推送
- 部署到目标环境

### NuGet 包发布
项目配置了 NuGet 包发布脚本：
```bash
.\nuget-pack.ps1
```

## 🤝 贡献指南

1. Fork 项目
2. 创建功能分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m 'Add some AmazingFeature'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 创建 Pull Request

## 📄 许可证

本项目采用 MIT 许可证 - 查看 [LICENSE](LICENSE) 文件了解详情。

## 📞 联系方式

- 项目维护者: [huangkemeng](https://github.com/huangkemeng)
- 项目链接: [https://github.com/huangkemeng/ContractFirst.Api.Template](https://github.com/huangkemeng/ContractFirst.Api.Template)

## 🙏 致谢

感谢所有为这个项目做出贡献的开发者。
