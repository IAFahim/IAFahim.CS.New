# IAFahim.Pathfinding.Recast

## Description
This package provides a navigation mesh building and path query system. It includes spatial heightfield generation, heightfield filtering, walkable area erosion, region building, polygon mesh generation, and path queries on generated navigation meshes.

## Complexity
Grid generation and filtering runs in O(Width * Depth * Height) steps where Width, Depth, and Height are grid dimensions. Region building runs in O(N) steps where N is the number of spans. Path queries run in O(E log V) steps where E is the number of edges and V is the number of polygons.

## API Signature
```csharp
namespace IAFahim.Pathfinding.Recast
{
    public static unsafe partial class Recast
    {
        public static RcHeightfield* AllocHeightfield(Unity.Collections.Allocator allocator);
        public static void FreeHeightfield(RcHeightfield* heightfield);
        public static void ErodeWalkableArea(int erosionRadius, RcCompactHeightfield* compactHeightfield);
    }
}
```

## Usage Example
```csharp
using Unity.Collections;
using IAFahim.Pathfinding.Recast;

public unsafe class Example
{
    public static void Run()
    {
        RcHeightfield* heightfield = Recast.AllocHeightfield(Allocator.Temp);
        try
        {
            int count = Recast.GetHeightFieldSpanCount(heightfield);
        }
        finally
        {
            Recast.FreeHeightfield(heightfield);
        }
    }
}
```