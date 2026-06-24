# IAFahim.DS.LinkCut

## Description
This package implements a Link-Cut Tree data structure. It represents a forest of trees and supports tree structural changes (linking and cutting paths) and path query operations. It is designed using splay trees and raw node pointers.

## Complexity
- Access / MakeRoot: O(log N) amortized.
- Link / Cut: O(log N) amortized.
- Path query: O(log N) amortized.

## API Signature
```csharp
public unsafe struct LctNode
{
    public int Index;
    public bool Rev;
}

public static unsafe class LinkCut
{
    public static void Access(LctNode* x);
    public static void MakeRoot(LctNode* x);
    public static LctNode* FindRoot(LctNode* x);
    public static void Link(LctNode* x, LctNode* y);
    public static void Cut(LctNode* x, LctNode* y);
    public static long Query(LctNode* x, LctNode* y);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.DS.LinkCut;

public static unsafe class Example
{
    public static void Run()
    {
        int nodeCount = 3;
        LctNode* nodes = (LctNode*)Marshal.AllocHGlobal(nodeCount * sizeof(LctNode));
        try
        {
            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i].Index = i;
                nodes[i].Rev = false;
            }
            LinkCut.MakeRoot(&nodes[0]);
            LinkCut.MakeRoot(&nodes[1]);
            LinkCut.Link(&nodes[0], &nodes[1]);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)nodes);
        }
    }
}
```