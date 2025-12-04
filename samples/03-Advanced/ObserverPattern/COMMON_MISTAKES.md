# Common Mistakes: Observer Pattern

## 1. Memory Leaks
❌ Event unsubscribe etmeyi unutma
✅ Dispose pattern ile unsubscribe

## 2. Exception in Observer
❌ Bir observer exception fırlatırsa diğerleri çalışmaz
✅ Try-catch her observer için

## 3. Circular Dependencies
❌ A observes B, B observes A
✅ Event aggregator kullan
