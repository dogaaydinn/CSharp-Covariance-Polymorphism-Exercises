# KARŞILAŞTIRMA: Microservice Communication

| Pattern | Coupling | Performance | Complexity | Fault Tolerance |
|---------|----------|-------------|------------|-----------------|
| **REST (Sync)** | Tight | Fast | ⭐ | ❌ Low |
| **Message Queue** | Loose | Async | ⭐⭐⭐ | ✅ High |
| **Event Sourcing** | Very Loose | Async | ⭐⭐⭐⭐⭐ | ✅ Very High |

**Öneri:**
- Simple system → REST
- Production microservices → **Message Queue**
- Enterprise/Complex → Event Sourcing + Saga
