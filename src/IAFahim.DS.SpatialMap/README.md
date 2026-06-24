# IAFahim.DS.SpatialMap

## Description
A collection of spatial hashing maps for multidimensional grid hashing. Includes 2D spatial maps, 3D spatial maps, hexagonal spatial maps, and local spatial maps to hash positions to grids.

## Complexity
O(1) query and insertion on average.

## API Signature
```csharp
public struct SpatialMap<T> : IDisposable
    where T : unmanaged
{
    public SpatialMap(float quantizeStep, int size, Allocator allocator = Allocator.Persistent)
    public readonly void Dispose()
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using IAFahim.DS.SpatialMap;

public static unsafe class Example
{
    public static void Run()
    {
        int* dummy = (int*)Marshal.AllocHGlobal(sizeof(int));
        try
        {
            SpatialMap<int> map = new SpatialMap<int>(1.0f, 16, default);
            try
            {
                int len = 0;
            }
            finally
            {
                map.Dispose();
            }
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)dummy);
        }
    }
}
```