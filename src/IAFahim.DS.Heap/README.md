# IAFahim.DS.Heap

## Description
This package provides priority queue and deque operations on raw buffers. It includes binary heap insertion, deletion, and heapify helpers, deque push and pop for double-ended queues, monotonic queue minimum queries, and monotonic stack processing.

## Complexity
- Heap push / pop: O(log N) where N is the heap size.
- Heapify (HeapFix): O(log N).
- Deque push / pop: O(1).
- Monotonic queue window queries: O(N) amortized.

## API Signature
```csharp
public static unsafe class HeapPush
{
    public static void Run<T>(T* ptr, int len, T val) where T : unmanaged, IComparable<T>;
}

public static unsafe class HeapPop
{
    public static T Run<T>(T* ptr, int len) where T : unmanaged, IComparable<T>;
}

public static unsafe class MonotonicQueueMin
{
    public static void Run(int* src, int* dst, int len, int windowSize);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.Heap;

public static unsafe class Example
{
    public static void Run()
    {
        int cap = 10;
        int* heap = (int*)Marshal.AllocHGlobal(cap * sizeof(int));
        try
        {
            HeapPush.Run(heap, 0, 42);
            HeapPush.Run(heap, 1, 15);
            int minVal = HeapPop.Run(heap, 2);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)heap);
        }
    }
}
```