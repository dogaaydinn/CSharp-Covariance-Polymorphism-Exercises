# Mistakes

1. Using Lazy for cheap operations (overhead not worth it)
2. Lazy in hot path (first access penalty)
3. Not thread-safe (use Lazy<T> default mode)
