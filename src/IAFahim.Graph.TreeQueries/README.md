# IAFahim.Graph.TreeQueries

## Description
This package provides tree query algorithms, including tree centroids, path color counting, Steiner trees, and tree hashing.

## Complexity
Steiner tree runs in O(V * 3^T) time where T is the terminal node count. Tree hashing runs in O(V log V) time.

## API Signature
```csharp
public static unsafe class TreeCentroid
{
    public static void AllCentroids(int n, int* head, int* to, int* next, int* centroids, ref int count)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 3;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(2 * sizeof(int));
    int* centroids = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int count = 0;
    try
    {
        head[0] = -1;
        head[1] = -1;
        head[2] = -1;
        IAFahim.Graph.TreeQueries.TreeCentroid.AllCentroids(n, head, to, next, centroids, ref count);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)centroids);
    }
}
```