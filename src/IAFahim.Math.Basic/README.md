# IAFahim.Math.Basic

## Description
Offers basic integer math utilities, including absolute values, minimum or maximum queries, rounding divisions, modulo normalization, swap functions, fast exponentiation, roots, power-of-two queries, log2 queries, and pointer-based value update helper functions.

## Complexity
- All operations: O(1) time, O(1) space except root and power functions which are O(log(N)) time.

## API Signature
- public static int MinInt.Run(int a, int b)
- public static long MinInt64.Run(long a, long b)
- public static int MaxInt.Run(int a, int b)
- public static long MaxInt64.Run(long a, long b)
- public static int AbsInt.Run(int v)
- public static long AbsInt64.Run(long v)
- public static int CeilDiv.Run(int a, int b)
- public static long CeilDiv.Run(long a, long b)
- public static int FloorDiv.Run(int a, int b)
- public static long FloorDiv.Run(long a, long b)
- public static int Clamp.Run(int v, int lo, int hi)
- public static long Clamp.Run(long v, long lo, long hi)
- public static long FastPow.Run(long a, long e, long mod)
- public static long IntegerSqrt.Run(long x)
- public static long NthRoot.Run(long x, int n)
- public static long IntegerCbrt.Run(long x)
- public static bool IsPerfectSquare.Run(long x)
- public static bool IsPowerOfTwo.Run(long x)
- public static long NextPowerOfTwo.Run(long x)
- public static long PrevPowerOfTwo.Run(long x)
- public static int FloorLog2.Run(long x)
- public static int CeilLog2.Run(long x)
- public static long SafeMulMod.Run(long a, long b, long mod)
- public static long NormalizeModulo.Run(long x, long mod)
- public static void Minimize.Run(long* a, long b)
- public static void Maximize.Run(long* a, long b)
- public static bool RelaxMin.Run(long* ptr, long val)
- public static bool RelaxMax.Run(long* ptr, long val)
- public static void SwapInts.Run(int* a, int* b)
- public static void SwapPairs.Run(long* a, long* b)

## Usage Example
```csharp
using System;
using IAFahim.Math.Basic;

public unsafe class Example
{
    public static void Main()
    {
        int x = 10;
        int y = 20;
        int minimum = MinInt.Run(x, y);
        long baseVal = 2;
        long exponent = 10;
        long mod = 1000000007;
        long power = FastPow.Run(baseVal, exponent, mod);
    }
}
```