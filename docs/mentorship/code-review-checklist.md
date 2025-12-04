# Code Review Checklist for Junior Developer Code

**Purpose:** Catch common issues while teaching, not just criticizing  
**Tone:** Constructive, educational, empowering  
**Goal:** Help juniors level up, not feel bad

---

## How to Use This Checklist

1. **First Pass:** Read code with fresh eyes, no checklist (get overall impression)
2. **Second Pass:** Use this checklist systematically
3. **Third Pass:** Prioritize feedback (Critical → Important → Suggestions)
4. **Write Review:** Frame as learning opportunities, not criticisms

**Remember:** Every senior wrote code like this once. Be the mentor you wish you had.

---

## 🔴 Critical Issues (Block PR Until Fixed)

### 1. Security Vulnerabilities

#### ❌ Password/Secrets Exposed
```csharp
// BAD
public class UserDto
{
    public string Password { get; set; } // ❌ NEVER return passwords!
}

// GOOD
public class UserDto
{
    // NO password property at all
}
```

**Review Comment Template:**
> "⚠️ **Security Issue:** We're returning the password in the API response. Even hashed passwords shouldn't be exposed to clients. Let's remove the Password property from the DTO.
>
> Why: An attacker could harvest password hashes and attempt offline cracking.
>
> Fix: Create a DTO that excludes sensitive fields.
>
> Resource: See `docs/code-reviews/02-API-Design-Review/review-feedback.md` Issue #1"

---

#### ❌ SQL Injection Vulnerable
```csharp
// BAD
var query = $"SELECT * FROM Users WHERE Name = '{userName}'"; // ❌ SQL injection!
var users = _db.Database.SqlQuery<User>(query).ToList();

// GOOD
var users = await _db.Users
    .Where(u => u.Name == userName)
    .ToListAsync(); // ✅ Parameterized
```

**Review Comment:**
> "🚨 **Critical: SQL Injection Vulnerability**
>
> String interpolation in SQL queries allows SQL injection attacks. An attacker could input: `'; DROP TABLE Users; --`
>
> Fix: Use Entity Framework's Where() clause or parameterized queries.
>
> Example:
> ```csharp
> var users = await _db.Users.Where(u => u.Name == userName).ToListAsync();
> ```"

---

#### ❌ No Authentication/Authorization
```csharp
// BAD
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id) // ❌ Anyone can delete anyone!
{
    await _userService.DeleteAsync(id);
    return NoContent();
}

// GOOD
[HttpDelete("{id}")]
[Authorize(Roles = "Admin")] // ✅ Only admins
public async Task<IActionResult> DeleteUser(int id)
{
    await _userService.DeleteAsync(id);
    return NoContent();
}
```

**Review Comment:**
> "🔒 **Security:** This endpoint is public - anyone can delete any user!
>
> We need to add:
> 1. `[Authorize]` attribute (requires authentication)
> 2. Role/policy check (only admins can delete users)
>
> Fix:
> ```csharp
> [Authorize(Roles = "Admin")]
> ```
>
> Also consider: Should users be able to delete themselves? If so, add policy: `[Authorize(Policy = \"SelfOrAdmin\")]`"

---

### 2. Data Loss / Corruption Risk

#### ❌ Hard Delete Without Confirmation
```csharp
// BAD
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    _db.Users.Remove(user); // ❌ Permanent deletion, no recovery
    await _db.SaveChangesAsync();
}
```

**Review Comment:**
> "⚠️ **Data Loss Risk:** Hard delete means data is gone forever. Consider:
>
> 1. **Soft delete instead:** Add `DeletedAt` timestamp, filter deleted records in queries
> 2. **Require confirmation:** For hard delete, require `{ \"confirmation\": \"DELETE\" }` in body
> 3. **Audit log:** Log who deleted what when
>
> Recommendation: Implement soft delete. See `samples/98-RealWorld-Problems/02-API-Design-Review/v2-api.md` for pattern."

---

#### ❌ Race Condition in Update
```csharp
// BAD
public async Task UpdateBalance(int userId, decimal amount)
{
    var user = await _db.Users.FindAsync(userId);
    user.Balance += amount; // ❌ Race condition if 2 requests happen simultaneously
    await _db.SaveChangesAsync();
}

// GOOD
public async Task UpdateBalance(int userId, decimal amount)
{
    await _db.Database.ExecuteSqlInterpolatedAsync(
        $"UPDATE Users SET Balance = Balance + {amount} WHERE Id = {userId}"
    ); // ✅ Atomic operation
}
```

**Review Comment:**
> "🐛 **Concurrency Bug:** If two requests update balance simultaneously, one update will be lost.
>
> Scenario:
> 1. Request A reads balance: $100
> 2. Request B reads balance: $100
> 3. Request A adds $50 → saves $150
> 4. Request B adds $30 → saves $130 (overwrites A's update!)
>
> Fix: Use atomic SQL UPDATE or add optimistic concurrency (row version)."

---

### 3. Performance Killers

#### ❌ N+1 Query Problem
```csharp
// BAD
var orders = await _db.Orders.ToListAsync();
foreach (var order in orders)
{
    order.Customer = await _db.Customers.FindAsync(order.CustomerId); // ❌ N queries!
}

// GOOD
var orders = await _db.Orders
    .Include(o => o.Customer) // ✅ 1 query with JOIN
    .ToListAsync();
```

**Review Comment:**
> "🐌 **Performance: N+1 Query Problem**
>
> This code makes 501 database queries:
> - 1 to load orders
> - 500 to load each customer (one per order)
>
> Fix: Use `.Include()` for eager loading.
>
> ```csharp
> var orders = await _db.Orders.Include(o => o.Customer).ToListAsync();
> ```
>
> Impact: Response time will drop from ~15s to ~200ms.
>
> See: `samples/98-RealWorld-Problems/03-N-Plus-One-Problem/` for detailed example."

---

#### ❌ Loading Entire Table
```csharp
// BAD
var allUsers = await _db.Users.ToListAsync(); // ❌ Loads 1 million users!
var activeUsers = allUsers.Where(u => u.IsActive).ToList();

// GOOD
var activeUsers = await _db.Users
    .Where(u => u.IsActive) // ✅ Filters in database
    .ToListAsync();
```

**Review Comment:**
> "⚠️ **Performance:** We're loading ALL users into memory (1M rows), then filtering in C#.
>
> This causes:
> - 500 MB memory usage
> - 10-second query time
> - OutOfMemoryException at scale
>
> Fix: Filter in database with `.Where()` before `.ToListAsync()`.
>
> Also consider pagination for large datasets."

---

## ⚠️ Important Issues (Should Fix Before Merge)

### 4. Maintainability Problems

#### ❌ Type Checking Anti-Pattern
```csharp
// BAD
public void ProcessPayment(string type, decimal amount)
{
    if (type == "CreditCard") { /* ... */ }
    else if (type == "PayPal") { /* ... */ }
    else if (type == "Bitcoin") { /* ... */ }
}
```

**Review Comment:**
> "🔧 **Design:** This uses type checking instead of polymorphism. When we add Apple Pay:
> - Must modify this method
> - Must update 4 other methods that check payment types
> - Risk of typos (\"Paypal\" vs \"PayPal\")
>
> Refactor using polymorphism:
> ```csharp
> public interface IPaymentProcessor {
>     void Process(decimal amount);
> }
> public class CreditCardProcessor : IPaymentProcessor { ... }
> ```
>
> Benefits:
> - Add Apple Pay = create 1 new class, NO changes to existing code
> - Compiler catches errors (no typos)
> - Easier to test
>
> See: `samples/01-Beginner/PolymorphismBasics/` for full example.
>
> Note: If this is only 2 types and won't grow, if/else is fine. But I see we're adding 3 more payment types next sprint, so polymorphism will save time."

---

#### ❌ God Class (Too Many Responsibilities)
```csharp
// BAD
public class UserService
{
    public void CreateUser() { }
    public void SendEmail() { } // ❌ Not user management!
    public void GenerateReport() { } // ❌ Not user management!
    public void ProcessPayment() { } // ❌ Not user management!
}
```

**Review Comment:**
> "📦 **Single Responsibility Principle:** UserService is doing too much:
> - User management ✅
> - Email sending ❌
> - Report generation ❌
> - Payment processing ❌
>
> This makes it:
> - Hard to test (must mock email, reports, payments to test user creation)
> - Hard to maintain (changes to reports affect user management)
> - Hard to reuse (can't use email sender elsewhere)
>
> Refactor:
> - Keep user CRUD in UserService
> - Move email → EmailService
> - Move reports → ReportService
> - Move payments → PaymentService
>
> Then inject dependencies:
> ```csharp
> public class UserService(IEmailService email, ...) { }
> ```"

---

### 5. Error Handling Issues

#### ❌ Swallowing Exceptions
```csharp
// BAD
try
{
    await _paymentService.ChargeAsync(amount);
}
catch (Exception)
{
    // ❌ Silent failure! User thinks payment succeeded!
}
```

**Review Comment:**
> "🐛 **Error Handling:** Exception is caught but not handled. User will think payment succeeded when it actually failed!
>
> Fix:
> ```csharp
> try
> {
>     await _paymentService.ChargeAsync(amount);
> }
> catch (PaymentFailedException ex)
> {
>     _logger.LogError(ex, \"Payment failed for order {OrderId}\", orderId);
>     return BadRequest(new { error = \"Payment failed. Your card was not charged.\" });
> }
> ```
>
> Key points:
> 1. Log the error (for debugging)
> 2. Return proper HTTP status (400 Bad Request)
> 3. User-friendly message (don't expose ex.Message)"

---

#### ❌ Exposing Exception Messages
```csharp
// BAD
catch (Exception ex)
{
    return Ok(new { success = false, error = ex.Message }); // ❌ Leaks internals!
}

// User sees: "Cannot insert duplicate key in dbo.Users..."
// Now attacker knows your table structure!
```

**Review Comment:**
> "🔒 **Information Disclosure:** Returning `ex.Message` exposes implementation details:
> - Database schema (\"dbo.Users\")
> - Stack traces (sometimes)
> - Library versions
>
> Fix:
> ```csharp
> catch (DuplicateEmailException)
> {
>     _logger.LogWarning(ex, \"Duplicate email: {Email}\", email);
>     return Conflict(new ProblemDetails {
>         Title = \"Email already in use\",
>         Detail = \"An account with this email exists. Try logging in.\",
>         Status = 409
>     });
> }
> catch (Exception ex)
> {
>     _logger.LogError(ex, \"Failed to create user\");
>     return StatusCode(500, new ProblemDetails {
>         Title = \"An error occurred\",
>         Detail = \"Please try again later.\",
>         Status = 500
>     });
> }
> ```"

---

### 6. Testing Gaps

#### ❌ No Tests for New Feature
```
Files changed:
+ OrderService.cs (150 lines of new code)
+ 0 test files
```

**Review Comment:**
> "🧪 **Testing:** I don't see tests for the new order processing logic. Let's add:
>
> 1. **Happy path:** Order is created successfully
> 2. **Validation:** Order fails if required fields missing
> 3. **Edge cases:** What if inventory is 0? What if payment fails?
>
> Don't need 100% coverage, but critical paths should be tested.
>
> Example:
> ```csharp
> [Fact]
> public async Task CreateOrder_ValidInput_CreatesOrder()
> {
>     // Arrange
>     var request = new CreateOrderRequest { /* ... */ };
>     
>     // Act
>     var result = await _service.CreateOrderAsync(request);
>     
>     // Assert
>     Assert.NotNull(result);
>     Assert.Equal(OrderStatus.Pending, result.Status);
> }
> ```
>
> Want to pair on writing tests? I'm available tomorrow 2pm."

---

## 💡 Suggestions (Nice to Have, Not Blocking)

### 7. Code Style & Readability

#### ⚠️ Magic Numbers
```csharp
// MEH
if (user.Age > 18) { } // What's special about 18?

// BETTER
const int LegalAdultAge = 18;
if (user.Age > LegalAdultAge) { }
```

**Review Comment:**
> "💡 **Suggestion:** Consider extracting magic numbers to named constants. Makes code self-documenting.
>
> Optional improvement:
> ```csharp
> private const int LegalAdultAge = 18;
> if (user.Age >= LegalAdultAge) { }
> ```
>
> Not blocking, but improves readability."

---

#### ⚠️ Long Method (>50 lines)
**Review Comment:**
> "💡 **Suggestion:** This method is 80 lines. Consider extracting to smaller methods:
>
> ```csharp
> public async Task ProcessOrder(Order order)
> {
>     ValidateOrder(order);
>     await ChargePayment(order);
>     await UpdateInventory(order);
>     await SendConfirmation(order);
> }
> ```
>
> Benefits:
> - Easier to test each step
> - Easier to understand what's happening
> - Easier to reuse validation/payment logic
>
> Not urgent, but worth considering for next refactoring."

---

### 8. Modern C# Features

#### ⚠️ Can Use Pattern Matching
```csharp
// OLD
if (user != null && user.IsActive)
{
    ProcessUser(user);
}

// MODERN
if (user is { IsActive: true })
{
    ProcessUser(user);
}
```

**Review Comment:**
> "💡 **C# 9:** You could use pattern matching here. Optional, but more concise:
>
> ```csharp
> if (user is { IsActive: true })
> {
>     ProcessUser(user);
> }
> ```
>
> Reads as: \"If user exists and IsActive is true\"
>
> Just FYI - your current code is fine too!"

---

## How to Write the Review

### Structure Your Feedback

```markdown
## Summary

Great work on the order processing feature! The logic is solid. I have a few suggestions:

- 🔴 **Critical:** SQL injection vulnerability in user search (must fix)
- ⚠️ **Important:** N+1 query in order loading (performance issue)
- 💡 **Suggestion:** Consider extracting validation to separate method

Overall looking good! Fix the critical item, and I'll approve. Happy to pair on the N+1 fix if you'd like.

---

## Critical Issues

### 🔴 SQL Injection in UserSearch

**File:** `UserService.cs:45`

[Explanation + code example + resources]

---

## Important Issues

### ⚠️ N+1 Query in GetOrders

**File:** `OrderController.cs:23`

[Explanation + code example + resources]

---

## Suggestions

### 💡 Extract Validation Method

**File:** `OrderService.cs:67`

[Explanation + optional improvement]

---

## Questions

1. Line 89: Why do we call `SaveChanges()` twice? Is this intentional?
2. Line 123: Should this be async? The method it calls is async.

---

## What I Liked ✨

- Great variable naming - very readable
- Good error handling for payment failures
- You added tests for the happy path - nice!

---

## Resources

- N+1 queries: `samples/98-RealWorld-Problems/03-N-Plus-One-Problem/`
- Polymorphism refactoring: `samples/01-Beginner/PolymorphismBasics/`

Let me know if you have questions! Happy to discuss any of this.
```

---

## Tone Guidelines

### ❌ Don't Say:
- "This is wrong"
- "Why did you do it this way?"
- "This is terrible"
- "You should know better"

### ✅ Do Say:
- "I see what you're trying to do. Let's make it safer by..."
- "Great start! Here's how we can improve X..."
- "I made this same mistake in my first year. Here's what I learned..."
- "Have you considered...?"

---

## Balancing Criticism with Praise

**For Every Critical Issue, Mention 2 Things Done Well:**

```
🔴 Critical: SQL injection vulnerability

But also:

✨ I love how you named your variables - very clear
✨ Your test coverage for the happy path is great
✨ The error handling for null inputs is solid

Let's fix the security issue, and this PR is gold!
```

---

## When to Approve vs Request Changes

### ✅ Approve If:
- No critical/security issues
- Important issues have clear fixes noted
- You trust them to address suggestions later

### 🔄 Request Changes If:
- Any critical/security issues
- Important issues that significantly impact users
- Multiple important issues combined

### 💬 Comment (No Block) If:
- Only suggestions/style issues
- Questions for clarification
- Positive feedback

---

## Follow-Up After Review

**If They Fixed Issues Quickly:**
> "Great turnaround! You addressed all the feedback quickly and correctly. This is exactly what we're looking for. ✅ Approved!"

**If They're Struggling:**
> "I see you're stuck on the N+1 fix. Want to pair on this? I have 30 minutes at 2pm today."

**If They Disagreed:**
> "I see your point about the if/else approach. You're right that polymorphism is more code upfront. Let's discuss - I'll show you why it saves time when we add the 4th and 5th payment types next month. Coffee chat?"

---

## Common Pitfalls for Mentors

### 🚫 Nitpicking Every Line
**Bad:** 47 comments, mostly style issues  
**Good:** 5 important comments + 2 style suggestions

### 🚫 Rewriting Their Code
**Bad:** "Here's how I would do it: [50 lines of code]"  
**Good:** "Consider extracting this to a method. Want to try? I'm here if you need help."

### 🚫 Assuming They Know Context
**Bad:** "This violates SOLID"  
**Good:** "This violates the Single Responsibility Principle - UserService is handling both user management and email sending. Let's separate those concerns."

### 🚫 No Positive Feedback
**Bad:** Only pointing out problems  
**Good:** "Great naming! The logic is clear. Here are 2 improvements..."

---

## Remember

**Your goal isn't to make their code perfect.**  
**Your goal is to make THEM a better developer.**

Sometimes, approving with suggestions is better than blocking for style issues. They'll learn more from shipping code and seeing its impact than from endless review cycles.

**Good Review:** Catches critical issues, teaches 1-2 concepts, encourages them  
**Great Review:** Same as good, plus makes them excited to improve

