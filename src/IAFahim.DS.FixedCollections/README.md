# IAFahim.DS.FixedCollections

## Description
This package provides fixed-size and unmanaged collection types that do not depend on garbage collection. It includes spin locks, fixed-size bitmasks, fixed-size hash maps, thread-local collections, thread-safe random number helpers, fast counters, and unmanaged object pools.

## Complexity
- FixedHashMap lookup / insertion: O(1) on average.
- FixedBitMask set / get: O(1).
- UnmanagedPool acquire / return: O(1).
- SpinLock acquire / release: O(1).

## API Signature
```csharp
public struct SpinLock
{
    public void Acquire();
    public bool TryAcquire();
    public void Release();
}

public unsafe struct FixedBitMask<T>
{
    public int Length { get; }
    public void Set(int pos, bool value);
    public bool IsSet(int pos);
    public void Reset();
}

public unsafe struct FixedHashMap<TKey, TValue, TCapacity>
{
    public int Capacity { get; }
    public int Count { get; }
    public bool TryAdd(TKey key, TValue item);
    public bool TryGetValue(TKey key, out TValue item);
}

public unsafe struct NativeCounter : IDisposable
{
    public int Increment();
    public int Count { get; }
    public bool IsCreated { get; }
    public void Dispose();
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.FixedCollections;

public static unsafe class Example
{
    public static void Run()
    {
        FixedHashMap<int, float, int> map = default;
        bool added = map.TryAdd(10, 3.14f);
        float val;
        bool found = map.TryGetValue(10, out val);
    }
}
```