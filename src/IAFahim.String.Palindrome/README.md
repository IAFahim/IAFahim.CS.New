# IAFahim.String.Palindrome

## Description
Palindromic string analysis package. Includes palindromic trees for tracking distinct palindromic substrings, Manacher's algorithm for finding palindromic radii, Lyndon decomposition of strings, and occurrence counting.

## Complexity
Time Complexity is O(N) linear time for building palindromic trees, Manacher's search, and Lyndon runs.
Space Complexity is O(N) space to store nodes or radii arrays.

## API Signature
```csharp
namespace IAFahim.String.Palindrome
{
    public static unsafe class Manacher
    {
        public static void Odd(byte* s, int n, int* d);
        public static void Even(byte* s, int n, int* d);
    }
    public static unsafe class OccurrenceCount
    {
        public static long Count(byte* s, int n);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 5;
    byte* s = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* d = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    try
    {
        s[0] = (byte)'a';
        s[1] = (byte)'b';
        s[2] = (byte)'a';
        s[3] = (byte)'b';
        s[4] = (byte)'a';
        IAFahim.String.Palindrome.Manacher.Odd(s, len, d);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)d);
    }
}
```
