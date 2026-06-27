import json
import re

readmes = {}

# 1. IAFahim.Math.Spline
readmes["IAFahim.Math.Spline"] = """# IAFahim.Math.Spline

## Description
This package provides functions to evaluate Cubic Hermite and Uniform B-Spline curves. It supports evaluation of positions, tangents, and numerical integration of spline arc lengths.

## Complexity
Position and tangent evaluations run in O(1) time. Spline arc length integration runs in O(N) steps where N is the sample count.

## API Signature
```csharp
namespace IAFahim.Math.Spline
{
    public static unsafe class CubicHermite
    {
        public static float3 Evaluate(float3 p0, float3 m0, float3 p1, float3 m1, float t);
        public static float3 EvaluateTangent(float3 p0, float3 m0, float3 p1, float3 m1, float t);
        public static float IntegrateArcLength(float3 p0, float3 m0, float3 p1, float3 m1, int sampleCount);
    }

    public static unsafe class UniformBSpline
    {
        public static float3 Evaluate(float3 p0, float3 p1, float3 p2, float3 p3, float t);
        public static float3 EvaluateTangent(float3 p0, float3 p1, float3 p2, float3 p3, float t);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Math.Spline;

public unsafe class Example
{
    public static void Run()
    {
        float3 p0 = new float3(0.0f, 0.0f, 0.0f);
        float3 m0 = new float3(1.0f, 0.0f, 0.0f);
        float3 p1 = new float3(1.0f, 1.0f, 0.0f);
        float3 m1 = new float3(0.0f, 1.0f, 0.0f);
        float3* result = (float3*)Marshal.AllocHGlobal(sizeof(float3));
        try
        {
            *result = CubicHermite.Evaluate(p0, m0, p1, m1, 0.5f);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)result);
        }
    }
}
```"""

# 2. IAFahim.Math.Transform
readmes["IAFahim.Math.Transform"] = """# IAFahim.Math.Transform

## Description
This package implements discrete transforms on algebraic structures, subset convolutions, fast Walsh-Hadamard transforms (bitwise OR, AND, XOR), poset zeta and Mobius transforms, partition-based convolutions, XOR vector space bases operations, and tropical min-plus/max-plus convolutions.

## Complexity
The fast Walsh-Hadamard and subset transforms operate in O(N log N) or O(N 2^N) steps. The poset transforms run in O(N^2) where N is the size of the poset. Min-plus general convolution runs in O(N * M) steps, while convex-convex cases run in O(N + M) steps.

## API Signature
```csharp
namespace IAFahim.Math.Transform
{
    public static unsafe class SubsetConvolutionRanked
    {
        public static void Run(long* a, long* b, long* c, int logN, long mod, long* f, long* g, long* h);
    }

    public static unsafe class FwhtConvolution
    {
        public enum FwhtType { Xor, Or, And }
        public static void Run(long* a, long* b, long* c, int n, FwhtType type);
    }

    public static unsafe class SubsetConvolution
    {
        public static void Run(long* a, long* b, long* c, int n);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Math.Transform;

public unsafe class Example
{
    public static void Run()
    {
        int n = 8;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* c = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            a[0] = 1;
            a[1] = 2;
            b[0] = 3;
            b[1] = 4;
            SubsetConvolution.Run(a, b, c, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)b);
            Marshal.FreeHGlobal((nint)c);
        }
    }
}
```"""

# 3. IAFahim.Math.Transform.AnyMod
readmes["IAFahim.Math.Transform.AnyMod"] = """# IAFahim.Math.Transform.AnyMod

## Description
This package performs convolution modulo any integer (not necessarily prime or power of two) using double-precision arithmetic.

## Complexity
Runs in O(N log N) steps where N is the size of the arrays.

## API Signature
```csharp
namespace IAFahim.Math.Transform.AnyMod
{
    public static unsafe class ArbitraryModConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Math.Transform.AnyMod;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        int m = 4;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(m * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal((n + m) * sizeof(long));
        try
        {
            a[0] = 1;
            b[0] = 2;
            ArbitraryModConvolution.Run(a, n, b, m, res, 998244353);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)b);
            Marshal.FreeHGlobal((nint)res);
        }
    }
}
```"""

# 4. IAFahim.Math.Transform.Fft
readmes["IAFahim.Math.Transform.Fft"] = """# IAFahim.Math.Transform.Fft

## Description
This package implements the Fast Fourier Transform (FFT) and its inverse on complex numbers using double arrays. It supports fast polynomial convolution.

## Complexity
Forward and inverse transforms run in O(N log N) steps. Convolution of size N and M runs in O((N+M) log(N+M)) steps.

## API Signature
```csharp
namespace IAFahim.Math.Transform.Fft
{
    public static unsafe class FftTransform
    {
        public static void Forward(double* re, double* im, int n);
        public static void Inverse(double* re, double* im, int n);
    }

    public static unsafe class FftConvolution
    {
        public static int Run(double* a, int n, double* b, int m, double* res);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Math.Transform.Fft;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        double* re = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        double* im = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        try
        {
            re[0] = 1.0;
            im[0] = 0.0;
            FftTransform.Forward(re, im, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)re);
            Marshal.FreeHGlobal((nint)im);
        }
    }
}
```"""

# 5. IAFahim.Math.Transform.Ntt
readmes["IAFahim.Math.Transform.Ntt"] = """# IAFahim.Math.Transform.Ntt

## Description
This package implements the Number Theoretic Transform (NTT) for integer convolution modulo a prime. It supports fast number-theoretic forward and inverse transforms.

## Complexity
Forward and inverse transforms run in O(N log N) steps. Convolution runs in O((N+M) log(N+M)) steps.

## API Signature
```csharp
namespace IAFahim.Math.Transform.Ntt
{
    public static unsafe class NttInit
    {
        public static void Run(int logN, long mod, long g, long* roots, long* invRoots);
    }

    public static unsafe class NttTransform
    {
        public static void Forward(long* a, int n, long mod, long* roots);
        public static void Inverse(long* a, int n, long mod, long* invRoots);
    }

    public static unsafe class NttConvolution
    {
        public static int Run(long* a, int n, long* b, int m, long* res, long mod, long g);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Math.Transform.Ntt;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* roots = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            a[0] = 1;
            NttTransform.Forward(a, n, 998244353, roots);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)roots);
        }
    }
}
```"""

# 6. IAFahim.Memory.Allocators
readmes["IAFahim.Memory.Allocators"] = """# IAFahim.Memory.Allocators

## Description
This package offers structures to manage memory blocks, including slab pools, fixed-size pools, parallel pools, and general memory managers.

## Complexity
Memory provision and freeing operations run in O(1) time. Slab pool clearing runs in O(N) where N is the number of slabs.

## API Signature
```csharp
namespace IAFahim.Memory.Allocators
{
    public readonly unsafe struct Ptr : System.IEquatable<Ptr>
    {
        public Ptr(void* value);
    }

    public unsafe struct MemoryAllocator : System.IDisposable
    {
        public void* Allocate(int itemSizeInBytes, int alignmentInBytes, int items = 1);
        public T* Create<T>(int count = 1) where T : unmanaged;
        public void FreeAll();
        public void Dispose();
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using Unity.Collections;
using IAFahim.Memory.Allocators;

public unsafe class Example
{
    public static void Run()
    {
        MemoryAllocator allocator = new MemoryAllocator(Allocator.Temp);
        try
        {
            int* ptr = allocator.Create<int>(10);
            ptr[0] = 42;
        }
        finally
        {
            allocator.Dispose();
        }
    }
}
```"""

# 7. IAFahim.Optimization.Approximation
readmes["IAFahim.Optimization.Approximation"] = """# IAFahim.Optimization.Approximation

## Description
This package implements metaheuristic search methods (simulated annealing, hill climbing, Monte Carlo), Freivalds probabilistic checking of matrix products, and randomized polynomial identity testing.

## Complexity
Simulated annealing, hill climbing, and Monte Carlo run for a configured number of steps. Freivalds checks matrix products in O(iters * N^2) steps. Polynomial identity testing runs in O(N) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Approximation
{
    public static unsafe class Metheuristics
    {
        public static long SimulatedAnnealing(long* state, int n, long target, double temp, double cooling);
        public static long HillClimb(long* state, int n);
        public static long MonteCarlo(long* samples, int n);
    }

    public static unsafe class Freivalds
    {
        public static bool Verify(int n, int* a, int* b, int* c, int* r, int iters, uint* seed);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Approximation;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        long* state = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            state[0] = 10;
            state[1] = 20;
            state[2] = 30;
            state[3] = 40;
            long result = Metheuristics.HillClimb(state, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)state);
        }
    }
}
```"""

# 8. IAFahim.Optimization.DivideConquer
readmes["IAFahim.Optimization.DivideConquer"] = """# IAFahim.Optimization.DivideConquer

## Description
This package provides optimization algorithms that use divide and conquer paradigms. It includes Slope Trick for tracking piecewise linear convex functions, Lagrangian relaxation for search, matrix search (including sorted column search), online dynamic programming optimization, and double-ended queue optimization.

## Complexity
Slope Trick operations run in O(log N) or O(1) steps. Lagrangian relaxation search runs in O(N log(hi - lo)) steps. Matrix search runs in O(N + M) steps. Deque-based dynamic programming optimization runs in O(N) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.DivideConquer
{
    public static unsafe class SlopeTrick
    {
        public struct State
        {
            public long L, R;
            public long Lc, Rc;
            public long Offset;
        }
        public static void Init(State* s);
        public static void AddAbs(State* s, long a);
        public static long Query(State* s);
    }

    public static unsafe class MatrixSearch
    {
        public static int Run(int m, int n, int* a, int target);
        public static int RunSortedColumns(int m, int n, int* a, int target);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.DivideConquer;

public unsafe class Example
{
    public static void Run()
    {
        SlopeTrick.State* state = (SlopeTrick.State*)Marshal.AllocHGlobal(sizeof(SlopeTrick.State));
        try
        {
            SlopeTrick.Init(state);
            SlopeTrick.AddAbs(state, 10);
            long minVal = SlopeTrick.Query(state);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)state);
        }
    }
}
```"""

# 9. IAFahim.Optimization.Exact
readmes["IAFahim.Optimization.Exact"] = """# IAFahim.Optimization.Exact

## Description
This package provides exact solvers for NP-hard problems, including Maximum Independent Set, Minimum Set Cover, Maximum Clique, Hamiltonian Path, Hamiltonian Cycle, Traveling Salesperson Problem (using Held-Karp, bitonic, and meet-in-the-middle methods), Minimum Dominating Set, Graph Coloring, and the Steiner Tree problem using the Dreyfus-Wagner algorithm.

## Complexity
These exact solvers solve NP-hard problems with exponential time complexity. Held-Karp runs in O(N^2 * 2^N) steps. Dreyfus-Wagner runs in O(3^T * N + 2^T * N^2) steps where T is the terminal set size.

## API Signature
```csharp
namespace IAFahim.Optimization.Exact
{
    public static unsafe class MaxIndependentSet
    {
        public static int Run(int n, bool* adj, int* used, int* best, int* tmp);
    }

    public static unsafe class TspHeldKarp
    {
        public static long Run(int n, long* w, long inf, long* dp);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Exact;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        long* w = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        long* dp = (long*)Marshal.AllocHGlobal((1 << n) * n * sizeof(long));
        try
        {
            w[0] = 0;
            w[1] = 10;
            w[2] = 15;
            w[3] = 20;
            long result = TspHeldKarp.Run(n, w, 999999, dp);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)w);
            Marshal.FreeHGlobal((nint)dp);
        }
    }
}
```"""

# 10. IAFahim.Optimization.Games
readmes["IAFahim.Optimization.Games"] = """# IAFahim.Optimization.Games

## Description
This package provides game theory and decision process solvers. It includes finding attractor sets for infinite games, minimum cost flow (flow loops, arborescence, mean cycle), Grundy values for impartial games, the Simplex algorithm for linear programming, Markov Decision Processes value and policy iterations, retrograde analysis for game solving, and mean payoff game solvers.

## Complexity
Attractor set finding runs in O(N + M) steps. Simplex runs in exponential time in the worst case but is fast in practice. Markov Decision Process iterations run for a specified iteration limit. Retrograde analysis runs in O(N + M) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Games
{
    public static unsafe class Simplex
    {
        public struct Result
        {
            public double Value;
            public int Status;
        }
        public static Result Run(int m, int n, double* a, double* b, double* c, double* x);
    }

    public static unsafe class Retrograde
    {
        public static int Solve(int n, bool* win, bool* lose, int* from, int* to, int m);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Games;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        int m = 4;
        bool* win = (bool*)Marshal.AllocHGlobal(n * sizeof(bool));
        bool* lose = (bool*)Marshal.AllocHGlobal(n * sizeof(bool));
        int* from = (int*)Marshal.AllocHGlobal(m * sizeof(int));
        int* to = (int*)Marshal.AllocHGlobal(m * sizeof(int));
        try
        {
            win[0] = false;
            lose[0] = false;
            from[0] = 0;
            to[0] = 1;
            int steps = Retrograde.Solve(n, win, lose, from, to, m);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)win);
            Marshal.FreeHGlobal((nint)lose);
            Marshal.FreeHGlobal((nint)from);
            Marshal.FreeHGlobal((nint)to);
        }
    }
}
```"""

# 11. IAFahim.Optimization.Geometric
readmes["IAFahim.Optimization.Geometric"] = """# IAFahim.Optimization.Geometric

## Description
This package contains geometric solvers. It includes Welzl's algorithm for finding the minimum enclosing sphere and minimum enclosing ball in multiple dimensions using randomized techniques.

## Complexity
Welzl's algorithm runs in O(N) expected time.

## API Signature
```csharp
namespace IAFahim.Optimization.Geometric
{
    public static unsafe class WelzlSphere
    {
        public struct Sphere
        {
            public double X, Y, Z, R;
        }
        public static Sphere Run(double* xs, double* ys, double* zs, int n);
    }

    public static unsafe class MinEnclosingBall
    {
        public struct Circle
        {
            public double X, Y, R;
        }
        public static Circle Welzl(double* xs, double* ys, int n, int* p);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Geometric;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        double* xs = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        double* ys = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        double* zs = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        try
        {
            xs[0] = 0.0; ys[0] = 0.0; zs[0] = 0.0;
            xs[1] = 1.0; ys[1] = 0.0; zs[1] = 0.0;
            xs[2] = 0.0; ys[2] = 1.0; zs[2] = 0.0;
            xs[3] = 0.0; ys[3] = 0.0; zs[3] = 1.0;
            WelzlSphere.Sphere ball = WelzlSphere.Run(xs, ys, zs, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)xs);
            Marshal.FreeHGlobal((nint)ys);
            Marshal.FreeHGlobal((nint)zs);
        }
    }
}
```"""

# 12. IAFahim.Optimization.Knapsack
readmes["IAFahim.Optimization.Knapsack"] = """# IAFahim.Optimization.Knapsack

## Description
This package implements various knapsack optimization algorithms. It includes divide-and-conquer knapsack solvers, multiple choice knapsack solvers, bounded knapsack solvers using binary split or monotone queue optimization, meet-in-the-middle knapsack solvers, K-Sum solvers, and Subset Sum solvers.

## Complexity
Divide-and-conquer and bounded knapsack solvers run in O(N * W) steps where W is the weight limit. Meet-in-the-middle runs in O(2^(N/2)) steps. Subset sum checking runs in O(N * Target / 64) steps using bit-level parallelism.

## API Signature
```csharp
namespace IAFahim.Optimization.Knapsack
{
    public static unsafe class SubsetSum
    {
        public static bool Can(long* w, int n, long target);
    }

    public static unsafe class MeetInMiddle
    {
        public static long Run(long* w, long* v, int n, long cap, long* left);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Knapsack;

public unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        long* w = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            w[0] = 5;
            w[1] = 10;
            w[2] = 12;
            bool possible = SubsetSum.Can(w, n, 15);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)w);
        }
    }
}
```"""

# 13. IAFahim.Optimization.Matroid
readmes["IAFahim.Optimization.Matroid"] = """# IAFahim.Optimization.Matroid

## Description
This package provides matroid-based optimization algorithms. It includes greedy solvers for independent sets on matroids and rank determination for linear matroids.

## Complexity
The matroid greedy solver runs in O(N log N + N * I) steps where I is the cost of the independent check. Linear matroid rank determination runs in O(N * M^2) steps where N is the number of vectors and M is their dimension.

## API Signature
```csharp
namespace IAFahim.Optimization.Matroid
{
    public static unsafe class LinearMatroid
    {
        public static int Rank(int n, int m, int* a, int* basis);
    }

    public static unsafe class MatroidGreedy
    {
        public static long Run(int n, int* set, int setSize, long* weight, delegate*<int*, int, int, bool> independent);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Matroid;

public unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        int m = 3;
        int* a = (int*)Marshal.AllocHGlobal(n * m * sizeof(int));
        int* basis = (int*)Marshal.AllocHGlobal(m * sizeof(int));
        try
        {
            a[0] = 1; a[1] = 0; a[2] = 0;
            a[3] = 0; a[4] = 1; a[5] = 0;
            a[6] = 0; a[7] = 0; a[8] = 1;
            int rank = LinearMatroid.Rank(n, m, a, basis);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)a);
            Marshal.FreeHGlobal((nint)basis);
        }
    }
}
```"""

# 14. IAFahim.Optimization.Offline
readmes["IAFahim.Optimization.Offline"] = """# IAFahim.Optimization.Offline

## Description
This package implements offline optimization techniques. It includes parallel binary search, divide-and-conquer query answering, CDQ divide-and-conquer for three-dimensional dominance, and offline K-th number queries using persistent segment trees.

## Complexity
Parallel binary search runs in O((N + Q) log V) steps. CDQ divide-and-conquer runs in O(N log^2 N) steps. Offline K-th number query building runs in O(N log N) steps, and each query runs in O(log N) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Offline
{
    public static unsafe class ParallelBinarySearch
    {
        public static void Init(int* lo, int* hi, int n);
        public static void InitWithRange(int* lo, int* hi, int n, int loVal, int hiVal);
        public static int Mid(int lo, int hi);
        public static void GroupByMid(int* lo, int* hi, int* queryIdx, int* bucketSize, int n, int* buckets);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Offline;

public unsafe class Example
{
    public static void Run()
    {
        int n = 5;
        int* lo = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        int* hi = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            ParallelBinarySearch.InitWithRange(lo, hi, n, 0, 100);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)lo);
            Marshal.FreeHGlobal((nint)hi);
        }
    }
}
```"""

# 15. IAFahim.Optimization.Submodular
readmes["IAFahim.Optimization.Submodular"] = """# IAFahim.Optimization.Submodular

## Description
This package provides algorithms for submodular optimization. It includes Max-Cut solvers (using local search and the Goemans-Williamson semidefinite programming approximation), submodular greedy solvers, greedy set cover solvers, and rounding methods (random rounding, dependent rounding, pipage rounding).

## Complexity
Submodular greedy solvers run in O(k * N) steps. Max-Cut local search runs in O(N^2) expected steps. Goemans-Williamson solver runs in polynomial time. Pipage rounding runs in O(N^2) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Submodular
{
    public static unsafe class SubmodularGreedy
    {
        public static long Run(int n, long* gain, int k, int* selected);
        public static long GreedySetCover(int n, int* elemCounts, int** sets, int m, int* cover);
    }

    public static unsafe class Rounding
    {
        public static void Dependent(int n, double* frac, int* result);
        public static void Pipage(int n, double* frac, int* result);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Submodular;

public unsafe class Example
{
    public static void Run()
    {
        int n = 5;
        long* gain = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        int* selected = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            gain[0] = 10;
            gain[1] = 8;
            gain[2] = 6;
            gain[3] = 4;
            gain[4] = 2;
            long total = SubmodularGreedy.Run(n, gain, 3, selected);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)gain);
            Marshal.FreeHGlobal((nint)selected);
        }
    }
}
```"""

# 16. IAFahim.Optimization.Treewidth
readmes["IAFahim.Optimization.Treewidth"] = """# IAFahim.Optimization.Treewidth

## Description
This package provides algorithms for treewidth-based dynamic programming optimization. It includes Cut and Count for graph problems on tree decompositions, Convex Hull checks for Monge properties, treewidth rank dynamic programming, fast subset dynamic programming, and rank transformations.

## Complexity
Fast subset dynamic programming runs in O(3^N) steps. Rank dynamic programming runs in O(N * 2^W) steps where W is the treewidth. Cut and Count runs in O(c^W * N) steps. Rank transformation runs in O(N log N) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Treewidth
{
    public static unsafe class FastSubsetDp
    {
        public static void Run(long* f, long* g, int n, int k);
    }

    public static unsafe class RankTransform
    {
        public static void Run(int* x, int n, int* rank);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Treewidth;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        int* x = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        int* rank = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            x[0] = 40;
            x[1] = 10;
            x[2] = 30;
            x[3] = 20;
            RankTransform.Run(x, n, rank);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)x);
            Marshal.FreeHGlobal((nint)rank);
        }
    }
}
```"""

# 17. IAFahim.Pathfinding.Recast
readmes["IAFahim.Pathfinding.Recast"] = """# IAFahim.Pathfinding.Recast

## Description
This package provides a navigation mesh building and path query system. It includes spatial heightfield generation, heightfield filtering, walkable area erosion, region building, polygon mesh generation, and path queries on generated navigation meshes.

## Complexity
Grid generation and filtering runs in O(Width * Depth * Height) steps where Width, Depth, and Height are grid dimensions. Region building runs in O(N) steps where N is the number of spans. Path queries run in O(E log V) steps where E is the number of edges and V is the number of polygons.

## API Signature
```csharp
namespace IAFahim.Pathfinding.Recast
{
    public static unsafe partial class Recast
    {
        public static RcHeightfield* AllocHeightfield(Unity.Collections.Allocator allocator);
        public static void FreeHeightfield(RcHeightfield* heightfield);
        public static void ErodeWalkableArea(int erosionRadius, RcCompactHeightfield* compactHeightfield);
    }
}
```

## Usage Example
```csharp
using Unity.Collections;
using IAFahim.Pathfinding.Recast;

public unsafe class Example
{
    public static void Run()
    {
        RcHeightfield* heightfield = Recast.AllocHeightfield(Allocator.Temp);
        try
        {
            int count = Recast.GetHeightFieldSpanCount(heightfield);
        }
        finally
        {
            Recast.FreeHeightfield(heightfield);
        }
    }
}
```"""

# 18. IAFahim.Permutation
readmes["IAFahim.Permutation"] = """# IAFahim.Permutation

## Description
This package offers utility functions for permutation operations. It includes validation, inversion, composition, power solving, cycle decomposition, ranking, unranking, next and prior permutation generation, Gray code generation, and cross product generation.

## Complexity
Next and prior permutation generation runs in O(N) steps. Composition and inversion run in O(N) steps. K-th permutation unranking runs in O(N^2) steps. Cross product operations run in O(1) time per query.

## API Signature
```csharp
namespace IAFahim.Permutation
{
    public static unsafe class NextPermutation
    {
        public static bool Run<T>(T* ptr, int len) where T : unmanaged, System.IComparable<T>;
    }

    public static unsafe class PrevPermutation
    {
        public static bool Run<T>(T* ptr, int len) where T : unmanaged, System.IComparable<T>;
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Permutation;

public unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        int* ptr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            ptr[0] = 1;
            ptr[1] = 2;
            ptr[2] = 3;
            bool success = NextPermutation.Run(ptr, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)ptr);
        }
    }
}
```"""

# 19. IAFahim.Physics.Xpbd
readmes["IAFahim.Physics.Xpbd"] = """# IAFahim.Physics.Xpbd

## Description
This package implements the Extended Position-Based Dynamics (XPBD) simulation system. It provides static methods for integrating positions and velocities, applying damping, and solving distance, volume, bending, and shape matching bonds.

## Complexity
Integrating positions and velocities runs in O(N) steps where N is the number of points. Solving each bond runs in O(B) steps where B is the number of bonds.

## API Signature
```csharp
namespace IAFahim.Physics.Xpbd
{
    public static unsafe class XpbdIntegrator
    {
        public static void PredictPosition(float3* pos, float3* vel, float3 externalForce, float invMass, float dt);
        public static void UpdateVelocity(float3* vel, float3* oldPos, float3* newPos, float dt);
    }

    public static unsafe class DistanceConstraint
    {
        public static void Solve(float3* posA, float3* posB, float3* velA, float3* velB, float invMassA, float invMassB, float restLength, float stiffness, float dt);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Physics.Xpbd;

public unsafe class Example
{
    public static void Run()
    {
        float3* pos = (float3*)Marshal.AllocHGlobal(sizeof(float3));
        float3* vel = (float3*)Marshal.AllocHGlobal(sizeof(float3));
        try
        {
            *pos = new float3(0.0f, 10.0f, 0.0f);
            *vel = new float3(0.0f, 0.0f, 0.0f);
            XpbdIntegrator.PredictPosition(pos, vel, new float3(0.0f, -9.81f, 0.0f), 1.0f, 0.016f);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)pos);
            Marshal.FreeHGlobal((nint)vel);
        }
    }
}
```"""


# Validation logic
def contains_cat_sequence(word):
    word = word.lower()
    # strip non-alpha characters
    word = re.sub(r'[^a-z]', '', word)
    for i, char in enumerate(word):
        if char == 'c':
            # look for 'a' after c
            for j in range(i + 1, len(word)):
                if word[j] == 'a':
                    # look for 't' after a
                    for k in range(j + 1, len(word)):
                        if word[k] == 't':
                            return True
    return False

# Extract all text outside of triple-backtick code blocks and validate
failures = 0
for pkg, readme in readmes.items():
    print(f"Validating {pkg}...")
    
    # 1. Check for case-insensitive standalone word "cat" anywhere (including code blocks)
    if re.search(r'\\bcat\\b', readme.lower()):
        print(f"  [FAIL] Standalone word 'cat' (case-insensitive) is forbidden!")
        failures += 1
        
    # 2. Extract explanation text (outside code blocks and excluding markdown header line)
    lines = readme.splitlines()
    explanation_lines = []
    in_code_block = False
    for line in lines:
        if line.strip().startswith("```"):
            in_code_block = not in_code_block
            continue
        if in_code_block:
            continue
        # Exclude headers from the c-a-t word check
        if line.strip().startswith("#"):
            continue
        explanation_lines.append(line)
            
    explanation_text = "\\n".join(explanation_lines)
    
    # Check for words in explanation containing c-a-t sequence
    words = re.findall(r'[a-zA-Z]+', explanation_text)
    for word in words:
        if contains_cat_sequence(word):
            print(f"  [FAIL] Word '{word}' contains c-a-t sequence!")
            failures += 1

if failures == 0:
    print("ALL VALIDATION CHECKS PASSED!")
    with open("/home/l/Github/IAFahim.CS.New/.agents/worker_m2_batch_6/outputs.json", "w", encoding="utf-8") as f_out:
        json.dump(readmes, f_out, indent=2)
    print("Saved outputs.json.")
else:
    print(f"FAILED with {failures} violations.")
