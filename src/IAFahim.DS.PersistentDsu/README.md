# IAFahim.DS.PersistentDsu

## Description
A persistent disjoint set union structure implemented using a persistent segment tree. It allows querying set membership and merging sets at any historical version.

## Complexity
O(log N) for Find, Union, and Query operations.

## API Signature
```csharp
public static unsafe class PersistentDsu
{
    public static int Build(int l, int r, int* parent, int* size, int* allocCnt, int* lc, int* rc)
    public static int Update(int root, int lIn, int rIn, int idx, int val, int s, int* parent, int* size, int* allocCnt, int* lc, int* rc)
    public static int Query(int root, int l, int r, int idx, int* parent, int* lc, int* rc, out int s, int* size)
    public static int Find(int root, int n, int x, int* parent, int* lc, int* rc, int* size, out int s)
    public static int Union(int root, int n, int a, int b, int* parent, int* size, int* allocCnt, int* lc, int* rc)
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
        int* allocCnt = (int*)Marshal.AllocHGlobal(sizeof(int));
        int* lc = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* rc = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        try
        {
            *allocCnt = 0;
            int root = PersistentDsu.Build(0, 9, parent, size, allocCnt, lc, rc);
            int s;
            int root2 = PersistentDsu.Union(root, 10, 1, 2, parent, size, allocCnt, lc, rc);
            int root3 = PersistentDsu.Find(root2, 10, 1, parent, lc, rc, size, out s);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)parent);
            Marshal.FreeHGlobal((IntPtr)size);
            Marshal.FreeHGlobal((IntPtr)allocCnt);
            Marshal.FreeHGlobal((IntPtr)lc);
            Marshal.FreeHGlobal((IntPtr)rc);
        }
    }
}
```