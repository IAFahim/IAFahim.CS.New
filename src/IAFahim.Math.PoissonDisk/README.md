# IAFahim.Math.PoissonDisk

## Description
Implements 2D and 3D Poisson disk sampling algorithms to generate blue noise distributions. Useful for random object placement, sampling patterns, and graphics.

## Complexity
- All operations: O(N) average time, O(grid_size) space.

## API Signature
- public static int PoissonDisk2D.Run(float2 min, float2 max, float minDistance, float2* output, int maxPoints, int seed)
- public static int PoissonDisk3D.Run(float3 min, float3 max, float minDistance, float3* output, int maxPoints, int seed)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Math.PoissonDisk;

public unsafe class Example
{
    public static void Main()
    {
        float2 min = new float2(0.0f, 0.0f);
        float2 max = new float2(10.0f, 10.0f);
        float minDistance = 2.0f;
        int maxPoints = 100;
        float2* output = (float2*)Marshal.AllocHGlobal(maxPoints * sizeof(float2));
        try
        {
            int count = PoissonDisk2D.Run(min, max, minDistance, output, maxPoints, 42);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)output);
        }
    }
}
```