namespace CastingExamples.Examples;

/// <summary>
/// Real-world example: Shape processing system with safe casting
/// </summary>
public static class RealWorldExample
{
    public static void Run()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║         REAL-WORLD: SHAPE PROCESSING SYSTEM                    ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════════╝\n");

        Console.WriteLine("Example: Graphics application that processes different shapes\n");

        // Create mixed collection of shapes
        List<Shape> shapes = new List<Shape>
        {
            new Circle { Name = "Circle1", Radius = 5 },
            new Rectangle { Name = "Rect1", Width = 10, Height = 5 },
            new Circle { Name = "Circle2", Radius = 3 },
            new Rectangle { Name = "Rect2", Width = 8, Height = 8 },
            new Triangle { Name = "Triangle1", Base = 6, Height = 4 }
        };

        // Process all shapes
        PrintSection("Processing All Shapes");
        foreach (var shape in shapes)
        {
            Console.WriteLine($"  {shape.Name}: Area = {shape.CalculateArea():F2}, Perimeter = {shape.CalculatePerimeter():F2}");
        }
        Console.WriteLine();

        // Filter and process specific shape types
        PrintSection("Processing Circles Only");
        ProcessCircles(shapes);
        Console.WriteLine();

        PrintSection("Processing Rectangles Only");
        ProcessRectangles(shapes);
        Console.WriteLine();

        // Calculate total area by type
        PrintSection("Area Summary by Type");
        CalculateAreaByType(shapes);
        Console.WriteLine();

        // Demonstrate shape transformations
        PrintSection("Shape Transformations");
        TransformShapes(shapes);
        Console.WriteLine();
    }

    static void ProcessCircles(List<Shape> shapes)
    {
        Console.WriteLine("Finding all circles and calculating diameters:\n");

        int count = 0;
        foreach (var shape in shapes)
        {
            // Safe casting with pattern matching
            if (shape is Circle circle)
            {
                double diameter = circle.Radius * 2;
                Console.WriteLine($"  🔵 {circle.Name}:");
                Console.WriteLine($"     Radius: {circle.Radius}, Diameter: {diameter}");
                Console.WriteLine($"     Area: {circle.CalculateArea():F2}");
                count++;
            }
        }
        Console.WriteLine($"\nTotal circles found: {count}");
    }

    static void ProcessRectangles(List<Shape> shapes)
    {
        Console.WriteLine("Finding all rectangles and checking if square:\n");

        int count = 0;
        foreach (var shape in shapes)
        {
            // Safe casting with 'as' operator
            Rectangle? rect = shape as Rectangle;
            if (rect != null)
            {
                bool isSquare = rect.Width == rect.Height;
                Console.WriteLine($"  🟦 {rect.Name}:");
                Console.WriteLine($"     Dimensions: {rect.Width} x {rect.Height}");
                Console.WriteLine($"     Is Square: {(isSquare ? "Yes" : "No")}");
                Console.WriteLine($"     Area: {rect.CalculateArea():F2}");
                count++;
            }
        }
        Console.WriteLine($"\nTotal rectangles found: {count}");
    }

    static void CalculateAreaByType(List<Shape> shapes)
    {
        double circleArea = 0;
        double rectangleArea = 0;
        double triangleArea = 0;

        foreach (var shape in shapes)
        {
            // Switch expression with pattern matching (C# 8+)
            _ = shape switch
            {
                Circle c => circleArea += c.CalculateArea(),
                Rectangle r => rectangleArea += r.CalculateArea(),
                Triangle t => triangleArea += t.CalculateArea(),
                _ => 0
            };
        }

        Console.WriteLine($"  Total Circle Area:    {circleArea:F2}");
        Console.WriteLine($"  Total Rectangle Area: {rectangleArea:F2}");
        Console.WriteLine($"  Total Triangle Area:  {triangleArea:F2}");
        Console.WriteLine($"  ─────────────────────────────────");
        Console.WriteLine($"  Total Area:           {(circleArea + rectangleArea + triangleArea):F2}");
    }

    static void TransformShapes(List<Shape> shapes)
    {
        Console.WriteLine("Scaling all shapes by 1.5x:\n");

        foreach (var shape in shapes)
        {
            double originalArea = shape.CalculateArea();

            // Type-specific transformations
            switch (shape)
            {
                case Circle circle:
                    circle.Radius *= 1.5;
                    Console.WriteLine($"  🔵 {circle.Name}: Radius {circle.Radius / 1.5:F1} → {circle.Radius:F1}");
                    break;

                case Rectangle rect:
                    rect.Width *= 1.5;
                    rect.Height *= 1.5;
                    Console.WriteLine($"  🟦 {rect.Name}: Dimensions {rect.Width / 1.5:F1}x{rect.Height / 1.5:F1} → {rect.Width:F1}x{rect.Height:F1}");
                    break;

                case Triangle triangle:
                    triangle.Base *= 1.5;
                    triangle.Height *= 1.5;
                    Console.WriteLine($"  🔺 {triangle.Name}: Base/Height {triangle.Base / 1.5:F1}/{triangle.Height / 1.5:F1} → {triangle.Base:F1}/{triangle.Height:F1}");
                    break;
            }

            double newArea = shape.CalculateArea();
            Console.WriteLine($"     Area: {originalArea:F2} → {newArea:F2} (factor: {newArea / originalArea:F2}x)\n");
        }
    }

    static void PrintSection(string title)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"▶ {title}");
        Console.WriteLine(new string('─', 64));
        Console.ResetColor();
    }
}

// Shape hierarchy
public abstract class Shape
{
    public string Name { get; set; } = "";
    public abstract double CalculateArea();
    public abstract double CalculatePerimeter();
}

public class Circle : Shape
{
    public double Radius { get; set; }

    public override double CalculateArea() => Math.PI * Radius * Radius;
    public override double CalculatePerimeter() => 2 * Math.PI * Radius;
}

public class Rectangle : Shape
{
    public double Width { get; set; }
    public double Height { get; set; }

    public override double CalculateArea() => Width * Height;
    public override double CalculatePerimeter() => 2 * (Width + Height);
}

public class Triangle : Shape
{
    public double Base { get; set; }
    public double Height { get; set; }

    public override double CalculateArea() => 0.5 * Base * Height;
    public override double CalculatePerimeter() => Base + Height + Math.Sqrt(Base * Base + Height * Height);
}
