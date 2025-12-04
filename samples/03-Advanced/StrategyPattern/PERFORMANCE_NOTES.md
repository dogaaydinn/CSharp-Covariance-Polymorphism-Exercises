# Performance: Strategy Pattern

## Benchmark
```
Method      | Mean  | Allocated
------------|-------|----------
If/Else     | 5ns   | 0 B
Strategy    | 7ns   | 0 B
```
Overhead: ~2ns (minimal)

## Optimization
- Cache strategies: Don't create new instances
- Use readonly fields
- Consider static strategies for stateless algorithms
