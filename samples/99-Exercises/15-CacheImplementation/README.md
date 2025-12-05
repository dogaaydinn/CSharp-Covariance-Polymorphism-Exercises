# LRU Cache

## Problem
Implement Least Recently Used (LRU) cache.

## API
```csharp
var cache = new LRUCache<int, string>(capacity: 3);
cache.Put(1, "one");
cache.Put(2, "two");
cache.Put(3, "three");
cache.Get(1); // "one", now most recent

cache.Put(4, "four"); // Evicts key 2 (least recently used)
cache.Get(2); // null (evicted)
```

## Requirements
- Get(key): O(1)
- Put(key, value): O(1)
- Evict LRU when capacity exceeded

## Data Structure
- HashMap for O(1) lookup
- Doubly linked list for O(1) eviction
- Move accessed items to head of list

## Use Cases
- Browser cache
- Database query cache
- CDN caching
