using System;
using System.Collections.Generic;

namespace PolymorphismBasics;

/// <summary>
/// Polymorphism Basics Tutorial
/// This sample demonstrates runtime polymorphism with practical examples
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine("POLYMORPHISM BASICS TUTORIAL");
        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine();

        // Example 1: Payment System
        Console.WriteLine("📝 Example 1: Payment Processing System");
        Console.WriteLine("-".PadRight(70, '-'));
        RunPaymentExample();
        Console.WriteLine();

        // Example 2: Shape Calculations
        Console.WriteLine("📐 Example 2: Shape Calculations");
        Console.WriteLine("-".PadRight(70, '-'));
        RunShapeExample();
        Console.WriteLine();

        // Example 3: Notification System
        Console.WriteLine("📧 Example 3: Notification System");
        Console.WriteLine("-".PadRight(70, '-'));
        RunNotificationExample();
        Console.WriteLine();

        // Example 4: Virtual vs Abstract
        Console.WriteLine("🔍 Example 4: Virtual vs Abstract Methods");
        Console.WriteLine("-".PadRight(70, '-'));
        RunVirtualVsAbstractExample();
        Console.WriteLine();

        Console.WriteLine("=".PadRight(70, '='));
        Console.WriteLine("✅ Tutorial Complete!");
        Console.WriteLine("=".PadRight(70, '='));
    }

    #region Example 1: Payment System

    static void RunPaymentExample()
    {
        // Create different payment types
        List<Payment> payments = new()
        {
            new CreditCardPayment
            {
                Amount = 99.99m,
                CardNumber = "**** **** **** 1234",
                CardHolderName = "John Doe"
            },
            new PayPalPayment
            {
                Amount = 49.50m,
                Email = "john.doe@example.com"
            },
            new CryptoPayment
            {
                Amount = 199.99m,
                WalletAddress = "0x742d35Cc6634C0532925a3b844Bc9e7595f0bEb",
                Cryptocurrency = "Ethereum"
            }
        };

        // Process all payments using polymorphism
        // Notice: We treat all different payment types uniformly!
        foreach (var payment in payments)
        {
            try
            {
                payment.ProcessPayment();
                Console.WriteLine($"✅ Payment processed successfully\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Payment failed: {ex.Message}\n");
            }
        }
    }

    abstract class Payment
    {
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        // Abstract method - must be implemented by derived classes
        public abstract void ProcessPayment();

        // Virtual method - can be overridden but has default implementation
        public virtual void ValidateAmount()
        {
            if (Amount <= 0)
                throw new ArgumentException("Amount must be greater than zero");

            if (Amount > 10000)
                throw new ArgumentException("Amount exceeds maximum transaction limit");
        }

        // Regular method - shared behavior
        public void LogTransaction()
        {
            Console.WriteLine($"   Transaction logged at {TransactionDate:yyyy-MM-dd HH:mm:ss}");
        }
    }

    class CreditCardPayment : Payment
    {
        public string CardNumber { get; set; } = string.Empty;
        public string CardHolderName { get; set; } = string.Empty;

        public override void ProcessPayment()
        {
            ValidateAmount();
            Console.WriteLine($"💳 Processing Credit Card Payment");
            Console.WriteLine($"   Card: {CardNumber}");
            Console.WriteLine($"   Holder: {CardHolderName}");
            Console.WriteLine($"   Amount: ${Amount:F2}");

            // Simulate processing
            Thread.Sleep(500);

            LogTransaction();
        }
    }

    class PayPalPayment : Payment
    {
        public string Email { get; set; } = string.Empty;

        public override void ProcessPayment()
        {
            ValidateAmount();
            Console.WriteLine($"🅿️  Processing PayPal Payment");
            Console.WriteLine($"   Email: {Email}");
            Console.WriteLine($"   Amount: ${Amount:F2}");

            // Simulate processing
            Thread.Sleep(500);

            LogTransaction();
        }
    }

    class CryptoPayment : Payment
    {
        public string WalletAddress { get; set; } = string.Empty;
        public string Cryptocurrency { get; set; } = "Bitcoin";

        public override void ProcessPayment()
        {
            ValidateAmount();
            Console.WriteLine($"₿  Processing Cryptocurrency Payment");
            Console.WriteLine($"   Wallet: {WalletAddress}");
            Console.WriteLine($"   Crypto: {Cryptocurrency}");
            Console.WriteLine($"   Amount: ${Amount:F2}");

            // Simulate blockchain confirmation
            Thread.Sleep(1000);

            LogTransaction();
        }

        // Override validation with crypto-specific rules
        public override void ValidateAmount()
        {
            base.ValidateAmount(); // Call base validation first

            // Additional crypto-specific validation
            if (Amount < 10)
                throw new ArgumentException("Minimum crypto transaction is $10");
        }
    }

    #endregion

    #region Example 2: Shape Calculations

    static void RunShapeExample()
    {
        List<Shape> shapes = new()
        {
            new Circle { Radius = 5, Name = "Small Circle" },
            new Rectangle { Width = 10, Height = 20, Name = "Wide Rectangle" },
            new Triangle { Base = 8, Height = 12, Name = "Right Triangle" }
        };

        foreach (var shape in shapes)
        {
            Console.WriteLine($"Shape: {shape.Name}");
            Console.WriteLine($"  Area: {shape.CalculateArea():F2} square units");
            Console.WriteLine($"  Perimeter: {shape.CalculatePerimeter():F2} units");
            shape.Draw();
            Console.WriteLine();
        }
    }

    abstract class Shape
    {
        public string Name { get; set; } = "Unnamed Shape";

        // Abstract methods - must be implemented
        public abstract double CalculateArea();
        public abstract double CalculatePerimeter();

        // Virtual method - can be overridden
        public virtual void Draw()
        {
            Console.WriteLine($"  Drawing {Name}...");
        }
    }

    class Circle : Shape
    {
        public double Radius { get; set; }

        public override double CalculateArea()
            => Math.PI * Radius * Radius;

        public override double CalculatePerimeter()
            => 2 * Math.PI * Radius;

        public override void Draw()
        {
            Console.WriteLine("  ⭕ Circle drawn");
        }
    }

    class Rectangle : Shape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public override double CalculateArea()
            => Width * Height;

        public override double CalculatePerimeter()
            => 2 * (Width + Height);

        public override void Draw()
        {
            Console.WriteLine("  ▭  Rectangle drawn");
        }
    }

    class Triangle : Shape
    {
        public double Base { get; set; }
        public double Height { get; set; }

        public override double CalculateArea()
            => 0.5 * Base * Height;

        public override double CalculatePerimeter()
        {
            // For simplicity, assume right triangle
            var hypotenuse = Math.Sqrt(Base * Base + Height * Height);
            return Base + Height + hypotenuse;
        }

        public override void Draw()
        {
            Console.WriteLine("  △  Triangle drawn");
        }
    }

    #endregion

    #region Example 3: Notification System

    static void RunNotificationExample()
    {
        var notifications = new List<Notification>
        {
            new EmailNotification
            {
                Recipient = "user@example.com",
                Subject = "Welcome!",
                Message = "Thank you for joining our platform"
            },
            new SmsNotification
            {
                PhoneNumber = "+1-555-0123",
                Message = "Your verification code is: 123456"
            },
            new PushNotification
            {
                DeviceId = "device-abc-123",
                Message = "You have a new message",
                Title = "New Message"
            }
        };

        Console.WriteLine("Sending all notifications...\n");

        foreach (var notification in notifications)
        {
            notification.Send();
            Console.WriteLine($"Status: {notification.Status}");
            Console.WriteLine($"Sent at: {notification.SentAt:HH:mm:ss}\n");
        }
    }

    abstract class Notification
    {
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; private set; }
        public string Status { get; protected set; } = "Pending";

        public abstract void Send();

        protected void MarkAsSent()
        {
            SentAt = DateTime.Now;
            Status = "Sent";
        }
    }

    class EmailNotification : Notification
    {
        public string Recipient { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;

        public override void Send()
        {
            Console.WriteLine($"📧 Sending Email");
            Console.WriteLine($"   To: {Recipient}");
            Console.WriteLine($"   Subject: {Subject}");
            Console.WriteLine($"   Body: {Message}");

            Thread.Sleep(300);
            MarkAsSent();
        }
    }

    class SmsNotification : Notification
    {
        public string PhoneNumber { get; set; } = string.Empty;

        public override void Send()
        {
            Console.WriteLine($"📱 Sending SMS");
            Console.WriteLine($"   To: {PhoneNumber}");
            Console.WriteLine($"   Text: {Message}");

            Thread.Sleep(200);
            MarkAsSent();
        }
    }

    class PushNotification : Notification
    {
        public string DeviceId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public override void Send()
        {
            Console.WriteLine($"🔔 Sending Push Notification");
            Console.WriteLine($"   Device: {DeviceId}");
            Console.WriteLine($"   Title: {Title}");
            Console.WriteLine($"   Message: {Message}");

            Thread.Sleep(100);
            MarkAsSent();
        }
    }

    #endregion

    #region Example 4: Virtual vs Abstract

    static void RunVirtualVsAbstractExample()
    {
        var animals = new List<Animal>
        {
            new Dog { Name = "Buddy" },
            new Cat { Name = "Whiskers" },
            new Bird { Name = "Tweety" }
        };

        foreach (var animal in animals)
        {
            Console.WriteLine($"Animal: {animal.Name}");
            animal.MakeSound();      // Abstract - must override
            animal.Eat();            // Virtual - can override
            animal.Sleep();          // Regular - cannot override
            Console.WriteLine();
        }
    }

    abstract class Animal
    {
        public string Name { get; set; } = string.Empty;

        // Abstract - MUST be overridden
        public abstract void MakeSound();

        // Virtual - CAN be overridden (has default implementation)
        public virtual void Eat()
        {
            Console.WriteLine($"  {Name} is eating...");
        }

        // Regular - CANNOT be overridden
        public void Sleep()
        {
            Console.WriteLine($"  {Name} is sleeping... 💤");
        }
    }

    class Dog : Animal
    {
        public override void MakeSound()
        {
            Console.WriteLine("  🐕 Woof! Woof!");
        }

        public override void Eat()
        {
            Console.WriteLine($"  {Name} is eating dog food");
        }
    }

    class Cat : Animal
    {
        public override void MakeSound()
        {
            Console.WriteLine("  🐈 Meow!");
        }

        public override void Eat()
        {
            Console.WriteLine($"  {Name} is eating fish");
        }
    }

    class Bird : Animal
    {
        public override void MakeSound()
        {
            Console.WriteLine("  🐦 Tweet! Tweet!");
        }

        // Note: We DON'T override Eat() - uses base implementation
    }

    #endregion
}
