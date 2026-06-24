# IAFahim.DS.RollbackSeg

## Description
A segment tree implementation supporting rollback operations to restore previous states, along with dynamic Li Chao trees and divide and conquer optimization utilities.

## Complexity
O(log N) for tree building, point/range updates, and queries. Rollback takes time proportional to the number of undone updates.

## API Signature
```csharp
public static unsafe class RollbackSegBuild
{
    public static void RunInt32(int* arr, int* tree, int node, int l, int r)
    public static void RunInt64(long* arr, long* tree, int node, int l, int r)
}
public static unsafe class RollbackSegUpdate
{
    public static void RangeAddInt64(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top, int node, int l, int r, int ql, int qr, long val)
    public static void PointSetInt64(long* tree, int* histNode, long* histVal, byte* histType, int* top, int node, int l, int r, int idx, long val)
}
public static unsafe class RollbackSegQuery
{
    public static long RangeSumInt64(long* tree, long* lazy, int node, int l, int r, int ql, int qr)
}
public static unsafe class RollbackSegRollback
{
    public static void Run(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top, int checkpoint)
    public static void UndoLast(long* tree, long* lazy, int* histNode, long* histVal, byte* histType, int* top)
    public static int GetCheckpoint(int* top)
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
        long* arr = (long*)Marshal.AllocHGlobal(10 * sizeof(long));
        long* tree = (long*)Marshal.AllocHGlobal(40 * sizeof(long));
        try
        {
            for (int i = 0; i < 10; i++)
            {
                arr[i] = i;
            }
            RollbackSegBuild.RunInt64(arr, tree, 1, 0, 9);
            long sum = RollbackSegQuery.RangeSumInt64(tree, null, 1, 0, 9, 2, 5);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)arr);
            Marshal.FreeHGlobal((IntPtr)tree);
        }
    }
}
```