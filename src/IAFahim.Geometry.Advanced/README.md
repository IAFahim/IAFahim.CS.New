# IAFahim.Geometry.Advanced

## Description
A collection of advanced geometric algorithms. Supports convex hull diameter using rotating calipers, closest pair of points, Minkowski sum, circumcenter, minimum enclosing circle, Pick's theorem, and polygon boolean operations.

## Complexity
O(N log N) for closest pair of points, O(N) for rotating calipers on a convex polygon, O(N) on average for minimum enclosing circle, and O((N + M) log(N + M)) for polygon boolean operations.

## API Signature
```csharp
public static unsafe class ConvexDiameter
{
    public static long Run(int n, long* x, long* y)
}
public static unsafe class RotatingCalipers
{
    public static long Run(int n, long* x, long* y, long* res)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Geometry.Advanced;

public static unsafe class Example
{
    public static void Run()
    {
        long* x = (long*)Marshal.AllocHGlobal(4 * sizeof(long));
        long* y = (long*)Marshal.AllocHGlobal(4 * sizeof(long));
        try
        {
            x[0] = 0; y[0] = 0;
            x[1] = 4; y[1] = 0;
            x[2] = 4; y[2] = 3;
            x[3] = 0; y[3] = 3;
            long d = ConvexDiameter.Run(4, x, y);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)x);
            Marshal.FreeHGlobal((IntPtr)y);
        }
    }
}
```