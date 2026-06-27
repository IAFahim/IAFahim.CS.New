import json
import re

# Validation function to ensure no words contain c...a...t in sequence (except parts of the package name)
def contains_cat_in_sequence(word):
    w = word.lower()
    # Strip any non-alphanumeric characters
    w = "".join([c for c in w if c.isalnum()])
    c_idx = w.find('c')
    if c_idx != -1:
        a_idx = w.find('a', c_idx + 1)
        if a_idx != -1:
            t_idx = w.find('t', a_idx + 1)
            if t_idx != -1:
                return True
    return False

def validate_readme(pkg_name, readme_text):
    # Check case-insensitive "cat" substring in the entire README
    if "cat" in readme_text.lower():
        indices = [m.start() for m in re.finditer('cat', readme_text, re.IGNORECASE)]
        for idx in indices:
            start = max(0, idx - 20)
            end = min(len(readme_text), idx + 20)
            print(f"Error: Substring 'cat' found in {pkg_name} around: ...{readme_text[start:end]}...")
        return False

    # Check words in explanations for c...a...t in sequence
    # Explanation includes Description and Complexity sections
    lines = readme_text.splitlines()
    in_explanation = False
    in_code_block = False
    
    for line_num, line in enumerate(lines):
        stripped = line.strip()
        if stripped.startswith("```"):
            in_code_block = not in_code_block
            continue
        if in_code_block:
            continue
            
        if stripped.startswith("## Description") or stripped.startswith("## Complexity"):
            in_explanation = True
            continue
        elif stripped.startswith("##") or stripped.startswith("#"):
            in_explanation = False
            # But wait, headers themselves shouldn't contain the sequence unless it's part of the package name
            # Let's validate headers too, but skip package name parts
        
        if in_explanation:
            # Split by non-alphanumeric characters to get words
            words = re.findall(r'[a-zA-Z]+', line)
            for word in words:
                # Skip if it is part of the package name or namespace parts
                skip = False
                for part in pkg_name.split('.'):
                    if word.lower() == part.lower():
                        skip = True
                if skip:
                    continue

                if contains_cat_in_sequence(word):
                    print(f"Error in {pkg_name} on line {line_num+1}: Word '{word}' contains 'c', 'a', 't' in sequence in explanation.")
                    return False

    # Check headers
    required_headers = [
        f"# {pkg_name}",
        "## Description",
        "## Complexity",
        "## API Signature",
        "## Usage Example"
    ]
    for header in required_headers:
        if header not in readme_text:
            print(f"Error: Header '{header}' is missing in {pkg_name}.")
            return False

    return True

# Define README contents for each package
readmes = {}

# 1. IAFahim.Linear.Matrix2
readmes["IAFahim.Linear.Matrix2"] = """# IAFahim.Linear.Matrix2

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
```"""

# 2. IAFahim.Math.Arithmetic
readmes["IAFahim.Math.Arithmetic"] = """# IAFahim.Math.Arithmetic

## Description
Provides checked arithmetic operations for 32-bit and 64-bit signed integers. These functions return a boolean value showing if the operation succeeded without overflow, and output the result.

## Complexity
- All operations: O(1) time, O(1) space.

## API Signature
- public static bool TryAdd.Run(int a, int b, out int res)
- public static bool TryAdd.Run(long a, long b, out long res)
- public static bool TrySub.Run(int a, int b, out int res)
- public static bool TrySub.Run(long a, long b, out long res)
- public static bool TryMul.Run(int a, int b, out int res)
- public static bool TryMul.Run(long a, long b, out long res)
- public static bool TryDiv.Run(int a, int b, out int res)
- public static bool TryDiv.Run(long a, long b, out long res)

## Usage Example
```csharp
using System;
using IAFahim.Math.Arithmetic;

public unsafe class Example
{
    public static void Main()
    {
        int a = 100;
        int b = 200;
        int res = 0;
        bool success = TryAdd.Run(a, b, out res);
    }
}
```"""

# 3. IAFahim.Math.Barycentric
readmes["IAFahim.Math.Barycentric"] = """# IAFahim.Math.Barycentric

## Description
Offers utilities for barycentric weights on triangles in 2D and 3D space. Includes weight solving, interpolation of vector and scalar values, inside-triangle testing, projection of points, and signed area.

## Complexity
- All operations: O(1) time, O(1) space.

## API Signature
- public static float3 BarycentricCoords.Compute(float3 a, float3 b, float3 c, float3 p)
- public static float3 BarycentricCoords.Interpolate(float3 a, float3 b, float3 c, float3 bary)
- public static float BarycentricCoords.InterpolateScalar(float va, float vb, float vc, float3 bary)
- public static bool BarycentricCoords.IsInside(float3 bary)
- public static float2 BarycentricCoords.Compute2D(float2 a, float2 b, float2 c, float2 p)
- public static float3 BarycentricCoords.ProjectOntoTriangle(float3 a, float3 b, float3 c, float3 p)
- public static float BarycentricCoords.SignedArea(float3 a, float3 b, float3 c)

## Usage Example
```csharp
using Unity.Mathematics;
using IAFahim.Math.Barycentric;

public unsafe class Example
{
    public static void Main()
    {
        float3 a = new float3(0.0f, 0.0f, 0.0f);
        float3 b = new float3(1.0f, 0.0f, 0.0f);
        float3 c = new float3(0.0f, 1.0f, 0.0f);
        float3 p = new float3(0.25f, 0.25f, 0.0f);
        float3 weights = BarycentricCoords.Compute(a, b, c, p);
        bool inside = BarycentricCoords.IsInside(weights);
    }
}
```"""

# 4. IAFahim.Math.Basic
readmes["IAFahim.Math.Basic"] = """# IAFahim.Math.Basic

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
```"""

# 5. IAFahim.Math.BigInt
readmes["IAFahim.Math.BigInt"] = """# IAFahim.Math.BigInt

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
```"""

# 6. IAFahim.Math.Combinatorics
readmes["IAFahim.Math.Combinatorics"] = """# IAFahim.Math.Combinatorics

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
```"""

# 7. IAFahim.Math.Gauss
readmes["IAFahim.Math.Gauss"] = """# IAFahim.Math.Gauss

## Description
Provides Gaussian elimination solver for linear equation systems over real numbers (double) and modular arithmetic (mod P). Also computes the determinant of a square matrix mod P.

## Complexity
- GaussEliminationDouble / GaussModP: O(N^2 * M) time, O(1) space.
- Determinant: O(N^3) time, O(1) space.

## API Signature
- public static int GaussEliminationDouble.Run(double* a, double* b, double* x, int n, int m)
- public static bool GaussModP.Run(long* a, long* b, long* x, int n, int m, long mod)
- public static long GaussModP.Determinant(long* a, int n, long mod)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Gauss;

public unsafe class Example
{
    public static void Main()
    {
        int n = 2;
        int m = 3;
        double* a = (double*)Marshal.AllocHGlobal(n * m * sizeof(double));
        double* b = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        double* x = (double*)Marshal.AllocHGlobal(n * sizeof(double));
        try
        {
            a[0] = 2.0; a[1] = 1.0; a[2] = 0.0;
            a[3] = 1.0; a[4] = -1.0; a[5] = 0.0;
            b[0] = 5.0; b[1] = 1.0;
            int rank = GaussEliminationDouble.Run(a, b, x, n, m);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
            Marshal.FreeHGlobal((IntPtr)x);
        }
    }
}
```"""

# 8. IAFahim.Math.Kalman
readmes["IAFahim.Math.Kalman"] = """# IAFahim.Math.Kalman

## Description
Implements 1D scalar and 3D vector Kalman filtering for noise reduction and state estimation. Provides prediction and update steps, as well as utility functions to filter a series of input measurements.

## Complexity
- Predict / Update / PredictCovariance: O(1) time, O(1) space.
- Run: O(N) time, O(1) space.

## API Signature
- public static float ScalarKalmanFilter.Predict(float state, float velocity, float processNoise, float dt)
- public static float ScalarKalmanFilter.PredictCovariance(float covariance, float processNoise, float dt)
- public static float ScalarKalmanFilter.Update(float predictedState, float predictedCovariance, float measurement, float measurementNoise, out float updatedCovariance)
- public static void ScalarKalmanFilter.Run(float* measurements, int count, float processNoise, float measurementNoise, float* outFiltered)
- public static float3 VectorKalmanFilter.Predict(float3 state, float3 velocity, float processNoise, float dt)
- public static float3 VectorKalmanFilter.PredictCovariance(float3 covariance, float3 processNoise, float dt)
- public static float3 VectorKalmanFilter.Update(float3 predictedState, float3 predictedCov, float3 measurement, float measurementNoise, out float3 updatedCov)
- public static void VectorKalmanFilter.Run(float3* measurements, int count, float processNoise, float measurementNoise, float3* outFiltered)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Kalman;

public unsafe class Example
{
    public static void Main()
    {
        int count = 5;
        float* measurements = (float*)Marshal.AllocHGlobal(count * sizeof(float));
        float* filtered = (float*)Marshal.AllocHGlobal(count * sizeof(float));
        try
        {
            measurements[0] = 1.0f;
            measurements[1] = 1.1f;
            measurements[2] = 0.9f;
            measurements[3] = 1.0f;
            measurements[4] = 1.2f;
            ScalarKalmanFilter.Run(measurements, count, 0.1f, 0.2f, filtered);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)measurements);
            Marshal.FreeHGlobal((IntPtr)filtered);
        }
    }
}
```"""

# 9. IAFahim.Math.Modular
readmes["IAFahim.Math.Modular"] = """# IAFahim.Math.Modular

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
```"""

# 10. IAFahim.Math.NT
readmes["IAFahim.Math.NT"] = """# IAFahim.Math.NT

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
```"""

# 11. IAFahim.Math.Noise
readmes["IAFahim.Math.Noise"] = """# IAFahim.Math.Noise

## Description
Provides 2D Perlin and Simplex noise algorithms. These are useful for procedural content generation, terrain generation, and visual effects.

## Complexity
- All operations: O(1) time, O(1) space.

## API Signature
- public static float PerlinNoise.Noise2D(float2 p)
- public static float SimplexNoise.Noise2D(float2 p)

## Usage Example
```csharp
using Unity.Mathematics;
using IAFahim.Math.Noise;

public unsafe class Example
{
    public static void Main()
    {
        float2 position = new float2(1.5f, 2.5f);
        float value1 = PerlinNoise.Noise2D(position);
        float value2 = SimplexNoise.Noise2D(position);
    }
}
```"""

# 12. IAFahim.Math.PoissonDisk
readmes["IAFahim.Math.PoissonDisk"] = """# IAFahim.Math.PoissonDisk

## Description
Implements 2D and 3D Poisson disk sampling algorithms to generate blue noise distributions. Useful for random object placement, sampling patterns, and graphics.

## Complexity
- All operations: O(N) average time, O(grid_size) space.

## API Signature
- public static int PoissonDisk2D.Run(float2 min, float2 max, float minDistance, float2* output, int maxPoints, int seed)
- public static int PoissonDisk3D.Run(float3 min, float3 max, float minDistance, float3* output, int maxPoints, int seed)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Math.PoissonDisk;

public unsafe class Example
{
    public static void Main()
    {
        float2 min = new float2(0.0f, 0.0f);
        float2 max = new float2(10.0f, 10.0f);
        float minDistance = 2.0f;
        int maxPoints = 100;
        float2* output = (float2*)Marshal.AllocHGlobal(maxPoints * sizeof(float2));
        try
        {
            int count = PoissonDisk2D.Run(min, max, minDistance, output, maxPoints, 42);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)output);
        }
    }
}
```"""

# 13. IAFahim.Math.Polynomial
readmes["IAFahim.Math.Polynomial"] = """# IAFahim.Math.Polynomial

## Description
Implements comprehensive operations on polynomials. Includes addition, subtraction, finding products, quotient and remainder division, derivative, integral, inverse, logarithm, exponent, power, square root, multipoint evaluation, Lagrange interpolation, Taylor shift, composition, and shift operations.

## Complexity
- Add/Sub/Shift: O(N) time.
- KaratsubaMultiply: O(N^1.585) time.
- Div/Mod/Derivative/Integral: O(N * M) or O(N) time.
- Inverse/Log/Exp/Pow/Sqrt: O(N * log(N)) time.
- MultipointEval/Interpolate: O(N * log^2(N)) time.

## API Signature
- public static int PolynomialAdd.Run(int n, long* a, int m, long* b, long* res)
- public static int PolynomialSub.Run(int n, long* a, int m, long* b, long* res)
- public static int PolynomialMul.Run(int n, long* a, int m, long* b, long* res)
- public static int PolynomialDiv.Run(int n, long* a, int m, long* b, long* q, long* r)
- public static int KaratsubaMultiply.Run(int n, long* a, int m, long* b, long* res, long* scratch)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Polynomial;

public unsafe class Example
{
    public static void Main()
    {
        int n = 2;
        int m = 2;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* b = (long*)Marshal.AllocHGlobal(m * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal((n + m) * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 2;
            b[0] = 3; b[1] = 4;
            int degree = PolynomialAdd.Run(n, a, m, b, res);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)b);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```"""

# 14. IAFahim.Math.Polynomial.Eval
readmes["IAFahim.Math.Polynomial.Eval"] = """# IAFahim.Math.Polynomial.Eval

## Description
Provides advanced polynomial evaluation techniques. Includes multi-point evaluation of a polynomial at multiple points, and the Chirp Z-Transform (CZT) for evaluating a polynomial at points in a geometric progression.

## Complexity
- MultiPointEval: O((N + M) * log^2(N)) time, O(N + M) space.
- ChirpZTransform: O((N + M) * log(N + M)) time, O(N + M) space.

## API Signature
- public static void MultiPointEval.Run(int n, long* poly, int m, long* x, long* res, long mod)
- public static int ChirpZTransform.Run(int n, long* a, long c, long d, long* res, long mod)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Polynomial.Eval;

public unsafe class Example
{
    public static void Main()
    {
        int n = 3;
        int m = 2;
        long mod = 998244353;
        long* poly = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* x = (long*)Marshal.AllocHGlobal(m * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal(m * sizeof(long));
        try
        {
            poly[0] = 1; poly[1] = 2; poly[2] = 1;
            x[0] = 2; x[1] = 3;
            MultiPointEval.Run(n, poly, m, x, res, mod);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)poly);
            Marshal.FreeHGlobal((IntPtr)x);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```"""

# 15. IAFahim.Math.Polynomial.Fps
readmes["IAFahim.Math.Polynomial.Fps"] = """# IAFahim.Math.Polynomial.Fps

## Description
Implements formal power series (FPS) operations modulo a prime. Includes computing the formal power series inverse, square root, natural logarithm, exponential, and arbitrary integer power of a formal power series.

## Complexity
- All operations: O(N * log(N)) time, O(N) space.

## API Signature
- public static int FormalPowerSeriesInverse.Run(int n, long* a, long* res, long mod)
- public static int FormalPowerSeriesLog.Run(int n, long* a, long* res, long mod)
- public static int FormalPowerSeriesExp.Run(int n, long* a, long* res, long mod)
- public static int FormalPowerSeriesPow.Run(int n, long* a, long k, long* res, long mod)
- public static int FormalPowerSeriesSqrt.Run(int n, long* a, long* res, long mod)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Math.Polynomial.Fps;

public unsafe class Example
{
    public static void Main()
    {
        int n = 4;
        long mod = 998244353;
        long* a = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        long* res = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            a[0] = 1; a[1] = 2; a[2] = 3; a[3] = 4;
            int len = FormalPowerSeriesInverse.Run(n, a, res, mod);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)a);
            Marshal.FreeHGlobal((IntPtr)res);
        }
    }
}
```"""

# 16. IAFahim.Math.PotentialField
readmes["IAFahim.Math.PotentialField"] = """# IAFahim.Math.PotentialField

## Description
Implements 2D and 3D potential field steering forces for path planning. Includes attractive forces towards targets, repulsive forces away from obstacles, tangential forces (2D only) to bypass obstacles, gradient evaluations, and simple pathfinding using gradient descent.

## Complexity
- Force evaluations: O(K) time where K is the obstacle count, O(1) space.
- GradientDescent: O(steps * K) time, O(1) space.

## API Signature
- public static float2 PotentialField2D.Attractive(float2 position, float2 target, float strength)
- public static float2 PotentialField2D.Repulsive(float2 position, float2 obstacle, float radius, float strength)
- public static float2 PotentialField2D.Tangential(float2 position, float2 obstacle, float radius, float strength)
- public static void PotentialField2D.ComputeGradient(float2 position, float2* attractors, int attractorCount, float attractStrength, float2* repulsors, int repulsorCount, float repulsorRadius, float repulsorStrength, float2* tangentials, int tangentialCount, float tangentialRadius, float tangentialStrength, out float2 gradient)
- public static int PotentialField2D.GradientDescent(float2 start, float2* attractors, int attractorCount, float attractStrength, float2* repulsors, int repulsorCount, float repulsorRadius, float repulsorStrength, float2* tangentials, int tangentialCount, float tangentialRadius, float tangentialStrength, float stepSize, float tolerance, int maxSteps, float2* path)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Math.PotentialField;

public unsafe class Example
{
    public static void Main()
    {
        float2 start = new float2(0.0f, 0.0f);
        float2 target = new float2(10.0f, 10.0f);
        float2 obstacle = new float2(5.0f, 5.0f);

        float2* attractors = (float2*)Marshal.AllocHGlobal(1 * sizeof(float2));
        float2* repulsors = (float2*)Marshal.AllocHGlobal(1 * sizeof(float2));
        float2* tangentials = (float2*)Marshal.AllocHGlobal(1 * sizeof(float2));
        try
        {
            attractors[0] = target;
            repulsors[0] = obstacle;
            tangentials[0] = obstacle;

            float2 gradient;
            PotentialField2D.ComputeGradient(
                start,
                attractors, 1, 1.0f,
                repulsors, 1, 2.0f, 5.0f,
                tangentials, 1, 2.0f, 2.0f,
                out gradient
            );
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)attractors);
            Marshal.FreeHGlobal((IntPtr)repulsors);
            Marshal.FreeHGlobal((IntPtr)tangentials);
        }
    }
}
```"""

# 17. IAFahim.Math.Quaternion
readmes["IAFahim.Math.Quaternion"] = """# IAFahim.Math.Quaternion

## Description
Offers mathematical operations for quaternions. Includes spherical linear interpolation (SLERP), conversions between quaternions and Euler angles or axis-angle representations, look rotation solvers, vector rotation, negating vector parts, normalization, and swing-twist decomposition.

## Complexity
- All operations: O(1) time, O(1) space.

## API Signature
- public static quaternion QuaternionSlerp.Run(quaternion from, quaternion to, float t)
- public static quaternion QuaternionOps.FromAxisAngle(float3 axis, float angleRadians)
- public static void QuaternionOps.ToAxisAngle(quaternion q, out float3 axis, out float angle)
- public static quaternion QuaternionOps.FromEuler(float3 eulerRadians)
- public static float3 QuaternionOps.ToEuler(quaternion q)
- public static quaternion QuaternionOps.LookRotation(float3 forward, float3 up)
- public static float3 QuaternionOps.RotateVector(quaternion q, float3 v)
- public static quaternion QuaternionOps.Conjugate(quaternion q)
- public static float QuaternionOps.Dot(quaternion a, quaternion b)
- public static float QuaternionOps.Length(quaternion q)
- public static quaternion QuaternionOps.Normalize(quaternion q)
- public static float QuaternionOps.AngleBetween(quaternion a, quaternion b)
- public static void SwingTwistDecomposition.Run(quaternion q, float3 twistAxis, out quaternion swing, out quaternion twist)
- public static float SwingTwistDecomposition.TwistAngle(quaternion q, float3 twistAxis)
- public static quaternion SwingTwistDecomposition.FromTwistAngle(float angle, float3 twistAxis)

## Usage Example
```csharp
using Unity.Mathematics;
using IAFahim.Math.Quaternion;

public unsafe class Example
{
    public static void Main()
    {
        quaternion q1 = quaternion.identity;
        float3 axis = new float3(0.0f, 1.0f, 0.0f);
        quaternion q2 = QuaternionOps.FromAxisAngle(axis, 1.57f);
        quaternion result = QuaternionSlerp.Run(q1, q2, 0.5f);
    }
}
```"""

# 18. IAFahim.Math.Sdf
readmes["IAFahim.Math.Sdf"] = """# IAFahim.Math.Sdf

## Description
Implements signed distance function (SDF) utilities for 3D computer graphics. Includes primitive shape evaluations, constructive solid geometry (CSG) boolean operations, space transforms, raymarching solvers, normal estimation, and ambient occlusion.

## Complexity
- Primitive evaluations/Booleans/Transforms/Normal estimation: O(1) time, O(1) space.
- March: O(maxSteps) time, O(1) space.
- AmbientOcclusion: O(steps) time, O(1) space.

## API Signature
- public delegate float SdfRayMarch.SdfFunction(float3 p)
- public static float SdfPrimitive.Sphere(float3 p, float radius)
- public static float SdfPrimitive.Box(float3 p, float3 halfExtents)
- public static float SdfBoolean.Union(float d1, float d2)
- public static float3 SdfRayMarch.EstimateNormal(SdfFunction sdf, float3 p)
- public static bool SdfRayMarch.March(SdfFunction sdf, float3 origin, float3 direction, float maxDistance, int maxSteps, out float t, out float3 hitPoint)

## Usage Example
```csharp
using System;
using Unity.Mathematics;
using IAFahim.Math.Sdf;

public unsafe class Example
{
    private static float SphereSdf(float3 p)
    {
        return SdfPrimitive.Sphere(p, 1.0f);
    }

    public static void Main()
    {
        float3 origin = new float3(0.0f, 0.0f, -5.0f);
        float3 dir = new float3(0.0f, 0.0f, 1.0f);
        float t;
        float3 hit;
        bool didHit = SdfRayMarch.March(SphereSdf, origin, dir, 10.0f, 64, out t, out hit);
    }
}
```"""

# 19. IAFahim.Math.SphericalHarmonics
readmes["IAFahim.Math.SphericalHarmonics"] = """# IAFahim.Math.SphericalHarmonics

## Description
Implements Spherical Harmonics projection and evaluation up to band 2 (9 coefficients). Provides functions for basis function evaluation, projection of directional samples, irradiance convolution, and reconstruction.

## Complexity
- Basis / EvaluateL2 / EvalL2 / Convolve: O(1) time, O(1) space.
- ProjectL2: O(sampleCount) time, O(1) space.

## API Signature
- public static float SHEvaluation.BasisL0M0()
- public static void SHEvaluation.EvaluateL2(float3 direction, float* outCoeffs)
- public static void SHEvaluation.ProjectL2(float3* directions, float* values, int sampleCount, float* outCoeffs)
- public static float SHEvaluation.EvalL2(float3 direction, float* coeffs)
- public static void SHEvaluation.ConvolveWithCosineKernelL2(float* irradianceCoeffs, float* radianceCoeffs)

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using IAFahim.Math.SphericalHarmonics;

public unsafe class Example
{
    public static void Main()
    {
        float3 dir = new float3(0.0f, 1.0f, 0.0f);
        float* coeffs = (float*)Marshal.AllocHGlobal(9 * sizeof(float));
        try
        {
            SHEvaluation.EvaluateL2(dir, coeffs);
            float val = SHEvaluation.EvalL2(dir, coeffs);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)coeffs);
        }
    }
}
```"""

# Validate all readmes
all_valid = True
for pkg, text in readmes.items():
    if not validate_readme(pkg, text):
        all_valid = False

if all_valid:
    print("All READMEs are strictly valid!")
    with open('outputs.json', 'w', encoding='utf-8') as f:
        json.dump(readmes, f, indent=2, ensure_ascii=False)
    print("Outputs written to outputs.json.")
else:
    print("Validation failed. Fix errors above.")
