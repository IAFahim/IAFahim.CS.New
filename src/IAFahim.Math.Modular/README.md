# IAFahim.Math.Modular

## Description
Implements common modular arithmetic operations and number theory functions. Includes greatest common divisor (GCD), least common multiple (LCM), modular addition, modular subtraction, modular product, modular division, modular exponentiation, modular inverse, modular square root, Chinese Remainder Theorem (CRT), and Extended Chinese Remainder Theorem (EXCRT).

## Complexity
- Add/Sub/Mul/Normalize: O(1) time, O(1) space.
- Gcd/Lcm/ExtendedGcd/ModPow/ModInv/ModDiv/ModSqrt: O(log(min(A, B))) time, O(1) space.
- Crt: O(log(min(M1, M2))) time, O(1) space.
- Excrt: O(N * log(lcm)) time, O(1) space.

## API Signature
- public static long Gcd.Run(long a, long b)
- public static int Gcd.Run(int a, int b)
- public static long Lcm.Run(long a, long b)
- public static int Lcm.Run(int a, int b)
- public static long ExtendedGcd.Run(long a, long b, out long x, out long y)
- public static long ModNormalize.Run(long v, long mod)
- public static long ModAdd.Run(long a, long b, long mod)
- public static long ModSub.Run(long a, long b, long mod)
- public static long ModMul.Run(long a, long b, long mod)
- public static long ModDiv.Run(long a, long b, long mod)
- public static long ModPow.Run(long b, long e, long mod)
- public static long ModInv.Run(long a, long mod)
- public static long ModSqrt.Run(long a, long mod)
- public static long Crt.Run(long r1, long m1, long r2, long m2)
- public static long Excrt.Run(long* remainders, long* moduli, int len)

## Usage Example
```csharp
using System;
using IAFahim.Math.Modular;

public unsafe class Example
{
    public static void Main()
    {
        long a = 15;
        long b = 25;
        long greatestCommonDivisor = Gcd.Run(a, b);
        long baseVal = 2;
        long expVal = 10;
        long modulo = 1000000007;
        long powVal = ModPow.Run(baseVal, expVal, modulo);
    }
}
```