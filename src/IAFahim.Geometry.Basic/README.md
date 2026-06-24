# IAFahim.Geometry.Basic

## Description
This package provides basic geometry operations. It includes point arithmetic, dot products, cross products, point rotation, orientation tests, segment intersection checks, projection and reflection, distance formulas, polygon area, centroid solving, and inclusion checks.

## Complexity
All primitive operations run in O(1) time complexity. Polygon operations like area, centroid, and inclusion run in O(N) time complexity where N is the vertex count.

## API Signature
public static class GeometryPoint
{
    public static void Run(long* x, long* y, long px, long py);
}
public static class SegmentIntersect
{
    public static bool Run(long ax, long ay, long bx, long by, long cx, long cy, long dx, long dy);
}
public static class PolygonArea
{
    public static long Run(int n, long* x, long* y);
}

## Usage Example
```csharp
unsafe
{
    int size = 3;
    long* xs = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(long));
    long* ys = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(long));
    try
    {
        xs[0] = 0; ys[0] = 0;
        xs[1] = 10; ys[1] = 0;
        xs[2] = 0; ys[2] = 10;
        long area = IAFahim.Geometry.Basic.PolygonArea.Run(size, xs, ys);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
    }
}
```
