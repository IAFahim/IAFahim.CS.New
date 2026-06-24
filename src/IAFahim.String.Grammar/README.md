# IAFahim.String.Grammar

## Description
Implements grammar-based string compression and Straight-Line Programs. Represents a string as a context-free grammar to shrink size and query individual symbols in logarithmic time.

## Complexity
Time Complexity is O(N log N) to construct the grammar representation, O(log N) to query a specific symbol position.
Space Complexity is O(G) where G is the grammar size.

## API Signature
```csharp
namespace IAFahim.String.Grammar
{
    public static unsafe class StraightLineProgram
    {
        public struct Rule
        {
            public int Left;
            public int Right;
            public int Len;
            public byte Char;
            public bool IsTerminal;
        }
        public static int Build(byte* s, int len, int maxRules, Rule* rules, ref int ruleCount);
        public static byte Query(Rule* rules, int ruleId, int pos);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 4;
    byte* s = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int maxRules = 10;
    IAFahim.String.Grammar.StraightLineProgram.Rule* rules = (IAFahim.String.Grammar.StraightLineProgram.Rule*)System.Runtime.InteropServices.Marshal.AllocHGlobal(maxRules * sizeof(IAFahim.String.Grammar.StraightLineProgram.Rule));
    try
    {
        s[0] = (byte)'a';
        s[1] = (byte)'b';
        s[2] = (byte)'a';
        s[3] = (byte)'b';
        int ruleCount = 0;
        IAFahim.String.Grammar.StraightLineProgram.Build(s, len, maxRules, rules, ref ruleCount);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)rules);
    }
}
```
