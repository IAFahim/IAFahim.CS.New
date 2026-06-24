# IAFahim.Geometry.Intersect

## Description
This package provides methods for geometric intersection solving. It computes polyhedron volume, line-sphere intersection, sphere-sphere intersection, point-plane distances, line-plane intersection, segment-plane intersection, and plane-plane intersections.

## Complexity
Intersection and distance methods run in O(1) time complexity. Polyhedron volume solver runs in O(F) where F is the face count.

## API Signature
public static class Plane
{
    public static double PointPlaneDistance(double px, double py, double pz, double nx, double ny, double nz, double d);
    public static bool LinePlaneIntersection(double lx, double ly, double lz, double ldx, double ldy, double ldz, double nx, double ny, double nz, double d, double* t);
}

## Usage Example
```csharp
unsafe
{
    double* t = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(sizeof(double));
    try
    {
        bool hit = IAFahim.Geometry.Intersect.Plane.LinePlaneIntersection(0.0, 0.0, 5.0, 0.0, 0.0, -1.0, 0.0, 0.0, 1.0, 0.0, t);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)t);
    }
}
```
