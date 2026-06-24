# IAFahim.Graph.Cut

## Description
This package provides algorithms for graph cuts and flow networks. It solves the minimum cut problem, identifying subsets of edges that partition the graph.

## Complexity
Minimum cut algorithms on planar graphs run in O(N log N) time, while general graphs run in polynomial time matching maximum flow bounds.

## API Signature
public static class MinimumCut
{
    public static int Solve(int n, int* head, int* to, int* next, int* cap, int* outCutEdges);
}

## Usage Example
```csharp
unsafe
{
    int n = 4;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* outCutEdges = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            outCutEdges[i] = -1;
        }
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)outCutEdges);
    }
}
```
