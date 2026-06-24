# IAFahim.Geometry.Spatial

## Description
This package provides spatial query data structures. It includes cover trees, kd-trees, quadtrees, range trees, segment trees, octrees, ball trees, 3D binary indexed trees, and methods for Euclidean, Manhattan, and rectilinear minimum spanning trees.

## Complexity
Tree building algorithms run in O(N log N) or O(N log^2 N) time complexity. Nearest neighbor and range queries run in O(log N) average time complexity.

## API Signature
public static class KdTree
{
    public struct Node
    {
        public double X, Y;
        public int PointIndex;
        public int Left, Right;
        public int Axis;
    }
    public static int Build(double* xs, double* ys, int n, Node* nodes);
}

## Usage Example
```csharp
unsafe
{
    int size = 2;
    double* xs = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    double* ys = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    IAFahim.Geometry.Spatial.KdTree.Node* nodes = (IAFahim.Geometry.Spatial.KdTree.Node*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(IAFahim.Geometry.Spatial.KdTree.Node));
    try
    {
        xs[0] = 1.0; ys[0] = 2.0;
        xs[1] = 3.0; ys[1] = 4.0;
        int root = IAFahim.Geometry.Spatial.KdTree.Build(xs, ys, size, nodes);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
    }
}
```
