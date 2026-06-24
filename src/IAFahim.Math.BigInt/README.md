# IAFahim.Math.BigInt

## Description
Implements arbitrary-precision integer arithmetic using raw integer arrays. Operations include addition, subtraction, finding products, exponentiation, division by a single-digit integer, and modulo operations.

## Complexity
- BigIntAdd/Sub: O(N + M) time, O(1) space.
- BigIntMul: O(N * M) time, O(1) space.
- BigIntPow: O(E * N * M) time, O(1) space.
- BigIntDiv/Mod: O(N) time, O(1) space.

## API Signature
- public static int BigIntAdd.Run(int n, int* a, int m, int* b, int* res)
- public static int BigIntSub.Run(int n, int* a, int m, int* b, int* res)
- public static int BigIntMul.Run(int n, int* a, int m, int* b, int* res)
- public static int BigIntPow.Run(int n, int* a, int e, int* res)
- public static int BigIntDiv.Run(int n, int* a, int divisor, int* res)
- public static int BigIntMod.Run(int n, int* a, int mod)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.BigInt;

public unsafe class Example
{
    public static void Main()
    {
        int n = 3;
        int m = 2;
        int* a = (int*)Marshal.AllocHGlobal(n * sizeof(int));
        int* b = (int*)Marshal.AllocHGlobal(m * sizeof(int));
        int* res = (int*)Marshal.AllocHGlobal((n + 1) * sizeof(int));
        try
        {
            a[0] = 9; a[1] = 9; a[2] = 9;
            b[0] = 1; b[1] = 2;
            int len = BigIntAdd.Run(n, a, m, b, res);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```