# Tree Traversal

## Problem
Implement tree traversal algorithms.

## Tree Structure
```
     1
   /   \
  2     3
 / \   / \
4   5 6   7
```

## Traversals
- **In-Order** (Left, Root, Right): 4, 2, 5, 1, 6, 3, 7
- **Pre-Order** (Root, Left, Right): 1, 2, 4, 5, 3, 6, 7
- **Post-Order** (Left, Right, Root): 4, 5, 2, 6, 7, 3, 1
- **Level-Order** (BFS): 1, 2, 3, 4, 5, 6, 7

## Implementation
- Recursive (simple but uses stack)
- Iterative with explicit stack/queue
