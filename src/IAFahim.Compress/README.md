# IAFahim.Compress

## Description
This package provides algorithms for compressing and restoring integer arrays. It transforms regular raw values into a compressed representation and provides tools to restore the original values. It helps minimize memory footprint when storing large lists of numbers.

## Complexity
- CompressValues: O(N) where N is the array length.
- RestoreCompressed: O(N) where N is the array length.

## API Signature
```csharp
public static unsafe class CompressValues
{
    public static void Run(int* src, long* dst, int len);
    public static int RunUnique(int* src, long* dst, int len);
}

public static unsafe class RestoreCompressed
{
    public static void Run(long* src, int* dst, int len);
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Compress;

public static unsafe class Example
{
    public static void Run()
    {
        int len = 5;
        int* src = (int*)Marshal.AllocHGlobal(len * sizeof(int));
        long* dst = (long*)Marshal.AllocHGlobal(len * sizeof(long));
        try
        {
            src[0] = 10; src[1] = 20; src[2] = 10; src[3] = 30; src[4] = 20;
            CompressValues.Run(src, dst, len);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)src);
            Marshal.FreeHGlobal((nint)dst);
        }
    }
}
```