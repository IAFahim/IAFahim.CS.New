# IAFahim.Math.Polynomial.Fps

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
```