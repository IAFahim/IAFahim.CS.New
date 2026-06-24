# IAFahim.Linear.Matrix2

## Description
Provides basic 2D matrix operations using raw long pointers, including initialization, identity matrix, addition, subtraction, matrix exponentiation, and matrix-vector product solver.

## Complexity
- MatrixNew: O(N * M) time, O(1) space.
- MatrixIdentity: O(N^2) time, O(1) space.
- MatrixAdd/Sub: O(N * M) time, O(1) space.
- MatrixMul: O(N * M * P) time, O(1) space.
- MatrixPow: O(N^3 * log(exp)) time, O(N^2) space.
- MatrixVecMul: O(N * M) time, O(1) space.

## API Signature
- public static void MatrixNew.Run(int n, int m, long* a)
- public static void MatrixNew.RunSquare(int n, long* a)
- public static void MatrixIdentity.Run(int n, long* a)
- public static void MatrixAdd.Run(int n, int m, long* a, long* b, long* c)
- public static void MatrixSub.Run(int n, int m, long* a, long* b, long* c)
- public static void MatrixMul.Run(int n, int m, int p, long* a, long* b, long* c)
- public static void MatrixPow.Run(int n, long* a, long* result, long* temp, long exp)
- public static void MatrixVecMul.Run(int n, int m, long* a, long* v, long* result)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Linear.Matrix2;

public unsafe class Example
{
    public static void Main()
    {
        int n = 2;
        long* a = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        long* c = (long*)Marshal.AllocHGlobal(n * n * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 2;
            a[2] = 3; a[3] = 4;
            b[0] = 5; b[1] = 6;
            b[2] = 7; b[3] = 8;
            MatrixMul.Run(n, n, n, a, b, c);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
            Marshal.FreeHGlobal((IntPtr)c);
        }
    }
}
```