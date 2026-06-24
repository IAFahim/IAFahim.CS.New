# IAFahim.Graph

## Description
This package provides core graph algorithms. It includes adjacency builders, minimum cut solvers, Eulerian path detection, 2-SAT solvers, minimum spanning tree variants, bipartite matching, shortest path routines, graph traversals, tournament analysis, topological sorting, and planar graph utilities.

## Complexity
BFS and DFS traversals run in O(V + E) time. Dijkstra shortest path runs in O(E log V) time. Minimum spanning tree algorithms run in O(E log V) or O(E log* V) time.

## API Signature
public static class Dijkstra
{
    public static void Run(int n, int start, int* head, int* to, int* next, int* weight, long* dist, int* parent);
}

## Usage Example
```csharp
unsafe
{
    int n = 5;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* to = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* weight = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    long* dist = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
    int* parent = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            dist[i] = long.MaxValue;
            parent[i] = -1;
        }
        IAFahim.Graph.Dijkstra.Run(n, 0, head, to, next, weight, dist, parent);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)to);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)weight);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)dist);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)parent);
    }
}
```
