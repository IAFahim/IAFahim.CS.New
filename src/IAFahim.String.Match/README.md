# IAFahim.String.Match

## Description
Implements string matching algorithms. Includes exact matching, rolling hash search, approximate matching, Lyndon runs search, and parameterized matching.

## Complexity
Time Complexity is O(N + M) for linear matching, O(N * K) or O(N + K^2) for approximate matching.
Space Complexity is O(M) for pattern preprocessing arrays.

## API Signature
```csharp
namespace IAFahim.String.Match
{
    public static unsafe class ZAlgorithm
    {
        public static void Run(byte* ptr, int len, int* zPtr);
        public static void Run(int* ptr, int len, int* zPtr);
    }
    public static unsafe class PrefixFunction
    {
        public static void Run(byte* ptr, int len, int* piPtr);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* ptr = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* zPtr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        ptr[0] = (byte)'a';
        ptr[1] = (byte)'b';
        ptr[2] = (byte)'a';
        ptr[3] = (byte)'b';
        IAFahim.String.Match.ZAlgorithm.Run(ptr, len, zPtr);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)zPtr);
    }
}
```
