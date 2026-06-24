# IAFahim.String.Automata

## Description
Implements finite automata algorithms. Includes DFA minimization, DFA operations like union and intersection, NFA to DFA conversion, and subsequence automata construction for quick subsequence queries.

## Complexity
Time Complexity is O(N * Sigma) for automaton building, O(M) for matching a pattern of size M.
Space Complexity is O(S * Sigma) to store transitions.

## API Signature
```csharp
namespace IAFahim.String.Automata
{
    public static unsafe class SubsequenceAutomaton
    {
        public static void Build(byte* text, int len, int* next, int sigma);
        public static bool Contains(int* next, byte* pattern, int patLen, int sigma);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    int sigma = 26;
    byte* text = (byte*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(byte));
    int* next = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal((len + 1) * sigma * sizeof(int));
    try
    {
        text[0] = (byte)'a';
        text[1] = (byte)'b';
        text[2] = (byte)'c';
        IAFahim.String.Automata.SubsequenceAutomaton.Build(text, len, next, sigma);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)text);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
    }
}
```
