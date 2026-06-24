# IAFahim.String.SuffixTree

## Description
Constructs suffix trees using Ukkonen's linear time algorithm. Allows efficient substring indexing and pattern search in text.

## Complexity
Time Complexity is O(N * Sigma) or O(N) to build, and O(M) to search for a pattern of length M.
Space Complexity is O(N * Sigma) to store transitions and tree nodes.

## API Signature
```csharp
namespace IAFahim.String.SuffixTree
{
    public static unsafe class SuffixTreeUkkonen
    {
        public struct Node { public int Link; public int Start; public int Len; public int FirstEdge; }
        public struct Edge { public int To; public int Char; public int Next; public int Min; public int Max; }
        public static void Build(int* s, int len, Node* nodes, Edge* edges, ref int nodeCount, ref int edgeCount, ref int last);
    }
}
```

## Usage Example
```csharp
unsafe
{
    int len = 3;
    int* s = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    IAFahim.String.SuffixTree.SuffixTreeUkkonen.Node* nodes = (IAFahim.String.SuffixTree.SuffixTreeUkkonen.Node*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * 2 * sizeof(IAFahim.String.SuffixTree.SuffixTreeUkkonen.Node));
    IAFahim.String.SuffixTree.SuffixTreeUkkonen.Edge* edges = (IAFahim.String.SuffixTree.SuffixTreeUkkonen.Edge*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * 4 * sizeof(IAFahim.String.SuffixTree.SuffixTreeUkkonen.Edge));
    try
    {
        s[0] = 97;
        s[1] = 98;
        s[2] = 0;
        int nodeCount = 0;
        int edgeCount = 0;
        int last = 0;
        IAFahim.String.SuffixTree.SuffixTreeUkkonen.Build(s, len, nodes, edges, ref nodeCount, ref edgeCount, ref last);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)edges);
    }
}
```
