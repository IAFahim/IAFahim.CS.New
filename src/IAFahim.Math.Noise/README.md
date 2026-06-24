# IAFahim.Math.Noise

## Description
Provides 2D Perlin and Simplex noise algorithms. These are useful for procedural content generation, terrain generation, and visual effects.

## Complexity
- All operations: O(1) time, O(1) space.

## API Signature
- public static float PerlinNoise.Noise2D(float2 p)
- public static float SimplexNoise.Noise2D(float2 p)

## Usage Example
```csharp
using Unity.Mathematics;
using IAFahim.Math.Noise;

public unsafe class Example
{
    public static void Main()
    {
        float2 position = new float2(1.5f, 2.5f);
        float value1 = PerlinNoise.Noise2D(position);
        float value2 = SimplexNoise.Noise2D(position);
    }
}
```