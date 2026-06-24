# IAFahim.DS.Rope

## Description
A rope data structure for managing long strings. It represents a string as a binary tree of nodes, allowing insertions, deletions, and substring operations on large texts.

## Complexity
O(log N) on average for insertion, deletion, and substring retrieval.

## API Signature
```csharp
public unsafe struct RopeNode
{
    public byte* Str;
    public int Len;
    public int Size;
    public int Weight;
    public RopeNode* Left;
    public RopeNode* Right;
}
public static unsafe class RopeInsert
{
    public static RopeNode* Run(RopeNode* root, int pos, RopeNode* node)
}
public static unsafe class RopeErase
{
    public static RopeNode* Run(RopeNode* root, int pos, int len)
}
public static unsafe class RopeSubstring
{
    public static RopeNode* Run(RopeNode* root, int pos, int len, byte* buf, out int count)
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
        RopeNode* root = null;
        RopeNode* child = (RopeNode*)Marshal.AllocHGlobal(sizeof(RopeNode));
        try
        {
            child->Left = null;
            child->Right = null;
            child->Size = 1;
            child->Weight = 1;
            root = RopeInsert.Run(root, 0, child);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)child);
        }
    }
}
```