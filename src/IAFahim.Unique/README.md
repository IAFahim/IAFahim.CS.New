# IAFahim.Unique

## Description
Filters out redundant values from a buffer of 64-bit or 32-bit integers in place. Returns the size of the filtered prefix.

## Complexity
Time Complexity is O(N log N) to sort and filter, or O(N) if already sorted.
Space Complexity is O(1) in-place auxiliary space.

## API Signature
```csharp
namespace IAFahim.Unique
{
    public static unsafe class UniqueInts
    {
        public static int Run(int* ptr, int len);
    }
    public static unsafe class UniqueInt64s
    {
        public static int Run(long* ptr, int len);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 5;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        ptr[0] = 10;
        ptr[1] = 20;
        ptr[2] = 10;
        ptr[3] = 30;
        ptr[4] = 20;
        int uniqueCount = IAFahim.Unique.UniqueInts.Run(ptr, len);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```
