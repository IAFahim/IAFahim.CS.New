# IAFahim.Sort.RadixSort

## Description
Sorts integer keys using digit-by-digit sorting based on their binary representation. Requires a helper buffer.

## Complexity
Time Complexity is O(N) linear time.
Space Complexity is O(N) helper space.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class RadixSort
    {
        public static void Run(int* ptr, int len, int* scratch);
        public static void Run(uint* ptr, int len, uint* scratch);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 3;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    int* helper = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        ptr[0] = 100;
        ptr[1] = 2;
        ptr[2] = 50;
        IAFahim.Sort.RadixSort.Run(ptr, length, helper);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)helper);
    }
}
```
