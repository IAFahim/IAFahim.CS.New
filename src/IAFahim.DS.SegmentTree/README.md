# IAFahim.DS.SegmentTree

## Description
A library of segment tree structures. Includes standard segment trees, lazy propagation segment trees, persistent segment trees (Chairman tree), merge sort trees, mergeable segment trees, and Li Chao trees.

## Complexity
O(log N) for point/range updates and query operations. Tree building takes O(N) time, or O(N log N) for merge sort trees.

## API Signature
```csharp
public static unsafe class SegmentTreeBuild
{
    public static void RunInt32(int* arr, int* tree, int node, int l, int r)
    public static void RunInt64(long* arr, long* tree, int node, int l, int r)
}
public static unsafe class SegmentTreeQuery
{
    public static int RunInt32(int* tree, int node, int l, int r, int ql, int qr)
    public static long RunInt64(long* tree, int node, int l, int r, int ql, int qr)
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
        int* arr = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* tree = (int*)Marshal.AllocHGlobal(40 * sizeof(int));
        try
        {
            for (int i = 0; i < 10; i++)
            {
                arr[i] = i;
            }
            SegmentTreeBuild.RunInt32(arr, tree, 1, 0, 9);
            int sum = SegmentTreeQuery.RunInt32(tree, 1, 0, 9, 2, 5);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)arr);
            Marshal.FreeHGlobal((IntPtr)tree);
        }
    }
}
```