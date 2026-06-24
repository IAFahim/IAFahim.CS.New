# IAFahim.Graph.Functional

## Description
This package provides algorithms for functional graphs, where every node has exactly one outgoing edge. It includes path queries, cycle detection, and meeting points.

## Complexity
Path successor query runs in O(log K) time using binary lifting. Cycle detection runs in O(V) time.

## API Signature
```csharp
public static unsafe class PermutationCyclePower
{
    public static void Run(int* p, int n, long k, int* res)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* p = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* res = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        p[0] = 1;
        p[1] = 2;
        p[2] = 0;
        IAFahim.Graph.Functional.PermutationCyclePower.Run(p, n, 5, res);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)p);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)res);
    }
}
```