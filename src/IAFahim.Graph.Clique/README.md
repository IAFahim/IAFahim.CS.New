# IAFahim.Graph.Clique

## Description
This package provides algorithms for finding fully connected subgraphs in a graph. It solves the clique search problem by identifying subsets of vertices that are mutually adjacent.

## Complexity
Finding a maximum clique is an NP-hard problem. Exponential-time algorithms are used for general graphs, while polynomial-time bounds apply to specific graph types.

## API Signature
public static class CliqueSearch
{
    public static int FindMaximal(int n, int* head, int* to, int* next, int* outVertices);
}

## Usage Example
```csharp
unsafe
{
    int n = 4;
    int* head = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* outVertices = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        for (int i = 0; i < n; i++)
        {
            head[i] = -1;
            outVertices[i] = -1;
        }
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)head);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)outVertices);
    }
}
```
