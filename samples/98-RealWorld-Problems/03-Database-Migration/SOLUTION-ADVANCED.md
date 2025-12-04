# ÇÖZÜM: EXPAND-CONTRACT PATTERN (Zero Downtime)

## 🎯 ÇÖZÜM ÖZETİ

Expand-Contract Pattern: Schema değişikliklerini aşamalı yaparak zero-downtime sağla

## 📊 ADIMLAR

### Phase 1: EXPAND (Yeni kolonları ekle)
```sql
-- Yeni kolonları ekle (eski kolonları silme!)
ALTER TABLE Users ADD FirstName NVARCHAR(100);
ALTER TABLE Users ADD LastName NVARCHAR(100);

-- Trigger ekle: FullName → FirstName + LastName sync et
CREATE TRIGGER trg_SyncNames ON Users
AFTER INSERT, UPDATE AS
BEGIN
    UPDATE u
    SET FirstName = LEFT(i.FullName, CHARINDEX(' ', i.FullName)-1),
        LastName = SUBSTRING(i.FullName, CHARINDEX(' ', i.FullName)+1, LEN(i.FullName))
    FROM Users u
    INNER JOIN inserted i ON u.Id = i.Id
END;
```

### Phase 2: MIGRATE (Mevcut datayı migrate et)
```csharp
public async Task MigrateExistingDataAsync()
{
    var batchSize = 1000;
    var offset = 0;
    
    while (true)
    {
        var users = await _context.Users
            .Where(u => u.FirstName == null) // Not migrated yet
            .OrderBy(u => u.Id)
            .Skip(offset)
            .Take(batchSize)
            .ToListAsync();
        
        if (!users.Any()) break;
        
        foreach (var user in users)
        {
            var names = user.FullName.Split(' ', 2);
            user.FirstName = names[0];
            user.LastName = names.Length > 1 ? names[1] : "";
        }
        
        await _context.SaveChangesAsync();
        offset += batchSize;
        
        await Task.Delay(100); // Throttle to avoid DB overload
    }
}
```

### Phase 3: CONTRACT (Eski kolonları kaldır)
```sql
-- Trigger'ı kaldır
DROP TRIGGER trg_SyncNames;

-- Eski kolonu kaldır (tüm kod deploy edildikten sonra!)
ALTER TABLE Users DROP COLUMN FullName;
```

## ✅ AVANTAJLAR
- ✅ Zero downtime
- ✅ Rollback capability (Phase 1-2'de geri dönebilirsin)
- ✅ Gradual migration
- ✅ Production-tested approach

## ⚠️ TRADE-OFFS
- ⚠️ Daha uzun sürer (3 aşama)
- ⚠️ Geçici data duplication
- ⚠️ Trigger overhead

**Seviye:** Senior Developer - Bu pattern production'da must-have!
