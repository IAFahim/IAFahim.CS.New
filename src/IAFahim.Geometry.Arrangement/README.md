# IAFahim.Geometry.Arrangement

## Description
This package provides algorithms for subdivision arrangement analysis. It constructs partitions, builds query grids, computes vertical decomposition, builds trapezoidal maps, and solves polygon union and intersection.

## Complexity
Grid build: O(N) where N is point count. KdTree build: O(N log N). Query: O(log N) for KdTree, O(1) for grid. Trapezoidal Map build: O(N log N) average. Decomposition: O(N log N).

## API Signature
public static class PointLocationBuild
{
    public static int Run(int* xs, int* ys, int n, int* grid, int gridSize);
    public static void BuildKdTree(long* points, int* tree, int node, int l, int r, int depth);
}
public static class PointLocationQuery
{
    public static int Run(int* grid, int gridSize, int minX, int minY, int cellW, int cellH, int px, int py);
    public static int QueryKdTree(long* points, int* tree, int node, int depth, long px, long py);
}

## Usage Example
```csharp
unsafe
{
    int size = 100;
    int* xs = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(int));
    int* ys = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(int));
    int* outX = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(int));
    int* outY = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(int));
    try
    {
        for (int i = 0; i < size; i++)
        {
            xs[i] = i;
            ys[i] = i * 2;
        }
        int result = IAFahim.Geometry.Arrangement.VerticalDecomposition.Run(xs, ys, size, outX, outY);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)outX);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)outY);
    }
}
```
