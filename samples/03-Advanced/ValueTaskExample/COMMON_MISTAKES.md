# Mistakes

1. Awaiting ValueTask twice (runtime error!)
2. Using ValueTask for always-async operations (use Task)
3. Storing ValueTask in field (consume immediately)
