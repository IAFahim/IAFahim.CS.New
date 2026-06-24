# IAFahim.Graph.Eulerian

## Description
This package provides algorithms to search for Eulerian paths and Eulerian cycles in a graph.

## Complexity
Time complexity is O(V + E) where V is the node count and E is the edge count.

## API Signature
```csharp
public static unsafe class EulerShared
{
    public static int Run(int n, int* head, int* to, int* next, int start, int* path)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(int));
    int* path = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(int));
    try
    {
        head[0] = -1;
        head[1] = -1;
        head[2] = -1;
        int count = IAFahim.Graph.Eulerian.EulerShared.Run(n, head, to, next, 0, path);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)path);
    }
}
```