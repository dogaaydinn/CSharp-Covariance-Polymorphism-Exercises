# Performance: DI

Container resolve: ~50ns
Singleton: Fastest (cached)
Transient: New instance overhead

Hot path: Avoid transient.
