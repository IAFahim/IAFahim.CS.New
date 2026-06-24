# IAFahim.Optimization.Geometric

## Description
This package contains geometric solvers. It includes Welzl's algorithm for finding the minimum enclosing sphere and minimum enclosing ball in multiple dimensions using randomized techniques.

## Complexity
Welzl's algorithm runs in O(N) expected time.

## API Signature
```csharp
namespace IAFahim.Optimization.Geometric
{
    public static unsafe class WelzlSphere
    {
        public struct Sphere
        {
            public double X, Y, Z, R;
        }
        public static Sphere Run(double* xs, double* ys, double* zs, int n);
    }

    public static unsafe class MinEnclosingBall
    {
        public struct Circle
        {
            public double X, Y, R;
        }
        public static Circle Welzl(double* xs, double* ys, int n, int* p);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Geometric;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        double* xs = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        double* ys = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        double* zs = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        try
        {
            xs[0] = 0.0; ys[0] = 0.0; zs[0] = 0.0;
            xs[1] = 1.0; ys[1] = 0.0; zs[1] = 0.0;
            xs[2] = 0.0; ys[2] = 1.0; zs[2] = 0.0;
            xs[3] = 0.0; ys[3] = 0.0; zs[3] = 1.0;
            WelzlSphere.Sphere ball = WelzlSphere.Run(xs, ys, zs, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)xs);
            Marshal.FreeHGlobal((nint)ys);
            Marshal.FreeHGlobal((nint)zs);
        }
    }
}
```