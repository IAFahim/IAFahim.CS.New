# IAFahim.DS.UnsafeArray

## Description
An unmanaged array wrapper that provisions raw memory using a specified memory manager. Implements disposal to prevent memory leaks.

## Complexity
O(1) for memory lookup, setup, and cleanup.

## API Signature
```csharp
public unsafe struct UnsafeArray<T> : IDisposable where T : unmanaged
{
    public T* Ptr;
    public readonly int Length;
    public UnsafeArray(int length, Allocator allocator)
    public void Dispose()
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        int* dummy = (int*)Marshal.AllocHGlobal(sizeof(int));
        try
        {
            UnsafeArray<int> array = new UnsafeArray<int>(10, default);
            try
            {
                int len = array.Length;
            }
            finally
            {
                array.Dispose();
            }
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)dummy);
        }
    }
}
```