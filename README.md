# Partner Management System

一个支持管理员和普通用户的Partner管理系统，包含用户管理、Partner管理和活动管理功能。

## 技术栈

### 后端
- ASP.NET Core 9.0
- Entity Framework Core 9.0
- PostgreSQL
- ASP.NET Core Identity
- JWT Authentication

### 前端
- React 18
- Vite
- Ant Design
- Axios
- React Router

## 项目结构

```
claudetest/
├── PartnerManagement.sln              # 解决方案文件
├── PartnerManagement.Api/             # Web API项目
├── PartnerManagement.Core/            # 核心业务逻辑层
├── PartnerManagement.Infrastructure/  # 基础设施层
├── PartnerManagement.Tests/           # 测试项目
├── frontend/                          # React前端项目
├── plan.md                            # 开发计划文档
└── README.md                          # 本文件
```

## 开始使用

### 前置要求

- .NET 9.0 SDK
- Node.js 18+ 和 npm
- PostgreSQL 15+

### 后端设置

1. **配置数据库连接**

编辑 `PartnerManagement.Api/appsettings.json`，更新PostgreSQL连接字符串：

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=partnermanagement;Username=postgres;Password=yourpassword;Pooling=true;MinPoolSize=10;MaxPoolSize=200;"
  }
}
```

2. **创建数据库迁移**

```bash
cd PartnerManagement.Api
dotnet ef migrations add InitialCreate --project ../PartnerManagement.Infrastructure
```

3. **应用数据库迁移**

```bash
dotnet ef database update --project ../PartnerManagement.Infrastructure
```

4. **运行后端API**

```bash
cd PartnerManagement.Api
dotnet run
```

后端API将在 `https://localhost:5001` 运行（或 `http://localhost:5000`）

### 前端设置

1. **安装依赖**

```bash
cd frontend
npm install
```

2. **配置API地址**

如果需要，编辑 `frontend/src/services/api.js` 中的 API_BASE_URL：

```javascript
const API_BASE_URL = 'http://localhost:5000/api';
```

3. **运行前端开发服务器**

```bash
npm run dev
```

前端应用将在 `http://localhost:5173` 运行

## API文档

后端运行后，可以访问Swagger文档：
- 开发环境: `https://localhost:5001/swagger`

### 主要API端点

#### 认证
- `POST /api/auth/register` - 用户注册
- `POST /api/auth/login` - 用户登录

#### Partners
- `GET /api/partners` - 获取Partner列表（支持分页、搜索、筛选）
- `GET /api/partners/{id}` - 获取Partner详情
- `POST /api/partners` - 创建Partner
- `PUT /api/partners/{id}` - 更新Partner
- `DELETE /api/partners/{id}` - 删除Partner

#### Events
- `GET /api/events` - 获取活动列表（支持分页、搜索、筛选）
- `GET /api/events/{id}` - 获取活动详情
- `POST /api/events` - 创建活动
- `PUT /api/events/{id}` - 更新活动
- `DELETE /api/events/{id}` - 删除活动
- `POST /api/events/{id}/partners` - 为活动关联Partner
- `DELETE /api/events/{id}/partners/{partnerId}` - 移除活动的Partner关联
- `GET /api/events/{id}/partners` - 获取活动关联的Partner列表

## 开发进度

详细的开发计划和进度请查看 [plan.md](plan.md)

### 已完成
- ✅ ASP.NET Core解决方案和项目结构
- ✅ Entity Framework Core和PostgreSQL配置
- ✅ 数据库实体类
- ✅ ASP.NET Core Identity和JWT认证
- ✅ 基础控制器和API接口
- ✅ React前端项目结构

### 进行中
- 🚧 前端页面组件开发
- 🚧 用户管理功能

### 待开始
- ⏳ 性能优化
- ⏳ 测试
- ⏳ 部署配置

## 安全配置

**重要**: 在生产环境部署前，请务必：

1. 修改 `appsettings.json` 中的JWT密钥
2. 使用环境变量或密钥管理服务存储敏感信息
3. 配置HTTPS
4. 更新CORS策略
5. 启用API限流

## 贡献

欢迎提交Issue和Pull Request。

## 许可证

MIT License
