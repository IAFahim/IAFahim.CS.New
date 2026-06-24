# IAFahim.Geometry.Hull

## Description
This package provides geometric hull and partition algorithms. It includes Minkowski sum solving, straight skeleton construction, convex hull trick with rollback history, half-space intersection, rotating calipers for bounding boxes, and 3D convex hull generation.

## Complexity
Minkowski sum runs in O(N + M) time. Straight skeleton construction runs in O(N^2 log N) worst-case time. Rotating calipers run in O(N) time. Convex hull 3D construction runs in O(N^2) time.

## API Signature
public static class RotatingCalipers
{
    public struct Rect
    {
        public double X, Y, W, H, Angle;
    }
    public static Rect MinArea(double* xs, double* ys, int n);
    public static double MinWidth(double* xs, double* ys, int n);
}

## Usage Example
```csharp
unsafe
{
    int size = 4;
    double* xs = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    double* ys = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(size * sizeof(double));
    try
    {
        xs[0] = 0.0; ys[0] = 0.0;
        xs[1] = 10.0; ys[1] = 0.0;
        xs[2] = 10.0; ys[2] = 10.0;
        xs[3] = 0.0; ys[3] = 10.0;
        IAFahim.Geometry.Hull.RotatingCalipers.Rect r = IAFahim.Geometry.Hull.RotatingCalipers.MinArea(xs, ys, size);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)xs);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ys);
    }
}
```
