# Month 6 Comprehensive Assessment - Capstone Preparation

**Month**: 6 (Weeks 21-24) | **Duration**: 90 min | **Pass**: 80% (24/30) | **Points**: 30

## Section 1: Multiple Choice (15 questions, 0.5 pts each = 7.5 pts)

1. Observer pattern relationship:
   - a) One-to-one | b) One-to-many | c) Many-to-one | d) Many-to-many

2. IObserver<T> methods:
   - a) Subscribe/Unsubscribe | b) OnNext/OnError/OnCompleted | c) Notify/Update | d) Publish/Subscribe

3. How to unsubscribe from IObservable<T>?
   - a) Call Unsubscribe() | b) Dispose the subscription | c) Set to null | d) No way

4. MVC stands for:
   - a) Model-View-Controller | b) Main-View-Class | c) Model-Virtual-Controller | d) Multiple-View-Control

5. ASP.NET Core routing order:
   - a) Attribute routes, then conventional | b) Conventional, then attribute | c) Random | d) Alphabetical

6. What's middleware in ASP.NET Core?
   - a) Database layer | b) Request/response pipeline component | c) Controller | d) View engine

7. Dependency injection in ASP.NET Core configured in:
   - a) Controller | b) View | c) Program.cs/Startup.cs | d) Web.config

8. What is DbContext?
   - a) Database connection string | b) Session with database (Unit of Work) | c) SQL query | d) Table

9. EF Core migrations purpose:
   - a) Move database | b) Version control for database schema | c) Backup | d) Query optimization

10. Eager loading in EF Core:
    - a) `.Load()` | b) `.Include()` | c) `.Lazy()` | d) `.Eager()`

11. Lazy loading requires:
    - a) Nothing | b) Virtual navigation properties | c) Static properties | d) Sealed classes

12. Authentication vs Authorization:
    - a) Same thing | b) Authentication = who, Authorization = what | c) Authorization = who, Authentication = what | d) No difference

13. JWT consists of:
    - a) Header.Payload | b) Header.Payload.Signature | c) Username.Password | d) Token only

14. Claims in ASP.NET Core represent:
    - a) Errors | b) User information/assertions | c) Database records | d) Routes

15. [Authorize] attribute does what?
    - a) Grants permission | b) Requires authentication | c) Logs user | d) Creates user

## Section 2: Short Answer (7 questions, 2 pts each = 14 pts)

16. Explain Observer pattern. How does it differ from the Pub/Sub pattern?

17. Explain the MVC pattern in ASP.NET Core. What's the responsibility of each component?

18. What's the middleware pipeline? Explain with 3 examples of middleware.

19. Explain DbContext lifecycle. Why is it typically registered as Scoped?

20. Compare eager loading, explicit loading, and lazy loading in EF Core. When to use each?

21. Explain how JWT authentication works. What's in the token?

22. What's the difference between claims-based and role-based authorization?

## Section 3: Code Implementation (4 questions, 2 pts each = 8 pts)

23. Implement a complete Observer pattern with unsubscribe:
```csharp
// Requirements:
// - StockTicker class (observable)
// - Maintains list of observers
// - NotifyObservers when price changes
// - Subscribe() returns IDisposable for unsubscribe
// - StockDisplay class (observer)
```

24. Create ASP.NET Core controller:
```csharp
// Requirements:
// - ProductsController with API endpoints
// - GET /api/products (return all)
// - GET /api/products/{id} (return one)
// - POST /api/products (create)
// - Inject IProductRepository via DI
```

25. Define EF Core entity relationship:
```csharp
// Requirements:
// - Order entity (Id, OrderDate, CustomerId)
// - OrderItem entity (Id, OrderId, ProductId, Quantity)
// - One-to-many relationship: Order -> OrderItems
// - Configure using Fluent API in DbContext
```

26. Implement JWT authentication:
```csharp
// Requirements:
// - Generate JWT token for user (username, email claims)
// - Use HS256 algorithm
// - Set expiration to 1 hour
// - Include user roles in claims
```

## Answer Key

**MC**: 1.b | 2.b | 3.b | 4.a | 5.a | 6.b | 7.c | 8.b | 9.b | 10.b | 11.b | 12.b | 13.b | 14.b | 15.b

### Short Answer

**16. Observer Pattern** (2 pts):
**Observer Pattern**:
- Subject maintains list of observers
- Direct coupling: Subject knows about observers
- Synchronous by default
- Example: `subject.Attach(observer)`

**Pub/Sub Pattern**:
- Publisher and subscribers don't know each other
- Message broker/event bus in between
- Asynchronous by default
- Example: Event-driven architecture

**Key Difference**:
- Observer: Direct relationship, object-to-object
- Pub/Sub: Decoupled, message-based communication

**C# Implementation**:
```csharp
// Observer: Direct
subject.Attach(observer);

// Pub/Sub: Via broker
eventBus.Subscribe<OrderCreated>(handler);
eventBus.Publish(new OrderCreated());
```

**17. MVC Pattern** (2 pts):
**Model**:
- Represents data and business logic
- Domain entities, validation rules
- Independent of presentation
- Example: `User`, `Product` classes

**View**:
- Presents data to user
- Razor pages, HTML templates
- No business logic
- Example: `Index.cshtml`

**Controller**:
- Handles HTTP requests
- Orchestrates Model and View
- Returns ActionResult
- Example:
```csharp
public class HomeController : Controller
{
    public IActionResult Index()
    {
        var model = GetData(); // Model
        return View(model);     // View
    }
}
```

**Flow**: Request → Controller → Model (logic) → View (render) → Response

**18. Middleware Pipeline** (2 pts):
**Definition**: Components that handle HTTP request/response

**Pipeline Order** (order matters!):
```
Request  → Middleware 1 → Middleware 2 → Middleware 3 → Endpoint
Response ← Middleware 1 ← Middleware 2 ← Middleware 3 ←
```

**Examples**:
1. **Exception Handler**:
```csharp
app.UseExceptionHandler("/Error");
// Catches exceptions, returns error page
```

2. **Authentication**:
```csharp
app.UseAuthentication();
// Reads JWT token, sets HttpContext.User
```

3. **Authorization**:
```csharp
app.UseAuthorization();
// Checks if user has permission
```

**Custom Middleware**:
```csharp
app.Use(async (context, next) =>
{
    // Before
    await next.Invoke();
    // After
});
```

**19. DbContext Lifecycle** (2 pts):
**DbContext responsibilities**:
- Tracks entity changes
- Manages database connection
- Implements Unit of Work pattern
- Querying and saving

**Why Scoped?**:
- **One per request**: Each HTTP request gets own DbContext
- **Automatic disposal**: Disposed at end of request
- **Change tracking**: Tracks entities within single operation
- **Thread safety**: Not thread-safe, scoped prevents sharing

**Lifetimes comparison**:
```csharp
// ❌ Singleton - WRONG! (not thread-safe)
services.AddSingleton<AppDbContext>();

// ✅ Scoped - CORRECT!
services.AddScoped<AppDbContext>();

// ❌ Transient - WASTEFUL (too many instances)
services.AddTransient<AppDbContext>();
```

**20. EF Core Loading Strategies** (2 pts):

**Eager Loading** (`.Include()`):
- Loads related data in single query
- Use: Always need related data
- Pro: One query, no N+1 problem
- Con: May load unnecessary data
```csharp
var orders = context.Orders
    .Include(o => o.Customer)
    .Include(o => o.OrderItems)
    .ToList();
```

**Explicit Loading** (`.Load()`):
- Load related data on demand
- Use: Sometimes need related data
- Pro: Control when to load
- Con: Multiple queries
```csharp
var order = context.Orders.Find(1);
context.Entry(order).Collection(o => o.OrderItems).Load();
```

**Lazy Loading** (`virtual` properties):
- Loads related data when accessed
- Use: Rarely need related data
- Pro: Automatic, simple
- Con: N+1 queries, requires proxies
```csharp
public class Order
{
    public virtual Customer Customer { get; set; } // Lazy loaded
}
```

**21. JWT Authentication** (2 pts):
**How JWT Works**:
1. User logs in with credentials
2. Server validates, generates JWT
3. Client stores JWT (localStorage, cookie)
4. Client sends JWT in Authorization header
5. Server validates JWT signature
6. Server extracts claims, authorizes user

**JWT Structure** (Header.Payload.Signature):
```
eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.
eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4iLCJpYXQiOjE1MTYyMzkwMjJ9.
SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c
```

**Decoded**:
```json
// Header
{"alg": "HS256", "typ": "JWT"}

// Payload (claims)
{
  "sub": "1234567890",
  "name": "John Doe",
  "email": "john@example.com",
  "role": "Admin",
  "exp": 1516239022
}

// Signature (ensures integrity)
HMACSHA256(base64(header) + "." + base64(payload), secret)
```

**22. Claims vs Roles** (2 pts):

**Role-Based**:
- User has roles: Admin, User, Manager
- Check role: `[Authorize(Roles = "Admin")]`
- Simple, traditional
- Limited flexibility

**Claims-Based**:
- User has claims (key-value pairs)
- Examples: `{"role": "Admin"}`, `{"department": "IT"}`, `{"clearance": "Secret"}`
- Check claim: `[Authorize(Policy = "RequireAdmin")]`
- Flexible, extensible
- Modern approach

**Comparison**:
```csharp
// Role-based
[Authorize(Roles = "Admin,Manager")]
public IActionResult Delete() { }

// Claims-based
[Authorize(Policy = "CanDeleteUsers")]
public IActionResult Delete() { }

// Policy definition
services.AddAuthorization(options =>
{
    options.AddPolicy("CanDeleteUsers", policy =>
        policy.RequireClaim("permission", "delete:users"));
});
```

Claims are more granular and flexible!

### Code Implementation

**23. Observer with Unsubscribe** (2 pts):
```csharp
public interface IObserver<T>
{
    void OnNext(T value);
}

public class Subscription : IDisposable
{
    private readonly Action _unsubscribe;

    public Subscription(Action unsubscribe)
    {
        _unsubscribe = unsubscribe;
    }

    public void Dispose()
    {
        _unsubscribe();
    }
}

public class StockTicker
{
    private readonly List<IObserver<decimal>> _observers = new();
    private decimal _price;

    public decimal Price
    {
        get => _price;
        set
        {
            _price = value;
            NotifyObservers();
        }
    }

    public IDisposable Subscribe(IObserver<decimal> observer)
    {
        _observers.Add(observer);
        return new Subscription(() => _observers.Remove(observer));
    }

    private void NotifyObservers()
    {
        foreach (var observer in _observers)
        {
            observer.OnNext(_price);
        }
    }
}

public class StockDisplay : IObserver<decimal>
{
    private readonly string _name;

    public StockDisplay(string name)
    {
        _name = name;
    }

    public void OnNext(decimal price)
    {
        Console.WriteLine($"{_name}: Stock price is ${price}");
    }
}

// Usage
var ticker = new StockTicker();
var display1 = new StockDisplay("Display 1");
var display2 = new StockDisplay("Display 2");

var subscription1 = ticker.Subscribe(display1);
var subscription2 = ticker.Subscribe(display2);

ticker.Price = 100.50m; // Both notified

subscription1.Dispose(); // Unsubscribe display1

ticker.Price = 105.75m; // Only display2 notified
```

**24. ASP.NET Core Controller** (2 pts):
```csharp
public interface IProductRepository
{
    IEnumerable<Product> GetAll();
    Product GetById(int id);
    void Add(Product product);
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repository;

    public ProductsController(IProductRepository repository)
    {
        _repository = repository;
    }

    // GET: api/products
    [HttpGet]
    public ActionResult<IEnumerable<Product>> GetAll()
    {
        var products = _repository.GetAll();
        return Ok(products);
    }

    // GET: api/products/5
    [HttpGet("{id}")]
    public ActionResult<Product> GetById(int id)
    {
        var product = _repository.GetById(id);
        if (product == null)
            return NotFound();

        return Ok(product);
    }

    // POST: api/products
    [HttpPost]
    public ActionResult<Product> Create([FromBody] Product product)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        _repository.Add(product);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }
}

// DI registration in Program.cs
builder.Services.AddScoped<IProductRepository, ProductRepository>();
```

**25. EF Core Relationship** (2 pts):
```csharp
public class Order
{
    public int Id { get; set; }
    public DateTime OrderDate { get; set; }
    public int CustomerId { get; set; }

    // Navigation property
    public ICollection<OrderItem> OrderItems { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }

    // Navigation property
    public Order Order { get; set; }
}

public class AppDbContext : DbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure one-to-many relationship using Fluent API
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Alternative: using attributes
        // [ForeignKey("OrderId")]
        // public Order Order { get; set; }
    }
}

// Usage
var order = new Order
{
    OrderDate = DateTime.Now,
    CustomerId = 1,
    OrderItems = new List<OrderItem>
    {
        new OrderItem { ProductId = 1, Quantity = 2 },
        new OrderItem { ProductId = 2, Quantity = 1 }
    }
};

context.Orders.Add(order);
context.SaveChanges();
```

**26. JWT Generation** (2 pts):
```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

public class JwtService
{
    private readonly string _secretKey = "YourSuperSecretKeyThatIsAtLeast32Characters";
    private readonly string _issuer = "YourApp";
    private readonly string _audience = "YourAppUsers";

    public string GenerateToken(string username, string email, string[] roles)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create claims
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Create token
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

// Usage
var jwtService = new JwtService();
var token = jwtService.GenerateToken(
    username: "john.doe",
    email: "john@example.com",
    roles: new[] { "Admin", "User" }
);

Console.WriteLine(token);
// eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqb2huLm...

// In ASP.NET Core Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "YourApp",
            ValidAudience = "YourAppUsers",
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("YourSuperSecretKeyThatIsAtLeast32Characters"))
        };
    });
```

## Grading Rubric

| Section | Max Points | Criteria |
|---------|-----------|----------|
| Multiple Choice | 7.5 | 0.5 per correct answer |
| Short Answer (each) | 2 × 7 = 14 | Full: Complete + examples. Partial: 1.0-1.5. Wrong: 0 |
| Code Implementation (each) | 2 × 4 = 8 | Full: Working implementation. Partial: 1.0-1.5. Wrong: 0 |
| **Total** | **30** | **Pass: 24 points (80%)** |

---

## Study Resources

**Week 21 - Observer Pattern**:
- `samples/99-Exercises/DesignPatterns/02-Observer/`
- IObservable<T>/IObserver<T> implementation

**Week 22 - ASP.NET Core**:
- MVC pattern, routing, middleware
- Dependency injection in ASP.NET Core

**Week 23 - Entity Framework Core**:
- DbContext, migrations
- Relationships: one-to-many, many-to-many
- Loading strategies

**Week 24 - Authentication**:
- JWT structure and validation
- Claims-based authorization
- ASP.NET Core Identity

---

## Next Steps

**If you passed (≥24 pts)**: Ready for Path 1 Final Exam!

**If you didn't pass (<24 pts)**: Review weak areas:
- Score 0-10: Review all Month 6 materials
- Score 11-18: Focus on ASP.NET Core and EF Core
- Score 19-23: Practice authentication implementation

---

## Capstone Project Readiness

After passing Month 6, you're ready to build:
- ✅ Full-stack web application
- ✅ RESTful API with proper architecture
- ✅ Database with EF Core migrations
- ✅ Authentication and authorization
- ✅ SOLID principles applied
- ✅ Design patterns implemented

**Suggested Capstone Projects**:
1. Task Management System (Trello clone)
2. E-commerce Platform
3. Blog Platform with comments
4. Social Media Dashboard
5. Inventory Management System

---

*Assessment Version: 1.0*
*Last Updated: 2025-12-02*
