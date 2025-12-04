# Final Capstone: Task Management Web API

**Difficulty**: ⭐⭐⭐⭐⭐ (Production-Ready)
**Estimated Time**: 60-80 hours (4 weeks)
**Prerequisites**: Completed all Path 1 content (Months 1-6)

---

## 🎯 Project Overview

Build a complete, production-ready RESTful API for task and project management. This is your final capstone project that demonstrates ALL concepts learned in Path 1.

### What You'll Build

A comprehensive task management API similar to Trello/Asana with:
- User authentication and authorization
- Projects with multiple tasks
- Task assignments to users
- Comments on tasks
- Tags and categories
- Advanced search and filtering
- Real-time notifications (optional)

---

## 📋 Complete Requirements

### 1. Core Entities

```csharp
// User
- Id (Guid)
- Username (string, unique)
- Email (string, unique)
- PasswordHash (string)
- FirstName (string)
- LastName (string)
- Role (enum: Admin, Manager, Member)
- CreatedAt (DateTime)

// Project
- Id (Guid)
- Name (string)
- Description (string)
- OwnerId (Guid) -> User
- CreatedAt (DateTime)
- DueDate (DateTime?)
- Status (enum: Active, Completed, Archived)

// Task
- Id (Guid)
- Title (string)
- Description (string)
- ProjectId (Guid) -> Project
- AssignedToId (Guid?) -> User
- CreatedById (Guid) -> User
- Priority (enum: Low, Medium, High, Critical)
- Status (enum: Todo, InProgress, Review, Done)
- DueDate (DateTime?)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)

// Comment
- Id (Guid)
- TaskId (Guid) -> Task
- UserId (Guid) -> User
- Content (string)
- CreatedAt (DateTime)

// Tag
- Id (Guid)
- Name (string, unique)
- Color (string)

// TaskTag (many-to-many)
- TaskId (Guid)
- TagId (Guid)
```

### 2. API Endpoints

#### Authentication Endpoints
```
POST   /api/auth/register          # Register new user
POST   /api/auth/login             # Login (returns JWT)
POST   /api/auth/refresh           # Refresh token
POST   /api/auth/logout            # Logout
GET    /api/auth/me                # Get current user
```

#### User Endpoints
```
GET    /api/users                  # List users (paginated)
GET    /api/users/{id}             # Get user by ID
PUT    /api/users/{id}             # Update user
DELETE /api/users/{id}             # Delete user (Admin only)
GET    /api/users/{id}/tasks       # Get user's assigned tasks
```

#### Project Endpoints
```
GET    /api/projects               # List projects (paginated, filtered)
POST   /api/projects               # Create project
GET    /api/projects/{id}          # Get project by ID
PUT    /api/projects/{id}          # Update project
DELETE /api/projects/{id}          # Delete project
GET    /api/projects/{id}/tasks    # Get project tasks
GET    /api/projects/{id}/members  # Get project members
POST   /api/projects/{id}/members  # Add member to project
DELETE /api/projects/{id}/members/{userId}  # Remove member
```

#### Task Endpoints
```
GET    /api/tasks                  # List tasks (paginated, filtered, sorted)
POST   /api/tasks                  # Create task
GET    /api/tasks/{id}             # Get task by ID
PUT    /api/tasks/{id}             # Update task
DELETE /api/tasks/{id}             # Delete task
PUT    /api/tasks/{id}/assign      # Assign task to user
PUT    /api/tasks/{id}/status      # Update task status
GET    /api/tasks/{id}/comments    # Get task comments
POST   /api/tasks/{id}/comments    # Add comment
PUT    /api/tasks/{id}/tags        # Update task tags
GET    /api/tasks/search           # Advanced search
```

#### Tag Endpoints
```
GET    /api/tags                   # List all tags
POST   /api/tags                   # Create tag
GET    /api/tags/{id}              # Get tag
PUT    /api/tags/{id}              # Update tag
DELETE /api/tags/{id}              # Delete tag
```

#### Statistics Endpoints
```
GET    /api/stats/overview         # Dashboard stats
GET    /api/stats/user/{id}        # User statistics
GET    /api/stats/project/{id}     # Project statistics
```

### 3. Query Parameters

**Filtering** (tasks endpoint example):
```
GET /api/tasks?status=InProgress&priority=High&assignedTo=<userId>
GET /api/tasks?projectId=<projectId>&dueBefore=2024-12-31
GET /api/tasks?tags=urgent,bug
```

**Pagination**:
```
GET /api/tasks?page=1&pageSize=20
```

**Sorting**:
```
GET /api/tasks?sortBy=dueDate&sortOrder=asc
GET /api/tasks?sortBy=priority&sortOrder=desc
```

**Search**:
```
GET /api/tasks/search?q=bug+fix&in=title,description
```

### 4. Request/Response Examples

**POST /api/auth/register**
```json
Request:
{
  "username": "john_doe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}

Response: 201 Created
{
  "id": "guid",
  "username": "john_doe",
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "role": "Member",
  "createdAt": "2024-01-15T10:30:00Z"
}
```

**POST /api/auth/login**
```json
Request:
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}

Response: 200 OK
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "refresh_token_here",
  "expiresAt": "2024-01-15T11:30:00Z",
  "user": {
    "id": "guid",
    "username": "john_doe",
    "email": "john@example.com",
    "role": "Member"
  }
}
```

**POST /api/tasks**
```json
Request:
{
  "title": "Fix login bug",
  "description": "Users cannot log in with special characters",
  "projectId": "project-guid",
  "assignedToId": "user-guid",
  "priority": "High",
  "dueDate": "2024-01-20T17:00:00Z",
  "tags": ["bug", "urgent"]
}

Response: 201 Created
{
  "id": "task-guid",
  "title": "Fix login bug",
  "description": "Users cannot log in with special characters",
  "projectId": "project-guid",
  "assignedTo": {
    "id": "user-guid",
    "username": "john_doe",
    "email": "john@example.com"
  },
  "createdBy": {
    "id": "creator-guid",
    "username": "manager",
    "email": "manager@example.com"
  },
  "priority": "High",
  "status": "Todo",
  "dueDate": "2024-01-20T17:00:00Z",
  "tags": [
    { "id": "tag1", "name": "bug", "color": "#ff0000" },
    { "id": "tag2", "name": "urgent", "color": "#ff9900" }
  ],
  "createdAt": "2024-01-15T10:45:00Z",
  "updatedAt": "2024-01-15T10:45:00Z"
}
```

**GET /api/tasks?status=InProgress&page=1&pageSize=10**
```json
Response: 200 OK
{
  "items": [
    {
      "id": "task1",
      "title": "Implement user authentication",
      "status": "InProgress",
      "priority": "High",
      "assignedTo": { "id": "user1", "username": "dev1" },
      "dueDate": "2024-01-25T17:00:00Z"
    },
    // ... more tasks
  ],
  "pagination": {
    "page": 1,
    "pageSize": 10,
    "totalItems": 45,
    "totalPages": 5,
    "hasNextPage": true,
    "hasPreviousPage": false
  }
}
```

---

## 🏗️ Project Structure

```
TaskManagementApi/
├── src/
│   ├── TaskManagementApi.Api/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── UsersController.cs
│   │   │   ├── ProjectsController.cs
│   │   │   ├── TasksController.cs
│   │   │   ├── TagsController.cs
│   │   │   └── StatsController.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   │   └── RequestLoggingMiddleware.cs
│   │   ├── Filters/
│   │   │   └── ValidationFilter.cs
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   └── ApplicationBuilderExtensions.cs
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Program.cs
│   │
│   ├── TaskManagementApi.Core/
│   │   ├── Entities/
│   │   │   ├── User.cs
│   │   │   ├── Project.cs
│   │   │   ├── Task.cs
│   │   │   ├── Comment.cs
│   │   │   ├── Tag.cs
│   │   │   └── TaskTag.cs
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   ├── IProjectRepository.cs
│   │   │   │   ├── ITaskRepository.cs
│   │   │   │   └── ITagRepository.cs
│   │   │   └── Services/
│   │   │       ├── IAuthService.cs
│   │   │       ├── ITokenService.cs
│   │   │       ├── ITaskService.cs
│   │   │       └── INotificationService.cs
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── TokenService.cs
│   │   │   ├── TaskService.cs
│   │   │   └── NotificationService.cs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   │   ├── RegisterRequest.cs
│   │   │   │   ├── LoginRequest.cs
│   │   │   │   └── AuthResponse.cs
│   │   │   ├── Tasks/
│   │   │   │   ├── CreateTaskRequest.cs
│   │   │   │   ├── UpdateTaskRequest.cs
│   │   │   │   └── TaskResponse.cs
│   │   │   └── Common/
│   │   │       └── PaginatedResponse.cs
│   │   ├── Validators/
│   │   │   ├── RegisterRequestValidator.cs
│   │   │   └── CreateTaskRequestValidator.cs
│   │   ├── Exceptions/
│   │   │   ├── NotFoundException.cs
│   │   │   ├── UnauthorizedException.cs
│   │   │   └── ValidationException.cs
│   │   └── Enums/
│   │       ├── Role.cs
│   │       ├── TaskStatus.cs
│   │       └── Priority.cs
│   │
│   └── TaskManagementApi.Infrastructure/
│       ├── Data/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/
│       │   │   ├── UserConfiguration.cs
│       │   │   ├── ProjectConfiguration.cs
│       │   │   ├── TaskConfiguration.cs
│       │   │   └── TagConfiguration.cs
│       │   └── Migrations/
│       ├── Repositories/
│       │   ├── GenericRepository.cs
│       │   ├── UserRepository.cs
│       │   ├── ProjectRepository.cs
│       │   ├── TaskRepository.cs
│       │   └── TagRepository.cs
│       └── Services/
│           └── EmailService.cs
│
├── tests/
│   ├── TaskManagementApi.UnitTests/
│   │   ├── Services/
│   │   │   ├── AuthServiceTests.cs
│   │   │   └── TaskServiceTests.cs
│   │   ├── Repositories/
│   │   │   └── TaskRepositoryTests.cs
│   │   └── Controllers/
│   │       └── TasksControllerTests.cs
│   │
│   └── TaskManagementApi.IntegrationTests/
│       ├── Controllers/
│       │   ├── AuthControllerTests.cs
│       │   └── TasksControllerTests.cs
│       └── TestFixtures/
│           └── WebApplicationFactory.cs
│
├── docker-compose.yml
├── Dockerfile
├── README.md
└── .gitignore
```

---

## 🚀 Implementation Steps

### Phase 1: Project Setup (Day 1-2)

1. **Create Solution**:
```bash
dotnet new sln -n TaskManagementApi
dotnet new webapi -n TaskManagementApi.Api
dotnet new classlib -n TaskManagementApi.Core
dotnet new classlib -n TaskManagementApi.Infrastructure
dotnet new nunit -n TaskManagementApi.UnitTests
dotnet new nunit -n TaskManagementApi.IntegrationTests

dotnet sln add **/*.csproj
```

2. **Add Packages**:
```bash
# Api project
dotnet add TaskManagementApi.Api package Microsoft.EntityFrameworkCore.Design
dotnet add TaskManagementApi.Api package Swashbuckle.AspNetCore
dotnet add TaskManagementApi.Api package Serilog.AspNetCore
dotnet add TaskManagementApi.Api package Microsoft.AspNetCore.Authentication.JwtBearer

# Infrastructure
dotnet add TaskManagementApi.Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add TaskManagementApi.Infrastructure package Microsoft.EntityFrameworkCore.Tools

# Core
dotnet add TaskManagementApi.Core package FluentValidation
dotnet add TaskManagementApi.Core package AutoMapper
```

3. **Configure Project References**:
```bash
dotnet add TaskManagementApi.Api reference TaskManagementApi.Core TaskManagementApi.Infrastructure
dotnet add TaskManagementApi.Infrastructure reference TaskManagementApi.Core
dotnet add TaskManagementApi.UnitTests reference TaskManagementApi.Core
```

### Phase 2: Data Layer (Day 3-5)

**TODO**: Implement all entities, DbContext, configurations, and repositories

### Phase 3: Business Logic (Day 6-10)

**TODO**: Implement services, DTOs, validators

### Phase 4: API Layer (Day 11-15)

**TODO**: Implement controllers, middleware, filters

### Phase 5: Authentication (Day 16-18)

**TODO**: JWT authentication, authorization policies

### Phase 6: Testing (Day 19-22)

**TODO**: Unit tests, integration tests

### Phase 7: Polish & Deploy (Day 23-28)

**TODO**: Documentation, Docker, final testing

---

## ✅ Evaluation Criteria

| Category | Weight | Min Score |
|----------|--------|-----------|
| **Functionality** | 30% | 24/30 |
| **Code Quality** | 25% | 20/25 |
| **Architecture** | 15% | 12/15 |
| **Testing** | 20% | 16/20 |
| **Documentation** | 10% | 8/10 |

**Overall Pass**: 75% (75/100)

---

## 📚 Resources

- ASP.NET Core Docs: https://learn.microsoft.com/en-us/aspnet/core/
- EF Core: https://learn.microsoft.com/en-us/ef/core/
- JWT: https://jwt.io/introduction
- Clean Architecture: https://github.com/jasontaylordev/CleanArchitecture

---

*Template Version: 1.0*
*Last Updated: 2025-12-02*
