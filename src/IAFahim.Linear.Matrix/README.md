# IAFahim.Linear.Matrix

## Description
This package provides matrix operations, including matrix products, matrix exponentiation, and Berlekamp-Massey recurrence solvers.

## Complexity
Matrix products run in O(N^3) time. Berlekamp-Massey runs in O(N^2) time.

## API Signature
```csharp
public static unsafe class BerlekampMassey
{
    public static int Run(long* s, int n, long* c)
}
```

## Usage Example
```csharp
unsafe
{
    int n = 4;
    long* s = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
    long* c = (long*)System.Runtime.InteropServices.Marshal.AllocHGlobal(n * sizeof(long));
    try
    {
        s[0] = 1;
        s[1] = 2;
        s[2] = 4;
        s[3] = 8;
        int len = IAFahim.Linear.Matrix.BerlekampMassey.Run(s, n, c);
    }
    finally
    {
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)s);
        System.Runtime.InteropServices.Marshal.FreeHGlobal((System.IntPtr)c);
    }
}
```