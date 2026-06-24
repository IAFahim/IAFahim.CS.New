# IAFahim.Geometry.Triangulation

## Description
This package provides methods for polygon triangulation. It implements ear clipping to decompose simple polygons into triangles.

## Complexity
Ear clipping triangulation runs in O(N^2) worst-case time complexity, where N is the vertex count.

## API Signature
public static class EarClipping
{
    public static void Triangulate(float3* positions, int count, int* outIndices);
}

## Usage Example
```csharp
unsafe
{
    int count = 3;
    float3* pos = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(count * sizeof(float3));
    int* indices = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(count * sizeof(int));
    try
    {
        pos[0] = new float3(0.0f, 0.0f, 0.0f);
        pos[1] = new float3(1.0f, 0.0f, 0.0f);
        pos[2] = new float3(0.0f, 1.0f, 0.0f);
        IAFahim.Geometry.Triangulation.EarClipping.Triangulate(pos, count, indices);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)pos);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)indices);
    }
}
```
