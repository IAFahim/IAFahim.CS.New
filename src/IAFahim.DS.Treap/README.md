# IAFahim.DS.Treap

## Description
A randomized binary search tree (treap) implementation. Supports implicit index queries, range sum updates, range minimum queries, range reversals, range rotations, and affine transformations.

## Complexity
O(log N) on average for tree split, merge, range updates, and query operations.

## API Signature
```csharp
public unsafe struct TreapNode
{
    public int Key;
    public int Priority;
    public int Size;
    public bool Rev;
    public long Sum;
    public TreapNode* Left;
    public TreapNode* Right;
}
public static unsafe class Treap
{
    public static void Insert(TreapNode** root, TreapNode* node)
    public static TreapNode* Find(TreapNode* root, int key)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.DS;

public static unsafe class Example
{
    public static void Run()
    {
        TreapNode* root = null;
        TreapNode* node = (TreapNode*)Marshal.AllocHGlobal(sizeof(TreapNode));
        try
        {
            node->Left = null;
            node->Right = null;
            node->Key = 42;
            node->Priority = 100;
            node->Size = 1;
            Treap.Insert(&root, node);
            TreapNode* found = Treap.Find(root, 42);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)node);
        }
    }
}
```