# IAFahim.DS.PersistentTreap

## Description
A persistent treap (randomized binary search tree) implementation. Supports split, merge, insert, erase, and find operations while preserving previous versions by copying nodes on updates.

## Complexity
O(log N) on average for split, merge, insert, erase, and find operations.

## API Signature
```csharp
public static unsafe class PersistentTreapNode
{
    public static int NewNode<T>(T* nodes, int* left, int* right, int* prio, int* size, T val, int* allocCnt)
    public static int CloneNode<T>(T* nodes, int* left, int* right, int* prio, int* size, int src, int* allocCnt)
    public static void Update(int* left, int* right, int* size, int x)
}
public static unsafe class PersistentTreapSplit
{
    public static void Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int root, T key, int* outLeft, int* outRight, int* allocCnt)
}
public static unsafe class PersistentTreapMerge
{
    public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int l, int r, int* allocCnt)
}
public static unsafe class PersistentTreapInsert
{
    public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int* allocCnt, int root, T val)
}
public static unsafe class PersistentTreapErase
{
    public static int Run<T>(T* nodes, int* left, int* right, int* prio, int* size, int* allocCnt, int root, T val)
}
public static unsafe class PersistentTreapFind
{
    public static bool Run<T>(T* nodes, int* left, int* right, int root, T val)
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
        int* left = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* right = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* prio = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* size = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* allocCnt = (int*)Marshal.AllocHGlobal(sizeof(int));
        int* nodes = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        try
        {
            *allocCnt = 0;
            int root = 0;
            root = PersistentTreapInsert.Run(nodes, left, right, prio, size, allocCnt, root, 42);
            bool found = PersistentTreapFind.Run(nodes, left, right, root, 42);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)left);
            Marshal.FreeHGlobal((IntPtr)right);
            Marshal.FreeHGlobal((IntPtr)prio);
            Marshal.FreeHGlobal((IntPtr)size);
            Marshal.FreeHGlobal((IntPtr)allocCnt);
            Marshal.FreeHGlobal((IntPtr)nodes);
        }
    }
}
```