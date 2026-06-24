# IAFahim.String.SuffixAutomaton

## Description
Suffix Automaton implementation. Supports generalized suffix automata for multiple strings, persistent versions, kth substring queries, and transition tree traversal.

## Complexity
Time Complexity is O(N * Sigma) to build the automaton, O(M) to traverse a pattern of size M.
Space Complexity is O(N * Sigma) state transition space.

## API Signature
```csharp
namespace IAFahim.String.SuffixAutomaton
{
    public static unsafe class SuffixAutomaton
    {
        public struct State
        {
            public int Link;
            public int Len;
            public int Head;
        }
        public struct Edge
        {
            public int To;
            public int Char;
            public int Next;
        }
        public static void Build(int* ptr, int len, State* st, Edge* e, ref int size, ref int last, ref int edgeCount);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    int* ptr = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    IAFahim.String.SuffixAutomaton.SuffixAutomaton.State* st = (IAFahim.String.SuffixAutomaton.SuffixAutomaton.State*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * 2 * sizeof(IAFahim.String.SuffixAutomaton.SuffixAutomaton.State));
    IAFahim.String.SuffixAutomaton.SuffixAutomaton.Edge* e = (IAFahim.String.SuffixAutomaton.SuffixAutomaton.Edge*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * 4 * sizeof(IAFahim.String.SuffixAutomaton.SuffixAutomaton.Edge));
    try
    {
        ptr[0] = 0;
        ptr[1] = 1;
        ptr[2] = 0;
        int size = 0;
        int last = 0;
        int edgeCount = 0;
        IAFahim.String.SuffixAutomaton.SuffixAutomaton.Build(ptr, len, st, e, ref size, ref last, ref edgeCount);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)ptr);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)st);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)e);
    }
}
```
