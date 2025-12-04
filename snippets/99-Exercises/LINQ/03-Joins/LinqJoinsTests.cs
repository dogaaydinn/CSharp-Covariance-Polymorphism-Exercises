using FluentAssertions;
using NUnit.Framework;

namespace Joins.Tests;

[TestFixture]
public class LinqJoinsTests
{
    private List<Product> _products = null!;
    private List<Supplier> _suppliers = null!;
    private List<Category> _categories = null!;
    private List<Order> _orders = null!;

    [SetUp]
    public void Setup()
    {
        _products = Program.GetSampleProducts();
        _suppliers = Program.GetSampleSuppliers();
        _categories = Program.GetSampleCategories();
        _orders = Program.GetSampleOrders();
    }

    // ========== TODO 1: InnerJoinProductsWithSuppliers Tests ==========
    [Test]
    public void InnerJoinProductsWithSuppliers_ShouldReturnAllProducts()
    {
        // Act
        var result = Program.InnerJoinProductsWithSuppliers(_products, _suppliers);

        // Assert
        result.Should().HaveCount(12, "all 12 products have suppliers");
    }

    [Test]
    public void InnerJoinProductsWithSuppliers_ShouldMatchCorrectSuppliers()
    {
        // Act
        var result = Program.InnerJoinProductsWithSuppliers(_products, _suppliers);

        // Assert
        var laptop = result.First(p => p.ProductName == "Laptop");
        laptop.SupplierName.Should().Be("TechCorp");
        laptop.SupplierCountry.Should().Be("USA");

        var desk = result.First(p => p.ProductName == "Desk");
        desk.SupplierName.Should().Be("FurniturePlus");
        desk.SupplierCountry.Should().Be("Sweden");

        var monitor = result.First(p => p.ProductName == "Monitor");
        monitor.SupplierName.Should().Be("DisplayCo");
        monitor.SupplierCountry.Should().Be("Japan");
    }

    [Test]
    public void InnerJoinProductsWithSuppliers_ShouldIncludePriceInformation()
    {
        // Act
        var result = Program.InnerJoinProductsWithSuppliers(_products, _suppliers);

        // Assert
        var laptop = result.First(p => p.ProductName == "Laptop");
        laptop.Price.Should().Be(1200m);
        laptop.ProductId.Should().Be(1);
    }

    [Test]
    public void InnerJoinProductsWithSuppliers_ShouldHandleEmptyLists()
    {
        // Act
        var result = Program.InnerJoinProductsWithSuppliers(new List<Product>(), _suppliers);

        // Assert
        result.Should().BeEmpty("no products to join");
    }

    // ========== TODO 2: LeftJoinProductsWithOrders Tests ==========
    [Test]
    public void LeftJoinProductsWithOrders_ShouldIncludeAllProducts()
    {
        // Act
        var result = Program.LeftJoinProductsWithOrders(_products, _orders);

        // Assert
        // All products should appear at least once
        var productIds = result.Select(r => r.ProductId).Distinct().ToList();
        productIds.Should().HaveCount(12, "all 12 products should be included");
    }

    [Test]
    public void LeftJoinProductsWithOrders_ShouldShowProductsWithoutOrders()
    {
        // Act
        var result = Program.LeftJoinProductsWithOrders(_products, _orders);

        // Assert
        // Products 3, 6, 7, 11, 12 have no orders
        var productsWithoutOrders = result.Where(r => r.OrderId == null).ToList();
        productsWithoutOrders.Should().HaveCountGreaterThanOrEqualTo(5, "at least 5 products have no orders");

        var keyboardOrders = result.Where(r => r.ProductName == "Keyboard").ToList();
        keyboardOrders.Should().HaveCount(1);
        keyboardOrders[0].OrderId.Should().BeNull("Keyboard has no orders");
        keyboardOrders[0].Quantity.Should().BeNull();
    }

    [Test]
    public void LeftJoinProductsWithOrders_ShouldShowProductsWithMultipleOrders()
    {
        // Act
        var result = Program.LeftJoinProductsWithOrders(_products, _orders);

        // Assert
        // Product 1 (Laptop) has 3 orders
        var laptopOrders = result.Where(r => r.ProductName == "Laptop").ToList();
        laptopOrders.Should().HaveCount(3, "Laptop has 3 orders");
        laptopOrders.All(o => o.OrderId.HasValue).Should().BeTrue("all laptop entries should have orders");
    }

    [Test]
    public void LeftJoinProductsWithOrders_ShouldIncludeOrderDetails()
    {
        // Act
        var result = Program.LeftJoinProductsWithOrders(_products, _orders);

        // Assert
        var laptopOrder = result.First(r => r.ProductName == "Laptop" && r.OrderId == 1);
        laptopOrder.Quantity.Should().Be(2);
        laptopOrder.CustomerName.Should().Be("Alice");
    }

    [Test]
    public void LeftJoinProductsWithOrders_ShouldHandleEmptyOrders()
    {
        // Act
        var result = Program.LeftJoinProductsWithOrders(_products, new List<Order>());

        // Assert
        result.Should().HaveCount(12, "all products should be included");
        result.All(r => r.OrderId == null).Should().BeTrue("no orders exist");
    }

    // ========== TODO 3: MultipleJoins Tests ==========
    [Test]
    public void MultipleJoins_ShouldCombineAllInformation()
    {
        // Act
        var result = Program.MultipleJoins(_products, _categories, _suppliers, _orders);

        // Assert
        result.Should().HaveCount(12, "one entry per product");

        var laptop = result.First(p => p.ProductName == "Laptop");
        laptop.CategoryName.Should().Be("Electronics");
        laptop.SupplierName.Should().Be("TechCorp");
        laptop.Price.Should().Be(1200m);
        laptop.TotalOrders.Should().Be(3, "Laptop has 3 orders");
    }

    [Test]
    public void MultipleJoins_ShouldCountOrdersCorrectly()
    {
        // Act
        var result = Program.MultipleJoins(_products, _categories, _suppliers, _orders);

        // Assert
        var productWithNoOrders = result.First(p => p.ProductName == "Keyboard");
        productWithNoOrders.TotalOrders.Should().Be(0, "Keyboard has no orders");

        var mouse = result.First(p => p.ProductName == "Mouse");
        mouse.TotalOrders.Should().Be(1, "Mouse has 1 order");
    }

    [Test]
    public void MultipleJoins_ShouldIncludeFurnitureCategory()
    {
        // Act
        var result = Program.MultipleJoins(_products, _categories, _suppliers, _orders);

        // Assert
        var furnitureProducts = result.Where(p => p.CategoryName == "Furniture").ToList();
        furnitureProducts.Should().HaveCount(4, "there are 4 furniture products");
        furnitureProducts.All(p => p.SupplierName == "FurniturePlus").Should().BeTrue();
    }

    [Test]
    public void MultipleJoins_ShouldIncludeStationeryCategory()
    {
        // Act
        var result = Program.MultipleJoins(_products, _categories, _suppliers, _orders);

        // Assert
        var stationeryProducts = result.Where(p => p.CategoryName == "Stationery").ToList();
        stationeryProducts.Should().HaveCount(2, "there are 2 stationery products");

        var notebook = stationeryProducts.First(p => p.ProductName == "Notebook");
        notebook.SupplierName.Should().Be("OfficePro");
        notebook.TotalOrders.Should().Be(1);
    }

    // ========== TODO 4: GroupJoinProductsByCategory Tests ==========
    [Test]
    public void GroupJoinProductsByCategory_ShouldReturnAllCategories()
    {
        // Act
        var result = Program.GroupJoinProductsByCategory(_categories, _products);

        // Assert
        result.Should().HaveCount(4, "there are 4 categories");
        result.Select(c => c.CategoryName).Should().Contain(new[] { "Electronics", "Furniture", "Stationery", "Books" });
    }

    [Test]
    public void GroupJoinProductsByCategory_ElectronicsShouldHaveCorrectProducts()
    {
        // Act
        var result = Program.GroupJoinProductsByCategory(_categories, _products);

        // Assert
        var electronics = result.First(c => c.CategoryName == "Electronics");
        electronics.ProductNames.Should().HaveCount(6, "Electronics has 6 products");
        electronics.ProductNames.Should().Contain(new[] { "Laptop", "Mouse", "Keyboard", "Monitor", "Tablet", "Webcam" });
    }

    [Test]
    public void GroupJoinProductsByCategory_FurnitureShouldHaveCorrectProducts()
    {
        // Act
        var result = Program.GroupJoinProductsByCategory(_categories, _products);

        // Assert
        var furniture = result.First(c => c.CategoryName == "Furniture");
        furniture.ProductNames.Should().HaveCount(4, "Furniture has 4 products");
        furniture.ProductNames.Should().Contain(new[] { "Desk", "Chair", "Lamp", "Bookshelf" });
    }

    [Test]
    public void GroupJoinProductsByCategory_BooksShouldBeEmpty()
    {
        // Act
        var result = Program.GroupJoinProductsByCategory(_categories, _products);

        // Assert
        var books = result.First(c => c.CategoryName == "Books");
        books.ProductNames.Should().BeEmpty("Books category has no products");
    }

    [Test]
    public void GroupJoinProductsByCategory_ShouldHandleEmptyCategories()
    {
        // Act
        var result = Program.GroupJoinProductsByCategory(new List<Category>(), _products);

        // Assert
        result.Should().BeEmpty("no categories to group by");
    }

    // ========== EDGE CASE TESTS ==========
    [Test]
    public void InnerJoinProductsWithSuppliers_ShouldReturnEmptyWhenNoSuppliers()
    {
        // Act
        var result = Program.InnerJoinProductsWithSuppliers(_products, new List<Supplier>());

        // Assert
        result.Should().BeEmpty("no suppliers to match");
    }

    [Test]
    public void MultipleJoins_ShouldHandleSingleProduct()
    {
        // Arrange
        var singleProduct = new List<Product>
        {
            new Product { Id = 1, Name = "Test", SupplierId = 1, CategoryId = 1, Price = 100 }
        };

        // Act
        var result = Program.MultipleJoins(singleProduct, _categories, _suppliers, _orders);

        // Assert
        result.Should().HaveCount(1);
        result[0].ProductName.Should().Be("Test");
    }
}
