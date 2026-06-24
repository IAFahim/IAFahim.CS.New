# IAFahim.DS.Splay

## Description
A splay tree implementation. This self-balancing binary search tree structure moves recently accessed nodes closer to the root. Supports range queries and range reversals.

## Complexity
O(log N) amortized time for tree restructuring, range updates, and query operations.

## API Signature
```csharp
public unsafe struct SplayNode
{
    public int Key;
    public int Size;
    public SplayNode* Parent;
    public SplayNode* Left;
    public SplayNode* Right;
}
public static unsafe class Splay
{
    public static void Update(SplayNode* x)
    public static void Splay_(SplayNode** root, SplayNode* x)
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
        SplayNode* root = null;
        SplayNode* node = (SplayNode*)Marshal.AllocHGlobal(sizeof(SplayNode));
        try
        {
            node->Parent = null;
            node->Left = null;
            node->Right = null;
            node->Key = 42;
            node->Size = 1;
            Splay.Update(node);
            Splay.Splay_(&root, node);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)node);
        }
    }
}
```