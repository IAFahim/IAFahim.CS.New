# IAFahim.String.Parse

## Description
Implements string parsing and recognition algorithms. Includes LL parsing, LR parsing, Earley parsing, the CYK parsing algorithm for context-free grammars, and suffix oracle construction for pattern queries.

## Complexity
Time Complexity is O(N) for LL and LR parsing, O(N^3) for general Earley and CYK parsing. Suffix oracle query is O(M) for pattern length M.
Space Complexity is O(N) for parsing tables and stacks.

## API Signature
```csharp
namespace IAFahim.String.Parse
{
    public static unsafe class SuffixOracle
    {
        public static void Build(byte* text, int len, int sigma);
        public static bool Contains(byte* pattern, int patLen);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    int sigma = 256;
    byte* text = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    try
    {
        text[0] = (byte)'a';
        text[1] = (byte)'b';
        text[2] = (byte)'a';
        IAFahim.String.Parse.SuffixOracle.Build(text, len, sigma);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)text);
    }
}
```
