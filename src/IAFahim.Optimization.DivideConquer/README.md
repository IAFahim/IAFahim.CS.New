# IAFahim.Optimization.DivideConquer

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
```