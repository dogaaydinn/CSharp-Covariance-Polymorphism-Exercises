# LINQ: Joins Exercise

## 📚 Learning Objectives

By completing this exercise, you will learn:
- **Inner joins** with `Join()` method
- **Left outer joins** with `GroupJoin()` + `SelectMany()` + `DefaultIfEmpty()`
- **Multiple joins** by chaining join operations
- **Group joins** with `GroupJoin()` method
- Working with related data from multiple collections
- Understanding SQL-like join operations in LINQ

## 🎯 Exercise Tasks

You need to complete **4 TODO methods** in `Program.cs`:

1. ✅ **InnerJoinProductsWithSuppliers()** - Basic inner join
2. ✅ **LeftJoinProductsWithOrders()** - Left outer join (includes unmatched records)
3. ✅ **MultipleJoins()** - Chain multiple joins across 4 tables
4. ✅ **GroupJoinProductsByCategory()** - Group join to create hierarchical data

## 🚀 Getting Started

### Step 1: Navigate to the Project
```bash
cd samples/99-Exercises/LINQ/03-Joins
```

### Step 2: Restore Dependencies
```bash
dotnet restore
```

### Step 3: Run the Tests (They Should FAIL)
```bash
dotnet test
```

You should see **FAILED** tests initially. This is expected!

### Step 4: Complete the TODOs
Open `Program.cs` and look for `// TODO` comments. Complete each method.

### Step 5: Re-run Tests
```bash
dotnet test
```

Keep working until **ALL TESTS PASS**! ✅

## 💡 Hints and Tips

### TODO 1: InnerJoinProductsWithSuppliers

**Goal**: Join products with suppliers, returning only products that have a matching supplier.

**Hint**: Use `Join()` method which takes 4 parameters:
1. Inner collection (suppliers)
2. Outer key selector (product => product.SupplierId)
3. Inner key selector (supplier => supplier.Id)
4. Result selector (combines product and supplier)

```csharp
// Solution approach:
return products
    .Join(
        suppliers,                       // Inner collection
        product => product.SupplierId,   // Outer key (from products)
        supplier => supplier.Id,         // Inner key (from suppliers)
        (product, supplier) => new ProductWithSupplier
        {
            ProductId = product.Id,
            ProductName = product.Name,
            Price = product.Price,
            SupplierName = supplier.Name,
            SupplierCountry = supplier.Country
        })
    .ToList();
```

**Key concepts**:
- Inner join only returns records where keys match
- Similar to SQL: `SELECT * FROM Products INNER JOIN Suppliers ON Products.SupplierId = Suppliers.Id`

### TODO 2: LeftJoinProductsWithOrders

**Goal**: Left outer join - show all products, even if they have no orders.

**Hint**: LINQ doesn't have a direct `LeftJoin()` method. Use `GroupJoin()` + `SelectMany()` + `DefaultIfEmpty()`.

```csharp
// Solution approach:
return products
    .GroupJoin(
        orders,                          // Inner collection
        product => product.Id,           // Outer key
        order => order.ProductId,        // Inner key
        (product, orderGroup) => new { product, orderGroup })  // Temporary object
    .SelectMany(
        x => x.orderGroup.DefaultIfEmpty(),  // DefaultIfEmpty() ensures products without orders are included
        (x, order) => new ProductWithOrders
        {
            ProductId = x.product.Id,
            ProductName = x.product.Name,
            OrderId = order?.Id,             // Use ? for null safety
            Quantity = order?.Quantity,
            CustomerName = order?.CustomerName
        })
    .ToList();
```

**Key concepts**:
- `GroupJoin()` creates groups (product → [orders])
- `DefaultIfEmpty()` adds a null entry if the group is empty
- `SelectMany()` flattens the groups into individual records
- Similar to SQL: `SELECT * FROM Products LEFT JOIN Orders ON Products.Id = Orders.ProductId`

### TODO 3: MultipleJoins

**Goal**: Join data from 4 tables: products, categories, suppliers, and orders (count).

**Hint**: Chain multiple `Join()` operations, use `GroupJoin()` for counting orders.

```csharp
// Solution approach:
return products
    .Join(
        suppliers,
        p => p.SupplierId,
        s => s.Id,
        (p, s) => new { Product = p, Supplier = s })
    .Join(
        categories,
        ps => ps.Product.CategoryId,
        c => c.Id,
        (ps, c) => new { ps.Product, ps.Supplier, Category = c })
    .GroupJoin(
        orders,
        psc => psc.Product.Id,
        o => o.ProductId,
        (psc, orderGroup) => new ProductDetail
        {
            ProductName = psc.Product.Name,
            CategoryName = psc.Category.Name,
            SupplierName = psc.Supplier.Name,
            Price = psc.Product.Price,
            TotalOrders = orderGroup.Count()  // Count orders for each product
        })
    .ToList();
```

**Key steps**:
1. Join products with suppliers
2. Join result with categories
3. GroupJoin with orders to count them
4. Project to ProductDetail

### TODO 4: GroupJoinProductsByCategory

**Goal**: Group products by category, returning categories with lists of product names.

**Hint**: Use `GroupJoin()` to create hierarchical data structure.

```csharp
// Solution approach:
return categories
    .GroupJoin(
        products,
        category => category.Id,
        product => product.CategoryId,
        (category, productGroup) => new CategoryWithProducts
        {
            CategoryName = category.Name,
            ProductNames = productGroup.Select(p => p.Name).ToList()
        })
    .ToList();
```

**Key concepts**:
- `GroupJoin()` returns the left item with a collection of matching right items
- Perfect for creating parent-child relationships
- Categories without products will have empty ProductNames lists

## ✅ Success Criteria

All tests should pass:
- [ ] `InnerJoinProductsWithSuppliers_ShouldReturnAllProducts` ✅
- [ ] `InnerJoinProductsWithSuppliers_ShouldMatchCorrectSuppliers` ✅
- [ ] `InnerJoinProductsWithSuppliers_ShouldIncludePriceInformation` ✅
- [ ] `InnerJoinProductsWithSuppliers_ShouldHandleEmptyLists` ✅
- [ ] `LeftJoinProductsWithOrders_ShouldIncludeAllProducts` ✅
- [ ] `LeftJoinProductsWithOrders_ShouldShowProductsWithoutOrders` ✅
- [ ] `LeftJoinProductsWithOrders_ShouldShowProductsWithMultipleOrders` ✅
- [ ] `LeftJoinProductsWithOrders_ShouldIncludeOrderDetails` ✅
- [ ] `LeftJoinProductsWithOrders_ShouldHandleEmptyOrders` ✅
- [ ] `MultipleJoins_ShouldCombineAllInformation` ✅
- [ ] `MultipleJoins_ShouldCountOrdersCorrectly` ✅
- [ ] `MultipleJoins_ShouldIncludeFurnitureCategory` ✅
- [ ] `MultipleJoins_ShouldIncludeStationeryCategory` ✅
- [ ] `GroupJoinProductsByCategory_ShouldReturnAllCategories` ✅
- [ ] `GroupJoinProductsByCategory_ElectronicsShouldHaveCorrectProducts` ✅
- [ ] `GroupJoinProductsByCategory_FurnitureShouldHaveCorrectProducts` ✅
- [ ] `GroupJoinProductsByCategory_BooksShouldBeEmpty` ✅
- [ ] `GroupJoinProductsByCategory_ShouldHandleEmptyCategories` ✅
- [ ] `InnerJoinProductsWithSuppliers_ShouldReturnEmptyWhenNoSuppliers` ✅
- [ ] `MultipleJoins_ShouldHandleSingleProduct` ✅

**Total: 20 tests must pass!**

## 📖 LINQ Join Methods Reference

### Join() - Inner Join
Correlates elements of two sequences based on matching keys.
```csharp
var result = products.Join(
    suppliers,                       // Inner collection
    product => product.SupplierId,   // Outer key selector
    supplier => supplier.Id,         // Inner key selector
    (product, supplier) => new { product, supplier }  // Result selector
);
```

### GroupJoin() - Group Join
Correlates elements and groups the results.
```csharp
var result = categories.GroupJoin(
    products,
    category => category.Id,
    product => product.CategoryId,
    (category, productGroup) => new { category, Products = productGroup }
);
```

### Left Join Pattern
Combination of GroupJoin + SelectMany + DefaultIfEmpty.
```csharp
var result = products
    .GroupJoin(orders, p => p.Id, o => o.ProductId, (p, orderGroup) => new { p, orderGroup })
    .SelectMany(
        x => x.orderGroup.DefaultIfEmpty(),
        (x, order) => new { Product = x.p, Order = order }
    );
```

### Multiple Joins
Chain multiple Join operations.
```csharp
var result = products
    .Join(suppliers, p => p.SupplierId, s => s.Id, (p, s) => new { p, s })
    .Join(categories, ps => ps.p.CategoryId, c => c.Id, (ps, c) => new { ps.p, ps.s, c });
```

## 🎓 What You'll Learn

### Inner Join vs Left Join
- **Inner Join**: Returns only matching records
  ```csharp
  products.Join(suppliers, ...) // Only products with suppliers
  ```

- **Left Join**: Returns all left records, even without matches
  ```csharp
  products.GroupJoin(orders, ...).SelectMany(x => x.DefaultIfEmpty())
  // All products, including those without orders
  ```

### Key Selectors
The key selectors must return values of compatible types:
```csharp
.Join(suppliers,
    product => product.SupplierId,  // Returns int
    supplier => supplier.Id,        // Returns int (must match!)
    ...)
```

### Anonymous Types for Intermediate Results
Use anonymous types to carry data through multiple joins:
```csharp
.Join(..., (p, s) => new { Product = p, Supplier = s })
.Join(..., ps => ps.Product.CategoryId, ...)
```

## 🐛 Common Mistakes to Avoid

1. **Wrong key order in Join()**:
   ```csharp
   // ❌ Wrong - keys are swapped
   products.Join(suppliers,
       supplier => supplier.Id,      // Wrong!
       product => product.SupplierId, // Wrong!
       ...)

   // ✅ Correct - outer key first, inner key second
   products.Join(suppliers,
       product => product.SupplierId,   // Outer key
       supplier => supplier.Id,         // Inner key
       ...)
   ```

2. **Forgetting DefaultIfEmpty() in left join**:
   ```csharp
   // ❌ Wrong - this is still an inner join!
   products.GroupJoin(orders, ...).SelectMany(x => x.orderGroup, ...)

   // ✅ Correct - DefaultIfEmpty() makes it a left join
   products.GroupJoin(orders, ...).SelectMany(x => x.orderGroup.DefaultIfEmpty(), ...)
   ```

3. **Not handling null values in left join**:
   ```csharp
   // ❌ Wrong - will throw NullReferenceException
   OrderId = order.Id

   // ✅ Correct - use nullable operator
   OrderId = order?.Id
   ```

4. **Forgetting to chain SelectMany after GroupJoin**:
   ```csharp
   // ❌ Wrong - returns grouped data
   products.GroupJoin(orders, ..., (p, orderGroup) => new { p, orderGroup })

   // ✅ Correct - flattens groups
   products.GroupJoin(...).SelectMany(x => x.orderGroup.DefaultIfEmpty(), ...)
   ```

## 🚀 Challenge: Bonus Tasks

Once you complete all TODOs, try these bonus challenges:

1. **GetProductsWithMultipleOrders()**: Find products that have more than 2 orders
   - Hint: Use GroupBy on the left join result

2. **GetSuppliersWithProductCount()**: Join suppliers with products and count products per supplier
   - Hint: Use GroupJoin and Count()

3. **GetTopCustomersBySpending()**: Join orders with products, group by customer, calculate total spending
   - Hint: Sum(order => order.Quantity * product.Price)

4. **GetCategoriesWithAveragePrice()**: Join categories with products, calculate average price per category
   - Hint: GroupJoin + Average()

## 📚 Additional Resources

- [LINQ Join Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.join)
- [LINQ GroupJoin Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.linq.enumerable.groupjoin)
- [Performing Left Outer Joins](https://learn.microsoft.com/en-us/dotnet/csharp/linq/perform-left-outer-joins)
- [Join Operations in LINQ](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/join-operations)

---

**Good luck! 🎉**

*Once you complete all TODOs, check `SOLUTION.md` for the complete solutions (but try to solve it yourself first!).*
