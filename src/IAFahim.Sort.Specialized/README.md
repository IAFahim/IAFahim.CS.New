# IAFahim.Sort.Specialized

## Description
Offers optimized, specialized sorting operations. This includes sorting key-value pairs simultaneously and highly optimized sorting routines for primitive integers and 64-bit integers.

## Complexity
Time Complexity is O(N log N) for general sorting, O(N) for integer-optimized methods.
Space Complexity is O(N) helper space for pair sorting, O(1) in-place for single array sorting.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class SortPairs
    {
        public static void Run<TKey, TValue>(TKey* keys, TValue* values, int len, TKey* scratchKeys, TValue* scratchValues)
            where TKey : unmanaged, System.IComparable<TKey>
            where TValue : unmanaged;
    }
    public static unsafe class SortInt64s
    {
        public static void Run(long* ptr, int len);
    }
    public static unsafe class SortInts
    {
        public static void Run(int* ptr, int len);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 3;
    int* keys = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    float* values = (float*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(float));
    int* scratchKeys = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    float* scratchValues = (float*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(float));
    try
    {
        keys[0] = 3; keys[1] = 1; keys[2] = 2;
        values[0] = 3.0f; values[1] = 1.0f; values[2] = 2.0f;
        IAFahim.Sort.SortPairs.Run(keys, values, length, scratchKeys, scratchValues);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)keys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)values);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)scratchKeys);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)scratchValues);
    }
}
```
