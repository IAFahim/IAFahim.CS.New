# IAFahim.Graph.Connectivity

## Description
This package provides methods for dynamic graph connectivity. It supports incremental union-find, decremental connectivity, offline dynamic connectivity, dynamic transitive closure, and fully dynamic connectivity.

## Complexity
Incremental connectivity operations run in nearly linear time using inverse Ackermann bounds. Fully dynamic connectivity queries run in O(log^2 V) amortized time.

## API Signature
public static class IncrementalConnectivity
{
    public static void Init(int* parent, int* size, int n);
    public static int Find(int* parent, int i);
    public static bool Union(int* parent, int* size, int i, int j);
    public static bool Connected(int* parent, int i, int j);
}

## Usage Example
```csharp
unsafe
{
    int n = 5;
    int* parent = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* size = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        IAFahim.Graph.Connectivity.IncrementalConnectivity.Init(parent, size, n);
        bool change = IAFahim.Graph.Connectivity.IncrementalConnectivity.Union(parent, size, 0, 1);
        bool connected = IAFahim.Graph.Connectivity.IncrementalConnectivity.Connected(parent, 0, 1);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)parent);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)size);
    }
}
```
