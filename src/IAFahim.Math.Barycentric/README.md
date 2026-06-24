# IAFahim.Math.Barycentric

## Description
Offers utilities for barycentric weights on triangles in 2D and 3D space. Includes weight solving, interpolation of vector and scalar values, inside-triangle testing, projection of points, and signed area.

## Complexity
- All operations: O(1) time, O(1) space.

## API Signature
- public static float3 BarycentricCoords.Compute(float3 a, float3 b, float3 c, float3 p)
- public static float3 BarycentricCoords.Interpolate(float3 a, float3 b, float3 c, float3 bary)
- public static float BarycentricCoords.InterpolateScalar(float va, float vb, float vc, float3 bary)
- public static bool BarycentricCoords.IsInside(float3 bary)
- public static float2 BarycentricCoords.Compute2D(float2 a, float2 b, float2 c, float2 p)
- public static float3 BarycentricCoords.ProjectOntoTriangle(float3 a, float3 b, float3 c, float3 p)
- public static float BarycentricCoords.SignedArea(float3 a, float3 b, float3 c)

## Usage Example
```csharp
using Unity.Mathematics;
using IAFahim.Math.Barycentric;

public unsafe class Example
{
    public static void Main()
    {
        float3 a = new float3(0.0f, 0.0f, 0.0f);
        float3 b = new float3(1.0f, 0.0f, 0.0f);
        float3 c = new float3(0.0f, 1.0f, 0.0f);
        float3 p = new float3(0.25f, 0.25f, 0.0f);
        float3 weights = BarycentricCoords.Compute(a, b, c, p);
        bool inside = BarycentricCoords.IsInside(weights);
    }
}
```