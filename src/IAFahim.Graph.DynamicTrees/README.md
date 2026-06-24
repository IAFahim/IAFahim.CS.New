# IAFahim.Graph.DynamicTrees

## Description
This package provides dynamic tree structures, including Top Trees, Link-Cut Trees, and Euler Tour Trees, supporting dynamic path queries and tree updates.

## Complexity
Amortized time complexity is O(log V) per tree update or path query.

## API Signature
```csharp
public static unsafe class LinkCutTree
{
    public static void Init(LctNode* nodes, int n)
    public static void Link(LctNode* nodes, int u, int v)
    public static void Cut(LctNode* nodes, int u, int v)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 100;
    IAFahim.Graph.DynamicTrees.LctNode* nodes = (IAFahim.Graph.DynamicTrees.LctNode*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(IAFahim.Graph.DynamicTrees.LctNode));
    try
    {
        IAFahim.Graph.DynamicTrees.LinkCutTree.Init(nodes, n);
        IAFahim.Graph.DynamicTrees.LinkCutTree.Link(nodes, 1, 2);
        IAFahim.Graph.DynamicTrees.LinkCutTree.Cut(nodes, 1, 2);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)nodes);
    }
}
```