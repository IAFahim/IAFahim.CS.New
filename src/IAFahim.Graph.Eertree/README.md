# IAFahim.Graph.Eertree

## Description
This package provides the Eertree structure for indexing all distinct palindromic substrings in a sequence.

## Complexity
Time complexity is O(N) for building the palindromic tree, where N is the sequence length.

## API Signature
```csharp
public static unsafe class Node
{
    public static void Build(int* s, int len, Node* nodes, Next* next, ref int nodeCount, ref int nextCount, ref int last, ref int cur)
}
```

## Usage Example
```csharp
unsafe
{
    int len = 5;
    int* s = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(len * sizeof(int));
    int nodeCount = 0;
    int nextCount = 0;
    int last = 0;
    int cur = 0;
    IAFahim.Graph.Eertree.Node* nodes = (IAFahim.Graph.Eertree.Node*)System.Runtime.InteropServices.Marshal.AllocHGlobal(10 * sizeof(IAFahim.Graph.Eertree.Node));
    IAFahim.Graph.Eertree.Next* next = (IAFahim.Graph.Eertree.Next*)System.Runtime.InteropServices.Marshal.AllocHGlobal(10 * sizeof(IAFahim.Graph.Eertree.Next));
    try
    {
        s[0] = 1;
        s[1] = 2;
        s[2] = 1;
        s[3] = 2;
        s[4] = 1;
        IAFahim.Graph.Eertree.Node.Build(s, len, nodes, next, ref nodeCount, ref nextCount, ref last, ref cur);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)next);
    }
}
```