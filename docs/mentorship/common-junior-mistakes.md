# Common Junior Developer Mistakes (And How to Fix Them)

**Purpose:** Help mentors identify patterns and teach effectively  
**Audience:** Senior/Mid-level engineers mentoring juniors  
**Philosophy:** Every mistake is a teaching opportunity

---

## How to Use This Guide

1. **Identify the pattern** - Don't just fix the specific bug, recognize the underlying mistake
2. **Teach the concept** - Explain WHY it's a problem, not just WHAT is wrong
3. **Show the fix** - Give them the solution pattern they can apply elsewhere
4. **Prevent recurrence** - Help them catch this mistake themselves next time

**Remember:** You made these mistakes too. Be empathetic.

---

## Category 1: Object-Oriented Design Mistakes

### Mistake #1: Using Type Checking Instead of Polymorphism

**What You'll See:**
```csharp
public void ProcessPayment(string paymentType)
{
    if (paymentType == "CreditCard")
    {
        // 20 lines of credit card logic
    }
    else if (paymentType == "PayPal")
    {
        // 20 lines of PayPal logic
    }
    else if (paymentType == "Bitcoin")
    {
        // 20 lines of Bitcoin logic
    }
}
```

**Why They Do This:**
- Most intuitive approach for beginners
- Haven't experienced the pain of maintaining it
- Don't know polymorphism yet

**The Problem:**
- Adding Apple Pay = modify this method + 4 other methods
- Typos aren't caught by compiler ("paypal" vs "PayPal")
- Logic is scattered (credit card processing in 5 places)
- Testing requires mocking all types in every test

**How to Fix:**
```csharp
public interface IPaymentProcessor
{
    void Process(decimal amount);
}

public class CreditCardProcessor : IPaymentProcessor { ... }
public class PayPalProcessor : IPaymentProcessor { ... }

public void ProcessPayment(IPaymentProcessor processor, decimal amount)
{
    processor.Process(amount); // That's it!
}
```

**How to Teach:**
1. Show them the pain: "What happens when we add 10 more payment types?"
2. Introduce polymorphism: "What if each type knew how to process itself?"
3. Compare side-by-side: "Which would you rather maintain?"
4. Have them refactor a similar example

**Prevention:**
- Code review: Flag any `if (type == "string")` patterns
- Ask: "Will this grow? If yes, consider polymorphism."

---

### Mistake #2: God Classes (Too Many Responsibilities)

**What You'll See:**
```csharp
public class UserService
{
    public void CreateUser() { }
    public void UpdateUser() { }
    public void DeleteUser() { }
    public void SendWelcomeEmail() { } // ❌
    public void LogAction() { } // ❌
    public void GenerateReport() { } // ❌
    public void ProcessPayment() { } // ❌
    public void ValidateAddress() { } // ❌
}
```

**Why They Do This:**
- "UserService handles everything about users, right?"
- Don't understand Single Responsibility Principle
- Didn't think about testing/maintenance

**The Problem:**
- Hard to test (need to mock email, logging, reports to test user creation)
- Hard to reuse (can't use email sender in OrderService)
- Changes to reports affect user management (shouldn't!)
- Class grows to 2000 lines

**How to Fix:**
```csharp
public class UserService
{
    private readonly IEmailService _email;
    private readonly ILogger _logger;
    private readonly IPaymentService _payment;

    public void CreateUser()
    {
        // User creation logic only
        _email.SendWelcomeEmail(); // Delegate to email service
        _logger.Log("User created"); // Delegate to logger
    }
}
```

**How to Teach:**
1. Ask: "What is the single responsibility of UserService?"
2. List methods: "Which of these are DIRECTLY about managing user data?"
3. Extract: "Let's move email logic to EmailService"
4. Show benefits: "Now we can test user creation without mocking email!"

**Prevention:**
- Rule of thumb: If class has >10 methods, probably doing too much
- Ask: "Does this method belong here?" during code review

---

### Mistake #3: Exposing Implementation Details

**What You'll See:**
```csharp
public class OrderService
{
    public SqlConnection Connection { get; set; } // ❌ Exposes SqlConnection!
    public List<Order> OrderCache { get; set; } // ❌ Exposes cache!
    
    public void SaveOrder(Order order)
    {
        Connection.Open(); // ❌ Caller must manage connection!
        // ...
    }
}
```

**Why They Do This:**
- Don't understand encapsulation
- "I need to access this from tests"
- Haven't learned dependency injection

**The Problem:**
- Callers depend on implementation (can't switch to Postgres without breaking callers)
- Can't change cache implementation (is it Redis? Memory? Callers know!)
- Testing requires setting up SqlConnection

**How to Fix:**
```csharp
public class OrderService
{
    private readonly IDbContext _db; // ✅ Private, abstracted
    private readonly ICache _cache; // ✅ Private, abstracted
    
    public async Task SaveOrderAsync(Order order)
    {
        // Connection management is hidden
        await _db.Orders.AddAsync(order);
        await _db.SaveChangesAsync();
    }
}
```

**How to Teach:**
1. Principle: "Hide how it works, expose what it does"
2. Ask: "If we switch from SQL Server to Postgres, what breaks?"
3. Refactor: "Let's hide SqlConnection behind IDbContext"
4. Show: "Now callers don't know or care about database type"

**Prevention:**
- Public properties should be business data only, not implementation details
- If it starts with `I` (interface), it's okay to expose

---

## Category 2: Database & Performance Mistakes

### Mistake #4: N+1 Query Problem

**What You'll See:**
```csharp
var orders = await _db.Orders.ToListAsync();
foreach (var order in orders)
{
    order.Customer = await _db.Customers.FindAsync(order.CustomerId); // N queries!
}
```

**Why They Do This:**
- Lazy loading is enabled (hidden queries)
- Don't see the problem until production (works fine with 10 orders in dev)
- Don't understand how ORMs work

**The Problem:**
- 1 query for orders + N queries for customers = 1,001 queries for 1,000 orders
- Response time: 200ms → 15 seconds
- Database connection pool exhausted

**How to Fix:**
```csharp
var orders = await _db.Orders
    .Include(o => o.Customer) // ✅ Single query with JOIN
    .ToListAsync();
```

**How to Teach:**
1. Enable SQL logging: Show them the 1,001 queries
2. Explain: "See how we're hitting database in a loop?"
3. Fix: "Include() tells EF to JOIN instead of lazy load"
4. Benchmark: "15s → 180ms. That's the impact."

**Resources to Share:**
- `samples/98-RealWorld-Problems/03-N-Plus-One-Problem/`

**Prevention:**
- Disable lazy loading in production
- Review any loop that accesses navigation properties

---

### Mistake #5: Loading Entire Table into Memory

**What You'll See:**
```csharp
var allUsers = await _db.Users.ToListAsync(); // ❌ Loads 1 million rows!
var activeUsers = allUsers.Where(u => u.IsActive).ToList(); // Filters in C#
```

**Why They Do This:**
- "I need to filter, so I load then filter"
- Don't understand LINQ translates to SQL
- Works in dev with 100 users, breaks in prod with 1M

**The Problem:**
- 1M rows × 200 bytes = 200 MB loaded into memory
- Query takes 30 seconds
- OutOfMemoryException in production

**How to Fix:**
```csharp
var activeUsers = await _db.Users
    .Where(u => u.IsActive) // ✅ Filters in database
    .ToListAsync();
```

**How to Teach:**
1. Show SQL: "Look at the query - no WHERE clause!"
2. Explain: "EF can't translate after ToListAsync(), so it loads everything"
3. Rule: "Filter BEFORE ToListAsync(), not after"
4. Show: "Now SQL has WHERE IsActive = 1. Database does the work."

**Prevention:**
- If you see `.ToListAsync()` followed by `.Where()`, flag it
- Rule: LINQ = SQL translation, stop as late as possible

---

### Mistake #6: Not Using Async/Await Correctly

**What You'll See:**
```csharp
// Pattern 1: Not awaiting
public async Task<User> GetUserAsync(int id)
{
    var user = _db.Users.FindAsync(id); // ❌ Not awaited!
    return user; // Compiler error: Task<User> != User
}

// Pattern 2: Unnecessary blocking
public async Task<User> GetUserAsync(int id)
{
    var user = await _db.Users.FindAsync(id);
    return user.Result; // ❌ .Result blocks!
}

// Pattern 3: Async void
public async void SaveUser(User user) // ❌ Should be Task, not void
{
    await _db.SaveChangesAsync();
}
```

**Why They Do This:**
- Async is confusing for beginners
- ".Result made the compile error go away!"
- Don't understand Task vs void

**The Problem:**
- Not awaiting: Method returns before work is done
- .Result: Blocks thread, can cause deadlock
- async void: Can't be awaited, exceptions aren't caught

**How to Fix:**
```csharp
// ✅ Correct pattern
public async Task<User> GetUserAsync(int id)
{
    var user = await _db.Users.FindAsync(id); // Await Task<User>
    return user; // Return User
}

// ✅ Fire-and-forget (rare)
public Task SaveUserAsync(User user) // Task, not void
{
    return _db.SaveChangesAsync(); // Return the Task
}
```

**How to Teach:**
1. Visual: "async/await is like ordering at coffee shop"
   - Place order (start Task)
   - Do other things (not blocking)
   - Get notification (await completes)
   - Get coffee (return result)
2. Rule: "If method name ends with Async, return Task"
3. Never: "Never use .Result or .Wait(). Always await."

**Prevention:**
- Compiler helps: Enable async warnings
- Search: Look for `.Result`, `.Wait()`, `async void` in PRs

---

## Category 3: Security Mistakes

### Mistake #7: Returning Sensitive Data

**What You'll See:**
```csharp
[HttpGet("{id}")]
public async Task<User> GetUser(int id)
{
    return await _db.Users.FindAsync(id); // ❌ Returns password hash!
}
```

**Why They Do This:**
- "I just return what the database gives me"
- Don't understand security implications
- Didn't create DTOs

**The Problem:**
- Password hashes exposed to clients (can be cracked offline)
- Internal IDs exposed (security through obscurity broken)
- GDPR violation (PII exposure without consent)

**How to Fix:**
```csharp
[HttpGet("{id}")]
public async Task<UserDto> GetUser(int id)
{
    var user = await _db.Users.FindAsync(id);
    return new UserDto
    {
        FirstName = user.FirstName,
        Email = user.Email
        // NO password, no sensitive data
    };
}
```

**How to Teach:**
1. Ask: "What data should clients see?"
2. Show impact: "In 2019, a company exposed hashes. Attackers cracked 30%, $5M fine."
3. Rule: "Never return entity directly. Always use DTO."
4. List: "Never return: passwords, reset tokens, API keys, SSNs"

**Prevention:**
- API endpoints should return DTOs, never entities
- Code review: Any `Task<User>` in controller is a red flag

---

### Mistake #8: No Input Validation

**What You'll See:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateUser(CreateUserRequest request)
{
    // ❌ No validation!
    var user = new User
    {
        Email = request.Email, // Could be null, empty, or "asdf"
        Age = request.Age // Could be -50 or 999
    };
    await _db.Users.AddAsync(user);
    await _db.SaveChangesAsync();
}
```

**Why They Do This:**
- "Database will validate"
- Didn't learn validation patterns yet
- Works in Postman with valid data

**The Problem:**
- Garbage data in database
- No user-friendly error messages
- Security: Can bypass business rules

**How to Fix:**
```csharp
[HttpPost]
public async Task<IActionResult> CreateUser(CreateUserRequest request)
{
    // ✅ Validation
    if (string.IsNullOrEmpty(request.Email))
        return BadRequest("Email is required");
    
    if (!IsValidEmail(request.Email))
        return BadRequest("Email format is invalid");
    
    if (request.Age < 0 || request.Age > 150)
        return BadRequest("Age must be between 0 and 150");
    
    // Or use FluentValidation
    var validator = new CreateUserValidator();
    var result = await validator.ValidateAsync(request);
    if (!result.IsValid)
        return BadRequest(result.Errors);
    
    // Now create user
}
```

**How to Teach:**
1. Principle: "Never trust client input"
2. Show: "Try sending Age: -50. It saves! That's bad."
3. Options: "Manual validation or FluentValidation library"
4. Practice: "Add validation for this endpoint"

**Prevention:**
- First line in every POST/PUT: validation
- Rule: If it comes from outside, validate it

---

## Category 4: Testing Mistakes

### Mistake #9: Not Writing Tests at All

**What You'll See:**
```
Pull Request:
+ OrderService.cs (200 new lines)
+ 0 test files

Junior: "I tested it manually in Postman. It works!"
```

**Why They Do This:**
- "Tests take too long to write"
- Don't know how to write tests
- Haven't experienced regression bugs yet

**The Problem:**
- No confidence that code works
- Refactoring is scary (might break things)
- Bugs slip into production

**How to Fix:**
Start simple, don't demand 100% coverage:
```csharp
[Fact]
public async Task CreateOrder_ValidInput_CreatesOrder()
{
    // Arrange
    var service = new OrderService(_mockDb.Object);
    var request = new CreateOrderRequest { /* ... */ };
    
    // Act
    var result = await service.CreateOrderAsync(request);
    
    // Assert
    Assert.NotNull(result);
    Assert.Equal(OrderStatus.Pending, result.Status);
}
```

**How to Teach:**
1. Pair: "Let's write one test together"
2. Show: "Now change the code. Test turns red. Fix it. Green!"
3. Benefit: "You just saved yourself 10 minutes of manual testing"
4. Habit: "Write 1-2 tests for each new feature"

**Prevention:**
- PR checklist: "Are critical paths tested?"
- Start small: Just happy path is fine
- Gamify: "You wrote 5 tests this sprint. Nice!"

---

### Mistake #10: Testing Implementation, Not Behavior

**What You'll See:**
```csharp
[Fact]
public void CreateUser_CallsSaveChanges() // ❌ Testing implementation detail
{
    _service.CreateUser(new User());
    _mockDb.Verify(db => db.SaveChanges(), Times.Once);
}
```

**Why They Do This:**
- "I need to test that SaveChanges is called"
- Learned mocking, applies it everywhere
- Doesn't understand what to test

**The Problem:**
- Test breaks when you refactor (even though behavior didn't change)
- Doesn't actually test that user was created
- Brittle tests = ignored tests

**How to Fix:**
```csharp
[Fact]
public async Task CreateUser_ValidInput_UserExistsInDatabase() // ✅ Test behavior
{
    // Arrange
    var user = new User { Email = "test@example.com" };
    
    // Act
    await _service.CreateUserAsync(user);
    
    // Assert
    var saved = await _db.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
    Assert.NotNull(saved);
}
```

**How to Teach:**
1. Principle: "Test behavior (what), not implementation (how)"
2. Ask: "What is this method supposed to do from the caller's perspective?"
3. Bad: "It calls SaveChanges"
4. Good: "It creates a user that can be retrieved later"

**Prevention:**
- Code review: If test uses `.Verify()` on internal method, question it
- Ask: "If I refactor this method, should the test break?"

---

## Category 5: API Design Mistakes

### Mistake #11: Inconsistent HTTP Status Codes

**What You'll See:**
```csharp
[HttpGet("{id}")]
public IActionResult GetUser(int id)
{
    var user = _db.Users.Find(id);
    if (user == null)
        return Ok(new { success = false, error = "Not found" }); // ❌ 200 OK for error!
    
    return Ok(user);
}

[HttpPost]
public IActionResult CreateUser(User user)
{
    try
    {
        _db.Users.Add(user);
        _db.SaveChanges();
        return Ok(new { success = true }); // ❌ Should be 201 Created!
    }
    catch (Exception ex)
    {
        return Ok(new { success = false, error = ex.Message }); // ❌ 200 OK for exception!
    }
}
```

**Why They Do This:**
- "200 OK means it worked, right?"
- Don't understand HTTP semantics
- Copying bad examples

**The Problem:**
- Frontend frameworks check `response.ok` (200-299), miss errors
- HTTP caches cache 200s but not 404s (wrong behavior)
- Monitoring breaks (alerts on 5xx, not `{success: false}`)

**How to Fix:**
```csharp
[HttpGet("{id}")]
public IActionResult GetUser(int id)
{
    var user = _db.Users.Find(id);
    if (user == null)
        return NotFound(); // ✅ 404 Not Found
    
    return Ok(user); // ✅ 200 OK
}

[HttpPost]
public IActionResult CreateUser(User user)
{
    _db.Users.Add(user);
    _db.SaveChanges();
    return Created($"/users/{user.Id}", user); // ✅ 201 Created
}
```

**How to Teach:**
1. Resource: Give them HTTP status code chart
2. Rule: "200-299 = success, 400-499 = client error, 500-599 = server error"
3. Common ones:
   - GET found: 200 OK
   - GET not found: 404 Not Found
   - POST created: 201 Created
   - PUT updated: 200 OK or 204 No Content
   - DELETE: 204 No Content
   - Validation failed: 400 Bad Request
   - Duplicate: 409 Conflict

**Prevention:**
- Code review: Any `return Ok(new { success = false })` is wrong

---

### Mistake #12: Non-RESTful Endpoints

**What You'll See:**
```
POST /api/users/create        ❌ Verb in URL
GET  /api/users/get?id=123    ❌ Verb + query param for ID
PUT  /api/users/update        ❌ Verb in URL
DELETE /api/users/delete?id=123  ❌ Verb + query param
POST /api/users/updateStatus  ❌ Should be PATCH
```

**Why They Do This:**
- "I need to name the endpoint, right?"
- Haven't learned REST conventions
- Copying non-RESTful examples

**The Problem:**
- Non-standard (every API is different)
- Can't leverage HTTP caching
- Can't use standard HTTP tooling

**How to Fix:**
```
POST   /api/users           ✅ Create
GET    /api/users/{id}      ✅ Get
PUT    /api/users/{id}      ✅ Full update
PATCH  /api/users/{id}      ✅ Partial update
DELETE /api/users/{id}      ✅ Delete
PATCH  /api/users/{id}/status  ✅ Update specific field
```

**How to Teach:**
1. Principle: "URLs are nouns, HTTP verbs are actions"
2. Show: "POST /users = create, GET /users/{id} = read"
3. Resources: RESTful API design guide
4. Practice: "Redesign your endpoints using REST conventions"

**Resources to Share:**
- `docs/code-reviews/02-API-Design-Review/v2-api.md`

**Prevention:**
- Code review: Any verb in URL route is a red flag
- Template: Give them RESTful route template to follow

---

## How to Approach Mentoring

### When Junior Makes the Same Mistake Twice

**Don't:**
- "I told you about this last week!"
- "Didn't you read the resource I sent?"

**Do:**
- "I see we're still having trouble with N+1 queries. Let's pair program this time so I can show you how I think through it."
- Ask: "What makes this hard? How can I help you remember?"
- Maybe they need: A checklist, more examples, pair programming, or deeper understanding

---

### When Junior Pushes Back

**Junior:** "But this way is simpler! Why do we need polymorphism?"

**Don't:**
- "Because I said so"
- "You'll understand when you're senior"

**Do:**
- "You're right, it IS simpler now. Let me show you what happens when we need to add 10 more types..."
- Show the pain, don't just assert authority
- "Try it your way. When you add the 4th type and realize it's painful, we'll refactor together."

---

### When You're Short on Time

**Don't:**
- Skip mentoring
- Just fix it yourself

**Do:**
- "I'll fix this one as an example. Next time, you do it."
- "Let's spend 15 minutes. I'll show you the pattern, you apply it to the rest."
- Async: "Watch this Loom video of me fixing it, then try the next one."

---

## Tracking Progress

**Week 1:** They make all these mistakes  
**Week 4:** They make half these mistakes  
**Week 8:** They catch their own mistakes before you do  
**Week 12:** They're teaching another junior  

**That's growth.** Celebrate it.

---

## Remember

- Every senior was a junior who made these mistakes
- Patience > perfection
- Teaching takes longer than fixing, but builds the team
- Your goal: Make yourself redundant by leveling them up

**The best code review is the one that teaches something.**

