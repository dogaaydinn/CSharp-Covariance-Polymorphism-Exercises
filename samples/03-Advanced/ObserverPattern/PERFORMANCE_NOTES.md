# Performance: Observer Pattern

## Benchmark
Observer count etkiliyor:
- 10 observers: ~100ns
- 100 observers: ~1µs
- 1000 observers: ~10µs

## Optimization
- Weak events (memory leaks prevent)
- Async observers (parallel notification)
- Event aggregator (centralized)
