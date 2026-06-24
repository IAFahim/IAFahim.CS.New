# IAFahim.DS.OrderedSet

## Description
An ordered set implementation built on a sorted pointer sequence. Supports insertions, deletions, rank checks, and index queries.

## Complexity
O(N) for Insert and Erase due to element shifts. O(log N) for Rank. O(1) for Kth.

## API Signature
```csharp
public static unsafe class OrderedSet
{
    public static int Insert<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
    public static int Erase<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
    public static int Rank<T>(T* ptr, int len, T key) where T : unmanaged, IComparable<T>
    public static T Kth<T>(T* ptr, int len, int k) where T : unmanaged, IComparable<T>
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS.OrderedSet;

public static unsafe class Example
{
    public static void Run()
    {
        int* ptr = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        try
        {
            int len = 0;
            len = OrderedSet.Insert(ptr, len, 5);
            len = OrderedSet.Insert(ptr, len, 3);
            int rank = OrderedSet.Rank(ptr, len, 5);
            int val = OrderedSet.Kth(ptr, len, 0);
            len = OrderedSet.Erase(ptr, len, 3);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)ptr);
        }
    }
}
```