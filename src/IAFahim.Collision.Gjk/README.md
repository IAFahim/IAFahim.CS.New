# IAFahim.Collision.Gjk

## Description
This package implements the Gilbert-Johnson-Keerthi (GJK) collision detection algorithm and the Expanding Polytope Algorithm (EPA) for three-dimensional physics queries. It computes overlap and minimum distance between convex shapes defined by support functions. Shape support functions include sphere, box, capsule, and convex hull.

## Complexity
- GJK intersection query: O(I) where I is the iteration count.
- EPA penetration depth: O(F) where F is the number of faces in the expanding polytope.
- Convex hull support query: O(V) where V is the number of points in the hull.

## API Signature
```csharp
public static unsafe class Gjk
{
    public delegate float3 SupportFunction(float3 direction);
    public static bool Intersect(SupportFunction supportA, SupportFunction supportB);
    public static bool Intersect(SupportFunction supportA, SupportFunction supportB, float3* outSimplex, out int outCount);
    public static float Distance(SupportFunction supportA, SupportFunction supportB);
}

public static unsafe class MinkowskiDifference
{
    public static float3 SphereSupport(float3 direction, float3 center, float radius);
    public static float3 BoxSupport(float3 direction, float3 center, float3 halfExtents);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Collision.Gjk;

public static unsafe class Example
{
    public static void Run()
    {
        Gjk.SupportFunction supportA = delegate(float3 dir)
        {
            return MinkowskiDifference.SphereSupport(dir, new float3(0, 0, 0), 1.0f);
        };
        Gjk.SupportFunction supportB = delegate(float3 dir)
        {
            return MinkowskiDifference.SphereSupport(dir, new float3(0.5f, 0, 0), 1.0f);
        };
        bool overlapping = Gjk.Intersect(supportA, supportB);
    }
}
```