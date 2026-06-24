# IAFahim.Graph.SpanningTrees

## Description
This package provides algorithms for spanning trees and cuts, including transitive closure construction.

## Complexity
Transitive closure construction runs in O(V * E) time.

## API Signature
```csharp
public static unsafe class StShared
{
    public static void BuildTransitiveClosure(int* eu, int* ev, int m, int n, bool* tc)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int m = 2;
    int* eu = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    int* ev = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    bool* tc = (bool*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * n * sizeof(bool));
    try
    {
        eu[0] = 0; ev[0] = 1;
        eu[1] = 1; ev[1] = 2;
        IAFahim.Graph.SpanningTrees.StShared.BuildTransitiveClosure(eu, ev, m, n, tc);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)eu);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ev);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tc);
    }
}
```