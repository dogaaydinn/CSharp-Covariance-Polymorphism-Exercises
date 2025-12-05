# Why Use the Unit of Work Pattern?

A deep dive into understanding when, why, and how to use the Unit of Work pattern effectively in .NET applications.

## The Problem: Multiple SaveChanges() Calls

Imagine you're building an e-commerce system. Without the Unit of Work pattern, you might have code like this:

```csharp
public class OrderController
{
    private readonly AppDbContext _context;

    public void ProcessOrder(int productId, int quantity, string customerName)
    {
        // Step 1: Reduce product stock
        var product = _context.Products.Find(productId);
        product.Stock -= quantity;
        _context.Products.Update(product);
        _context.SaveChanges();  // ← First commit

        // Step 2: Create order
        var order = new Order
        {
            CustomerName = customerName,
            TotalAmount = product.Price * quantity
        };
        _context.Orders.Add(order);
        _context.SaveChanges();  // ← Second commit

        // Step 3: Create order items
        var orderItem = new OrderItem
        {
            OrderId = order.Id,
            ProductId = productId,
            Quantity = quantity
        };
        _context.OrderItems.Add(orderItem);
        _context.SaveChanges();  // ← Third commit
    }
}
```

**What could go wrong?**

### Scenario 1: Stock Reduced but Order Fails

```
Timeline:
1. ✓ Product stock reduced (committed to database)
2. ✓ Order created (committed to database)
3. ❌ OrderItem creation fails (network error, validation error, etc.)

Result:
- Stock is reduced: 10 → 8 (permanent!)
- Order exists in database (orphaned)
- OrderItems are missing
- Customer sees "Order failed" but stock is gone! 😱
```

### Scenario 2: Inventory Inconsistency

```csharp
// Customer A orders last 2 items
productStock: 2 → 0  // First SaveChanges()
// Network timeout before creating order...

// Meanwhile, Customer B also orders same product
productStock: 0 → -2  // No validation! Stock goes negative!
```

**Result:** Inventory becomes inconsistent, overselling occurs

## The Solution: Unit of Work Pattern

```csharp
public class OrderService
{
    private readonly IUnitOfWork _unitOfWork;

    public void ProcessOrder(int productId, int quantity, string customerName)
    {
        try
        {
            _unitOfWork.BeginTransaction();

            // Step 1: Reduce product stock (staged)
            var product = _unitOfWork.Products.GetById(productId);
            if (product.Stock < quantity)
            {
                throw new InsufficientStockException();
            }

            product.Stock -= quantity;
            _unitOfWork.Products.Update(product);  // ← Staged, not committed

            // Step 2: Create order (staged)
            var order = new Order
            {
                CustomerName = customerName,
                TotalAmount = product.Price * quantity
            };
            _unitOfWork.Orders.Add(order);  // ← Staged, not committed

            // Step 3: Create order items (staged)
            var orderItem = new OrderItem
            {
                OrderId = order.Id,
                ProductId = productId,
                Quantity = quantity
            };
            _unitOfWork.OrderItems.Add(orderItem);  // ← Staged, not committed

            // All or nothing: Single atomic commit
            _unitOfWork.Commit();  // ← All changes committed together
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();  // ← Undo all changes
            throw;
        }
    }
}
```

**What happens now?**

### Scenario 1: Atomic Success

```
Timeline:
1. ⏳ Product stock reduction (staged)
2. ⏳ Order creation (staged)
3. ⏳ OrderItem creation (staged)
4. ✅ Commit() - All changes applied atomically

Result:
- All changes committed together
- Database is consistent
- Customer gets confirmation
```

### Scenario 2: Atomic Failure

```
Timeline:
1. ⏳ Product stock reduction (staged)
2. ⏳ Order creation (staged)
3. ❌ OrderItem validation fails
4. 🔄 Rollback() - All changes undone

Result:
- No changes committed
- Stock unchanged
- No orphaned order
- Customer sees clear error message
- Database remains consistent ✅
```

## When to Use Unit of Work

### ✅ Use Unit of Work when:

#### 1. Multiple Repositories Are Involved

**E-commerce Order Processing**
```csharp
// Need to coordinate 3 repositories
unitOfWork.BeginTransaction();
unitOfWork.Products.Update(product);      // Reduce stock
unitOfWork.Orders.Add(order);             // Create order
unitOfWork.OrderItems.Add(items);         // Add items
unitOfWork.Commit();  // ← All or nothing
```

**User Registration**
```csharp
// Create user with profile and permissions
unitOfWork.BeginTransaction();
unitOfWork.Users.Add(user);
unitOfWork.UserProfiles.Add(profile);
unitOfWork.UserPermissions.Add(permissions);
unitOfWork.Commit();  // ← Atomic
```

#### 2. Atomic Transactions Are Required

**Financial Transfer**
```csharp
// Transfer money between accounts
unitOfWork.BeginTransaction();
fromAccount.Balance -= 1000;  // Debit
toAccount.Balance += 1000;    // Credit
unitOfWork.Accounts.Update(fromAccount);
unitOfWork.Accounts.Update(toAccount);
unitOfWork.Commit();  // ← Both or neither

// Without Unit of Work:
// Debit succeeds, credit fails → Money lost! 💸
```

**Inventory Management**
```csharp
// Transfer stock between warehouses
unitOfWork.BeginTransaction();
warehouseA.Stock -= quantity;  // Remove from A
warehouseB.Stock += quantity;  // Add to B
unitOfWork.Warehouses.Update(warehouseA);
unitOfWork.Warehouses.Update(warehouseB);
unitOfWork.Commit();  // ← Atomic transfer
```

#### 3. Complex Business Logic with Validation

**Order Processing with Stock Validation**
```csharp
public void CreateOrder(string customerName, List<OrderItem> items)
{
    _unitOfWork.BeginTransaction();

    decimal totalAmount = 0;

    foreach (var item in items)
    {
        var product = _unitOfWork.Products.GetById(item.ProductId);

        // Validation
        if (product.Stock < item.Quantity)
        {
            throw new InsufficientStockException(product.Name);
            // Rollback happens automatically - no changes committed
        }

        product.Stock -= item.Quantity;
        _unitOfWork.Products.Update(product);
        totalAmount += product.Price * item.Quantity;
    }

    var order = new Order
    {
        CustomerName = customerName,
        TotalAmount = totalAmount
    };

    _unitOfWork.Orders.Add(order);
    _unitOfWork.Commit();  // ← All validations passed, commit
}
```

#### 4. Data Consistency Is Critical

**Banking System**
```csharp
// Account balance must always match transaction history
unitOfWork.BeginTransaction();

account.Balance -= amount;
unitOfWork.Accounts.Update(account);

var transaction = new Transaction
{
    AccountId = account.Id,
    Amount = -amount,
    Type = "Withdrawal"
};
unitOfWork.Transactions.Add(transaction);

unitOfWork.Commit();  // ← Balance and history in sync
```

### ❌ Avoid Unit of Work when:

#### 1. Simple CRUD Operations

```csharp
// Simple update - no coordination needed
public void UpdateProductName(int id, string newName)
{
    var product = context.Products.Find(id);
    product.Name = newName;
    context.SaveChanges();  // ← Direct SaveChanges() is fine
}
```

**Why?** Unit of Work adds overhead for no benefit in simple scenarios.

#### 2. Read-Only Operations

```csharp
// Queries don't need transactions
public List<Product> GetExpensiveProducts()
{
    return context.Products
                  .Where(p => p.Price > 100)
                  .ToList();  // ← No Unit of Work needed
}
```

**Why?** Reads don't modify data, no transaction coordination required.

#### 3. Performance Is Critical

```csharp
// High-frequency logging
public void LogUserActivity(string userId, string action)
{
    context.ActivityLogs.Add(new ActivityLog
    {
        UserId = userId,
        Action = action,
        Timestamp = DateTime.UtcNow
    });
    context.SaveChanges();  // ← Direct is faster
}
```

**Why?** Every abstraction adds latency (~5-25%). For performance-critical paths, direct DbContext is better.

#### 4. Single Entity Updates

```csharp
// Only one entity, no coordination
public void MarkOrderAsShipped(int orderId)
{
    var order = context.Orders.Find(orderId);
    order.Status = "Shipped";
    context.SaveChanges();  // ← No Unit of Work needed
}
```

**Why?** Single entity updates don't need multi-repository coordination.

## Repository Pattern vs Unit of Work

### How They Work Together

```
Unit of Work (Transaction Boundary)
│
├─── IRepository<Product>
│    └─── Add, Update, Delete (staged)
│
├─── IRepository<Order>
│    └─── Add, Update, Delete (staged)
│
└─── SaveChanges() / Commit()
     └─── All changes committed atomically
```

**Key Insight:** Unit of Work **coordinates** multiple repositories, ensuring atomic commits.

### The Relationship

```csharp
public interface IUnitOfWork
{
    // Unit of Work provides repositories
    IGenericRepository<Product> Products { get; }
    IGenericRepository<Order> Orders { get; }
    IGenericRepository<OrderItem> OrderItems { get; }

    // Unit of Work manages transactions
    void BeginTransaction();
    void Commit();    // ← Commits all repository changes together
    void Rollback();  // ← Undoes all repository changes
}
```

**Without Unit of Work:**
```csharp
productRepo.Add(product);
productRepo.SaveChanges();  // ← Repository commits individually

orderRepo.Add(order);
orderRepo.SaveChanges();  // ← Another individual commit

// Problem: Not atomic!
```

**With Unit of Work:**
```csharp
unitOfWork.Products.Add(product);  // ← Staged
unitOfWork.Orders.Add(order);      // ← Staged
unitOfWork.Commit();               // ← Single atomic commit
```

## Real-World Scenarios

### Scenario 1: E-Commerce Checkout

**Problem:**
```csharp
// ❌ BAD: Without Unit of Work
void Checkout(Cart cart)
{
    // Reduce stock
    foreach (var item in cart.Items)
    {
        var product = context.Products.Find(item.ProductId);
        product.Stock -= item.Quantity;
        context.SaveChanges();  // ← Commit 1
    }

    // Create order
    var order = new Order { CustomerId = cart.CustomerId };
    context.Orders.Add(order);
    context.SaveChanges();  // ← Commit 2

    // Process payment
    var payment = ProcessPayment(order.TotalAmount);
    if (!payment.Success)
    {
        // 😱 Stock already reduced!
        // Order already created!
        // How to rollback?!
    }
}
```

**Solution:**
```csharp
// ✅ GOOD: With Unit of Work
void Checkout(Cart cart)
{
    try
    {
        _unitOfWork.BeginTransaction();

        // Reduce stock (staged)
        foreach (var item in cart.Items)
        {
            var product = _unitOfWork.Products.GetById(item.ProductId);
            product.Stock -= item.Quantity;
            _unitOfWork.Products.Update(product);
        }

        // Create order (staged)
        var order = new Order { CustomerId = cart.CustomerId };
        _unitOfWork.Orders.Add(order);

        // Process payment
        var payment = ProcessPayment(order.TotalAmount);
        if (!payment.Success)
        {
            throw new PaymentFailedException();  // ← Automatic rollback
        }

        _unitOfWork.Commit();  // ← All or nothing
    }
    catch
    {
        _unitOfWork.Rollback();  // ← Undo everything
        throw;
    }
}
```

### Scenario 2: Bank Transfer

**Problem:**
```csharp
// ❌ BAD: Without Unit of Work
void Transfer(int fromAccountId, int toAccountId, decimal amount)
{
    var fromAccount = context.Accounts.Find(fromAccountId);
    fromAccount.Balance -= amount;
    context.SaveChanges();  // ← Commit 1

    // 💥 Server crashes here!
    // Money deducted from sender but not added to recipient!

    var toAccount = context.Accounts.Find(toAccountId);
    toAccount.Balance += amount;
    context.SaveChanges();  // ← Commit 2 (never happens!)
}
```

**Solution:**
```csharp
// ✅ GOOD: With Unit of Work
void Transfer(int fromAccountId, int toAccountId, decimal amount)
{
    try
    {
        _unitOfWork.BeginTransaction();

        var fromAccount = _unitOfWork.Accounts.GetById(fromAccountId);
        fromAccount.Balance -= amount;
        _unitOfWork.Accounts.Update(fromAccount);  // ← Staged

        var toAccount = _unitOfWork.Accounts.GetById(toAccountId);
        toAccount.Balance += amount;
        _unitOfWork.Accounts.Update(toAccount);  // ← Staged

        _unitOfWork.Commit();  // ← Atomic - both or neither
        // Even if server crashes, database transaction ensures atomicity
    }
    catch
    {
        _unitOfWork.Rollback();
        throw;
    }
}
```

### Scenario 3: Inventory Transfer

**Problem:**
```csharp
// ❌ BAD: Without Unit of Work
void TransferInventory(int productId, int fromWarehouse, int toWarehouse, int quantity)
{
    var product = context.Products.Find(productId);
    var source = product.WarehouseInventories.Find(w => w.WarehouseId == fromWarehouse);
    source.Quantity -= quantity;
    context.SaveChanges();  // ← Commit 1

    // Validation error: destination warehouse full!
    var dest = product.WarehouseInventories.Find(w => w.WarehouseId == toWarehouse);
    if (dest.Quantity + quantity > dest.Capacity)
    {
        throw new Exception("Warehouse full");
        // 😱 Source inventory already reduced!
    }
}
```

**Solution:**
```csharp
// ✅ GOOD: With Unit of Work
void TransferInventory(int productId, int fromWarehouse, int toWarehouse, int quantity)
{
    try
    {
        _unitOfWork.BeginTransaction();

        var product = _unitOfWork.Products.GetById(productId);
        var source = product.WarehouseInventories.Find(w => w.WarehouseId == fromWarehouse);
        var dest = product.WarehouseInventories.Find(w => w.WarehouseId == toWarehouse);

        // Validation before any changes
        if (dest.Quantity + quantity > dest.Capacity)
        {
            throw new WarehouseFullException();
            // No changes committed yet - safe to fail
        }

        source.Quantity -= quantity;
        dest.Quantity += quantity;

        _unitOfWork.Products.Update(product);
        _unitOfWork.Commit();  // ← Atomic transfer
    }
    catch
    {
        _unitOfWork.Rollback();
        throw;
    }
}
```

## Common Mistakes

### ❌ Mistake 1: Not Using Transactions for Multi-Step Operations

```csharp
// BAD
void CreateUserWithProfile(User user, UserProfile profile)
{
    context.Users.Add(user);
    context.SaveChanges();  // ← First commit

    profile.UserId = user.Id;
    context.UserProfiles.Add(profile);
    context.SaveChanges();  // ← Second commit

    // If second fails, user exists without profile!
}
```

**Solution:**
```csharp
// GOOD
void CreateUserWithProfile(User user, UserProfile profile)
{
    _unitOfWork.BeginTransaction();
    _unitOfWork.Users.Add(user);
    profile.UserId = user.Id;
    _unitOfWork.UserProfiles.Add(profile);
    _unitOfWork.Commit();  // ← Atomic
}
```

### ❌ Mistake 2: Forgetting to Rollback on Failure

```csharp
// BAD
void ProcessOrder(Order order)
{
    _unitOfWork.BeginTransaction();
    _unitOfWork.Orders.Add(order);
    // Exception thrown here
    // Transaction left open! 😱
}
```

**Solution:**
```csharp
// GOOD
void ProcessOrder(Order order)
{
    try
    {
        _unitOfWork.BeginTransaction();
        _unitOfWork.Orders.Add(order);
        _unitOfWork.Commit();
    }
    catch
    {
        _unitOfWork.Rollback();  // ← Always rollback on error
        throw;
    }
}
```

### ❌ Mistake 3: Using Unit of Work for Read-Only Operations

```csharp
// BAD: Unnecessary overhead
List<Product> GetProducts()
{
    _unitOfWork.BeginTransaction();  // ← Not needed for reads!
    var products = _unitOfWork.Products.GetAll();
    _unitOfWork.Commit();
    return products;
}
```

**Solution:**
```csharp
// GOOD: Direct query
List<Product> GetProducts()
{
    return _unitOfWork.Products.GetAll();  // ← No transaction needed
}
```

## The Debate: Is Unit of Work Necessary with EF Core?

### Arguments AGAINST Unit of Work

> "EF Core's `DbContext` already implements Unit of Work!"

**Valid points:**
- `DbContext.SaveChanges()` is already a unit of work
- Change tracking handles staging
- Single SaveChanges() commits all tracked changes

**Example:**
```csharp
// DbContext as Unit of Work
context.Products.Update(product);  // ← Tracked
context.Orders.Add(order);         // ← Tracked
context.SaveChanges();             // ← Commits both atomically
```

### Arguments FOR Unit of Work

**Counter-arguments:**

#### 1. **Transaction Control**

```csharp
// Without Unit of Work Pattern
context.Products.Update(product);
context.Orders.Add(order);
context.SaveChanges();  // ← How to rollback if business logic fails?

// With Unit of Work Pattern
_unitOfWork.BeginTransaction();
_unitOfWork.Products.Update(product);
_unitOfWork.Orders.Add(order);
// Business logic validation...
if (validationFails)
{
    _unitOfWork.Rollback();  // ← Explicit rollback
    return;
}
_unitOfWork.Commit();
```

#### 2. **Multiple DbContext Instances**

```csharp
// Problem: Coordinating multiple contexts
using var productContext = new ProductDbContext();
using var orderContext = new OrderDbContext();

productContext.Products.Update(product);
orderContext.Orders.Add(order);

productContext.SaveChanges();  // ← First commit
orderContext.SaveChanges();    // ← Second commit (not atomic!)

// With Unit of Work: Single transaction across contexts
```

#### 3. **Testability**

```csharp
// Without Unit of Work: Must mock DbContext
var mockContext = new Mock<AppDbContext>();
// Complex setup required...

// With Unit of Work: Simple interface mock
var mockUnitOfWork = new Mock<IUnitOfWork>();
mockUnitOfWork.Setup(u => u.Products).Returns(mockProductRepo.Object);
// Easy!
```

### When DbContext Alone Is Enough

For simple applications:
- Single DbContext
- No complex transaction logic
- No need for explicit rollback
- Team understands EF Core well

### When Unit of Work Adds Value

For complex applications:
- Multiple repositories need coordination
- Explicit transaction control required
- Testing with mocks is important
- Clear transaction boundaries needed
- Multiple DbContext instances

## Conclusion

### Key Takeaways

1. **Unit of Work ensures atomic transactions** across multiple repositories
2. **Prevents partial updates** and data inconsistency
3. **Essential for complex business logic** with multi-step operations
4. **Simplifies testing** with easy-to-mock interfaces
5. **Trade-off**: Added complexity vs guaranteed data consistency

### Decision Matrix

| Scenario | Use Unit of Work? | Reason |
|----------|------------------|--------|
| E-commerce order processing | ✅ Yes | Multiple entities, stock validation, atomic commit required |
| Financial transfers | ✅ Yes | ACID properties critical, no partial updates allowed |
| User registration with profile | ✅ Yes | User + Profile must be created together |
| Simple product name update | ❌ No | Single entity, no coordination needed |
| Read-only product listing | ❌ No | No data modification, no transaction needed |
| High-frequency activity logging | ❌ No | Performance overhead not justified |

### Final Recommendation

**Start simple:**
- Use `DbContext.SaveChanges()` for simple scenarios
- Add Unit of Work when you need:
  - Explicit transaction control
  - Multi-repository coordination
  - Complex business logic with rollback
  - Better testability

**Remember:** The goal is **data consistency**, not pattern purity. Choose the right tool for the job!
