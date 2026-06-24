# IAFahim.String

## Description
Contains core and advanced string processing routines. Includes Lyndon decomposition, run-length encoding and decoding, period finding, De Bruijn sequence generation, expression parsing, NFA-based regex matching, XML and JSON tree hashing, and subsequence or substring enumeration.

## Complexity
Time Complexity is O(N) for linear string scans, expression parsing and regex matching vary depending on size.
Space Complexity is O(1) auxiliary space for in-place algorithms, or O(N) for DP tables in shortest subsequence search.

## API Signature
```csharp
namespace IAFahim.String
{
    public static unsafe class ManacherOdd
    {
        public static void Run(byte* s, int len, int* radii);
    }
    public static unsafe class DuvalLyndon
    {
        public static int Run(byte* s, int len, int* starts, int* lengths);
    }
    public static unsafe class RunLengthEncode
    {
        public static int Run(byte* s, int len, byte* values, int* counts);
    }
    public static unsafe class CountOccurrences
    {
        public static int Run(byte* text, int textLen, byte* pattern, int patLen);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int length = 5;
    byte* s = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(byte));
    int* radii = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(length * sizeof(int));
    try
    {
        s[0] = (byte)'a';
        s[1] = (byte)'b';
        s[2] = (byte)'a';
        s[3] = (byte)'b';
        s[4] = (byte)'a';
        IAFahim.String.ManacherOdd.Run(s, length, radii);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)radii);
    }
}
```
