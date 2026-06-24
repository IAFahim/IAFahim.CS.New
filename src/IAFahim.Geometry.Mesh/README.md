# IAFahim.Geometry.Mesh

## Description
This package provides algorithms for mesh updates. It supports vertex deformation and normal recomputing.

## Complexity
All methods run in O(N) time complexity where N is the vertex count.

## API Signature
public static class MeshProjection
{
    public static void DeformVertices(float3* positions, int count, float3 direction, float force);
    public static void RecalculateNormals(float3* positions, int* indices, int indexCount, float3* normals);
}

## Usage Example
```csharp
unsafe
{
    int count = 3;
    float3* pos = (float3*)System.Runtime.InteropServices.Marshal.AllocHGlobal(count * sizeof(float3));
    try
    {
        pos[0] = new float3(0.0f, 0.0f, 0.0f);
        pos[1] = new float3(1.0f, 0.0f, 0.0f);
        pos[2] = new float3(0.0f, 1.0f, 0.0f);
        IAFahim.Geometry.Mesh.MeshProjection.DeformVertices(pos, count, new float3(0.0f, 0.0f, 1.0f), 0.5f);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)pos);
    }
}
```
