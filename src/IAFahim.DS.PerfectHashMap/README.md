# IAFahim.DS.PerfectHashMap

## Description
A perfect hash map structure. Resolves key queries in O(1) time.

## Complexity
O(1) search time.

## API Signature
```csharp
public unsafe struct NativePerfectHashMap<TKey, TValue>
    where TKey : unmanaged, IEquatable<TKey>
    where TValue : unmanaged, IEquatable<TValue>
{
    public NativePerfectHashMap(NativeArray<TKey> keys, NativeArray<TValue> values, TValue nullValue, AllocatorManager.AllocatorHandle allocator)
    public void Dispose()
    public bool TryGetValue(TKey key, out TValue item)
}
public unsafe struct UnsafePerfectHashMap<TKey, TValue> : IDisposable
    where TKey : unmanaged, IEquatable<TKey>
    where TValue : unmanaged, IEquatable<TValue>
{
    public static UnsafePerfectHashMap<TKey, TValue>* Alloc(NativeArray<TKey> keys, NativeArray<TValue> values, TValue nullValue, AllocatorManager.AllocatorHandle allocator)
    public static void Free(UnsafePerfectHashMap<TKey, TValue>* data)
    public void Dispose()
    public bool TryGetValue(TKey key, out TValue item)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using IAFahim.DS.PerfectHashMap;

public static unsafe class Example
{
    public static void Run()
    {
        int* dummy = (int*)Marshal.AllocHGlobal(sizeof(int));
        try
        {
            NativeArray<int> keys = default;
            NativeArray<int> values = default;
            UnsafePerfectHashMap<int, int>* map = UnsafePerfectHashMap<int, int>.Alloc(keys, values, -1, default);
            try
            {
                int item;
                bool found = map->TryGetValue(10, out item);
            }
            finally
            {
                UnsafePerfectHashMap<int, int>.Free(map);
            }
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)dummy);
        }
    }
}
```