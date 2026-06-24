# IAFahim.Geometry.Curve

## Description
This package provides curve evaluation algorithms. It includes cubic Bezier curve evaluation, tangent evaluation, arc length integration, and uniform sampling along a path.

## Complexity
Cubic curve evaluation and tangent solving run in O(1) time complexity. Arc length integration runs in O(S) where S is the step count.

## API Signature
public static class CubicBezier
{
    public static float3 Evaluate(float3 p0, float3 p1, float3 p2, float3 p3, float t);
    public static float3 EvaluateTangent(float3 p0, float3 p1, float3 p2, float3 p3, float t);
    public static float IntegrateArcLength(float3 p0, float3 p1, float3 p2, float3 p3);
}

## Usage Example
```csharp
unsafe
{
    float3* points = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(4 * sizeof(float3));
    try
    {
        points[0] = new float3(0.0f, 0.0f, 0.0f);
        points[1] = new float3(1.0f, 0.0f, 0.0f);
        points[2] = new float3(1.0f, 1.0f, 0.0f);
        points[3] = new float3(2.0f, 2.0f, 0.0f);
        float3 res = IAFahim.Geometry.Curve.CubicBezier.Evaluate(points[0], points[1], points[2], points[3], 0.5f);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)points);
    }
}
```
