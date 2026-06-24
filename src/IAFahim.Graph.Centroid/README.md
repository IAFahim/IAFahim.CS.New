# IAFahim.Graph.Centroid

## Description
This package provides centroid decomposition for tree structures. It enables divide-and-conquer algorithms on trees by finding tree centroids and building centroid trees.

## Complexity
Building the centroid tree runs in O(N log N) time complexity, where N is the vertex count.

## API Signature
public static class CentroidDecomposition
{
    public static int Build(int n, int* head, int* to, int* next, int* centroid, int* sz, byte* removed);
    public static void Decompose(int n, int* head, int* to, int* next, int u, byte* removed, int* sz, int* centroids, int* centroidCount);
}

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* centroid = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* sz = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    byte* removed = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(byte));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            removed[i] = 0;
        }
        int root = IAFahim.Graph.Centroid.CentroidDecomposition.Build(n, head, to, next, centroid, sz, removed);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)centroid);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)sz);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)removed);
    }
}
```
