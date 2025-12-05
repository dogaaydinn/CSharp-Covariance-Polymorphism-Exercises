# Graph Algorithms

## Problems

### 1. BFS (Breadth-First Search)
Find shortest path in unweighted graph.

### 2. DFS (Depth-First Search)
Detect cycles, topological sort.

### 3. Dijkstra's Algorithm
Shortest path in weighted graph.

## Example Graph
```
    A --1--> B --2--> C
    |        |        |
    3        4        5
    |        |        |
    v        v        v
    D --1--> E --1--> F
```

Shortest path A to F: A → B → E → F (distance: 6)

## Data Structure
```csharp
var graph = new Dictionary<string, List<(string node, int weight)>>();
```
