using FluentAssertions;
using NUnit.Framework;

namespace GenericConstraints.Tests;

[TestFixture]
public class GenericConstraintsTests
{
    // ========== TODO 1: Repository<T> where T : class, new() ==========
    [Test]
    public void Repository_Add_ShouldAddItem()
    {
        var repo = new Program.Repository<Product>();
        var product = new Product("Test", 10.0m);

        repo.Add(product);

        repo.GetAll().Should().Contain(product);
    }

    [Test]
    public void Repository_Create_ShouldCreateNewInstance()
    {
        var repo = new Program.Repository<Customer>();

        var customer = repo.Create();

        customer.Should().NotBeNull();
        customer.Should().BeOfType<Customer>();
    }

    // ========== TODO 2: ValueTypeProcessor<T> where T : struct ==========
    [Test]
    public void ValueTypeProcessor_GetDefault_ShouldReturnDefaultValue()
    {
        var processor = new Program.ValueTypeProcessor<int>();

        int result = processor.GetDefault();

        result.Should().Be(0);
    }

    [Test]
    public void ValueTypeProcessor_IsDefault_ShouldDetectDefaultValues()
    {
        var processor = new Program.ValueTypeProcessor<Point>();

        bool result1 = processor.IsDefault(new Point(0, 0));
        bool result2 = processor.IsDefault(new Point(1, 2));

        result1.Should().BeTrue();
        result2.Should().BeFalse();
    }

    // ========== TODO 3: EntityRepository<T> where T : Entity, new() ==========
    [Test]
    public void EntityRepository_Add_ShouldAddEntity()
    {
        var repo = new Program.EntityRepository<Product>();
        var product = new Product("Test", 10.0m) { Id = 1 };

        repo.Add(product);

        repo.FindById(1).Should().Be(product);
    }

    [Test]
    public void EntityRepository_FindById_ShouldReturnCorrectEntity()
    {
        var repo = new Program.EntityRepository<Customer>();
        var customer = new Customer("John", "john@test.com") { Id = 5 };
        repo.Add(customer);

        var found = repo.FindById(5);

        found.Should().NotBeNull();
        found!.Name.Should().Be("John");
    }

    [Test]
    public void EntityRepository_GetRecent_ShouldReturnMostRecent()
    {
        var repo = new Program.EntityRepository<Product>();
        Thread.Sleep(10);
        var p1 = new Product("P1", 10) { Id = 1 };
        repo.Add(p1);
        Thread.Sleep(10);
        var p2 = new Product("P2", 20) { Id = 2 };
        repo.Add(p2);

        var recent = repo.GetRecent(1);

        recent.Should().HaveCount(1);
        recent[0].Id.Should().Be(2);
    }

    // ========== TODO 4: SortedList<T> where T : IComparable<T> ==========
    [Test]
    public void SortedList_Add_ShouldMaintainSortedOrder()
    {
        var list = new Program.SortedList<int>();

        list.Add(5);
        list.Add(2);
        list.Add(8);
        list.Add(1);

        list.GetAll().Should().Equal(1, 2, 5, 8);
    }

    [Test]
    public void SortedList_GetMin_ShouldReturnSmallest()
    {
        var list = new Program.SortedList<Product>();
        list.Add(new Product("Expensive", 100));
        list.Add(new Product("Cheap", 10));

        var min = list.GetMin();

        min.Should().NotBeNull();
        min!.Price.Should().Be(10);
    }

    [Test]
    public void SortedList_GetMax_ShouldReturnLargest()
    {
        var list = new Program.SortedList<Point>();
        list.Add(new Point(1, 1));
        list.Add(new Point(3, 4));

        var max = list.GetMax();

        max.Should().NotBeNull();
        max!.CompareTo(new Point(3, 4)).Should().Be(0);
    }

    // ========== TODO 5: DisposableManager<T> where T : class, IDisposable, new() ==========
    [Test]
    public void DisposableManager_CreateAndTrack_ShouldCreateAndAddResource()
    {
        var manager = new Program.DisposableManager<Resource>();

        var resource = manager.CreateAndTrack();

        resource.Should().NotBeNull();
        manager.Count.Should().Be(1);
    }

    [Test]
    public void DisposableManager_DisposeAll_ShouldDisposeAllResources()
    {
        var manager = new Program.DisposableManager<Resource>();
        var r1 = manager.CreateAndTrack();
        var r2 = manager.CreateAndTrack();

        manager.DisposeAll();

        r1.IsDisposed.Should().BeTrue();
        r2.IsDisposed.Should().BeTrue();
    }

    // ========== TODO 6: CreateInstance<T>() where T : new() ==========
    [Test]
    public void CreateInstance_ShouldCreateNewInstance()
    {
        var product = Program.CreateInstance<Product>();

        product.Should().NotBeNull();
        product.Should().BeOfType<Product>();
    }

    [Test]
    public void CreateInstance_ShouldWorkWithValueTypes()
    {
        var point = Program.CreateInstance<Point>();

        point.Should().Be(default(Point));
    }

    // ========== Integration Test ==========
    [Test]
    public void GenericConstraints_Integration_ShouldWorkTogether()
    {
        // Test 1: Repository with class constraint
        var repo = new Program.Repository<Customer>();
        repo.Add(new Customer("Test", "test@test.com"));
        repo.GetAll().Should().HaveCount(1);

        // Test 2: SortedList with IComparable constraint
        var sorted = new Program.SortedList<int>();
        sorted.Add(5);
        sorted.Add(1);
        sorted.GetMin().Should().Be(1);

        // Test 3: Factory with new() constraint
        var product = Program.CreateInstance<Product>();
        product.Should().NotBeNull();
    }
}
