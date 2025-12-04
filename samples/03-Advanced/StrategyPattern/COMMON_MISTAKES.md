# Common Mistakes: Strategy Pattern

## 1. Creating New Strategy Every Time
❌ `new CreditCardStrategy()` her kullanımda
✅ Strategy'leri cache et veya DI ile inject et

## 2. Strategy with State
❌ Strategy içinde mutable state
✅ Stateless tutun, context'e state koy

## 3. Too Many Strategies
❌ Her küçük varyasyon için strategy
✅ Gerçekten farklı algoritmalar için kullan
