# Partner管理系统开发计划

## 项目概述
开发一个支持管理员和普通用户的Partner管理系统，包含用户管理、Partner管理和活动管理功能，需支持1000+并发用户。

## 功能需求

### 1. 用户管理模块
- **用户注册与认证**
  - Admin用户注册和登录
  - 普通用户注册和登录
  - 基于JWT的身份验证
  - 密码加密存储（bcrypt）
  - 角色权限管理（RBAC）

- **用户管理功能**
  - 用户列表查看
  - 用户信息编辑
  - 用户状态管理（启用/禁用）
  - 用户角色分配

### 2. Partner管理模块
- **Partner基本操作**
  - 添加Partner（名称、联系方式、描述、状态等）
  - 编辑Partner信息
  - 删除Partner
  - Partner列表查看（支持分页、搜索、筛选）
  - Partner详情查看

- **Partner数据字段**
  - ID（主键）
  - 名称
  - 联系人
  - 联系电话
  - 邮箱
  - 地址
  - 描述
  - 状态（活跃/非活跃）
  - 创建时间
  - 更新时间

### 3. 活动管理模块
- **活动基本操作**
  - 创建活动
  - 编辑活动信息
  - 删除活动
  - 活动列表查看（支持分页、搜索、筛选）
  - 活动详情查看

- **活动与Partner关联**
  - 为活动关联多个Partner
  - 查看活动关联的Partner列表
  - 移除活动的Partner关联
  - 查看Partner参与的活动列表

- **活动数据字段**
  - ID（主键）
  - 活动名称
  - 活动描述
  - 开始时间
  - 结束时间
  - 地点
  - 状态（计划中/进行中/已结束）
  - 创建人
  - 创建时间
  - 更新时间

## 技术架构

### 前端技术栈
- **框架**: React 18+
- **样式**: LESS
- **UI组件库**: Ant Design（推荐）或自定义组件
- **状态管理**: Redux Toolkit 或 Zustand
- **路由**: React Router v6
- **HTTP客户端**: Axios
- **表单管理**: React Hook Form
- **构建工具**: Vite 或 Create React App

### 后端技术栈
- **框架**: ASP.NET Core 8.0
- **语言**: C# 12
- **ORM**: Entity Framework Core 8.0
- **数据库提供者**: Npgsql.EntityFrameworkCore.PostgreSQL
- **认证**: ASP.NET Core Identity + JWT Bearer
- **授权**: 基于策略的授权（Policy-based Authorization）
- **验证**: FluentValidation
- **日志**: Serilog + Seq（可选）
- **API文档**: Swagger/Swashbuckle
- **依赖注入**: 内置DI容器
- **缓存**: IDistributedCache + Redis
- **API限流**: AspNetCoreRateLimit
- **健康检查**: ASP.NET Core Health Checks

### 数据库
- **数据库**: PostgreSQL 15+
- **连接池**: 配置适当的连接池大小
- **索引优化**: 为常用查询字段添加索引
- **备份策略**: 定期自动备份

## 数据库设计

### 用户表 (users)
```sql
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) UNIQUE NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(20) NOT NULL, -- 'admin' or 'user'
    status VARCHAR(20) DEFAULT 'active',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_users_email ON users(email);
CREATE INDEX idx_users_role ON users(role);
```

### Partner表 (partners)
```sql
CREATE TABLE partners (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    contact_person VARCHAR(100),
    phone VARCHAR(20),
    email VARCHAR(100),
    address TEXT,
    description TEXT,
    status VARCHAR(20) DEFAULT 'active',
    created_by INTEGER REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_partners_name ON partners(name);
CREATE INDEX idx_partners_status ON partners(status);
```

### 活动表 (events)
```sql
CREATE TABLE events (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200) NOT NULL,
    description TEXT,
    start_date TIMESTAMP NOT NULL,
    end_date TIMESTAMP NOT NULL,
    location VARCHAR(200),
    status VARCHAR(20) DEFAULT 'planned',
    created_by INTEGER REFERENCES users(id),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_events_start_date ON events(start_date);
CREATE INDEX idx_events_status ON events(status);
```

### 活动-Partner关联表 (event_partners)
```sql
CREATE TABLE event_partners (
    id SERIAL PRIMARY KEY,
    event_id INTEGER REFERENCES events(id) ON DELETE CASCADE,
    partner_id INTEGER REFERENCES partners(id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    UNIQUE(event_id, partner_id)
);

CREATE INDEX idx_event_partners_event ON event_partners(event_id);
CREATE INDEX idx_event_partners_partner ON event_partners(partner_id);
```

## 性能优化方案（支持1000+并发）

### 后端优化
1. **API层面**
   - 实现响应缓存（Redis）
   - 使用连接池管理数据库连接
   - 实现API限流（Rate Limiting）
   - 启用GZIP压缩

2. **数据库层面**
   - 优化查询语句，避免N+1问题
   - 合理使用索引
   - 实现数据库读写分离（如需要）
   - 配置合适的连接池大小（建议：100-200）

3. **缓存策略**
   - Redis缓存热点数据
   - 用户会话缓存
   - Partner列表缓存（短期TTL）
   - 活动列表缓存

### 前端优化
1. **加载优化**
   - 代码分割和懒加载
   - 图片懒加载
   - 使用CDN加速静态资源

2. **渲染优化**
   - 虚拟列表（大数据量表格）
   - 防抖和节流处理
   - React.memo优化组件渲染

### 基础设施
1. **负载均衡**
   - 使用Nginx作为反向代理
   - 配置多个后端实例
   - 实现健康检查

2. **监控和日志**
   - 应用性能监控（APM）
   - 错误日志收集
   - 访问日志分析

## API设计示例

### 用户相关API
- `POST /api/auth/register` - 用户注册
- `POST /api/auth/login` - 用户登录
- `GET /api/users` - 获取用户列表（Admin）
- `GET /api/users/:id` - 获取用户详情
- `PUT /api/users/:id` - 更新用户信息
- `DELETE /api/users/:id` - 删除用户（Admin）

### Partner相关API
- `POST /api/partners` - 创建Partner
- `GET /api/partners` - 获取Partner列表（支持分页、搜索）
- `GET /api/partners/:id` - 获取Partner详情
- `PUT /api/partners/:id` - 更新Partner信息
- `DELETE /api/partners/:id` - 删除Partner

### 活动相关API
- `POST /api/events` - 创建活动
- `GET /api/events` - 获取活动列表（支持分页、搜索）
- `GET /api/events/:id` - 获取活动详情
- `PUT /api/events/:id` - 更新活动信息
- `DELETE /api/events/:id` - 删除活动
- `POST /api/events/:id/partners` - 为活动关联Partner
- `DELETE /api/events/:id/partners/:partnerId` - 移除活动的Partner关联
- `GET /api/events/:id/partners` - 获取活动关联的Partner列表

## 安全考虑
1. **认证和授权**
   - JWT token过期机制
   - Refresh token机制
   - 基于角色的访问控制（RBAC）

2. **数据安全**
   - SQL注入防护（使用ORM参数化查询）
   - XSS防护
   - CSRF防护
   - 密码强度验证
   - 敏感数据加密

3. **API安全**
   - HTTPS通信
   - API限流
   - 输入验证
   - CORS配置

## 项目结构建议

### 前端结构
```
frontend/
├── public/
├── src/
│   ├── components/        # 通用组件
│   ├── pages/            # 页面组件
│   │   ├── auth/         # 登录注册页面
│   │   ├── users/        # 用户管理页面
│   │   ├── partners/     # Partner管理页面
│   │   └── events/       # 活动管理页面
│   ├── services/         # API服务
│   ├── store/            # 状态管理
│   ├── styles/           # LESS样式文件
│   ├── utils/            # 工具函数
│   ├── hooks/            # 自定义Hooks
│   └── App.tsx
├── package.json
└── vite.config.ts
```

### 后端结构（ASP.NET Core）
```
backend/
├── PartnerManagement.Api/          # Web API项目
│   ├── Controllers/                # API控制器
│   ├── Middlewares/                # 中间件
│   ├── Program.cs                  # 应用程序入口
│   ├── appsettings.json           # 配置文件
│   └── appsettings.Development.json
├── PartnerManagement.Core/         # 核心业务逻辑
│   ├── Entities/                   # 实体类
│   ├── Interfaces/                 # 接口定义
│   ├── Services/                   # 业务服务
│   ├── DTOs/                       # 数据传输对象
│   └── Validators/                 # FluentValidation验证器
├── PartnerManagement.Infrastructure/ # 基础设施层
│   ├── Data/                       # 数据访问
│   │   ├── ApplicationDbContext.cs
│   │   ├── Repositories/           # 仓储实现
│   │   └── Migrations/             # EF Core迁移
│   ├── Identity/                   # 身份验证
│   └── Configurations/             # EF配置类
├── PartnerManagement.Tests/        # 单元测试项目
│   ├── UnitTests/
│   └── IntegrationTests/
└── PartnerManagement.sln           # 解决方案文件
```

## 开发阶段建议

### 第一阶段：基础搭建（2-3周）
- 项目初始化和技术栈搭建
- 数据库设计和创建
- 用户认证系统实现
- 基础UI框架搭建

### 第二阶段：核心功能（3-4周）
- Partner管理功能实现
- 活动管理功能实现
- 活动与Partner关联功能
- 用户管理功能（Admin）

### 第三阶段：优化和测试（2-3周）
- 性能优化
- 压力测试（模拟1000+并发）
- 安全测试
- Bug修复

### 第四阶段：部署上线（1周）
- 生产环境配置
- 部署和监控设置
- 文档编写
- 上线和培训

## 测试策略
- 单元测试（Jest/Vitest）
- 集成测试
- E2E测试（Playwright/Cypress）
- 性能测试（Apache JMeter/k6）
- 安全测试

## 部署建议
- **容器化**: 使用Docker
- **编排**: Docker Compose或Kubernetes
- **CI/CD**: GitHub Actions或GitLab CI
- **云平台**: AWS/Azure/阿里云
- **数据库**: 使用托管PostgreSQL服务

## 预估资源需求（支持1000并发）
- **应用服务器**: 2-4个实例（每个2核4GB）
- **数据库**: PostgreSQL（4核8GB，可扩展）
- **缓存**: Redis（2核4GB）
- **负载均衡**: Nginx
- **带宽**: 至少100Mbps

## ASP.NET Core实现要点

### 1. Entity Framework Core配置
```csharp
// ApplicationDbContext.cs
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Partner> Partners { get; set; }
    public DbSet<Event> Events { get; set; }
    public DbSet<EventPartner> EventPartners { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // 配置实体关系和索引
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

### 2. JWT认证配置
```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
```

### 3. 依赖注入配置
```csharp
// Program.cs - 服务注册
builder.Services.AddScoped<IPartnerService, PartnerService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IUserService, UserService>();

// 配置Redis缓存
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// 配置限流
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
```

### 4. 仓储模式实现
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
```

### 5. NuGet包依赖
- Microsoft.AspNetCore.Identity.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Npgsql.EntityFrameworkCore.PostgreSQL
- Microsoft.AspNetCore.Authentication.JwtBearer
- FluentValidation.AspNetCore
- Serilog.AspNetCore
- Swashbuckle.AspNetCore
- StackExchange.Redis
- AspNetCoreRateLimit
- AutoMapper.Extensions.Microsoft.DependencyInjection

### 6. 性能优化（ASP.NET Core特定）
- 启用响应压缩（Response Compression）
- 使用异步控制器方法（async/await）
- 配置Kestrel服务器性能选项
- 启用输出缓存（Output Caching）
- 使用AsNoTracking()进行只读查询
- 配置连接池：
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=partnermanagement;Username=postgres;Password=yourpassword;Pooling=true;MinPoolSize=10;MaxPoolSize=200;"
  }
  ```

### 7. 中间件管道配置
```csharp
// Program.cs - 中间件配置顺序
app.UseHttpsRedirection();
app.UseRouting();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseIpRateLimiting();
app.UseResponseCompression();
app.MapControllers();
```

### 8. 健康检查配置
```csharp
builder.Services.AddHealthChecks()
    .AddNpgSql(connectionString)
    .AddRedis(redisConnection);

app.MapHealthChecks("/health");
```

## 总结
该系统采用ASP.NET Core 8.0作为后端技术栈，具有企业级的稳定性、性能和安全性。通过Clean Architecture分层设计、Entity Framework Core ORM、JWT认证、Redis缓存和合理的性能优化策略，可以轻松支持1000+并发用户。ASP.NET Core的成熟生态系统、强类型特性和内置的性能优化使其非常适合构建高性能的企业级应用。
