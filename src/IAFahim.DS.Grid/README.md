# IAFahim.DS.Grid

## Description
This package provides helper functions for manipulating two-dimensional grids stored in flat arrays. It supports grid generation, rotation, reversal, cell shuffling, neighbor collection (4-way and 8-way), breadth-first search pathfinding, and fast cell filling.

## Complexity
- Grid rotation: O(W * H) where W and H are the grid width and height.
- Neighbor collection: O(1).
- Breadth-first search: O(W * H) time.

## API Signature
```csharp
public static unsafe class MakeGrid
{
    public static void Run(int* ptr, int len, int width, int height);
}

public static unsafe class Rotate
{
    public static void Run<T>(T* ptr, int width, int height, bool clockwise, T* temp) where T : unmanaged;
}

public static unsafe class GridNeighbors4
{
    public const int MaxNeighbors = 4;
    public static int Collect(int r, int c, int height, int width, int* nr, int* nc);
    public static int CollectFlat(int r, int c, int height, int width, int* outIndices);
}

public static unsafe class GridBfs
{
    public static int Run(int height, int width, int sr, int sc, int* dist, long* visited, int* queue);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.Grid;

public static unsafe class Example
{
    public static void Run()
    {
        int width = 3;
        int height = 3;
        int len = width * height;
        int* grid = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        try
        {
            MakeGrid.Run(grid, len, width, height);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)grid);
        }
    }
}
```