using FluentAssertions;
using NUnit.Framework;

namespace GroupingAggregation.Tests;

[TestFixture]
public class LinqGroupingTests
{
    private List<Product> _products = null!;

    [SetUp]
    public void Setup()
    {
        _products = Program.GetSampleProducts();
    }

    // ========== TODO 1: GroupByCategory Tests ==========
    [Test]
    public void GroupByCategory_ShouldReturnGroupsForEachCategory()
    {
        // Act
        var result = Program.GroupByCategory(_products).ToList();

        // Assert
        result.Should().HaveCount(3, "there are 3 categories: Electronics, Furniture, Stationery");

        var categoryNames = result.Select(g => g.Key).ToList();
        categoryNames.Should().Contain(new[] { "Electronics", "Furniture", "Stationery" });
    }

    [Test]
    public void GroupByCategory_ShouldHaveCorrectProductCounts()
    {
        // Act
        var result = Program.GroupByCategory(_products).ToList();

        // Assert
        var electronicsGroup = result.First(g => g.Key == "Electronics");
        electronicsGroup.Should().HaveCount(6, "there are 6 electronics products");

        var furnitureGroup = result.First(g => g.Key == "Furniture");
        furnitureGroup.Should().HaveCount(5, "there are 5 furniture products");

        var stationeryGroup = result.First(g => g.Key == "Stationery");
        stationeryGroup.Should().HaveCount(4, "there are 4 stationery products");
    }

    [Test]
    public void GroupByCategory_ShouldContainCorrectProducts()
    {
        // Act
        var result = Program.GroupByCategory(_products).ToList();

        // Assert
        var electronicsGroup = result.First(g => g.Key == "Electronics");
        var electronicsNames = electronicsGroup.Select(p => p.Name).ToList();
        electronicsNames.Should().Contain(new[] { "Laptop", "Mouse", "Keyboard", "Monitor", "Tablet", "Webcam" });
    }

    // ========== TODO 2: CalculateAveragePricePerCategory Tests ==========
    [Test]
    public void CalculateAveragePricePerCategory_ShouldReturnCorrectAverages()
    {
        // Act
        var result = Program.CalculateAveragePricePerCategory(_products);

        // Assert
        result.Should().HaveCount(3, "there are 3 categories");
        result.Keys.Should().Contain(new[] { "Electronics", "Furniture", "Stationery" });

        // Electronics: (1200 + 25 + 75 + 300 + 600 + 80) / 6 = 380
        result["Electronics"].Should().BeApproximately(380m, 0.01m);

        // Furniture: (450 + 200 + 40 + 350 + 800) / 5 = 368
        result["Furniture"].Should().BeApproximately(368m, 0.01m);

        // Stationery: (5 + 15 + 20 + 12) / 4 = 13
        result["Stationery"].Should().BeApproximately(13m, 0.01m);
    }

    [Test]
    public void CalculateAveragePricePerCategory_ShouldReturnDictionary()
    {
        // Act
        var result = Program.CalculateAveragePricePerCategory(_products);

        // Assert
        result.Should().BeOfType<Dictionary<string, decimal>>();
        result.Should().ContainKeys("Electronics", "Furniture", "Stationery");
    }

    // ========== TODO 3: GetCategoryStats Tests ==========
    [Test]
    public void GetCategoryStats_ShouldReturnStatsForAllCategories()
    {
        // Act
        var result = Program.GetCategoryStats(_products);

        // Assert
        result.Should().HaveCount(3, "there are 3 categories");
        result.Select(s => s.Category).Should().Contain(new[] { "Electronics", "Furniture", "Stationery" });
    }

    [Test]
    public void GetCategoryStats_ElectronicsShouldHaveCorrectStats()
    {
        // Act
        var result = Program.GetCategoryStats(_products);

        // Assert
        var electronics = result.First(s => s.Category == "Electronics");
        electronics.ProductCount.Should().Be(6);
        electronics.AveragePrice.Should().BeApproximately(380m, 0.01m);
        electronics.MinPrice.Should().Be(25m, "Mouse is cheapest at $25");
        electronics.MaxPrice.Should().Be(1200m, "Laptop is most expensive at $1200");

        // Total Value = (1200*5) + (25*50) + (75*30) + (300*10) + (600*12) + (80*20)
        //             = 6000 + 1250 + 2250 + 3000 + 7200 + 1600 = 21300
        electronics.TotalValue.Should().Be(21300m);
    }

    [Test]
    public void GetCategoryStats_FurnatureShouldHaveCorrectStats()
    {
        // Act
        var result = Program.GetCategoryStats(_products);

        // Assert
        var furniture = result.First(s => s.Category == "Furniture");
        furniture.ProductCount.Should().Be(5);
        furniture.MinPrice.Should().Be(40m);
        furniture.MaxPrice.Should().Be(800m);

        // Total Value = (450*3) + (200*8) + (40*15) + (350*2) + (800*4)
        //             = 1350 + 1600 + 600 + 700 + 3200 = 7450
        furniture.TotalValue.Should().Be(7450m);
    }

    [Test]
    public void GetCategoryStats_StationeryShouldHaveCorrectStats()
    {
        // Act
        var result = Program.GetCategoryStats(_products);

        // Assert
        var stationery = result.First(s => s.Category == "Stationery");
        stationery.ProductCount.Should().Be(4);
        stationery.MinPrice.Should().Be(5m);
        stationery.MaxPrice.Should().Be(20m);
        stationery.AveragePrice.Should().BeApproximately(13m, 0.01m);
    }

    // ========== TODO 4: GetTopCategoriesByTotalValue Tests ==========
    [Test]
    public void GetTopCategoriesByTotalValue_ShouldReturnTop2Categories()
    {
        // Act
        var result = Program.GetTopCategoriesByTotalValue(_products, 2);

        // Assert
        result.Should().HaveCount(2);
        result[0].Should().Be("Electronics", "Electronics has highest total value ($21,300)");
        result[1].Should().Be("Furniture", "Furniture has second highest total value ($7,450)");
    }

    [Test]
    public void GetTopCategoriesByTotalValue_ShouldReturnAllWhenTopNIsLarge()
    {
        // Act
        var result = Program.GetTopCategoriesByTotalValue(_products, 10);

        // Assert
        result.Should().HaveCount(3, "there are only 3 categories");
        result[0].Should().Be("Electronics");
        result[1].Should().Be("Furniture");
        result[2].Should().Be("Stationery");
    }

    [Test]
    public void GetTopCategoriesByTotalValue_ShouldReturnTop1()
    {
        // Act
        var result = Program.GetTopCategoriesByTotalValue(_products, 1);

        // Assert
        result.Should().HaveCount(1);
        result[0].Should().Be("Electronics");
    }

    // ========== TODO 5: CountProductsBySupplier Tests ==========
    [Test]
    public void CountProductsBySupplier_ShouldReturnCorrectCounts()
    {
        // Act
        var result = Program.CountProductsBySupplier(_products);

        // Assert
        result.Should().HaveCount(4, "there are 4 suppliers");
        result["TechCorp"].Should().Be(4, "TechCorp supplies 4 products");
        result["FurniturePlus"].Should().Be(5, "FurniturePlus supplies 5 products");
        result["OfficePro"].Should().Be(4, "OfficePro supplies 4 products");
        result["DisplayCo"].Should().Be(2, "DisplayCo supplies 2 products");
    }

    [Test]
    public void CountProductsBySupplier_ShouldReturnDictionary()
    {
        // Act
        var result = Program.CountProductsBySupplier(_products);

        // Assert
        result.Should().BeOfType<Dictionary<string, int>>();
        result.Keys.Should().Contain(new[] { "TechCorp", "FurniturePlus", "OfficePro", "DisplayCo" });
    }

    // ========== EDGE CASE TESTS ==========
    [Test]
    public void GroupByCategory_ShouldHandleEmptyList()
    {
        // Act
        var result = Program.GroupByCategory(new List<Product>()).ToList();

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void CalculateAveragePricePerCategory_ShouldHandleEmptyList()
    {
        // Act
        var result = Program.CalculateAveragePricePerCategory(new List<Product>());

        // Assert
        result.Should().BeEmpty();
    }

    [Test]
    public void GetCategoryStats_ShouldHandleSingleCategory()
    {
        // Arrange
        var singleCategoryProducts = new List<Product>
        {
            new Product { Id = 1, Name = "Item1", Category = "Test", Price = 100, StockQuantity = 5, Supplier = "SupplierA" },
            new Product { Id = 2, Name = "Item2", Category = "Test", Price = 200, StockQuantity = 3, Supplier = "SupplierA" }
        };

        // Act
        var result = Program.GetCategoryStats(singleCategoryProducts);

        // Assert
        result.Should().HaveCount(1);
        result[0].Category.Should().Be("Test");
        result[0].ProductCount.Should().Be(2);
        result[0].AveragePrice.Should().Be(150m);
        result[0].MinPrice.Should().Be(100m);
        result[0].MaxPrice.Should().Be(200m);
        result[0].TotalValue.Should().Be(1100m); // (100*5) + (200*3) = 500 + 600
    }
}
