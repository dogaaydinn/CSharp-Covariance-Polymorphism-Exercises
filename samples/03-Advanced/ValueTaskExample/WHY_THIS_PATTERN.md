# Why ValueTask?

Task<T> always allocates on heap.
ValueTask<T> is struct - no allocation when synchronous.
