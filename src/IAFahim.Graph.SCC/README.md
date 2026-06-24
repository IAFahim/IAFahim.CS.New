# IAFahim.Graph.SCC

## Description
This package provides algorithms for finding strongly connected components in a directed graph, including Tarjan's algorithm and online SCC maintenance.

## Complexity
Tarjan's algorithm runs in O(V + E) time. Online SCC maintains components dynamically.

## API Signature
```csharp
public static unsafe class TarjanScc
{
    public static void Find(int n, int* head, int* next, int* to, int* sccId, int* sccCount)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* sccId = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int sccCount = 0;
    try
    {
        head[0] = -1;
        head[1] = -1;
        IAFahim.Graph.SCC.TarjanScc.Find(n, head, next, to, sccId, &sccCount);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)sccId);
    }
}
```