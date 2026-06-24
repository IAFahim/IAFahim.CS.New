# IAFahim.Sort.Partition

## Description
Reorders elements in an unmanaged buffer around a pivot. Elements smaller than or equal to the pivot move to the left, while larger elements move to the right.

## Complexity
Time Complexity is O(N) linear scan.
Space Complexity is O(1) auxiliary space.

## API Signature
```csharp
namespace IAFahim.Sort
{
    public static unsafe class Partition
    {
        public static int Run<T>(T* ptr, int len, T pivot) where T : unmanaged, System.IComparable<T>;
        public static void Hoare<T>(T* ptr, int len, T pivot, out int splitIndex) where T : unmanaged, System.IComparable<T>;
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 5;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        ptr[0] = 5;
        ptr[1] = 2;
        ptr[2] = 9;
        ptr[3] = 1;
        ptr[4] = 6;
        int pivot = 5;
        int index = IAFahim.Sort.Partition.Run(ptr, length, pivot);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
    }
}
```
