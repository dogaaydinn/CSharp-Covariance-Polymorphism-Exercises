// PR #123: Add Payment Processing Feature
// Author: Junior Developer (6 months experience)
// Date: 2024-12-03
// Description: Implement payment processing for different payment methods

using System;
using System.Collections.Generic;

namespace PaymentSystem
{
    // ❌ PROBLEM: Type checking instead of polymorphism
    public class PaymentProcessor
    {
        public void ProcessPayment(string paymentType, decimal amount, string accountInfo)
        {
            // ❌ PROBLEM: Using string comparison instead of polymorphism
            if (paymentType == "CreditCard")
            {
                Console.WriteLine("Processing credit card payment...");
                // Hardcoded credit card logic
                if (accountInfo.Length == 16)
                {
                    Console.WriteLine($"Charging ${amount} to card {accountInfo}");
                    // ❌ PROBLEM: No validation, no error handling
                }
                else
                {
                    Console.WriteLine("Invalid card number");
                }
            }
            else if (paymentType == "PayPal")
            {
                Console.WriteLine("Processing PayPal payment...");
                // ❌ PROBLEM: Duplicated validation logic
                if (accountInfo.Contains("@"))
                {
                    Console.WriteLine($"Charging ${amount} to PayPal {accountInfo}");
                }
                else
                {
                    Console.WriteLine("Invalid PayPal email");
                }
            }
            else if (paymentType == "BankTransfer")
            {
                Console.WriteLine("Processing bank transfer...");
                // ❌ PROBLEM: More duplication
                if (accountInfo.Length >= 10)
                {
                    Console.WriteLine($"Transferring ${amount} to account {accountInfo}");
                }
                else
                {
                    Console.WriteLine("Invalid account number");
                }
            }
            // ❌ PROBLEM: What if we need to add Crypto? Bitcoin? Apple Pay?
            // This method will become HUGE!
        }

        // ❌ PROBLEM: Another method with type checking
        public decimal CalculateFee(string paymentType, decimal amount)
        {
            if (paymentType == "CreditCard")
            {
                return amount * 0.029m; // 2.9% fee
            }
            else if (paymentType == "PayPal")
            {
                return amount * 0.034m; // 3.4% fee
            }
            else if (paymentType == "BankTransfer")
            {
                return 2.50m; // Flat $2.50 fee
            }
            
            return 0; // ❌ PROBLEM: Silent failure for unknown types
        }

        // ❌ PROBLEM: Magic strings everywhere
        public string GetPaymentMethodName(string paymentType)
        {
            if (paymentType == "CreditCard") return "Credit Card";
            if (paymentType == "PayPal") return "PayPal";
            if (paymentType == "BankTransfer") return "Bank Transfer";
            return "Unknown"; // ❌ PROBLEM: No type safety
        }
    }

    // ❌ PROBLEM: Client code also does type checking
    public class PaymentService
    {
        private PaymentProcessor _processor = new PaymentProcessor();

        public void MakePayment(string type, decimal amount, string account)
        {
            // ❌ PROBLEM: Duplicating validation logic AGAIN
            if (type == "CreditCard")
            {
                if (amount > 10000)
                {
                    Console.WriteLine("Credit card limit exceeded");
                    return;
                }
            }
            
            var fee = _processor.CalculateFee(type, amount);
            var total = amount + fee;
            
            Console.WriteLine($"Total with fee: ${total}");
            _processor.ProcessPayment(type, amount, account);
        }

        // ❌ PROBLEM: List all types manually
        public List<string> GetAvailablePaymentMethods()
        {
            return new List<string> { "CreditCard", "PayPal", "BankTransfer" };
            // ❌ PROBLEM: Easy to forget updating this when adding new payment method
        }
    }

    // Usage Example
    public class Program
    {
        public static void Main()
        {
            var service = new PaymentService();
            
            // ❌ PROBLEM: Magic strings, no compile-time safety
            service.MakePayment("CreditCard", 100.00m, "1234567890123456");
            service.MakePayment("PayPal", 50.00m, "user@example.com");
            
            // ❌ PROBLEM: Typo won't be caught at compile time
            service.MakePayment("CrediCard", 25.00m, "1234567890123456"); // Oops!
        }
    }
}

/*
 * PROBLEMS SUMMARY:
 * 1. ❌ Type checking (if/else chains) instead of polymorphism
 * 2. ❌ Magic strings everywhere (no type safety)
 * 3. ❌ Code duplication (validation logic repeated)
 * 4. ❌ Hard to extend (adding Bitcoin requires changes in 10 places)
 * 5. ❌ No error handling
 * 6. ❌ Tight coupling (PaymentService knows all payment types)
 * 7. ❌ No abstraction (concrete types everywhere)
 * 8. ❌ Silent failures (unknown payment types return 0 fee)
 * 9. ❌ Open-Closed Principle violation (not open for extension)
 * 10. ❌ Single Responsibility violation (PaymentProcessor does too much)
 */
