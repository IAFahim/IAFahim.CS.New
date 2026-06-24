# IAFahim.Math.Sdf

## Description
Implements signed distance function (SDF) utilities for 3D computer graphics. Includes primitive shape evaluations, constructive solid geometry (CSG) boolean operations, space transforms, raymarching solvers, normal estimation, and ambient occlusion.

## Complexity
- Primitive evaluations/Booleans/Transforms/Normal estimation: O(1) time, O(1) space.
- March: O(maxSteps) time, O(1) space.
- AmbientOcclusion: O(steps) time, O(1) space.

## API Signature
- public delegate float SdfRayMarch.SdfFunction(float3 p)
- public static float SdfPrimitive.Sphere(float3 p, float radius)
- public static float SdfPrimitive.Box(float3 p, float3 halfExtents)
- public static float SdfBoolean.Union(float d1, float d2)
- public static float3 SdfRayMarch.EstimateNormal(SdfFunction sdf, float3 p)
- public static bool SdfRayMarch.March(SdfFunction sdf, float3 origin, float3 direction, float maxDistance, int maxSteps, out float t, out float3 hitPoint)

## Usage Example
```csharp
using System;
using Unity.Mathematics;
using IAFahim.Math.Sdf;

public unsafe class Example
{
    private static float SphereSdf(float3 p)
    {
        return SdfPrimitive.Sphere(p, 1.0f);
    }

    public static void Main()
    {
        float3 origin = new float3(0.0f, 0.0f, -5.0f);
        float3 dir = new float3(0.0f, 0.0f, 1.0f);
        float t;
        float3 hit;
        bool didHit = SdfRayMarch.March(SphereSdf, origin, dir, 10.0f, 64, out t, out hit);
    }
}
```