# IAFahim.String.SuffixArray

## Description
Suffix array library for string search and query. Contains static suffix array building, LCP interval tree construction, suffix matching, and dynamic suffix arrays using balanced search trees.

## Complexity
Time Complexity is O(N log^2 N) or O(N log N) to construct the suffix array, O(M log N) to search for pattern of length M. Dynamic operations run in O(log^2 N) time.
Space Complexity is O(N) space.

## API Signature
```csharp
namespace IAFahim.String.SuffixArray
{
    public static unsafe class SuffixArray
    {
        public static void Build(byte* ptr, int len, int* sa, int* rank, int* tmpSa, int* count, int* tmpRank);
    }
    public static unsafe class Locate
    {
        public static int Find(int* sa, int saLen, byte* text, int textLen, byte* pattern, int patLen);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* ptr = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* sa = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int* rank = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int* tmpSa = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int* count = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(256 * sizeof(int));
    int* tmpRank = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        ptr[0] = (byte)'b';
        ptr[1] = (byte)'a';
        ptr[2] = (byte)'b';
        ptr[3] = (byte)'a';
        IAFahim.String.SuffixArray.SuffixArray.Build(ptr, len, sa, rank, tmpSa, count, tmpRank);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)sa);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)rank);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tmpSa);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)count);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)tmpRank);
    }
}
```
