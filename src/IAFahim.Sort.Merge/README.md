# IAFahim.Sort.Merge

## Description
Sorts elements in an unmanaged buffer by splitting the range, sorting sub-segments recursively, and combining them using a helper buffer.

## Complexity
Time Complexity is O(N log N).
Space Complexity is O(N) auxiliary space.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class Merge
    {
        public static void Run<T>(T* ptr, int len, T* scratch) where T : unmanaged, System.IComparable<T>;
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 4;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    int* helper = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        ptr[0] = 40;
        ptr[1] = 10;
        ptr[2] = 30;
        ptr[3] = 20;
        IAFahim.Sort.Merge.Run(ptr, length, helper);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)helper);
    }
}
```
