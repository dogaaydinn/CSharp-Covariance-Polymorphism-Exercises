# GERÇEK DÜNYA PROBLEMİ: Legacy Code Refactoring

## 🚨 PROBLEM SENARYOSU

**Şirket:** 10-year old .NET application
**Code:** 500K lines, no tests, tightly coupled
**Challenge:** "Bu kodu refactor etmeliyiz ama production bozulmamalı"

**Legacy Code (2013'ten kalma):**
```csharp
public class OrderProcessor
{
    // ❌ 800-line method!
    // ❌ Multiple responsibilities
    // ❌ No tests
    // ❌ Global state
    // ❌ Hard-coded dependencies
    public void ProcessOrder(int orderId)
    {
        // 800 lines of spaghetti code...
        var connection = new SqlConnection("Server=...");
        connection.Open();
        var command = new SqlCommand("SELECT * FROM Orders WHERE Id = " + orderId);
        // SQL injection vulnerability!
        
        var reader = command.ExecuteReader();
        if (reader.Read())
        {
            // Business logic mixed with data access
            // Validation mixed with processing
            // Email sending mixed with DB operations
            
            // No error handling
            // No logging
            // No transactions
        }
    }
}
```

## 🎯 PROBLEM STATEMENT

> "Nasıl legacy code'u refactor edebiliriz ki:
> - Production bozulmasın (zero downtime)
> - Her adımda test edebilir olsun
> - Incremental progress (big bang rewrite değil)
> - Team velocity düşmesin"

## 🔗 ÇÖZÜMLER

1. **BAD:** Big Bang Rewrite (risky, takes months)
2. **GOOD:** Strangler Fig Pattern (incremental, safe)
3. **BEST:** Characterization Tests + Extract-Refactor-Inject

Devam → `SOLUTION-ADVANCED.md` (Strangler Fig Pattern!)
