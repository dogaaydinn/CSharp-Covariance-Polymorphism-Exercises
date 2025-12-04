abstract record Shape;
record Circle(double Radius) : Shape;
record Rectangle(double W, double H) : Shape;

class Program {
    static void Main() {
        Console.WriteLine("=== Pattern Matching ===\n");
        Shape[] shapes = [new Circle(5), new Rectangle(4, 6)];
        foreach (var s in shapes) {
            var area = s switch {
                Circle c => Math.PI * c.Radius * c.Radius,
                Rectangle r => r.W * r.H,
                _ => 0
            };
            Console.WriteLine($"{s.GetType().Name}: {area:F2}");
        }
    }
}
