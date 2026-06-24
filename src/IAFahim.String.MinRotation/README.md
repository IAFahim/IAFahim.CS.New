# IAFahim.String.MinRotation

## Description
Finds the starting index of the lexicographically smallest cyclic shift of a string or integer sequence using Booth's algorithm.

## Complexity
Time Complexity is O(N) linear time.
Space Complexity is O(N) space for failure function.

## API Signature
```csharp
namespace IAFahim.String.MinRotation
{
    public static unsafe class Booth
    {
        public static int Run(byte* s, int len);
        public static int Run(int* s, int len);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* s = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    try
    {
        s[0] = (byte)'b';
        s[1] = (byte)'a';
        s[2] = (byte)'b';
        s[3] = (byte)'a';
        int index = IAFahim.String.MinRotation.Booth.Run(s, len);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
    }
}
```
