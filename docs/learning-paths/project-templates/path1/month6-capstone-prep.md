# Month 6: Capstone Preparation Guide

**Difficulty**: ⭐⭐⭐⭐☆ (Advanced)
**Duration**: Weeks 21-24 (4 weeks)
**Prerequisites**: Completed Months 1-5 of Path 1

---

## 🎯 Overview

Month 6 focuses on final preparation and completing your capstone project. Unlike previous months, this period is dedicated to building a production-ready application that demonstrates ALL concepts learned in Path 1.

---

## 📅 Week-by-Week Breakdown

### Week 21: Observer Pattern & Event-Driven Architecture

**Study Materials**:
- Review `samples/99-Exercises/DesignPatterns/02-Observer/`
- Study event-driven patterns
- Learn about IObservable<T>/IObserver<T>

**Practice**:
- Implement observer pattern in your capstone
- Create event-driven components
- Build notification system

### Week 22: ASP.NET Core Basics

**Topics**:
- MVC pattern
- Routing
- Dependency Injection in ASP.NET Core
- Middleware pipeline

**Preparation for Capstone**:
- Set up ASP.NET Core Web API project
- Configure DI container
- Create initial controller structure

### Week 23: Entity Framework Core

**Topics**:
- DbContext and migrations
- Relationships (one-to-many, many-to-many)
- Loading strategies (eager, lazy, explicit)
- LINQ to Entities

**Preparation for Capstone**:
- Design database schema
- Create entity models
- Set up EF Core
- Create initial migration

### Week 24: Authentication & Authorization

**Topics**:
- JWT authentication
- Claims-based authorization
- ASP.NET Core Identity
- Security best practices

**Preparation for Capstone**:
- Implement JWT authentication
- Add authorization policies
- Secure API endpoints

---

## 🚀 Capstone Project Options

Choose ONE of the following projects:

### Option 1: Task Management Web API ⭐ (Recommended)

Full REST API for task/project management with authentication, CRUD operations, and advanced features.

**See**: `final-task-management-api.md` for full template

### Option 2: E-Commerce Product Catalog API

API for managing products, categories, orders, and customers with search and filtering.

### Option 3: Blog Platform API

API for blogging platform with posts, comments, tags, and user management.

---

## 📋 Capstone Requirements (All Projects)

### Technical Requirements

1. **ASP.NET Core Web API** (.NET 8)
2. **Entity Framework Core** with SQL Server/SQLite
3. **JWT Authentication**
4. **Authorization** with policies
5. **Design Patterns**: Minimum 3 (Builder, Observer, Decorator, Repository)
6. **SOLID Principles**: Demonstrated throughout
7. **Unit Tests**: 90%+ coverage
8. **Integration Tests**
9. **API Documentation**: Swagger/OpenAPI
10. **Performance**: 1000+ requests/second

### Functional Requirements

1. **CRUD Operations** for main entities
2. **Search & Filtering** with LINQ
3. **Pagination** for list endpoints
4. **Sorting** by multiple fields
5. **Validation** with proper error messages
6. **Logging** (Serilog recommended)
7. **Error Handling** with global exception handler
8. **Health Checks** endpoints

### Code Quality Requirements

1. **Clean Architecture** (or layered architecture)
2. **Separation of Concerns**
3. **Dependency Injection** used throughout
4. **Repository Pattern** for data access
5. **DTOs** for API contracts
6. **AutoMapper** for object mapping
7. **FluentValidation** for validation rules
8. **XML Documentation** for controllers

---

## 🏗️ Recommended Project Structure

```
YourCapstoneApi/
├── YourCapstoneApi.Api/              # Web API project
│   ├── Controllers/
│   ├── Middleware/
│   ├── Filters/
│   ├── Extensions/
│   └── Program.cs
├── YourCapstoneApi.Core/             # Domain/Business logic
│   ├── Entities/
│   ├── Interfaces/
│   ├── Services/
│   ├── DTOs/
│   └── Exceptions/
├── YourCapstoneApi.Infrastructure/   # Data access & external
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── Configurations/
│   ├── Repositories/
│   └── Services/
├── YourCapstoneApi.Tests/            # Unit tests
│   ├── Controllers/
│   ├── Services/
│   └── Repositories/
└── YourCapstoneApi.IntegrationTests/ # Integration tests
```

---

## 📝 Week-by-Week Checklist

### Week 21 Checklist

- [ ] Project chosen and requirements reviewed
- [ ] Architecture designed (diagram created)
- [ ] Database schema designed
- [ ] Initial project structure created
- [ ] Git repository initialized
- [ ] README started

### Week 22 Checklist

- [ ] ASP.NET Core project set up
- [ ] Controllers created (stubs)
- [ ] Routing configured
- [ ] Dependency injection configured
- [ ] Swagger documentation set up
- [ ] First endpoint working

### Week 23 Checklist

- [ ] Entity models created
- [ ] DbContext configured
- [ ] Initial migration created
- [ ] Repository pattern implemented
- [ ] CRUD operations working
- [ ] LINQ queries implemented

### Week 24 Checklist

- [ ] JWT authentication implemented
- [ ] Authorization policies created
- [ ] All endpoints secured
- [ ] Unit tests written (90%+ coverage)
- [ ] Integration tests written
- [ ] Performance tested
- [ ] Documentation complete

---

## ✅ Final Checklist Before Submission

### Functionality
- [ ] All CRUD operations work
- [ ] Search and filtering work
- [ ] Pagination works
- [ ] Sorting works
- [ ] Authentication works
- [ ] Authorization works
- [ ] Validation works with proper errors

### Code Quality
- [ ] Design patterns used (3+)
- [ ] SOLID principles applied
- [ ] No code smells
- [ ] Clean architecture followed
- [ ] Proper separation of concerns

### Testing
- [ ] Unit tests: 90%+ coverage
- [ ] Integration tests written
- [ ] All tests pass
- [ ] Performance tested (1000+ req/sec)

### Documentation
- [ ] README complete with setup instructions
- [ ] API documented with Swagger
- [ ] Architecture diagram included
- [ ] Database schema documented
- [ ] XML comments on public methods

### Deployment
- [ ] Docker support (optional but recommended)
- [ ] Environment configuration
- [ ] Connection strings secured
- [ ] Logging configured

---

## 🎯 Success Criteria

Your capstone will be evaluated on:

| Criteria | Weight | Requirements |
|----------|--------|--------------|
| **Functionality** | 30% | All features working |
| **Code Quality** | 25% | Clean, SOLID, patterns |
| **Testing** | 20% | 90%+ coverage, tests pass |
| **Architecture** | 15% | Well-structured, scalable |
| **Documentation** | 10% | Complete, clear |

**Minimum Pass**: 75% overall

---

## 💡 Tips for Success

1. **Start Early**: Don't wait until week 24
2. **Commit Often**: Use Git from day 1
3. **Test as You Go**: Don't save testing for the end
4. **Ask for Help**: Use community resources
5. **Keep It Simple**: Focus on requirements first
6. **Refactor**: Improve code quality iteratively
7. **Document**: Write README and comments as you code
8. **Performance**: Test with realistic data volumes

---

## 📚 Resources

### ASP.NET Core
- https://learn.microsoft.com/en-us/aspnet/core/
- https://learn.microsoft.com/en-us/aspnet/core/web-api/

### Entity Framework Core
- https://learn.microsoft.com/en-us/ef/core/
- https://learn.microsoft.com/en-us/ef/core/modeling/relationships

### Authentication
- https://learn.microsoft.com/en-us/aspnet/core/security/authentication/
- https://jwt.io/

### Testing
- https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests
- https://docs.nunit.org/

### Best Practices
- Clean Architecture: https://github.com/jasontaylordev/CleanArchitecture
- REST API Best Practices

---

## 🎓 After Completion

Once your capstone passes:

1. **Add to Portfolio**: Showcase on GitHub
2. **Write Blog Post**: Document your journey
3. **Take Final Exam**: Path 1 certification exam
4. **Update Resume**: Add skills and project
5. **Choose Next Path**: Path 2 or job search

---

**Good luck with your capstone project!** This is your chance to demonstrate everything you've learned and create something you're proud to show employers.

---

*Template Version: 1.0*
*Last Updated: 2025-12-02*
