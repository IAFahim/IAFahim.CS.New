# IAFahim.DS.Sparse

## Description
A library for range query structures including sparse tables, disjoint sparse tables, and square root decomposition. Primarily useful for range minimum query (RMQ) operations.

## Complexity
O(N log N) setup and O(1) query for sparse tables. O(N log N) setup and O(1) query for disjoint sparse tables. O(sqrt(N)) query for square root decomposition.

## API Signature
```csharp
public static unsafe class SparseTableBuild
{
    public static void RunInt32(int* arr, int* table, int* log, int n)
    public static void RunInt64(long* arr, long* table, int* log, int n)
}
public static unsafe class SparseTableQuery
{
    public static int MinInt32(int* table, int* log, int l, int r, int n)
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
        int* table = (int*)Marshal.AllocHGlobal(40 * sizeof(int));
        int* log = (int*)Marshal.AllocHGlobal(11 * sizeof(int));
        try
        {
            for (int i = 0; i < 10; i++)
            {
                arr[i] = i;
            }
            SparseTableBuild.RunInt32(arr, table, log, 10);
            int min = SparseTableQuery.MinInt32(table, log, 2, 5, 10);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)arr);
            Marshal.FreeHGlobal((IntPtr)table);
            Marshal.FreeHGlobal((IntPtr)log);
        }
    }
}
```