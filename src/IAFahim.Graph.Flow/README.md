# IAFahim.Graph.Flow

## Description
This package provides flow network routines, including maximum flow, minimum cut, minimum cost maximum flow, and vertex-limited flows.

## Complexity
Time complexity depends on the chosen method; push-relabel runs in O(V^2 * E) time, Dinic runs in O(V^2 * E) time.

## API Signature
```csharp
public static unsafe class PushRelabelGap
{
    public static long Run(int n, int s, int t, int* head, int* to, int* next, int* cap, int* flow)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* cap = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* flow = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    try
    {
        head[0] = -1;
        head[1] = -1;
        long total = IAFahim.Graph.Flow.PushRelabelGap.Run(n, 0, 1, head, to, next, cap, flow);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)cap);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)flow);
    }
}
```