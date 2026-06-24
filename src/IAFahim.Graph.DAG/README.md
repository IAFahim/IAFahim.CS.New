# IAFahim.Graph.DAG

## Description
This package provides algorithms for directed acyclic graphs. It supports topological sorting, path counts, longest antichain search, minimum path covers, and cycle checks.

## Complexity
Time complexity for topological sorting is O(V + E) where V is the node count and E is the edge count. Minimum path cover runs in O(V * E) time.

## API Signature
```csharp
public static unsafe class CountTopologicalOrders
{
    public static long Run(int* adjMask, int n, long* dp)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 4;
    int* adjMask = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    long* dp = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal((1 << n) * sizeof(long));
    try
    {
        adjMask[0] = 2;
        adjMask[1] = 4;
        adjMask[2] = 8;
        adjMask[3] = 0;
        long total = IAFahim.Graph.DAG.CountTopologicalOrders.Run(adjMask, n, dp);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)adjMask);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)dp);
    }
}
```