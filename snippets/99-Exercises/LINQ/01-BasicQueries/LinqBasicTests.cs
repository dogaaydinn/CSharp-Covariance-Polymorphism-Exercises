using FluentAssertions;
using NUnit.Framework;

namespace BasicQueries.Tests;

[TestFixture]
public class LinqBasicTests
{
    private List<Product> _products = null!;

    [SetUp]
    public void Setup()
    {
        _products = Program.GetSampleProducts();
    }

    // ========== TODO 1: GetExpensiveProducts Tests ==========
    [Test]
    public void GetExpensiveProducts_ShouldReturnOnlyProductsOver100()
    {
        // Act
        var result = Program.GetExpensiveProducts(_products);

        // Assert
        result.Should().HaveCount(5, "there are 5 products with price > 100");
        result.Should().OnlyContain(p => p.Price > 100, "all products should have price > 100");
        result.Select(p => p.Name).Should().Contain(new[] { "Laptop", "Monitor", "Desk", "Chair", "Bookshelf" });
    }

    [Test]
    public void GetExpensiveProducts_ShouldReturnEmptyList_WhenNoExpensiveProducts()
    {
        // Arrange
        var cheapProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Item1", Price = 50 },
            new Product { Id = 2, Name = "Item2", Price = 75 }
        };

        // Act
        var result = Program.GetExpensiveProducts(cheapProducts);

        // Assert
        result.Should().BeEmpty("no products have price > 100");
    }

    // ========== TODO 2: GetInStockProducts Tests ==========
    [Test]
    public void GetInStockProducts_ShouldReturnOnlyInStockAndActiveProducts()
    {
        // Act
        var result = Program.GetInStockProducts(_products);

        // Assert
        result.Should().HaveCount(9, "there are 9 products that are in stock AND active");
        result.Should().OnlyContain(p => p.StockQuantity > 0 && p.IsActive, "all products should be in stock and active");
        result.Should().NotContain(p => p.Name == "Lamp", "Lamp is not active");
    }

    // ========== TODO 3: OrderByCategoryThenPrice Tests ==========
    [Test]
    public void OrderByCategoryThenPrice_ShouldOrderCorrectly()
    {
        // Act
        var result = Program.OrderByCategoryThenPrice(_products);

        // Assert
        result.Should().HaveCount(10);

        // Check first items in each category
        result[0].Category.Should().Be("Electronics");
        result[0].Price.Should().Be(25, "Mouse is cheapest in Electronics");

        result[4].Category.Should().Be("Furniture");
        result[4].Price.Should().Be(40, "Lamp is cheapest in Furniture");

        result[8].Category.Should().Be("Stationery");
        result[8].Price.Should().Be(5, "Notebook is cheapest in Stationery");

        // Verify categories are grouped together
        var categories = result.Select(p => p.Category).ToList();
        var expectedCategoryOrder = new[] { "Electronics", "Electronics", "Electronics", "Electronics", "Furniture", "Furniture", "Furniture", "Furniture", "Stationery", "Stationery" };
        categories.Should().Equal(expectedCategoryOrder);
    }

    // ========== TODO 4: OrderByMostRecent Tests ==========
    [Test]
    public void OrderByMostRecent_ShouldOrderByDateDescending()
    {
        // Act
        var result = Program.OrderByMostRecent(_products);

        // Assert
        result.Should().HaveCount(10);
        result[0].Name.Should().Be("Bookshelf", "it was added on 2024-03-10");
        result[1].Name.Should().Be("Monitor", "it was added on 2024-03-05");
        result[2].Name.Should().Be("Notebook", "it was added on 2024-03-01");

        // Verify dates are in descending order
        for (int i = 0; i < result.Count - 1; i++)
        {
            result[i].AddedDate.Should().BeOnOrAfter(result[i + 1].AddedDate, "dates should be in descending order");
        }
    }

    // ========== TODO 5: GetProductNames Tests ==========
    [Test]
    public void GetProductNames_ShouldReturnAllProductNames()
    {
        // Act
        var result = Program.GetProductNames(_products);

        // Assert
        result.Should().HaveCount(10);
        result.Should().BeOfType<List<string>>();
        result.Should().Contain(new[] { "Laptop", "Mouse", "Keyboard", "Monitor", "Desk", "Chair", "Lamp", "Notebook", "Pen Set", "Bookshelf" });
    }

    [Test]
    public void GetProductNames_ShouldReturnEmptyList_WhenNoProducts()
    {
        // Act
        var result = Program.GetProductNames(new List<Product>());

        // Assert
        result.Should().BeEmpty();
    }

    // ========== TODO 6: GetProductSummaries Tests ==========
    [Test]
    public void GetProductSummaries_ShouldReturnAnonymousTypesWithNameAndPrice()
    {
        // Act
        var result = Program.GetProductSummaries(_products);

        // Assert
        result.Should().HaveCount(10);

        // Check first item has Name and Price properties
        dynamic firstItem = result[0];
        string name = firstItem.Name;
        decimal price = firstItem.Price;

        name.Should().Be("Laptop");
        price.Should().Be(1200);

        // Check that all items are anonymous types with these properties
        foreach (dynamic item in result)
        {
            string itemName = item.Name;
            decimal itemPrice = item.Price;

            itemName.Should().NotBeNullOrEmpty();
            itemPrice.Should().BeGreaterThanOrEqualTo(0);
        }
    }

    // ========== EDGE CASE TESTS ==========
    [Test]
    public void GetExpensiveProducts_ShouldHandleEmptyList()
    {
        // Act
        var result = Program.GetExpensiveProducts(new List<Product>());

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void OrderByCategoryThenPrice_ShouldHandleSingleItem()
    {
        // Arrange
        var singleProduct = new List<Product>
        {
            new Product { Id = 1, Name = "Test", Category = "Test", Price = 100 }
        };

        // Act
        var result = Program.OrderByCategoryThenPrice(singleProduct);

        // Assert
        result.Should().HaveCount(1);
        result[0].Name.Should().Be("Test");
    }
}
