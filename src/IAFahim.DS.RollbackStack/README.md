# IAFahim.DS.RollbackStack

## Description
A collection of undoable data structures. Includes rollback stacks, undoable union find (DSU), undoable bipartite DSU, and undoable binary heaps to support reverting updates.

## Complexity
O(1) for snapshot, O(K) for rollback where K is the number of reverted operations. Undoable DSU operations take O(log N) time.

## API Signature
```csharp
public static unsafe class RollbackStack
{
    public static void Init(void* mem, int capacity)
    public static int Snapshot(void* mem)
    public static void Rollback(void* mem, int targetSize, int sizeOfT)
}
public static unsafe class UndoableUnionFind
{
    public static int Snapshot(int* parent, int* size, int* history, int histSize)
    public static void Rollback(int* parent, int* size, int* history, int targetHistSize, int* currentHistSize)
    public static int Find(int* parent, int x)
    public static bool Union(int* parent, int* size, int* history, int* histSize, int a, int b)
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
        int* parent = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* size = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* history = (int*)Marshal.AllocHGlobal(20 * sizeof(int));
        int* histSize = (int*)Marshal.AllocHGlobal(sizeof(int));
        try
        {
            *histSize = 0;
            for (int i = 0; i < 10; i++)
            {
                parent[i] = i;
                size[i] = 1;
            }
            bool joined = UndoableUnionFind.Union(parent, size, history, histSize, 1, 2);
            int snap = UndoableUnionFind.Snapshot(parent, size, history, *histSize);
            UndoableUnionFind.Rollback(parent, size, history, 0, histSize);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)parent);
            Marshal.FreeHGlobal((IntPtr)size);
            Marshal.FreeHGlobal((IntPtr)history);
            Marshal.FreeHGlobal((IntPtr)histSize);
        }
    }
}
```