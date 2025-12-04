# KARŞILAŞTIRMA: Database Migration Strategies

| Strategy | Downtime | Risk | Rollback | Complexity |
|----------|----------|------|----------|------------|
| **Maintenance Window** | 2-4 hours | ⚠️ High | Hard | ⭐ |
| **Expand-Contract** | Zero | ✅ Low | Easy | ⭐⭐⭐ |
| **Blue-Green DB** | Zero | ✅ Very Low | Easy | ⭐⭐⭐⭐⭐ |

**Öneri:**
- Small changes → **Expand-Contract**
- Large migrations → **Blue-Green**
- NEVER use maintenance window for production!
