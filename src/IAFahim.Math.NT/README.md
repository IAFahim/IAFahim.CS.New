# IAFahim.Math.NT

## Description
Implements comprehensive number theory algorithms. Includes primality testing via Miller-Rabin, integer factoring via Pollard's rho, sieve helpers (Euler totient, Mobius function, divisor sum and count tables), arithmetic function prefix sums, discrete log solvers, Legendre/Jacobi symbols, continued fractions, Stern-Brocot tree, division transforms, and bitwise utilities.

## Complexity
- MillerRabin: O(K * log^3(N)) time.
- PollardRho: O(N^(1/4) * log(N)) time.
- Sieves: O(N) time.
- Min25: O(N^(3/4) / log(N)) time.

## API Signature
- public static bool MillerRabin.Run(long n)
- public static long PollardRho.Run(long n)
- public static int Factorize.Run(long n, long* factors)
- public static long Phi.Run(long n)
- public static int Divisors.Run(long n, long* divs)

## Usage Example
```csharp
using System;
using IAFahim.Math.NT;

public unsafe class Example
{
    public static void Main()
    {
        long n = 1000000007;
        bool prime = MillerRabin.Run(n);
        long composite = 999999999;
        long factor = PollardRho.Run(composite);
    }
}
```