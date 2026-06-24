# IAFahim.Graph.Cactus

## Description
This package provides algorithms for graphs where any two simple cycles share at most one vertex. It includes cycle decomposition, shortest path queries, bridge tree diameter solving, and lowest common ancestor query support.

## Complexity
Cycle decomposition and bridge tree diameter solving run in O(V + E) time. Shortest path and ancestor queries run in O(log V) time.

## API Signature
public static class CactusCycleDecompose
{
    public static int Run(int* head, int* to, int* next, int n, int m, int* cycleId);
}

## Usage Example
```csharp
unsafe
{
    int n = 5;
    int m = 4;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    int* cycleId = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(m * sizeof(int));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
        }
        for (int i = 0; i < m; i++)
        {
            to[i] = 0;
            next[i] = -1;
            cycleId[i] = -1;
        }
        int count = IAFahim.Graph.Cactus.CactusCycleDecompose.Run(head, to, next, n, m, cycleId);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)cycleId);
    }
}
```
