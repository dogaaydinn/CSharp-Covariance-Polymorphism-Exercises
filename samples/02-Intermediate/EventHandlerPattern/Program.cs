class Order {
    public event EventHandler? OrderPlaced;
    public void Place() {
        Console.WriteLine("📦 Order placed");
        OrderPlaced?.Invoke(this, EventArgs.Empty);
    }
}

class Program {
    static void Main() {
        Console.WriteLine("=== Events ===\n");
        var order = new Order();
        order.OrderPlaced += (s, e) => Console.WriteLine("✅ Event: Order placed");
        order.Place();
    }
}
