# IAFahim.Graph.TreeDecomposition

## Description
This package provides dynamic programming algorithms on nice tree decompositions, pathwidth decompositions, and tree Mo algorithms.

## Complexity
Independent set query runs in linear time with respect to the tree decomposition size.

## API Signature
```csharp
public static unsafe class PathwidthDpAlgorithm
{
    public static long PathwidthDpIndependentSet(int n, int width, int* bagSize, int* bagVertices, int* parent, int* vertexWeight)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 2;
    int width = 1;
    int* bagSize = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* bagVertices = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * (width + 1) * sizeof(int));
    int* parent = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    int* vertexWeight = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(int));
    try
    {
        bagSize[0] = 1;
        bagSize[1] = 1;
        long total = IAFahim.Graph.TreeDecomposition.PathwidthDpAlgorithm.PathwidthDpIndependentSet(n, width, bagSize, bagVertices, parent, vertexWeight);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)bagSize);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)bagVertices);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)parent);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)vertexWeight);
    }
}
```