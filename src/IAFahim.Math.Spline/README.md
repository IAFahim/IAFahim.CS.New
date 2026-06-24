# IAFahim.Math.Spline

## Description
This package provides functions to evaluate Cubic Hermite and Uniform B-Spline curves. It supports evaluation of positions, tangents, and numerical integration of spline arc lengths.

## Complexity
Position and tangent evaluations run in O(1) time. Spline arc length integration runs in O(N) steps where N is the sample count.

## API Signature
```csharp
namespace IAFahim.Math.Spline
{
    public static unsafe class CubicHermite
    {
        public static float3 Evaluate(float3 p0, float3 m0, float3 p1, float3 m1, float t);
        public static float3 EvaluateTangent(float3 p0, float3 m0, float3 p1, float3 m1, float t);
        public static float IntegrateArcLength(float3 p0, float3 m0, float3 p1, float3 m1, int sampleCount);
    }

    public static unsafe class UniformBSpline
    {
        public static float3 Evaluate(float3 p0, float3 p1, float3 p2, float3 p3, float t);
        public static float3 EvaluateTangent(float3 p0, float3 p1, float3 p2, float3 p3, float t);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Math.Spline;

public unsafe class Example
{
    public static void Run()
    {
        float3 p0 = new float3(0.0f, 0.0f, 0.0f);
        float3 m0 = new float3(1.0f, 0.0f, 0.0f);
        float3 p1 = new float3(1.0f, 1.0f, 0.0f);
        float3 m1 = new float3(0.0f, 1.0f, 0.0f);
        float3* result = (float3*)Marshal.AllocHGlobal(sizeof(float3));
        try
        {
            *result = CubicHermite.Evaluate(p0, m0, p1, m1, 0.5f);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)result);
        }
    }
}
```