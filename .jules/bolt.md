## 2024-05-18 - DSU Path Halving vs Compression
**Learning:** In C# high-performance zero-dependency libraries where allocating scratch arrays is prohibited, recursive path compression can be noticeably slower than iterative path halving in tight loop benchmarks due to call stack overhead.
**Action:** When optimizing Disjoint Set Union structures without a pre-allocated stack for iterative full compression, prefer `parent[x] = parent[parent[x]]; x = parent[x];` (path halving) with `[MethodImpl(MethodImplOptions.AggressiveInlining)]` for best constant factors.
