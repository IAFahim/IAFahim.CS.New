import json
import re

readmes = {}

# 0: IAFahim.Algebra.GraphPoly
readmes["IAFahim.Algebra.GraphPoly"] = """# IAFahim.Algebra.GraphPoly

## Description
This package provides functions to evaluate graph polynomials. It supports Tutte polynomials, independence polynomials, matching polynomials, reliability polynomials, rook polynomials, and chromatic polynomials. All calculations use unsafe raw pointers to achieve maximum efficiency without managed overhead.

## Complexity
- Tutte polynomial subset evaluation: O(2^E) where E is the number of edges.
- Independence polynomial evaluation: O(2^V) where V is the number of vertices.
- Chromatic polynomial subset evaluation: O(2^V * V) where V is the number of vertices.
- Matching polynomial evaluation: O(2^V) where V is the number of vertices.
- Reliability polynomial: O(2^E) where E is the number of edges.
- Rook polynomial evaluation: O(2^(N*M)) where N and M are the grid dimensions.

## API Signature
```csharp
public static unsafe class Tutte
{
    public static long Subset(int n, int edges, int* from, int* to, long x, long y, int MOD);
}

public static unsafe class Independence
{
    public static long Polynomial(int n, bool* adj, long x, int MOD);
}

public static unsafe class Chromatic
{
    public static void Subset(int n, bool* adj, int MOD, long* coeffs);
    public static int NumberDp(int n, bool* adj, int MOD);
    public static void DeletionContraction(int n, bool* adj, int edges, int* from, int* to, int MOD, long* coeffs);
}

public static unsafe class Matching
{
    public static long Polynomial(int n, bool* adj, long x, int MOD);
}

public static unsafe class Reliability
{
    public static long Run(int n, int edges, int* from, int* to, long p, int MOD);
}

public static unsafe class Rook
{
    public static long Run(int n, int m, bool* blocked, long x, int MOD);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Algebra.GraphPoly;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        int edges = 3;
        int* from = (int*)Marshal.AllocHGlobal(edges * sizeof(int));
        int* to = (int*)Marshal.AllocHGlobal(edges * sizeof(int));
        try
        {
            from[0] = 0; to[0] = 1;
            from[1] = 1; to[1] = 2;
            from[2] = 2; to[2] = 0;
            long result = Tutte.Subset(n, edges, from, to, 2, 2, 998244353);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)from);
            Marshal.FreeHGlobal((nint)to);
        }
    }
}
```"""

# 1: IAFahim.Algebra.Polynomial
readmes["IAFahim.Algebra.Polynomial"] = """# IAFahim.Algebra.Polynomial

## Description
This package provides algorithms for univariate polynomial operations over finite fields. It includes division, greatest common divisor, multipoint evaluation, interpolation, roots searching, and factorization. It also implements polynomial product computation using Number Theoretic Transform, Schonhage-Strassen, and Toom-Cook algorithms. All methods run on raw pointers for maximum performance.

## Complexity
- Polynomial product computation (NTT): O(N log N) where N is the polynomial degree.
- Division and GCD: O(N log^2 N) where N is the polynomial degree.
- Multipoint evaluation and interpolation: O(N log^2 N) where N is the number of points.
- Cantor-Zassenhaus factorization: O(D^3 * log Q) where D is the degree and Q is the field size.
- Berlekamp-Massey algorithm: O(N^2) where N is the sequence length.

## API Signature
```csharp
public static unsafe class BerlekampMassey
{
    public static int Run(long* s, int n, int MOD, long* c);
}

public static unsafe class CantorZassenhaus
{
    public static int Run(long* poly, int n, int MOD, long* outF, int* outL);
}

public static unsafe class PowMod
{
    public static void Run(long* poly, int lenPoly, long exponent, long* modPoly, int lenModPoly, long* result, out int lenResult, int MOD);
}

public static unsafe class Gcd
{
    public static void Run(long* a, int lenA, long* b, int lenB, long* gcd, out int lenGcd, int MOD);
}

public static unsafe class BostanMori
{
    public static long Run(long* p, int pLen, long* q, int qLen, long k, int MOD);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Algebra.Polynomial;

public static unsafe class Example
{
    public static void Run()
    {
        int lenA = 3;
        int lenB = 2;
        long* a = (long*)Marshal.AllocHGlobal(lenA * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(lenB * sizeof(long));
        long* gcd = (long*)Marshal.AllocHGlobal(lenA * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 2; a[2] = 1;
            b[0] = 1; b[1] = 1;
            int lenGcd;
            Gcd.Run(a, lenA, b, lenB, gcd, out lenGcd, 998244353);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)b);
            Marshal.FreeHGlobal((nint)gcd);
        }
    }
}
```"""

# 2: IAFahim.Algebra.Sequence
readmes["IAFahim.Algebra.Sequence"] = """# IAFahim.Algebra.Sequence

## Description
This package provides methods to generate, rank, and transform combinatorial sequences and values. It supports Prufer sequence transformations, binomial transforms, Stirling numbers of the first and second kind, Bell numbers, Eulerian numbers, Narayana numbers, and Lah numbers. It also supports generating function operations such as exponential and ordinary generating function products.

## Complexity
- Stirling numbers row computation: O(N log N) where N is the row index.
- Binomial transform: O(N log N) where N is the sequence length.
- Egf / Ogf product: O(N log N) where N is the sequence length.
- Prufer sequence rank: O(N log N) where N is the sequence length.

## API Signature
```csharp
public static unsafe class Prufer
{
    public static long Rank(int* seq, int n, int MOD);
    public static void Unrank(long rank, int n, int MOD, int* seq);
}

public static unsafe class Transform
{
    public static void Binomial(long* a, int n, int MOD, long* b);
    public static void InverseBinomial(long* a, int n, int MOD, long* b);
}

public static unsafe class Combinatorial
{
    public static long Eulerian(int n, int k, int MOD);
    public static long Lah(int n, int k, int MOD);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Algebra.Sequence;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 5;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 1; a[2] = 1; a[3] = 1; a[4] = 1;
            Transform.Binomial(a, n, 998244353, b);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)b);
        }
    }
}
```"""

# 3: IAFahim.Collision.Gjk
readmes["IAFahim.Collision.Gjk"] = """# IAFahim.Collision.Gjk

## Description
This package implements the Gilbert-Johnson-Keerthi (GJK) collision detection algorithm and the Expanding Polytope Algorithm (EPA) for three-dimensional physics queries. It computes overlap and minimum distance between convex shapes defined by support functions. Shape support functions include sphere, box, capsule, and convex hull.

## Complexity
- GJK intersection query: O(I) where I is the iteration count.
- EPA penetration depth: O(F) where F is the number of faces in the expanding polytope.
- Convex hull support query: O(V) where V is the number of points in the hull.

## API Signature
```csharp
public static unsafe class Gjk
{
    public delegate float3 SupportFunction(float3 direction);
    public static bool Intersect(SupportFunction supportA, SupportFunction supportB);
    public static bool Intersect(SupportFunction supportA, SupportFunction supportB, float3* outSimplex, out int outCount);
    public static float Distance(SupportFunction supportA, SupportFunction supportB);
}

public static unsafe class MinkowskiDifference
{
    public static float3 SphereSupport(float3 direction, float3 center, float radius);
    public static float3 BoxSupport(float3 direction, float3 center, float3 halfExtents);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Collision.Gjk;

public static unsafe class Example
{
    public static void Run()
    {
        Gjk.SupportFunction supportA = delegate(float3 dir)
        {
            return MinkowskiDifference.SphereSupport(dir, new float3(0, 0, 0), 1.0f);
        };
        Gjk.SupportFunction supportB = delegate(float3 dir)
        {
            return MinkowskiDifference.SphereSupport(dir, new float3(0.5f, 0, 0), 1.0f);
        };
        bool overlapping = Gjk.Intersect(supportA, supportB);
    }
}
```"""

# 4: IAFahim.Combinatorics.Generation
readmes["IAFahim.Combinatorics.Generation"] = """# IAFahim.Combinatorics.Generation

## Description
This package provides enumerators and generators for combinatorial objects. It supports set partitions, permutations, combinations, necklaces, bracelets, and random graph structures. It also includes methods to rank and unrank these objects to convert them to and from integers.

## Complexity
- Permutation generation: O(1) amortized.
- Combination generation: O(1) amortized.
- Random tree generation: O(N) where N is the number of nodes.

## API Signature
```csharp
public static unsafe class Permutations
{
    public static bool NextPermutation(int* ptr, int len);
    public static void RandomPermutation(int n, int* a, ref uint seed);
}

public static unsafe class Combinations
{
    public static bool TryNextMultiset(int* m, int n, int k, int* comb, ref bool first);
}

public static unsafe class SetPartitions
{
    public static bool UnrankIntegerPartition(long rank, int n, int* outPart, out int outLen);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Combinatorics.Generation;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            arr[0] = 1; arr[1] = 2; arr[2] = 3; arr[3] = 4;
            bool active = true;
            while (active)
            {
                active = Permutations.NextPermutation(arr, n);
            }
        }
        finally
        {
            Marshal.FreeHGlobal((nint)arr);
        }
    }
}
```"""

# 5: IAFahim.Compress
readmes["IAFahim.Compress"] = """# IAFahim.Compress

## Description
This package provides algorithms for compressing and restoring integer arrays. It transforms regular raw values into a compressed representation and provides tools to restore the original values. It helps minimize memory footprint when storing large lists of numbers.

## Complexity
- CompressValues: O(N) where N is the array length.
- RestoreCompressed: O(N) where N is the array length.

## API Signature
```csharp
public static unsafe class CompressValues
{
    public static void Run(int* src, long* dst, int len);
    public static int RunUnique(int* src, long* dst, int len);
}

public static unsafe class RestoreCompressed
{
    public static void Run(long* src, int* dst, int len);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Compress;

public static unsafe class Example
{
    public static void Run()
    {
        int len = 5;
        int* src = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        long* dst = (long*)Marshal.AllocHGlobal(len * sizeof(long));
        try
        {
            src[0] = 10; src[1] = 20; src[2] = 10; src[3] = 30; src[4] = 20;
            CompressValues.Run(src, dst, len);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)src);
            Marshal.FreeHGlobal((nint)dst);
        }
    }
}
```"""

# 6: IAFahim.Compress.Coordinate
readmes["IAFahim.Compress.Coordinate"] = """# IAFahim.Compress.Coordinate

## Description
This package provides coordinate discretization and rank compression for coordinates. It transforms an array of numbers into their relative sorted rank offsets, reducing the range of values to [0, U-1] where U is the count of unique values. This is useful for data structures that require small coordinate ranges.

## Complexity
- RankCompress: O(N log N) where N is the array length.
- CoordinateCompress: O(N log N) where N is the array length.
- Discretize: O(N log N) where N is the array length.

## API Signature
```csharp
public static unsafe class RankCompress
{
    public static int Run(int* src, int* dst, int* tmpSorted, int len);
}

public static unsafe class CoordinateCompress
{
    public static int Run(int* src, int* tmp, int* dstMap, int len);
}

public static unsafe class Discretize
{
    public static int Run(int* src, int len);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Compress.Coordinate;

public static unsafe class Example
{
    public static void Run()
    {
        int len = 4;
        int* src = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* dst = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        int* tmp = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            src[0] = 100; src[1] = 500; src[2] = 200; src[3] = 500;
            int uniqueCount = RankCompress.Run(src, dst, tmp, len);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)src);
            Marshal.FreeHGlobal((nint)dst);
            Marshal.FreeHGlobal((nint)tmp);
        }
    }
}
```"""

# 7: IAFahim.DP
readmes["IAFahim.DP"] = """# IAFahim.DP

## Description
This package provides a collection of dynamic programming algorithms and optimizations. It includes multiple knapsack variants, subset sum solvers, divide and conquer optimization, Knuth optimization, Convex Hull Trick, Li Chao Tree, SMAWK algorithm, Alien DP, and Sum over Subsets (SOS) DP.

## Complexity
- Knapsack 01: O(N * W) where N is the item count and W is the capacity.
- SMAWK: O(N + M) for an N x M totally monotone grid.
- Sum over Subsets: O(N * 2^N) where N is the number of bits.
- Convex Hull Trick query: O(log N) or O(1).

## API Signature
```csharp
public static unsafe class Knapsack01
{
    public static long Run(int n, long capacity, long* weight, long* value, long* dp);
    public static long RunSpaceOptimized(int n, long capacity, long* weight, long* value, long* dp);
}

public static unsafe class SubsetSum
{
    public static bool Run(int n, long target, long* arr, bool* dp);
}

public static unsafe class SosDp
{
    public static void Run(int n, long* f);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DP;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        long target = 10;
        long* arr = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        bool* dp = (bool*)Marshal.AllocHGlobal((target + 1) * sizeof(bool));
        try
        {
            arr[0] = 3; arr[1] = 5; arr[2] = 8;
            bool possible = SubsetSum.Run(n, target, arr, dp);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)arr);
            Marshal.FreeHGlobal((nint)dp);
        }
    }
}
```"""

# 8: IAFahim.DP.General
readmes["IAFahim.DP.General"] = """# IAFahim.DP.General

## Description
This package provides general dynamic programming routines. It implements profile DP, broken profile DP, tree knapsack, interval DP, min-plus convolution, and quadrangle inequality DP optimizations. It operates using raw pointers for speed.

## Complexity
- Profile DP: O(N * 2^M) where M is the profile width and N is the grid length.
- Tree Knapsack: O(N * W) where N is the tree node count and W is the capacity constraint.
- Interval DP: O(N^3) or O(N^2) with optimization.

## API Signature
```csharp
public static unsafe class ProfileDp
{
    public static long Run(int m, int n, int* a, long* dp, long* tmp);
}

public static unsafe class TreeKnapsack
{
    public static void Run(int u, int p, int* head, int* to, int* next, int* w, long* v, long* dp, long* tmp, int cap);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DP.General;

public static unsafe class Example
{
    public static void Run()
    {
        int m = 3;
        int n = 3;
        int* a = (int*)Marshal.AllocHGlobal((m * n) * sizeof(int));
        long* dp = (long*)Marshal.AllocHGlobal((1 << m) * sizeof(long));
        long* tmp = (long*)Marshal.AllocHGlobal((1 << m) * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 0; a[2] = 1;
            a[3] = 0; a[4] = 1; a[5] = 0;
            a[6] = 1; a[7] = 0; a[8] = 1;
            long result = ProfileDp.Run(m, n, a, dp, tmp);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)dp);
            Marshal.FreeHGlobal((nint)tmp);
        }
    }
}
```"""

# 9: IAFahim.DP.Knapsack
readmes["IAFahim.DP.Knapsack"] = """# IAFahim.DP.Knapsack

## Description
This package provides dynamic programming algorithms specifically for knapsack optimization. It features implementations for the 0-1 knapsack, unbounded knapsack, and bounded knapsack models, along with subset sum and bitset-accelerated subset sum solvers.

## Complexity
- 0-1 Knapsack: O(N * W) where N is the number of items and W is the capacity.
- Unbounded Knapsack: O(N * W) where N is the number of items and W is the capacity.
- Bounded Knapsack: O(W * Sum(log(Count_i))) using binary split.

## API Signature
```csharp
public static unsafe class Knapsack01
{
    public static long Run(int n, long cap, long* w, long* v, long* dp);
}

public static unsafe class KnapsackUnbounded
{
    public static long Run(int n, long cap, long* w, long* v, long* dp);
}

public static unsafe class KnapsackBounded
{
    public static long Run(int n, long cap, long* w, long* v, long* cnt, long* dp);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DP.Knapsack;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        long cap = 50;
        long* w = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* v = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* dp = (long*)Marshal.AllocHGlobal((cap + 1) * sizeof(long));
        try
        {
            w[0] = 10; v[0] = 60;
            w[1] = 20; v[1] = 100;
            w[2] = 30; v[2] = 120;
            long maxVal = Knapsack01.Run(n, cap, w, v, dp);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)w);
            Marshal.FreeHGlobal((nint)v);
            Marshal.FreeHGlobal((nint)dp);
        }
    }
}
```"""

# 10: IAFahim.DP.Optimization
readmes["IAFahim.DP.Optimization"] = """# IAFahim.DP.Optimization

## Description
This package provides dynamic programming optimizations. It features Knuth optimization for reducing complexity on interval DP, and a Li Chao tree line-insertion optimization for linear function queries.

## Complexity
- Knuth Optimization: O(N^2) instead of O(N^3) for interval DP.
- Li Chao Tree insertion/query: O(log R) where R is the coordinate range.

## API Signature
```csharp
public static unsafe class KnuthOptimization
{
    public static long Run(int n, long* dp, long* a, long* opt);
}

public static unsafe class LiChaoAddLine
{
    public static void Run(long* seg, long m, long b, int node, int l, int r, long x1, long x2);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DP.Optimization;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        long* dp = (long*)Marshal.AllocHGlobal((n * n) * sizeof(long));
        long* a = (long*)Marshal.AllocHGlobal((n * n) * sizeof(long));
        long* opt = (long*)Marshal.AllocHGlobal((n * n) * sizeof(long));
        try
        {
            long cost = KnuthOptimization.Run(n, dp, a, opt);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)dp);
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)opt);
        }
    }
}
```"""

# 11: IAFahim.DS.Dsu
readmes["IAFahim.DS.Dsu"] = """# IAFahim.DS.Dsu

## Description
This package provides a Disjoint Set Union (DSU) implementation. It supports path compression, union by size, rollback operations, bipartite graph checks with parity, and small-to-large merging.

## Complexity
- Find with path compression: O(alpha(N)) amortized.
- Union: O(alpha(N)) amortized.
- Rollback Union: O(log N) per operation.

## API Signature
```csharp
public static unsafe class DsuInit
{
    public static void Run(int* parent, int* size, int n);
}

public static unsafe class DsuFind
{
    public static int Run(int* parent, int x);
    public static int RunPathCompression(int* parent, int x);
}

public static unsafe class DsuUnion
{
    public static bool Run(int* parent, int* size, int a, int b);
}

public static unsafe class DsuRollback
{
    public static void Run(int* parent, int* size, int* history, int targetHistSize, int* currentHistSize);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.Dsu;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 5;
        int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        int* size = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            DsuInit.Run(parent, size, n);
            DsuUnion.Run(parent, size, 0, 1);
            int root0 = DsuFind.Run(parent, 0);
            int root1 = DsuFind.Run(parent, 1);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)parent);
            Marshal.FreeHGlobal((nint)size);
        }
    }
}
```"""

# 12: IAFahim.DS.Fenwick
readmes["IAFahim.DS.Fenwick"] = """# IAFahim.DS.Fenwick

## Description
This package provides a Fenwick Tree (Binary Indexed Tree) implementation. It supports point updates, range sum queries in both one and two dimensions, range updates with point queries, and persistent variants.

## Complexity
- Point update / Prefix sum query: O(log N) where N is the array size.
- 2D point update / 2D prefix query: O(log N * log M) where N x M is the grid size.
- Persistent update / query: O(log N) time and space.

## API Signature
```csharp
public static unsafe class Fenwick
{
    public static void AddInt64(long* bit, int n, int idx, long val);
    public static long SumInt64(long* bit, int idx);
    public static long RangeSumInt64(long* bit, int l, int r);
}

public static unsafe class Fenwick2DAdd
{
    public static void Run(long* bit, int n, int m, int x, int y, long val);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.Fenwick;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 10;
        long* bit = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
        try
        {
            for (int i = 0; i <= n; i++)
                bit[i] = 0;
            Fenwick.AddInt64(bit, n, 3, 5);
            long sum = Fenwick.SumInt64(bit, 3);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)bit);
        }
    }
}
```"""

# 13: IAFahim.DS.FixedCollections
readmes["IAFahim.DS.FixedCollections"] = """# IAFahim.DS.FixedCollections

## Description
This package provides fixed-size and unmanaged collection types that do not depend on garbage collection. It includes spin locks, fixed-size bitmasks, fixed-size hash maps, thread-local collections, thread-safe random number helpers, fast counters, and unmanaged object pools.

## Complexity
- FixedHashMap lookup / insertion: O(1) on average.
- FixedBitMask set / get: O(1).
- UnmanagedPool acquire / return: O(1).
- SpinLock acquire / release: O(1).

## API Signature
```csharp
public struct SpinLock
{
    public void Acquire();
    public bool TryAcquire();
    public void Release();
}

public unsafe struct FixedBitMask<T>
{
    public int Length { get; }
    public void Set(int pos, bool value);
    public bool IsSet(int pos);
    public void Reset();
}

public unsafe struct FixedHashMap<TKey, TValue, TCapacity>
{
    public int Capacity { get; }
    public int Count { get; }
    public bool TryAdd(TKey key, TValue item);
    public bool TryGetValue(TKey key, out TValue item);
}

public unsafe struct NativeCounter : IDisposable
{
    public int Increment();
    public int Count { get; }
    public bool IsCreated { get; }
    public void Dispose();
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.FixedCollections;

public static unsafe class Example
{
    public static void Run()
    {
        FixedHashMap<int, float, int> map = default;
        bool added = map.TryAdd(10, 3.14f);
        float val;
        bool found = map.TryGetValue(10, out val);
    }
}
```"""

# 14: IAFahim.DS.GapBuffer
readmes["IAFahim.DS.GapBuffer"] = """# IAFahim.DS.GapBuffer

## Description
This package provides a gap buffer structure for efficient text editing operations. It keeps an empty gap at the current edit position, enabling fast insertion and deletion at that cursor offset. It avoids copying the entire buffer on consecutive edits.

## Complexity
- Insertion at cursor: O(K) where K is the length of inserted data.
- Deletion at cursor: O(L) where L is the length of deleted data.
- Moving cursor: O(D) where D is the distance moved.

## API Signature
```csharp
public unsafe struct GapBufferState
{
    public int Capacity;
    public int GapStart;
    public int GapEnd;
}

public static unsafe class GapBufferInsert
{
    public static void Run(ref GapBufferState s, int pos, byte* data, int len);
}

public static unsafe class GapBufferDelete
{
    public static void Run(ref GapBufferState s, int pos, int len);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.GapBuffer;

public static unsafe class Example
{
    public static void Run()
    {
        GapBufferState state = default;
        state.Capacity = 100;
        state.GapStart = 0;
        state.GapEnd = 100;
        byte* buffer = (byte*)Marshal.AllocHGlobal(state.Capacity * sizeof(byte));
        try
        {
            byte val = 65;
            GapBufferInsert.Run(ref state, 0, &val, 1);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)buffer);
        }
    }
}
```"""

# 15: IAFahim.DS.Grid
readmes["IAFahim.DS.Grid"] = """# IAFahim.DS.Grid

## Description
This package provides helper functions for manipulating two-dimensional grids stored in flat arrays. It supports grid generation, rotation, reversal, cell shuffling, neighbor collection (4-way and 8-way), breadth-first search pathfinding, and fast cell filling.

## Complexity
- Grid rotation: O(W * H) where W and H are the grid width and height.
- Neighbor collection: O(1).
- Breadth-first search: O(W * H) time.

## API Signature
```csharp
public static unsafe class MakeGrid
{
    public static void Run(int* ptr, int len, int width, int height);
}

public static unsafe class Rotate
{
    public static void Run<T>(T* ptr, int width, int height, bool clockwise, T* temp) where T : unmanaged;
}

public static unsafe class GridNeighbors4
{
    public const int MaxNeighbors = 4;
    public static int Collect(int r, int c, int height, int width, int* nr, int* nc);
    public static int CollectFlat(int r, int c, int height, int width, int* outIndices);
}

public static unsafe class GridBfs
{
    public static int Run(int height, int width, int sr, int sc, int* dist, long* visited, int* queue);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.Grid;

public static unsafe class Example
{
    public static void Run()
    {
        int width = 3;
        int height = 3;
        int len = width * height;
        int* grid = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            MakeGrid.Run(grid, len, width, height);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)grid);
        }
    }
}
```"""

# 16: IAFahim.DS.Heap
readmes["IAFahim.DS.Heap"] = """# IAFahim.DS.Heap

## Description
This package provides priority queue and deque operations on raw buffers. It includes binary heap insertion, deletion, and heapify helpers, deque push and pop for double-ended queues, monotonic queue minimum queries, and monotonic stack processing.

## Complexity
- Heap push / pop: O(log N) where N is the heap size.
- Heapify (HeapFix): O(log N).
- Deque push / pop: O(1).
- Monotonic queue window queries: O(N) amortized.

## API Signature
```csharp
public static unsafe class HeapPush
{
    public static void Run<T>(T* ptr, int len, T val) where T : unmanaged, IComparable<T>;
}

public static unsafe class HeapPop
{
    public static T Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>;
}

public static unsafe class MonotonicQueueMin
{
    public static void Run(int* src, int* dst, int len, int windowSize);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.Heap;

public static unsafe class Example
{
    public static void Run()
    {
        int cap = 10;
        int* heap = (int*)Marshal.AllocHGlobal(cap * sizeof(int));
        try
        {
            HeapPush.Run(heap, 0, 42);
            HeapPush.Run(heap, 1, 15);
            int minVal = HeapPop.Run(heap, 2);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)heap);
        }
    }
}
```"""

# 17: IAFahim.DS.HilbertOrder
readmes["IAFahim.DS.HilbertOrder"] = """# IAFahim.DS.HilbertOrder

## Description
This package provides algorithms to encode multi-dimensional coordinates into one-dimensional order values. It features the Hilbert space-filling curve, Gilbert curve for arbitrary grid sizes, and block-based query ordering for offline query sorting algorithms.

## Complexity
- Hilbert encode: O(log N) where N is the grid dimension.
- Gilbert encode: O(log(W * H)) where W and H are the grid dimensions.
- Block sort order encode / decode: O(1).

## API Signature
```csharp
public static unsafe class HilbertOrder
{
    public static long Run(long x, long y, int pow, int rot);
    public static long Encode(long x, long y, int logN);
}

public static unsafe class GilbertOrder
{
    public static long Encode(long x, long y, int w, int h);
}

public static unsafe class BlockOrder
{
    public static long Encode(int l, int r, int blockSize);
    public static void Decode(long code, int n, int blockSize, int* l, int* r);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.HilbertOrder;

public static unsafe class Example
{
    public static void Run()
    {
        long x = 5;
        long y = 12;
        int logN = 4;
        long hilbertCode = HilbertOrder.Encode(x, y, logN);
    }
}
```"""

# 18: IAFahim.DS.LinkCut
readmes["IAFahim.DS.LinkCut"] = """# IAFahim.DS.LinkCut

## Description
This package implements a Link-Cut Tree data structure. It represents a forest of trees and supports tree structural changes (linking and cutting paths) and path query operations. It is designed using splay trees and raw node pointers.

## Complexity
- Access / MakeRoot: O(log N) amortized.
- Link / Cut: O(log N) amortized.
- Path query: O(log N) amortized.

## API Signature
```csharp
public unsafe struct LctNode
{
    public int Index;
    public bool Rev;
}

public static unsafe class LinkCut
{
    public static void Access(LctNode* x);
    public static void MakeRoot(LctNode* x);
    public static LctNode* FindRoot(LctNode* x);
    public static void Link(LctNode* x, LctNode* y);
    public static void Cut(LctNode* x, LctNode* y);
    public static long Query(LctNode* x, LctNode* y);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.LinkCut;

public static unsafe class Example
{
    public static void Run()
    {
        int nodeCount = 3;
        LctNode* nodes = (LctNode*)Marshal.AllocHGlobal(nodeCount * sizeof(LctNode));
        try
        {
            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i].Index = i;
                nodes[i].Rev = false;
            }
            LinkCut.MakeRoot(&nodes[0]);
            LinkCut.MakeRoot(&nodes[1]);
            LinkCut.Link(&nodes[0], &nodes[1]);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)nodes);
        }
    }
}
```"""

# Let's perform validation:
# The word "cat" (case-insensitive) is strictly forbidden in the entire README.
# Also, check each word to ensure it doesn't contain 'c', 'a', 't' in sequence.
errors = 0
for pkg, text in readmes.items():
    if 'cat' in text.lower():
        print(f"Error: {pkg} contains 'cat' (case-insensitive)!")
        # Find and print the context
        for match in re.finditer('cat', text.lower()):
            start = max(0, match.start() - 25)
            end = min(len(text), match.end() + 25)
            print(f"  Context: ... {text[start:end].strip()} ...")
        errors += 1
        
    # Split by non-alphabetic characters to get words
    words = re.findall(r'[a-zA-Z]+', text)
    for word in words:
        if 'cat' in word.lower():
            print(f"Error: {pkg} contains forbidden word '{word}' which has 'cat' in sequence!")
            errors += 1

if errors == 0:
    print("All validations PASSED!")
    with open("/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_1/outputs.json", "w", encoding="utf-8") as f:
        json.dump(readmes, f, indent=2, ensure_ascii=False)
    print("Successfully wrote outputs.json")
else:
    print(f"Validation FAILED with {errors} errors.")
