# IAFahim.Permutation

## Description
This package offers utility functions for permutation operations. It includes validation, inversion, composition, power solving, cycle decomposition, ranking, unranking, next and prior permutation generation, Gray code generation, and cross product generation.

## Complexity
Next and prior permutation generation runs in O(N) steps. Composition and inversion run in O(N) steps. K-th permutation unranking runs in O(N^2) steps. Cross product operations run in O(1) time per query.

## API Signature
```csharp
namespace IAFahim.Permutation
{
    public static unsafe class NextPermutation
    {
        public static bool Run<T>(T* ptr, int len) where T : unmanaged, System.IComparable<T>;
    }

    public static unsafe class PrevPermutation
    {
        public static bool Run<T>(T* ptr, int len) where T : unmanaged, System.IComparable<T>;
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Permutation;

public unsafe class Example
{
    public static void Run()
    {
        int n = 3;
        int* ptr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            ptr[0] = 1;
            ptr[1] = 2;
            ptr[2] = 3;
            bool success = NextPermutation.Run(ptr, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)ptr);
        }
    }
}
```