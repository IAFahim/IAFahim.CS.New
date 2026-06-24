# IAFahim.Geometry.Azimuth

## Description
This package provides methods for azimuth solving. It supports spherical azimuth, spherical distance on a sphere, and planar 2D azimuth.

## Complexity
All methods execute in O(1) time complexity.

## API Signature
public static class SphericalAzimuth
{
    public static double Run(double lat1, double lon1, double lat2, double lon2);
}
public static class SphericalDistance
{
    public static double Run(double lat1, double lon1, double lat2, double lon2, double radius);
}
public static class CartesianAzimuth
{
    public static double Run(double x1, double y1, double x2, double y2);
}

## Usage Example
```csharp
unsafe
{
    double* coords = (double*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(double));
    try
    {
        coords[0] = 0.0;
        coords[1] = 0.0;
        coords[2] = 1.0;
        coords[3] = 1.0;
        double result = IAFahim.Geometry.Azimuth.CartesianAzimuth.Run(coords[0], coords[1], coords[2], coords[3]);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)coords);
    }
}
```
