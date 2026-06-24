# IAFahim.Math.Combinatorics

## Description
Offers functions for discrete counting and prime numbers. Includes stirling numbers, bell numbers, partition numbers, derangements, stars and bars, factorial and modular inverse factorial tables, binomial coefficient solving, linear congruences, and prime sieve utilities (segmented and linear).

## Complexity
- Stirling/Partition/Bell: O(N * K) time, O(N * K) space.
- Binom/Lucas: O(K) or O(log(P)) time, O(1) space.
- Factorial tables: O(N) time, O(1) space.
- Linear/Segmented Sieve: O(N) time, O(N) space.
- IsPrime: O(sqrt(N)) time, O(1) space.

## API Signature
- public static long PermuteCount.Run(int n, long mod)
- public static long MultisetPermutations.Run(int n, int* counts, int k, long mod)
- public static long StirlingFirst.Run(long n, long k, long mod)
- public static long StirlingSecond.Run(long n, long k, long mod)
- public static long BellNumbers.Run(long n, long mod)
- public static long PartitionNumbers.Run(long n, long mod)
- public static long Derangements.Run(long n, long mod)
- public static long StarsBars.Run(long n, long k, long mod)
- public static void Factorial.Run(long* fact, long* invFact, int n, long mod)
- public static long Factorial.Run(long n, long mod)
- public static bool LinearCongruence.Run(long a, long b, long m, out long x, out long g)
- public static long Binom.Run(long n, long k, long mod)
- public static long BinomLucas.Run(long n, long k, long p)
- public static long BinomLarge.Run(long n, long k, long mod)
- public static int SievePrimes.Run(int* primes, bool* isPrime, int n)
- public static int LinearSieve.Run(int* primes, int* lp, int n)
- public static int SegmentedSieve.Run(long low, long high, int* primes, int primeCount, int* result)
- public static bool IsPrime.Run(long n)

## Usage Example
```csharp
using System;
using IAFahim.Math.Combinatorics;

public unsafe class Example
{
    public static void Main()
    {
        long n = 10;
        long k = 3;
        long mod = 1000000007;
        long ways = Binom.Run(n, k, mod);
    }
}
```