# IAFahim.Geometry.Bvh

## Description
This package provides a bounding volume hierarchy tree for 3D meshes. It enables efficient ray query operations and spatial partitioning for collision tests.

## Complexity
Tree construction runs in O(N log N) time complexity. Ray query runs in O(log N) average time complexity, where N is the triangle count.

## API Signature
public struct BvhNode
{
    public float3 Min;
    public float3 Max;
    public int Left;
    public int Right;
    public int TriangleIndex;
}
public static class BvhTree
{
    public static int Build(BvhNode* nodes, float3* centroids, int* triangleIndices, int count);
}

## Usage Example
```csharp
unsafe
{
    int count = 2;
    IAFahim.Geometry.Bvh.BvhNode* nodes = (IAFahim.Geometry.Bvh.BvhNode*)System.Runtime.InteropServices.Marshal.AllocHGlobal(count * sizeof(IAFahim.Geometry.Bvh.BvhNode));
    try
    {
        nodes[0].Left = -1;
        nodes[0].Right = -1;
        nodes[0].TriangleIndex = 0;
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
    }
}
```
