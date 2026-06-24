# IAFahim.Graph.Misc

## Description
This package provides miscellaneous graph utility algorithms, including topological dynamic programming and node access closure checks.

## Complexity
Time complexity for topological dynamic programming is O(V + E) where V is the node count and E is the edge count.

## API Signature
```csharp
public static unsafe class TopologicalDp
{
    public static long Run(int n, int* order, long* dp, int* to, int* next, int* head)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int* order = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    long* dp = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        order[0] = 0;
        order[1] = 1;
        head[0] = -1;
        head[1] = -1;
        long total = IAFahim.Graph.Misc.TopologicalDp.Run(n, order, dp, to, next, head);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)order);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)dp);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
    }
}
```