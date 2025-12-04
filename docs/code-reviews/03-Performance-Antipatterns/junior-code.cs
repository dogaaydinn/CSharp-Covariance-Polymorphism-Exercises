// PR #167: Add Order Processing Feature
// Author: Junior Developer (10 months experience)
// Date: 2024-12-03
// Description: Process customer orders and generate reports

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderProcessing
{
    // ❌ PROBLEM: Synchronous code in async method
    public class OrderService
    {
        private List<Order> _orders = new List<Order>();
        private List<Customer> _customers = new List<Customer>();
        private List<Product> _products = new List<Product>();

        // ❌ PROBLEM 1: Using async void (should be async Task)
        // ❌ PROBLEM 2: Blocking with .Result (deadlock risk)
        public async void ProcessOrder(int orderId)
        {
            var order = GetOrderById(orderId).Result; // ❌ Blocking async call!

            // ❌ PROBLEM 3: String concatenation in loop
            string log = "";
            foreach (var item in order.Items)
            {
                log += $"Processing item {item.ProductId}\n"; // ❌ Creates new string each iteration!
            }

            Console.WriteLine(log);
        }

        // ❌ PROBLEM 4: Not async (synchronous sleep)
        public Task<Order> GetOrderById(int id)
        {
            Thread.Sleep(1000); // ❌ Blocks thread!

            var order = _orders.FirstOrDefault(o => o.Id == id);
            return Task.FromResult(order);
        }

        // ❌ PROBLEM 5: Multiple database calls in loop (N+1 query problem)
        public List<OrderDto> GetOrdersWithCustomerInfo()
        {
            var orders = _orders.ToList();
            var result = new List<OrderDto>();

            foreach (var order in orders)
            {
                // ❌ Database call for EACH order!
                var customer = GetCustomerById(order.CustomerId).Result;

                result.Add(new OrderDto
                {
                    OrderId = order.Id,
                    CustomerName = customer.Name,
                    Total = order.Total
                });
            }

            // ❌ PROBLEM 6: No pagination (returns all orders)
            return result;
        }

        // ❌ PROBLEM 7: LINQ query in loop
        public decimal GetTotalRevenue()
        {
            decimal total = 0;

            foreach (var order in _orders)
            {
                // ❌ LINQ query executed for EACH order!
                var orderTotal = order.Items.Where(i => !i.IsCancelled).Sum(i => i.Price);
                total += orderTotal;
            }

            return total;
        }

        // ❌ PROBLEM 8: Unnecessary ToList() calls
        public List<Order> GetRecentOrders()
        {
            return _orders
                .ToList() // ❌ Unnecessary
                .Where(o => o.CreatedAt > DateTime.Now.AddDays(-7))
                .ToList() // ❌ Unnecessary
                .OrderByDescending(o => o.CreatedAt)
                .ToList(); // Only this one is needed!
        }

        // ❌ PROBLEM 9: Loading all data before filtering
        public List<Order> GetHighValueOrders()
        {
            // ❌ Loads ALL orders from database into memory
            var allOrders = _orders.ToList();

            // Then filters (should filter at database level)
            return allOrders.Where(o => o.Total > 1000).ToList();
        }

        // ❌ PROBLEM 10: Closure in loop capturing wrong variable
        public void ProcessOrdersAsync()
        {
            var tasks = new List<Task>();

            for (int i = 0; i < _orders.Count; i++)
            {
                // ❌ Closure captures 'i' by reference, not value!
                tasks.Add(Task.Run(() =>
                {
                    Console.WriteLine($"Processing order {i}"); // Will print wrong value!
                    ProcessOrder(_orders[i].Id);
                }));
            }

            Task.WaitAll(tasks.ToArray()); // ❌ Blocking wait
        }

        // ❌ PROBLEM 11: Exception in loop doesn't stop processing
        public void UpdateOrderStatuses()
        {
            foreach (var order in _orders)
            {
                try
                {
                    // ❌ Catching and swallowing exceptions
                    UpdateOrderStatus(order.Id, "Processed");
                }
                catch
                {
                    // ❌ Silent failure, no logging
                }
            }
        }

        // ❌ PROBLEM 12: Not disposing resources properly
        public string ReadOrderFile(string path)
        {
            // ❌ No using statement, file handle not closed!
            var stream = new System.IO.FileStream(path, System.IO.FileMode.Open);
            var reader = new System.IO.StreamReader(stream);
            return reader.ReadToEnd();
            // ❌ Stream and reader never disposed
        }

        // ❌ PROBLEM 13: Creating new objects in loop unnecessarily
        public List<string> GetCustomerEmails()
        {
            var emails = new List<string>();

            foreach (var order in _orders)
            {
                // ❌ Creates new DateTime object for each iteration
                var threshold = DateTime.Now.AddDays(-30);

                if (order.CreatedAt > threshold)
                {
                    var customer = _customers.FirstOrDefault(c => c.Id == order.CustomerId);
                    emails.Add(customer.Email);
                }
            }

            return emails;
        }

        // ❌ PROBLEM 14: Using Select() when not needed
        public int GetOrderCount()
        {
            // ❌ Select creates unnecessary enumeration
            return _orders.Select(o => o.Id).Count();
            // Should be: _orders.Count()
        }

        // ❌ PROBLEM 15: Inefficient string operations
        public string GenerateOrderReport()
        {
            string report = "";

            report += "Order Report\n";
            report += "============\n";

            foreach (var order in _orders)
            {
                report += $"Order #{order.Id}\n"; // ❌ String concatenation in loop
                report += $"Customer: {order.CustomerId}\n";
                report += $"Total: ${order.Total}\n";
                report += "----\n";
            }

            return report;
        }

        // ❌ PROBLEM 16: Async over sync (fake async)
        public async Task<int> GetTotalOrdersAsync()
        {
            // ❌ No actual async work, just wrapping sync code
            return await Task.Run(() =>
            {
                return _orders.Count;
            });
            // Should be: return Task.FromResult(_orders.Count);
        }

        // ❌ PROBLEM 17: Nested loops with expensive operations
        public List<OrderSummary> GetOrderSummaries()
        {
            var summaries = new List<OrderSummary>();

            // ❌ O(n²) complexity
            foreach (var order in _orders)
            {
                foreach (var item in order.Items)
                {
                    // ❌ Database lookup in nested loop!
                    var product = _products.FirstOrDefault(p => p.Id == item.ProductId);

                    // ❌ Another database lookup!
                    var customer = _customers.FirstOrDefault(c => c.Id == order.CustomerId);

                    summaries.Add(new OrderSummary
                    {
                        ProductName = product?.Name,
                        CustomerName = customer?.Name,
                        Quantity = item.Quantity
                    });
                }
            }

            return summaries;
        }

        // ❌ PROBLEM 18: No caching for frequently accessed data
        public Product GetProductById(int id)
        {
            // ❌ Every call queries the list
            // Should cache frequently accessed products
            return _products.FirstOrDefault(p => p.Id == id);
        }

        // ❌ PROBLEM 19: Boxing/Unboxing in loop
        public void LogOrderIds()
        {
            var ids = new List<object>(); // ❌ List of object (causes boxing)

            foreach (var order in _orders)
            {
                ids.Add(order.Id); // ❌ Boxing int to object
            }

            foreach (var id in ids)
            {
                Console.WriteLine((int)id); // ❌ Unboxing object to int
            }
        }

        // Helper methods
        private Task<Customer> GetCustomerById(int id)
        {
            Thread.Sleep(100); // Simulate database call
            return Task.FromResult(_customers.FirstOrDefault(c => c.Id == id));
        }

        private void UpdateOrderStatus(int orderId, string status)
        {
            var order = _orders.FirstOrDefault(o => o.Id == orderId);
            if (order != null)
            {
                order.Status = status;
            }
        }
    }

    // Domain models
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }

    public class OrderItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public bool IsCancelled { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderDto
    {
        public int OrderId { get; set; }
        public string CustomerName { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderSummary
    {
        public string ProductName { get; set; }
        public string CustomerName { get; set; }
        public int Quantity { get; set; }
    }
}

/*
 * PERFORMANCE ANTI-PATTERNS SUMMARY:
 *
 * ASYNC/AWAIT ISSUES:
 * 1. ❌ async void (should be async Task)
 * 2. ❌ Blocking async calls with .Result (deadlock risk)
 * 3. ❌ Thread.Sleep in async method (use await Task.Delay)
 * 4. ❌ Task.WaitAll (blocking, use await Task.WhenAll)
 * 5. ❌ Async over sync (fake async with Task.Run)
 *
 * N+1 QUERY PROBLEM:
 * 6. ❌ Database calls in loop (GetOrdersWithCustomerInfo)
 * 7. ❌ Nested loops with database lookups (GetOrderSummaries)
 *
 * LINQ PERFORMANCE:
 * 8. ❌ Multiple ToList() calls
 * 9. ❌ ToList() before filtering (loads all data into memory)
 * 10. ❌ LINQ query in loop (GetTotalRevenue)
 * 11. ❌ Unnecessary Select() (GetOrderCount)
 *
 * STRING OPERATIONS:
 * 12. ❌ String concatenation in loop (use StringBuilder)
 * 13. ❌ String concatenation for reports (GenerateOrderReport)
 *
 * MEMORY LEAKS:
 * 14. ❌ Not disposing IDisposable (FileStream, StreamReader)
 *
 * INEFFICIENT CODE:
 * 15. ❌ Creating objects in loop unnecessarily (DateTime.Now)
 * 16. ❌ Closure capturing wrong variable in loop
 * 17. ❌ O(n²) nested loops
 * 18. ❌ No caching for frequently accessed data
 * 19. ❌ Boxing/Unboxing in loop
 * 20. ❌ Exception swallowing in loop
 */
