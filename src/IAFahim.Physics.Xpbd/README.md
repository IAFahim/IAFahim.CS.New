# IAFahim.Physics.Xpbd

## Description
This package implements the Extended Position-Based Dynamics (XPBD) simulation system. It provides static methods for integrating positions and velocities, applying damping, and solving distance, volume, bending, and shape matching bonds.

## Complexity
Integrating positions and velocities runs in O(N) steps where N is the number of points. Solving each bond runs in O(B) steps where B is the number of bonds.

## API Signature
```csharp
namespace IAFahim.Physics.Xpbd
{
    public static unsafe class XpbdIntegrator
    {
        public static void PredictPosition(float3* pos, float3* vel, float3 externalForce, float invMass, float dt);
        public static void UpdateVelocity(float3* vel, float3* oldPos, float3* newPos, float dt);
    }

    public static unsafe class DistanceConstraint
    {
        public static void Solve(float3* posA, float3* posB, float3* velA, float3* velB, float invMassA, float invMassB, float restLength, float stiffness, float dt);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Physics.Xpbd;

public unsafe class Example
{
    public static void Run()
    {
        float3* pos = (float3*)Marshal.AllocHGlobal(sizeof(float3));
        float3* vel = (float3*)Marshal.AllocHGlobal(sizeof(float3));
        try
        {
            *pos = new float3(0.0f, 10.0f, 0.0f);
            *vel = new float3(0.0f, 0.0f, 0.0f);
            XpbdIntegrator.PredictPosition(pos, vel, new float3(0.0f, -9.81f, 0.0f), 1.0f, 0.016f);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)pos);
            Marshal.FreeHGlobal((nint)vel);
        }
    }
}
```