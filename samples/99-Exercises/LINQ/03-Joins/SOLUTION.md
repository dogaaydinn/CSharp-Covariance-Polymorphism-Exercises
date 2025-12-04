# ⚠️ SPOILER WARNING ⚠️

**DO NOT READ THIS FILE UNTIL YOU'VE TRIED TO SOLVE THE EXERCISES YOURSELF!**

This file contains complete solutions to all TODO exercises. Try to complete them on your own first!

---

# LINQ Joins - Complete Solutions

## TODO 1: InnerJoinProductsWithSuppliers

```csharp
public static List<ProductWithSupplier> InnerJoinProductsWithSuppliers(
    List<Product> products,
    List<Supplier> suppliers)
{
    return products
        .Join(
            suppliers,
            product => product.SupplierId,
            supplier => supplier.Id,
            (product, supplier) => new ProductWithSupplier
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                SupplierName = supplier.Name,
                SupplierCountry = supplier.Country
            })
        .ToList();
}
```

**Explanation:**
- `Join()` performs an inner join between two collections
- **First parameter**: Inner collection (suppliers)
- **Second parameter**: Outer key selector - extracts SupplierId from each product
- **Third parameter**: Inner key selector - extracts Id from each supplier
- **Fourth parameter**: Result selector - combines matching product and supplier
- Only returns products that have a matching supplier
- Similar to SQL: `SELECT * FROM Products INNER JOIN Suppliers ON Products.SupplierId = Suppliers.Id`

**How it works:**
1. For each product, LINQ looks at its `SupplierId`
2. Finds the supplier where `supplier.Id` equals that `SupplierId`
3. Creates a new `ProductWithSupplier` combining data from both
4. If no matching supplier is found, the product is excluded

---

## TODO 2: LeftJoinProductsWithOrders

```csharp
public static List<ProductWithOrders> LeftJoinProductsWithOrders(
    List<Product> products,
    List<Order> orders)
{
    return products
        .GroupJoin(
            orders,
            product => product.Id,
            order => order.ProductId,
            (product, orderGroup) => new { product, orderGroup })
        .SelectMany(
            x => x.orderGroup.DefaultIfEmpty(),
            (x, order) => new ProductWithOrders
            {
                ProductId = x.product.Id,
                ProductName = x.product.Name,
                OrderId = order?.Id,
                Quantity = order?.Quantity,
                CustomerName = order?.CustomerName
            })
        .ToList();
}
```

**Explanation:**
- LINQ doesn't have a direct `LeftJoin()` method
- We combine three methods to achieve left outer join:
  1. **GroupJoin()**: Groups orders by product (creates product → [orders] pairs)
  2. **DefaultIfEmpty()**: Adds a null entry if the order group is empty
  3. **SelectMany()**: Flattens the groups into individual records

**Step-by-step breakdown:**
```csharp
// Step 1: GroupJoin creates grouped data
// Product1 → [Order1, Order2, Order3]
// Product2 → [Order4]
// Product3 → [] (empty group)

// Step 2: DefaultIfEmpty() adds null for empty groups
// Product1 → [Order1, Order2, Order3]
// Product2 → [Order4]
// Product3 → [null] (now has one null order)

// Step 3: SelectMany flattens to individual records
// { Product1, Order1 }
// { Product1, Order2 }
// { Product1, Order3 }
// { Product2, Order4 }
// { Product3, null }
```

**Why use `order?.Id` instead of `order.Id`?**
- The `?` is the null-conditional operator
- When a product has no orders, `order` will be null
- `order?.Id` returns null instead of throwing NullReferenceException
- This makes OrderId, Quantity, and CustomerName nullable properties

---

## TODO 3: MultipleJoins

```csharp
public static List<ProductDetail> MultipleJoins(
    List<Product> products,
    List<Category> categories,
    List<Supplier> suppliers,
    List<Order> orders)
{
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
                TotalOrders = orderGroup.Count()
            })
        .ToList();
}
```

**Explanation:**
- Chains multiple join operations to combine data from 4 tables
- Uses anonymous types to carry data between joins

**Step-by-step breakdown:**
```csharp
// Step 1: Join products with suppliers
// Result: { Product, Supplier }
products.Join(suppliers, p => p.SupplierId, s => s.Id, (p, s) => new { Product = p, Supplier = s })

// Step 2: Join previous result with categories
// Access Product through ps.Product (from step 1)
// Result: { Product, Supplier, Category }
.Join(categories, ps => ps.Product.CategoryId, c => c.Id, (ps, c) => new { ps.Product, ps.Supplier, Category = c })

// Step 3: GroupJoin with orders to count them
// Use Count() to get total orders per product
// Result: ProductDetail with all information
.GroupJoin(orders, psc => psc.Product.Id, o => o.ProductId, (psc, orderGroup) => new ProductDetail { ... })
```

**Why use GroupJoin for orders?**
- We want to count orders, not list each one individually
- `GroupJoin` groups all orders for each product
- `orderGroup.Count()` gives us the total number of orders
- Products with no orders get a count of 0 (empty group)

**Alternative approach with more readable intermediate variables:**
```csharp
var productsWithSuppliers = products
    .Join(suppliers, p => p.SupplierId, s => s.Id, (p, s) => new { Product = p, Supplier = s });

var withCategories = productsWithSuppliers
    .Join(categories, ps => ps.Product.CategoryId, c => c.Id, (ps, c) => new { ps.Product, ps.Supplier, Category = c });

var result = withCategories
    .GroupJoin(orders, psc => psc.Product.Id, o => o.ProductId,
        (psc, orderGroup) => new ProductDetail { ... })
    .ToList();
```

---

## TODO 4: GroupJoinProductsByCategory

```csharp
public static List<CategoryWithProducts> GroupJoinProductsByCategory(
    List<Category> categories,
    List<Product> products)
{
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
}
```

**Explanation:**
- `GroupJoin()` creates a one-to-many relationship
- Each category gets paired with a collection of its products
- Unlike `SelectMany()` in TODO 2, we keep the grouped structure

**How GroupJoin works:**
```csharp
// Input:
// Categories: [Electronics, Furniture, Stationery, Books]
// Products: [Laptop(Electronics), Desk(Furniture), Mouse(Electronics), ...]

// GroupJoin result:
// Electronics → [Laptop, Mouse, Keyboard, Monitor, Tablet, Webcam]
// Furniture → [Desk, Chair, Lamp, Bookshelf]
// Stationery → [Notebook, Pen Set]
// Books → [] (empty - no products)

// Final output:
// CategoryWithProducts {
//     CategoryName = "Electronics",
//     ProductNames = ["Laptop", "Mouse", "Keyboard", "Monitor", "Tablet", "Webcam"]
// }
// ... (etc for each category)
```

**Key difference from TODO 2:**
- In TODO 2, we used `SelectMany()` to flatten groups into individual records
- Here, we keep the grouped structure for hierarchical data
- Perfect for parent-child relationships (category → products)

---

## Alternative Solutions

### TODO 1 (Alternative with Query Syntax)
```csharp
public static List<ProductWithSupplier> InnerJoinProductsWithSuppliers(
    List<Product> products,
    List<Supplier> suppliers)
{
    return (from product in products
            join supplier in suppliers on product.SupplierId equals supplier.Id
            select new ProductWithSupplier
            {
                ProductId = product.Id,
                ProductName = product.Name,
                Price = product.Price,
                SupplierName = supplier.Name,
                SupplierCountry = supplier.Country
            }).ToList();
}
```

### TODO 2 (Alternative with Query Syntax)
```csharp
public static List<ProductWithOrders> LeftJoinProductsWithOrders(
    List<Product> products,
    List<Order> orders)
{
    return (from product in products
            join order in orders on product.Id equals order.ProductId into orderGroup
            from order in orderGroup.DefaultIfEmpty()
            select new ProductWithOrders
            {
                ProductId = product.Id,
                ProductName = product.Name,
                OrderId = order?.Id,
                Quantity = order?.Quantity,
                CustomerName = order?.CustomerName
            }).ToList();
}
```

---

## Bonus Solutions

### Bonus 1: GetProductsWithMultipleOrders

```csharp
public static List<string> GetProductsWithMultipleOrders(List<Product> products, List<Order> orders, int minOrders)
{
    return products
        .GroupJoin(orders, p => p.Id, o => o.ProductId, (p, orderGroup) => new { Product = p, Orders = orderGroup })
        .Where(x => x.Orders.Count() >= minOrders)
        .Select(x => x.Product.Name)
        .ToList();
}
```

### Bonus 2: GetSuppliersWithProductCount

```csharp
public static Dictionary<string, int> GetSuppliersWithProductCount(List<Supplier> suppliers, List<Product> products)
{
    return suppliers
        .GroupJoin(products, s => s.Id, p => p.SupplierId, (s, productGroup) => new { Supplier = s.Name, Count = productGroup.Count() })
        .ToDictionary(x => x.Supplier, x => x.Count);
}
```

### Bonus 3: GetTopCustomersBySpending

```csharp
public static List<(string Customer, decimal TotalSpending)> GetTopCustomersBySpending(
    List<Order> orders,
    List<Product> products,
    int topN)
{
    return orders
        .Join(products, o => o.ProductId, p => p.Id, (o, p) => new { o.CustomerName, Spending = o.Quantity * p.Price })
        .GroupBy(x => x.CustomerName)
        .Select(g => (Customer: g.Key, TotalSpending: g.Sum(x => x.Spending)))
        .OrderByDescending(x => x.TotalSpending)
        .Take(topN)
        .ToList();
}
```

### Bonus 4: GetCategoriesWithAveragePrice

```csharp
public static Dictionary<string, decimal> GetCategoriesWithAveragePrice(List<Category> categories, List<Product> products)
{
    return categories
        .Join(products, c => c.Id, p => p.CategoryId, (c, p) => new { Category = c.Name, p.Price })
        .GroupBy(x => x.Category)
        .ToDictionary(g => g.Key, g => g.Average(x => x.Price));
}
```

---

## Key Takeaways

### Join Types Comparison

| Join Type | LINQ Method | SQL Equivalent | Returns |
|-----------|-------------|----------------|---------|
| **Inner Join** | `Join()` | `INNER JOIN` | Only matching records |
| **Left Join** | `GroupJoin()` + `SelectMany()` + `DefaultIfEmpty()` | `LEFT JOIN` | All left records, nulls for unmatched |
| **Group Join** | `GroupJoin()` | `LEFT JOIN` + `GROUP BY` | Hierarchical data |
| **Multiple Joins** | Chain multiple `Join()` | Multiple `JOIN` clauses | Combined data |

### When to Use Each Join

- **Inner Join (`Join()`)**: When you only want records that exist in both collections
  - Example: Products with valid suppliers

- **Left Join (`GroupJoin()` + `SelectMany()`)**: When you want all records from the left, even without matches
  - Example: All products, including those with no orders

- **Group Join (`GroupJoin()`)**: When you want hierarchical/grouped data
  - Example: Categories with their list of products

- **Multiple Joins**: When you need data from 3+ collections
  - Example: Product details from products, suppliers, categories, and orders

### Performance Considerations

1. **Join order matters**: Join smaller collections first when possible
2. **Use anonymous types**: They're lightweight for intermediate results
3. **Materialize once**: Call `.ToList()` at the end, not in between
4. **Consider pre-filtering**: Use `Where()` before joins to reduce data

---

##Common Join Patterns

### Pattern 1: Simple Inner Join
```csharp
collectionA.Join(collectionB,
    a => a.ForeignKey,
    b => b.PrimaryKey,
    (a, b) => new Result { ... })
```

### Pattern 2: Left Outer Join
```csharp
collectionA
    .GroupJoin(collectionB, a => a.Key, b => b.Key, (a, bGroup) => new { a, bGroup })
    .SelectMany(x => x.bGroup.DefaultIfEmpty(), (x, b) => new Result { ... })
```

### Pattern 3: Multiple Joins
```csharp
collectionA
    .Join(collectionB, ...)
    .Join(collectionC, ...)
    .Join(collectionD, ...)
```

### Pattern 4: Group Join
```csharp
parents.GroupJoin(children,
    parent => parent.Id,
    child => child.ParentId,
    (parent, childGroup) => new ParentWithChildren { Parent = parent, Children = childGroup.ToList() })
```

---

**Congratulations on completing the LINQ Joins exercise! 🎉**

You now understand:
- ✅ Inner joins with `Join()`
- ✅ Left outer joins with `GroupJoin()` + `SelectMany()` + `DefaultIfEmpty()`
- ✅ Chaining multiple joins
- ✅ Creating hierarchical data with `GroupJoin()`

*This completes the LINQ category! Next, move on to the Algorithms exercises.*
