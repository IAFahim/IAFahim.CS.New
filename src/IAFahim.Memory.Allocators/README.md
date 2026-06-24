# IAFahim.Memory.Allocators

## Description
This package offers structures to manage memory blocks, including slab pools, fixed-size pools, parallel pools, and general memory managers.

## Complexity
Memory provision and freeing operations run in O(1) time. Slab pool clearing runs in O(N) where N is the number of slabs.

## API Signature
```csharp
namespace IAFahim.Memory.Allocators
{
    public readonly unsafe struct Ptr : System.IEquatable<Ptr>
    {
        public Ptr(void* value);
    }

    public unsafe struct MemoryAllocator : System.IDisposable
    {
        public void* Allocate(int itemSizeInBytes, int alignmentInBytes, int items = 1);
        public T* Create<T>(int count = 1) where T : unmanaged;
        public void FreeAll();
        public void Dispose();
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using Unity.Collections;
using IAFahim.Memory.Allocators;

public unsafe class Example
{
    public static void Run()
    {
        MemoryAllocator allocator = new MemoryAllocator(Allocator.Temp);
        try
        {
            int* ptr = allocator.Create<int>(10);
            ptr[0] = 42;
        }
        finally
        {
            allocator.Dispose();
        }
    }
}
```