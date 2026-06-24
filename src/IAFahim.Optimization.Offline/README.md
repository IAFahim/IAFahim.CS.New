# IAFahim.Optimization.Offline

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
```