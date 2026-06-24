# IAFahim.Math.PotentialField

## Description
Implements 2D and 3D potential field steering forces for path planning. Includes attractive forces towards targets, repulsive forces away from obstacles, tangential forces (2D only) to bypass obstacles, gradient evaluations, and simple pathfinding using gradient descent.

## Complexity
- Force evaluations: O(K) time where K is the obstacle count, O(1) space.
- GradientDescent: O(steps * K) time, O(1) space.

## API Signature
- public static float2 PotentialField2D.Attractive(float2 position, float2 target, float strength)
- public static float2 PotentialField2D.Repulsive(float2 position, float2 obstacle, float radius, float strength)
- public static float2 PotentialField2D.Tangential(float2 position, float2 obstacle, float radius, float strength)
- public static void PotentialField2D.ComputeGradient(float2 position, float2* attractors, int attractorCount, float attractStrength, float2* repulsors, int repulsorCount, float repulsorRadius, float repulsorStrength, float2* tangentials, int tangentialCount, float tangentialRadius, float tangentialStrength, out float2 gradient)
- public static int PotentialField2D.GradientDescent(float2 start, float2* attractors, int attractorCount, float attractStrength, float2* repulsors, int repulsorCount, float repulsorRadius, float repulsorStrength, float2* tangentials, int tangentialCount, float tangentialRadius, float tangentialStrength, float stepSize, float tolerance, int maxSteps, float2* path)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Math.PotentialField;

public unsafe class Example
{
    public static void Main()
    {
        float2 start = new float2(0.0f, 0.0f);
        float2 target = new float2(10.0f, 10.0f);
        float2 obstacle = new float2(5.0f, 5.0f);

        float2* attractors = (float2*)Marshal.AllocHGlobal(1 * sizeof(float2));
        float2* repulsors = (float2*)Marshal.AllocHGlobal(1 * sizeof(float2));
        float2* tangentials = (float2*)Marshal.AllocHGlobal(1 * sizeof(float2));
        try
        {
            attractors[0] = target;
            repulsors[0] = obstacle;
            tangentials[0] = obstacle;

            float2 gradient;
            PotentialField2D.ComputeGradient(
                start,
                attractors, 1, 1.0f,
                repulsors, 1, 2.0f, 5.0f,
                tangentials, 1, 2.0f, 2.0f,
                out gradient
            );
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)attractors);
            Marshal.FreeHGlobal((IntPtr)repulsors);
            Marshal.FreeHGlobal((IntPtr)tangentials);
        }
    }
}
```