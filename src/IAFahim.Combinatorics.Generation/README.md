# IAFahim.Combinatorics.Generation

## Description
This package provides enumerators and generators for combinatorial objects. It supports set partitions, permutations, combinations, necklaces, bracelets, and random graph structures. It also includes methods to rank and unrank these objects to convert them to and from integers.

## Complexity
- Permutation generation: O(1) amortized.
- Combination generation: O(1) amortized.
- Random tree generation: O(N) where N is the number of nodes.

## API Signature
```csharp
public static unsafe class Permutations
{
    public static bool NextPermutation(int* ptr, int len);
    public static void RandomPermutation(int n, int* a, ref uint seed);
}

public static unsafe class Combinations
{
    public static bool TryNextMultiset(int* m, int n, int k, int* comb, ref bool first);
}

public static unsafe class SetPartitions
{
    public static bool UnrankIntegerPartition(long rank, int n, int* outPart, out int outLen);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Combinatorics.Generation;

public static unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        int* arr = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        try
        {
            arr[0] = 1; arr[1] = 2; arr[2] = 3; arr[3] = 4;
            bool active = true;
            while (active)
            {
                active = Permutations.NextPermutation(arr, n);
            }
        }
        finally
        {
            Marshal.FreeHGlobal((nint)arr);
        }
    }
}
```