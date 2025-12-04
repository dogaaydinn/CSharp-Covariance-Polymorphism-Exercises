# KARAR REHBERI: Database Migration

## 🎯 HIZLI KARAR

1. **Downtime acceptable?**
   - Evet (internal tool) → Maintenance Window
   - Hayır (production) → Devam

2. **Data size?**
   - <100GB → Expand-Contract
   - >100GB → Consider Blue-Green

3. **Schema change complexity?**
   - Simple (add/remove column) → Expand-Contract
   - Complex (restructure tables) → Blue-Green

**En güvenli:** Expand-Contract Pattern

## 💡 BEST PRACTICES

1. **Always** test migration on staging first
2. **Always** have rollback plan
3. **Always** backup before migration
4. **Monitor** during migration (CPU, locks, deadlocks)
5. **Gradual** rollout (canary deployment)

## 🚨 RED FLAGS (YAPMA!)

- ❌ Doğrudan DROP COLUMN (downtime risk!)
- ❌ Büyük transaction'lar (lock tüm tabloyu)
- ❌ Peak hours'ta migration
- ❌ Backup almadan migration
- ❌ Rollback planı olmadan

**Golden Rule:** If you can't rollback in 5 minutes, don't migrate!
