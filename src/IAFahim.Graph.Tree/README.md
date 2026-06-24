# IAFahim.Graph.Tree

## Description
This package provides basic and advanced tree algorithms, including Lowest Common Ancestor queries and Heavy-Light Decomposition.

## Complexity
Lowest Common Ancestor query runs in O(log V) time after O(V log V) preprocessing.

## API Signature
```csharp
public static unsafe class LcaBuild
{
    public static void Run(int n, int root, int* head, int* to, int* next, int* parent, int* depth, int* ancestors, int logN)
    public static int Run(int u, int v, int* depth, int* ancestors, int logN)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int logN = 2;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* parent = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* depth = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* ancestors = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * logN * sizeof(int));
    try
    {
        head[0] = -1;
        head[1] = -1;
        head[2] = -1;
        IAFahim.Graph.Tree.LcaBuild.Run(n, 0, head, to, next, parent, depth, ancestors, logN);
        int lca = IAFahim.Graph.Tree.LcaBuild.Run(1, 2, depth, ancestors, logN);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)parent);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)depth);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ancestors);
    }
}
```