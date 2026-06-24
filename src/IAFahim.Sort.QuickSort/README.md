# IAFahim.Sort.QuickSort

## Description
Sorts elements in place using partition operations. Includes single pivot and dual pivot variations.

## Complexity
Time Complexity is O(N log N) average, O(N^2) worst case.
Space Complexity is O(log N) stack depth.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class QuickSort
    {
        public static void Run<T>(T* ptr, int len) where T : unmanaged, System.IComparable<T>;
        public static void DualPivot<T>(T* ptr, int len) where T : unmanaged, System.IComparable<T>;
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 4;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        ptr[0] = 4;
        ptr[1] = 1;
        ptr[2] = 3;
        ptr[3] = 2;
        IAFahim.Sort.QuickSort.Run(ptr, length);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```
