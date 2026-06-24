# IAFahim.Geometry.Voronoi

## Description
This package provides Voronoi diagrams and related spatial graph algorithms. It includes Delaunay triangulation, Fortune's sweep-line solver, visibility graph construction, nearest neighbor search on KD-trees, and shortest path solving.

## Complexity
Delaunay triangulation and Fortune's algorithm run in O(N log N) time complexity. Visibility graph construction runs in O(N^2 log N) time complexity.

## API Signature
public static class Delaunay
{
    public struct Triangle
    {
        public int A, B, C;
    }
    public static int Build(double* xs, double* ys, int n, Triangle* outTri);
}

## Usage Example
```csharp
unsafe
{
    int size = 3;
    double* xs = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    double* ys = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    IAFahim.Geometry.Voronoi.Delaunay.Triangle* tris = (IAFahim.Geometry.Voronoi.Delaunay.Triangle*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(IAFahim.Geometry.Voronoi.Delaunay.Triangle));
    try
    {
        xs[0] = 0.0; ys[0] = 0.0;
        xs[1] = 10.0; ys[1] = 0.0;
        xs[2] = 0.0; ys[2] = 10.0;
        int triCount = IAFahim.Geometry.Voronoi.Delaunay.Build(xs, ys, size, tris);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tris);
    }
}
```
