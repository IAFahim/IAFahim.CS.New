# IAFahim.Math.Transform.Fft

## Description
This package implements the Fast Fourier Transform (FFT) and its inverse on complex numbers using double arrays. It supports fast polynomial convolution.

## Complexity
Forward and inverse transforms run in O(N log N) steps. Convolution of size N and M runs in O((N+M) log(N+M)) steps.

## API Signature
```csharp
namespace IAFahim.Math.Transform.Fft
{
    public static unsafe class FftTransform
    {
        public static void Forward(double* re, double* im, int n);
        public static void Inverse(double* re, double* im, int n);
    }

    public static unsafe class FftConvolution
    {
        public static int Run(double* a, int n, double* b, int m, double* res);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Math.Transform.Fft;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        double* re = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        double* im = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        try
        {
            re[0] = 1.0;
            im[0] = 0.0;
            FftTransform.Forward(re, im, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)re);
            Marshal.FreeHGlobal((nint)im);
        }
    }
}
```