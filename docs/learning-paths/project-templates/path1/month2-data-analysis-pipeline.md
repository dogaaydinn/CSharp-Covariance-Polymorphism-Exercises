# Month 2 Capstone: Data Analysis Pipeline

**Difficulty**: ⭐⭐⭐☆☆ (Intermediate)
**Estimated Time**: 20-25 hours
**Prerequisites**: Completed Week 5-8 of Path 1 (LINQ Mastery)

---

## 🎯 Project Overview

Build a complete data analysis system that processes CSV data files using advanced LINQ queries, functional programming patterns, and generates comprehensive reports.

### Learning Objectives

By completing this project, you will demonstrate:
- ✅ Advanced LINQ queries (filters, aggregations, joins)
- ✅ Deferred vs immediate execution understanding
- ✅ Functional programming with Func<T>, Action<T>
- ✅ Closures and lambda expressions
- ✅ Performance optimization for large datasets
- ✅ Multi-format data export (JSON, CSV, XML)
- ✅ Complex grouping and statistical calculations

---

## 📋 Requirements

### Functional Requirements

1. **Data Sources**:
   - CSV file reader (1000+ rows)
   - Support for: Sales data, Customer data, Product data
   - Multiple file imports
   - Data validation

2. **20+ LINQ Queries** (minimum):
   - **Filtering**: By date range, amount, category, customer
   - **Aggregations**: Sum, Average, Count, Min, Max
   - **Grouping**: By category, customer, date, region
   - **Joins**: Inner joins between Sales, Customers, Products
   - **Complex**: Multi-level grouping, nested aggregations
   - **Sorting**: Multi-column ordering
   - **Projections**: Anonymous types, custom DTOs

3. **Statistical Reports**:
   - Total sales by product
   - Average order value by customer
   - Top 10 products by revenue
   - Top 10 customers by total spent
   - Sales by date (daily, monthly, yearly)
   - Category performance analysis
   - Customer segmentation (high/medium/low value)
   - Trend analysis (growth rates)

4. **Data Export Formats**:
   - JSON (using System.Text.Json)
   - CSV (custom formatter)
   - XML (using System.Xml)
   - Console table format

5. **Console Interface**:
   ```
   === DATA ANALYSIS PIPELINE ===
   1. Load Data Files
   2. Run Sales Analysis
   3. Run Customer Analysis
   4. Run Product Analysis
   5. Generate Custom Report
   6. Export Results
   7. Exit
   ```

### Technical Requirements

1. **Performance Optimized**: Handle 10,000+ rows efficiently
2. **Functional Programming**: Use Func<T>, Action<T>, closures
3. **Deferred Execution**: Demonstrate understanding with examples
4. **LINQ Variety**: Must use Where, Select, GroupBy, Join, OrderBy, Aggregate, etc.
5. **Unit Tests**: 15+ tests covering LINQ queries
6. **Benchmarks**: Performance tests with BenchmarkDotNet
7. **Error Handling**: Graceful handling of malformed CSV

---

## 🏗️ Project Structure

```
DataAnalysisPipeline/
├── DataAnalysisPipeline.csproj
├── Program.cs
├── Models/
│   ├── Sale.cs
│   ├── Customer.cs
│   ├── Product.cs
│   └── ReportResult.cs
├── Services/
│   ├── CsvReaderService.cs
│   ├── DataAnalysisService.cs
│   ├── SalesAnalyzer.cs
│   ├── CustomerAnalyzer.cs
│   └── ProductAnalyzer.cs
├── Exporters/
│   ├── IExporter.cs
│   ├── JsonExporter.cs
│   ├── CsvExporter.cs
│   ├── XmlExporter.cs
│   └── ConsoleExporter.cs
├── Utilities/
│   ├── FunctionalExtensions.cs  // Map, Filter, Reduce
│   └── QueryBuilder.cs          // Fluent LINQ builder
├── Data/
│   ├── sales.csv
│   ├── customers.csv
│   └── products.csv
└── DataAnalysisPipeline.Tests/
    ├── DataAnalysisPipeline.Tests.csproj
    ├── SalesAnalyzerTests.cs
    └── LinqQueryTests.cs
```

---

## 🚀 Getting Started

### Step 1: Create the Project

```bash
dotnet new console -n DataAnalysisPipeline
cd DataAnalysisPipeline
dotnet add package BenchmarkDotNet
dotnet new nunit -n DataAnalysisPipeline.Tests
dotnet add DataAnalysisPipeline.Tests reference DataAnalysisPipeline.csproj
```

### Step 2: Define Models

```csharp
// Models/Sale.cs
namespace DataAnalysisPipeline.Models;

public class Sale
{
    public int SaleId { get; set; }
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public DateTime SaleDate { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount => Quantity * UnitPrice;
    public string Region { get; set; }
}
```

```csharp
// Models/Customer.cs
namespace DataAnalysisPipeline.Models;

public class Customer
{
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Country { get; set; }
    public DateTime RegisterDate { get; set; }
}
```

```csharp
// Models/Product.cs
namespace DataAnalysisPipeline.Models;

public class Product
{
    public int ProductId { get; set; }
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
}
```

### Step 3: Implement CSV Reader

```csharp
// Services/CsvReaderService.cs
using DataAnalysisPipeline.Models;

namespace DataAnalysisPipeline.Services;

public class CsvReaderService
{
    // TODO: Implement ReadSales(string filePath)
    public List<Sale> ReadSales(string filePath)
    {
        // TODO: Read CSV file, parse to Sale objects
        // Handle header row
        // Validate data
        // Return list
        throw new NotImplementedException();
    }

    // TODO: Implement ReadCustomers(string filePath)
    public List<Customer> ReadCustomers(string filePath)
    {
        // TODO: Similar to ReadSales
        throw new NotImplementedException();
    }

    // TODO: Implement ReadProducts(string filePath)
    public List<Product> ReadProducts(string filePath)
    {
        // TODO: Similar to ReadSales
        throw new NotImplementedException();
    }
}
```

### Step 4: Implement Sales Analyzer

```csharp
// Services/SalesAnalyzer.cs
using DataAnalysisPipeline.Models;

namespace DataAnalysisPipeline.Services;

public class SalesAnalyzer
{
    private readonly List<Sale> _sales;
    private readonly List<Customer> _customers;
    private readonly List<Product> _products;

    public SalesAnalyzer(List<Sale> sales, List<Customer> customers, List<Product> products)
    {
        _sales = sales;
        _customers = customers;
        _products = products;
    }

    // TODO: Query 1 - Total sales amount
    public decimal GetTotalSales()
    {
        // TODO: Use LINQ Sum()
        throw new NotImplementedException();
    }

    // TODO: Query 2 - Average order value
    public decimal GetAverageOrderValue()
    {
        // TODO: Use LINQ Average()
        throw new NotImplementedException();
    }

    // TODO: Query 3 - Total sales by product
    public Dictionary<string, decimal> GetSalesByProduct()
    {
        // TODO: Join Sales with Products
        // Group by product name
        // Sum total amount
        // Order by amount descending
        throw new NotImplementedException();
    }

    // TODO: Query 4 - Top 10 products by revenue
    public IEnumerable<(string ProductName, decimal Revenue)> GetTop10Products()
    {
        // TODO: Join, Group, Sum, OrderByDescending, Take(10)
        throw new NotImplementedException();
    }

    // TODO: Query 5 - Sales by month
    public Dictionary<string, decimal> GetSalesByMonth()
    {
        // TODO: Group by SaleDate.Month
        // Format as "2024-01"
        // Sum amounts
        throw new NotImplementedException();
    }

    // TODO: Query 6 - Sales by region
    public Dictionary<string, decimal> GetSalesByRegion()
    {
        // TODO: GroupBy Region, Sum
        throw new NotImplementedException();
    }

    // TODO: Query 7 - Sales by category
    public Dictionary<string, decimal> GetSalesByCategory()
    {
        // TODO: Join Sales with Products
        // Group by Product.Category
        // Sum amounts
        throw new NotImplementedException();
    }

    // TODO: Query 8 - Daily sales (last 30 days)
    public IEnumerable<(DateTime Date, decimal Amount)> GetDailySales(int days = 30)
    {
        // TODO: Filter by date range
        // Group by date
        // Sum amounts
        // Order by date
        throw new NotImplementedException();
    }

    // TODO: Query 9 - Sales with product and customer info (join 3 tables)
    public IEnumerable<dynamic> GetDetailedSales()
    {
        // TODO: Join Sales -> Products -> Customers
        // Select with all info
        throw new NotImplementedException();
    }

    // TODO: Query 10 - Category performance statistics
    public IEnumerable<dynamic> GetCategoryStats()
    {
        // TODO: Join Sales with Products
        // Group by Category
        // Calculate: Count, Sum, Average, Min, Max
        throw new NotImplementedException();
    }
}
```

### Step 5: Implement Customer Analyzer

```csharp
// Services/CustomerAnalyzer.cs
using DataAnalysisPipeline.Models;

namespace DataAnalysisPipeline.Services;

public class CustomerAnalyzer
{
    private readonly List<Sale> _sales;
    private readonly List<Customer> _customers;

    public CustomerAnalyzer(List<Sale> sales, List<Customer> customers)
    {
        _sales = sales;
        _customers = customers;
    }

    // TODO: Query 11 - Top 10 customers by total spent
    public IEnumerable<(string CustomerName, decimal TotalSpent, int OrderCount)> GetTop10Customers()
    {
        // TODO: Join Sales with Customers
        // Group by customer
        // Sum amounts, count orders
        // Order descending
        // Take 10
        throw new NotImplementedException();
    }

    // TODO: Query 12 - Customer segmentation
    public Dictionary<string, int> GetCustomerSegmentation()
    {
        // TODO: Calculate total spent per customer
        // Segment: High (>$1000), Medium ($500-$1000), Low (<$500)
        // Count per segment
        throw new NotImplementedException();
    }

    // TODO: Query 13 - Customers with no purchases
    public IEnumerable<Customer> GetInactiveCustomers()
    {
        // TODO: Left join Customers with Sales
        // Filter where no sales
        throw new NotImplementedException();
    }

    // TODO: Query 14 - Average purchases per customer
    public decimal GetAveragePurchasesPerCustomer()
    {
        // TODO: Group sales by customer
        // Calculate average
        throw new NotImplementedException();
    }

    // TODO: Query 15 - Customer retention (purchases > 1)
    public int GetRetainedCustomersCount()
    {
        // TODO: Group by customer
        // Count where orders > 1
        throw new NotImplementedException();
    }
}
```

### Step 6: Implement Functional Extensions

```csharp
// Utilities/FunctionalExtensions.cs
namespace DataAnalysisPipeline.Utilities;

public static class FunctionalExtensions
{
    // TODO: Implement Map (similar to Select)
    public static IEnumerable<TResult> Map<T, TResult>(
        this IEnumerable<T> source,
        Func<T, TResult> selector)
    {
        // TODO: Implement using yield return
        throw new NotImplementedException();
    }

    // TODO: Implement Filter (similar to Where)
    public static IEnumerable<T> Filter<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        // TODO: Implement using yield return
        throw new NotImplementedException();
    }

    // TODO: Implement Reduce (similar to Aggregate)
    public static TResult Reduce<T, TResult>(
        this IEnumerable<T> source,
        TResult seed,
        Func<TResult, T, TResult> accumulator)
    {
        // TODO: Implement fold/reduce
        throw new NotImplementedException();
    }

    // TODO: Implement Compose (function composition)
    public static Func<T, TResult> Compose<T, TIntermediate, TResult>(
        this Func<T, TIntermediate> first,
        Func<TIntermediate, TResult> second)
    {
        // TODO: Return composed function
        throw new NotImplementedException();
    }
}
```

### Step 7: Implement Exporters

```csharp
// Exporters/IExporter.cs
namespace DataAnalysisPipeline.Exporters;

public interface IExporter
{
    void Export<T>(IEnumerable<T> data, string filePath);
}
```

```csharp
// Exporters/JsonExporter.cs
using System.Text.Json;

namespace DataAnalysisPipeline.Exporters;

public class JsonExporter : IExporter
{
    public void Export<T>(IEnumerable<T> data, string filePath)
    {
        // TODO: Serialize to JSON and write to file
        var options = new JsonSerializerOptions { WriteIndented = true };
        // TODO: Implement
        throw new NotImplementedException();
    }
}
```

### Step 8: Generate Sample Data

```csharp
// Utilities/DataGenerator.cs
public static class DataGenerator
{
    public static void GenerateSampleData()
    {
        // TODO: Generate 1000+ sales records
        // TODO: Generate 100+ customers
        // TODO: Generate 50+ products
        // Save to CSV files in Data/ folder
    }
}
```

### Sample CSV Format:

**sales.csv**:
```csv
SaleId,ProductId,CustomerId,SaleDate,Quantity,UnitPrice,Region
1,101,1001,2024-01-15,2,29.99,North
2,102,1002,2024-01-16,1,49.99,South
3,101,1001,2024-01-17,3,29.99,North
...
```

---

## 🎯 Milestones

### Milestone 1: Data Loading (Day 1-2)
- ✅ CSV reader implemented
- ✅ Sample data generated (1000+ rows)
- ✅ Models defined
- ✅ Data validation working

### Milestone 2: Core LINQ Queries (Day 3-5)
- ✅ 10 sales analysis queries implemented
- ✅ 5 customer analysis queries implemented
- ✅ 5 product analysis queries implemented
- ✅ All queries tested with sample data

### Milestone 3: Functional Programming (Day 6-7)
- ✅ Map, Filter, Reduce implemented
- ✅ Function composition working
- ✅ Closures demonstrated
- ✅ Higher-order functions used

### Milestone 4: Export & UI (Day 8-9)
- ✅ JSON, CSV, XML exporters working
- ✅ Console UI complete
- ✅ All reports can be generated
- ✅ Performance tested

### Milestone 5: Testing & Benchmarks (Day 10)
- ✅ 15+ unit tests passing
- ✅ BenchmarkDotNet tests running
- ✅ Performance optimized
- ✅ Documentation complete

---

## ✅ Evaluation Criteria

| Criteria | Points | Requirements |
|----------|--------|--------------|
| **Functionality** | 40 | All 20+ LINQ queries working |
| **Code Quality** | 30 | Functional programming, clean code |
| **Tests** | 15 | 15+ tests passing |
| **Performance** | 10 | Handles 10k+ rows efficiently |
| **Documentation** | 5 | README, comments |
| **TOTAL** | **100** | **Pass: 75+** |

### Detailed Rubric

**Functionality (40 points)**:
- 20+ LINQ queries: 20 pts
- Data import/export: 10 pts
- Console UI: 5 pts
- Statistical reports: 5 pts

**Code Quality (30 points)**:
- Functional programming patterns: 10 pts
- Clean code and organization: 10 pts
- Proper use of LINQ: 5 pts
- Error handling: 5 pts

**Tests (15 points)**:
- 15+ tests: 10 pts
- Tests pass: 5 pts

**Performance (10 points)**:
- Handles 10k+ rows: 5 pts
- Benchmarks included: 5 pts

---

## 💡 Tips

1. **Start with Small Data**: Test with 10 rows first, then scale up
2. **Use Query Syntax**: Practice both query and method syntax
3. **Deferred Execution**: Understand when queries execute
4. **Joins**: Master inner join and left join patterns
5. **Grouping**: Practice multi-level grouping
6. **Performance**: Profile with BenchmarkDotNet
7. **Functional**: Use Map, Filter, Reduce where appropriate
8. **Export**: Test all export formats

---

## 🚀 Extensions (Optional)

1. **Advanced Analytics**: Trend analysis, forecasting
2. **Data Visualization**: Generate charts (using a library)
3. **Real-time Processing**: Stream processing simulation
4. **More Queries**: Expand to 30+ queries
5. **Database**: Use EF Core instead of CSV
6. **Web API**: Expose queries via REST API
7. **Caching**: Implement query result caching

---

## 📚 Resources

- **Reference Material**:
  - `samples/99-Exercises/LINQ/` (all 3 exercises)
  - Review all LINQ solutions

- **LINQ Documentation**:
  - [LINQ Query Syntax](https://learn.microsoft.com/en-us/dotnet/csharp/linq/)
  - [Grouping Data](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/grouping-data)
  - [Join Operations](https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/join-operations)

- **BenchmarkDotNet**:
  - https://benchmarkdotnet.org/

---

*Template Version: 1.0*
*Last Updated: 2025-12-02*
