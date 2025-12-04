using FluentAssertions;
using Xunit;

namespace StrategyPattern;

/// <summary>
/// Test suite for Strategy Pattern exercise.
///
/// Run with: dotnet test
///
/// All tests should pass when your implementation is correct.
/// </summary>
public class ShoppingCartTests
{
    [Fact]
    public void CreditCard_ProcessesPaymentCorrectly()
    {
        // Arrange: Create cart with credit card payment strategy
        var cart = new ShoppingCart(new CreditCardPaymentStrategy("1234-5678-9012-3456", "12/25"));
        cart.AddItem(new CartItem("Laptop", 999.99m));

        // Act: Process payment
        var result = cart.ProcessPayment();

        // Assert: Verify credit card payment was processed
        result.Should().Contain("Credit card");
        result.Should().Contain("999.99");
        result.Should().Contain("****-****-****-3456"); // Masked card number
    }

    [Fact]
    public void PayPal_ProcessesPaymentCorrectly()
    {
        // Arrange: Create cart with PayPal payment strategy
        var cart = new ShoppingCart(new PayPalPaymentStrategy("user@example.com"));
        cart.AddItem(new CartItem("Mouse", 29.99m));

        // Act: Process payment
        var result = cart.ProcessPayment();

        // Assert: Verify PayPal payment was processed
        result.Should().Contain("PayPal");
        result.Should().Contain("29.99");
        result.Should().Contain("user@example.com");
    }

    [Fact]
    public void BankTransfer_ProcessesPaymentCorrectly()
    {
        // Arrange: Create cart with bank transfer payment strategy
        var cart = new ShoppingCart(new BankTransferPaymentStrategy("US1234567890"));
        cart.AddItem(new CartItem("Monitor", 299.99m));

        // Act: Process payment
        var result = cart.ProcessPayment();

        // Assert: Verify bank transfer payment was processed
        result.Should().Contain("Bank transfer");
        result.Should().Contain("299.99");
        result.Should().Contain("US1234567890");
    }

    [Fact]
    public void MultipleItems_CalculatesTotalCorrectly()
    {
        // Arrange: Create cart with multiple items
        var cart = new ShoppingCart(new PayPalPaymentStrategy("user@example.com"));
        cart.AddItem(new CartItem("Laptop", 999.99m));
        cart.AddItem(new CartItem("Mouse", 29.99m));
        cart.AddItem(new CartItem("Keyboard", 79.99m));

        // Act: Process payment
        var result = cart.ProcessPayment();

        // Assert: Verify total is calculated correctly (999.99 + 29.99 + 79.99 = 1109.97)
        result.Should().Contain("1109.97");
    }

    [Fact]
    public void EmptyCart_ProcessesZeroPayment()
    {
        // Arrange: Create empty cart
        var cart = new ShoppingCart(new CreditCardPaymentStrategy("1234-5678-9012-3456", "12/25"));

        // Act: Process payment with no items
        var result = cart.ProcessPayment();

        // Assert: Verify zero payment is processed
        result.Should().Contain("0.00");
        result.Should().Contain("0 item(s)");
    }

    [Fact]
    public void DifferentStrategies_ProduceDifferentResults()
    {
        // Arrange: Create two carts with same items but different strategies
        var creditCardCart = new ShoppingCart(new CreditCardPaymentStrategy("1234-5678-9012-3456", "12/25"));
        creditCardCart.AddItem(new CartItem("Book", 19.99m));

        var paypalCart = new ShoppingCart(new PayPalPaymentStrategy("user@example.com"));
        paypalCart.AddItem(new CartItem("Book", 19.99m));

        // Act: Process payments
        var creditCardResult = creditCardCart.ProcessPayment();
        var paypalResult = paypalCart.ProcessPayment();

        // Assert: Verify different strategies produce different results
        creditCardResult.Should().Contain("Credit card");
        paypalResult.Should().Contain("PayPal");
        creditCardResult.Should().NotContain("PayPal");
        paypalResult.Should().NotContain("Credit card");
    }

    [Fact]
    public void GetTotal_ReturnsCorrectSum()
    {
        // Arrange: Create cart and add items
        var cart = new ShoppingCart(new CreditCardPaymentStrategy("1234-5678-9012-3456", "12/25"));
        cart.AddItem(new CartItem("Item1", 10.50m));
        cart.AddItem(new CartItem("Item2", 20.75m));
        cart.AddItem(new CartItem("Item3", 5.25m));

        // Act: Get total
        var total = cart.GetTotal();

        // Assert: Verify total calculation
        total.Should().Be(36.50m);
    }

    [Fact]
    public void GetItemCount_ReturnsCorrectCount()
    {
        // Arrange: Create cart and add items
        var cart = new ShoppingCart(new PayPalPaymentStrategy("user@example.com"));
        cart.AddItem(new CartItem("Item1", 10.00m));
        cart.AddItem(new CartItem("Item2", 20.00m));
        cart.AddItem(new CartItem("Item3", 30.00m));

        // Act: Get item count
        var count = cart.GetItemCount();

        // Assert: Verify count
        count.Should().Be(3);
    }
}
