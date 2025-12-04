using FluentAssertions;
using NUnit.Framework;

namespace BuilderPattern.Tests;

[TestFixture]
public class BuilderPatternTests
{
    // ========== TODO 1: Pizza Class Tests ==========
    [Test]
    public void Pizza_WhenCreated_ShouldHaveDefaultValues()
    {
        // Act
        var pizza = new Pizza();

        // Assert
        pizza.Should().NotBeNull();
        pizza.Toppings.Should().NotBeNull();
        pizza.Toppings.Should().BeEmpty();
    }

    // ========== TODO 2: PizzaBuilder Fluent Interface Tests ==========
    [Test]
    public void PizzaBuilder_WithSize_ShouldSetSize()
    {
        // Arrange
        var builder = new PizzaBuilder();

        // Act
        var result = builder.WithSize(PizzaSize.Large);

        // Assert
        result.Should().BeSameAs(builder, "fluent interface should return this");
        var pizza = builder.Build();
        pizza.Size.Should().Be(PizzaSize.Large);
    }

    [Test]
    public void PizzaBuilder_WithDough_ShouldSetDoughType()
    {
        // Arrange & Act
        var pizza = new PizzaBuilder()
            .WithSize(PizzaSize.Medium)
            .WithDough(DoughType.Thick)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Cheese")
            .Build();

        // Assert
        pizza.Dough.Should().Be(DoughType.Thick);
    }

    [Test]
    public void PizzaBuilder_WithSauce_ShouldSetSauceType()
    {
        // Arrange & Act
        var pizza = new PizzaBuilder()
            .WithSize(PizzaSize.Small)
            .WithDough(DoughType.Thin)
            .WithSauce(SauceType.BBQ)
            .AddTopping("Cheese")
            .Build();

        // Assert
        pizza.Sauce.Should().Be(SauceType.BBQ);
    }

    [Test]
    public void PizzaBuilder_AddTopping_ShouldAddToToppingsList()
    {
        // Arrange & Act
        var pizza = new PizzaBuilder()
            .WithSize(PizzaSize.Medium)
            .WithDough(DoughType.Thin)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Mozzarella")
            .AddTopping("Pepperoni")
            .AddTopping("Mushrooms")
            .Build();

        // Assert
        pizza.Toppings.Should().HaveCount(3);
        pizza.Toppings.Should().Contain(new[] { "Mozzarella", "Pepperoni", "Mushrooms" });
    }

    [Test]
    public void PizzaBuilder_WithExtraCheese_ShouldSetExtraCheeseTrue()
    {
        // Arrange & Act
        var pizza = new PizzaBuilder()
            .WithSize(PizzaSize.Large)
            .WithDough(DoughType.Thick)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Cheese")
            .WithExtraCheese()
            .Build();

        // Assert
        pizza.ExtraCheese.Should().BeTrue();
    }

    [Test]
    public void PizzaBuilder_WithSpicyLevel_ShouldSetSpicyLevel()
    {
        // Arrange & Act
        var pizza = new PizzaBuilder()
            .WithSize(PizzaSize.Medium)
            .WithDough(DoughType.Thin)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Cheese")
            .WithSpicyLevel(4)
            .Build();

        // Assert
        pizza.SpicyLevel.Should().Be(4);
    }

    [Test]
    public void PizzaBuilder_ChainMethods_ShouldWorkCorrectly()
    {
        // Arrange & Act
        var pizza = new PizzaBuilder()
            .WithSize(PizzaSize.ExtraLarge)
            .WithDough(DoughType.Stuffed)
            .WithSauce(SauceType.WhiteSauce)
            .AddTopping("Chicken")
            .AddTopping("Bacon")
            .WithExtraCheese()
            .WithSpicyLevel(3)
            .Build();

        // Assert
        pizza.Size.Should().Be(PizzaSize.ExtraLarge);
        pizza.Dough.Should().Be(DoughType.Stuffed);
        pizza.Sauce.Should().Be(SauceType.WhiteSauce);
        pizza.Toppings.Should().Contain("Chicken");
        pizza.Toppings.Should().Contain("Bacon");
        pizza.ExtraCheese.Should().BeTrue();
        pizza.SpicyLevel.Should().Be(3);
    }

    // ========== TODO 3: PizzaDirector Tests ==========
    [Test]
    public void PizzaDirector_CreateMargherita_ShouldReturnCorrectPizza()
    {
        // Act
        var pizza = PizzaDirector.CreateMargherita();

        // Assert
        pizza.Size.Should().Be(PizzaSize.Medium);
        pizza.Dough.Should().Be(DoughType.Thin);
        pizza.Sauce.Should().Be(SauceType.Tomato);
        pizza.Toppings.Should().Contain("Mozzarella");
        pizza.Toppings.Should().Contain("Basil");
    }

    [Test]
    public void PizzaDirector_CreatePepperoni_ShouldReturnCorrectPizza()
    {
        // Act
        var pizza = PizzaDirector.CreatePepperoni();

        // Assert
        pizza.Size.Should().Be(PizzaSize.Large);
        pizza.Dough.Should().Be(DoughType.Thick);
        pizza.Sauce.Should().Be(SauceType.Tomato);
        pizza.Toppings.Should().Contain("Pepperoni");
        pizza.Toppings.Should().Contain("Mozzarella");
        pizza.ExtraCheese.Should().BeTrue();
        pizza.SpicyLevel.Should().Be(2);
    }

    [Test]
    public void PizzaDirector_CreateVeggie_ShouldReturnCorrectPizza()
    {
        // Act
        var pizza = PizzaDirector.CreateVeggie();

        // Assert
        pizza.Size.Should().Be(PizzaSize.Medium);
        pizza.Dough.Should().Be(DoughType.Thin);
        pizza.Sauce.Should().Be(SauceType.Pesto);
        pizza.Toppings.Should().Contain("Bell Peppers");
        pizza.Toppings.Should().Contain("Mushrooms");
        pizza.Toppings.Should().Contain("Olives");
        pizza.Toppings.Should().Contain("Onions");
    }

    [Test]
    public void PizzaDirector_CreateMeatLovers_ShouldReturnCorrectPizza()
    {
        // Act
        var pizza = PizzaDirector.CreateMeatLovers();

        // Assert
        pizza.Size.Should().Be(PizzaSize.ExtraLarge);
        pizza.Dough.Should().Be(DoughType.Thick);
        pizza.Sauce.Should().Be(SauceType.BBQ);
        pizza.Toppings.Should().Contain("Pepperoni");
        pizza.Toppings.Should().Contain("Sausage");
        pizza.Toppings.Should().Contain("Bacon");
        pizza.Toppings.Should().Contain("Ham");
        pizza.ExtraCheese.Should().BeTrue();
        pizza.SpicyLevel.Should().Be(3);
    }

    // ========== TODO 4: GlutenFreePizzaBuilder Tests ==========
    [Test]
    public void GlutenFreePizzaBuilder_ShouldUseGlutenFreeDough()
    {
        // Arrange & Act
        var pizza = new GlutenFreePizzaBuilder()
            .WithSize(PizzaSize.Medium)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Cheese")
            .Build();

        // Assert
        pizza.Dough.Should().Be(DoughType.GlutenFree);
    }

    [Test]
    public void GlutenFreePizzaBuilder_ShouldAllowCustomization()
    {
        // Arrange & Act
        var pizza = new GlutenFreePizzaBuilder()
            .WithSize(PizzaSize.Large)
            .WithSauce(SauceType.Pesto)
            .AddTopping("Vegetables")
            .WithSpicyLevel(1)
            .Build();

        // Assert
        pizza.Dough.Should().Be(DoughType.GlutenFree);
        pizza.Size.Should().Be(PizzaSize.Large);
        pizza.Sauce.Should().Be(SauceType.Pesto);
        pizza.SpicyLevel.Should().Be(1);
    }

    // ========== Integration Tests ==========
    [Test]
    public void BuilderPattern_Integration_ShouldAllowComplexBuilding()
    {
        // Test 1: Simple pizza
        var simple = new PizzaBuilder()
            .WithSize(PizzaSize.Small)
            .WithDough(DoughType.Thin)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Cheese")
            .Build();

        simple.Should().NotBeNull();

        // Test 2: Complex pizza
        var complex = new PizzaBuilder()
            .WithSize(PizzaSize.ExtraLarge)
            .WithDough(DoughType.Thick)
            .WithSauce(SauceType.BBQ)
            .AddTopping("Pepperoni")
            .AddTopping("Sausage")
            .AddTopping("Bacon")
            .WithExtraCheese()
            .WithSpicyLevel(5)
            .Build();

        complex.Toppings.Should().HaveCount(3);
        complex.ExtraCheese.Should().BeTrue();

        // Test 3: Director-created pizza
        var margherita = PizzaDirector.CreateMargherita();
        margherita.Toppings.Should().Contain("Basil");
    }

    [Test]
    public void Pizza_ToString_ShouldReturnReadableFormat()
    {
        // Arrange
        var pizza = new PizzaBuilder()
            .WithSize(PizzaSize.Medium)
            .WithDough(DoughType.Thin)
            .WithSauce(SauceType.Tomato)
            .AddTopping("Cheese")
            .Build();

        // Act
        var result = pizza.ToString();

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().Contain("Medium");
        result.Should().Contain("Thin");
        result.Should().Contain("Cheese");
    }
}
