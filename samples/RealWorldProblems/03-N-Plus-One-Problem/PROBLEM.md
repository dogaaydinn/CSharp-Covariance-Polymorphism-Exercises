# Real-World Problem: The Dreaded N+1 Query Problem

## The Situation

**Date:** Monday morning, 9:15 AM  
**From:** DevOps Team  
**Slack Message:**
> "🚨 Database alerts firing! Connection pool exhausted. 500 active connections (max 500). Users seeing 'Connection timeout' errors. Last deploy was Friday - what changed?"

**You check New Relic:**
```
Endpoint: GET /api/orders
Response time: 15 seconds (was 200ms last week)
Database queries: 1,527 queries per request (!!)
```

**The code that was deployed Friday:**

```csharp
[HttpGet]
public async Task<IActionResult> GetOrders()
{
    var orders = await _context.Orders.ToListAsync();
    
    var result = orders.Select(order => new OrderDto
    {
        Id = order.Id,
        CustomerName = order.Customer.Name,  // ❌ Lazy load: 1 query per order
        Items = order.Items.Select(item => new ItemDto
        {
            ProductName = item.Product.Name,  // ❌ Lazy load: 1 query per item
            CategoryName = item.Product.Category.Name  // ❌ Another query!
        }).ToList()
    }).ToList();
    
    return Ok(result);
}
```

**What happened:**
- 1 query to get orders (500 orders)
- 500 queries to get customer names (1 per order)
- 500 queries to get order items (1 per order)
- 527 queries to get product names (1 per item, 527 total items)
- **Total: 1,527 queries!**

---

## The Business Impact

**Current State:**
- API response time: 15 seconds
- Database CPU: 98%
- Active connections: 500/500 (maxed out)
- User complaints: 47 in last hour

**Cost:**
- Lost sales: ~$2,000/hour
- Database struggling: May need to scale up ($500/month extra)

---

## Your Task

Fix the N+1 problem to:
- Reduce queries from 1,527 → ~3
- Reduce response time from 15s → <200ms
- Keep database connections < 50

**Time available:** 2 hours (URGENT!)

---

## Solutions

1. **SOLUTION-A.md** - Eager Loading (Include/ThenInclude)
2. **SOLUTION-B.md** - Projection (Select directly to DTO)
3. **SOLUTION-C.md** - GraphQL (Optimal queries)
4. **COMPARISON.md** - When to use each

**Next:** Read SOLUTION-A.md for the quickest fix.
