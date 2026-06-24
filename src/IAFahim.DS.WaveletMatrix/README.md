# IAFahim.DS.WaveletMatrix

## Description
A wavelet matrix data structure for succinct representation of sequences. Supports retrieving the kth smallest element in a range, quantile queries, and rank/select operations.

## Complexity
O(N * log Sigma) build time, and O(log Sigma) query time where Sigma is the alphabet size.

## API Signature
```csharp
public static unsafe class WaveletMatrixBuild
{
    public static int Run(int* data, int n, int maxVal, int* bitmaps, int* ranks, int* mids, int log)
}
public static unsafe class WaveletMatrixKth
{
    public static int Run(int* bitmapPtr, int* rankPtr, int* mids, int l, int r, int k, int log)
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
        int* data = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        int* bitmaps = (int*)Marshal.AllocHGlobal(40 * sizeof(int));
        int* ranks = (int*)Marshal.AllocHGlobal(40 * sizeof(int));
        int* mids = (int*)Marshal.AllocHGlobal(10 * sizeof(int));
        try
        {
            for (int i = 0; i < 10; i++)
            {
                data[i] = i;
            }
            int root = WaveletMatrixBuild.Run(data, 10, 15, bitmaps, ranks, mids, 4);
            int kth = WaveletMatrixKth.Run(bitmaps, ranks, mids, 0, 9, 2, 4);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)data);
            Marshal.FreeHGlobal((IntPtr)bitmaps);
            Marshal.FreeHGlobal((IntPtr)ranks);
            Marshal.FreeHGlobal((IntPtr)mids);
        }
    }
}
```