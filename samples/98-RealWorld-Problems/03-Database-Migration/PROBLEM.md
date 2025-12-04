# GERÇEK DÜNYA PROBLEMİ: Zero-Downtime Database Migration

## 🚨 PROBLEM SENARYOSU

**Şirket:** SaaS platform, 5000 enterprise customers
**Database:** SQL Server, 2TB data, 500 tables
**Challenge:** "Users" tablosunu refactor etmeliyiz ama ZERO DOWNTIME olmalı

**Problem:**
```sql
-- ESKİ SCHEMA (KÖTÜ):
CREATE TABLE Users (
    Id INT PRIMARY KEY,
    FullName NVARCHAR(200), -- ❌ Split etmeliyiz: FirstName + LastName
    Email NVARCHAR(200)
);

-- YENİ SCHEMA (İYİ):
CREATE TABLE Users (
    Id INT PRIMARY KEY,
    FirstName NVARCHAR(100), -- ✅ Ayrı kolonlar
    LastName NVARCHAR(100),
    Email NVARCHAR(200)
);
```

**Risk:** 5000 customer, %99.99 SLA, downtime = $100K/hour revenue loss

## 🎯 PROBLEM STATEMENT

> "Nasıl database schema'yı değiştirebiliriz ki:
> - Zero downtime (no maintenance window)
> - No data loss
> - Rollback capability
> - Blue-green deployment support"

## 🔗 ÇÖZÜMLER

1. **BASIC:** Maintenance Window (risky, requires downtime)
2. **ADVANCED:** Expand-Contract Pattern (production-safe)
3. **ENTERPRISE:** Blue-Green Database Migration with Read Replicas

Devam → `SOLUTION-ADVANCED.md` (Expand-Contract pattern öner!)
