# IAFahim.Optimization.Approximation

## Description
This package implements metaheuristic search methods (simulated annealing, hill climbing, Monte Carlo), Freivalds probabilistic checking of matrix products, and randomized polynomial identity testing.

## Complexity
Simulated annealing, hill climbing, and Monte Carlo run for a configured number of steps. Freivalds checks matrix products in O(iters * N^2) steps. Polynomial identity testing runs in O(N) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Approximation
{
    public static unsafe class Metheuristics
    {
        public static long SimulatedAnnealing(long* state, int n, long target, double temp, double cooling);
        public static long HillClimb(long* state, int n);
        public static long MonteCarlo(long* samples, int n);
    }

    public static unsafe class Freivalds
    {
        public static bool Verify(int n, int* a, int* b, int* c, int* r, int iters, uint* seed);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Approximation;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        long* state = (long*)Marshal.AllocHGlobal(n * sizeof(long));
        try
        {
            state[0] = 10;
            state[1] = 20;
            state[2] = 30;
            state[3] = 40;
            long result = Metheuristics.HillClimb(state, n);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)state);
        }
    }
}
```