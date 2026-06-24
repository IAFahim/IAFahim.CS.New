# IAFahim.Optimization.Games

## Description
This package provides game theory and decision process solvers. It includes finding attractor sets for infinite games, minimum cost flow (flow loops, arborescence, mean cycle), Grundy values for impartial games, the Simplex algorithm for linear programming, Markov Decision Processes value and policy iterations, retrograde analysis for game solving, and mean payoff game solvers.

## Complexity
Attractor set finding runs in O(N + M) steps. Simplex runs in exponential time in the worst case but is fast in practice. Markov Decision Process iterations run for a specified iteration limit. Retrograde analysis runs in O(N + M) steps.

## API Signature
```csharp
namespace IAFahim.Optimization.Games
{
    public static unsafe class Simplex
    {
        public struct Result
        {
            public double Value;
            public int Status;
        }
        public static Result Run(int m, int n, double* a, double* b, double* c, double* x);
    }

    public static unsafe class Retrograde
    {
        public static int Solve(int n, bool* win, bool* lose, int* from, int* to, int m);
    }
}
```

## Usage Example
```csharp
using System.Runtime.InteropServices;
using IAFahim.Optimization.Games;

public unsafe class Example
{
    public static void Run()
    {
        int n = 4;
        int m = 4;
        bool* win = (bool*)Marshal.AllocHGlobal(n * sizeof(bool));
        bool* lose = (bool*)Marshal.AllocHGlobal(n * sizeof(bool));
        int* from = (int*)Marshal.AllocHGlobal(m * sizeof(int));
        int* to = (int*)Marshal.AllocHGlobal(m * sizeof(int));
        try
        {
            win[0] = false;
            lose[0] = false;
            from[0] = 0;
            to[0] = 1;
            int steps = Retrograde.Solve(n, win, lose, from, to, m);
        }
        finally
        {
            Marshal.FreeHGlobal((nint)win);
            Marshal.FreeHGlobal((nint)lose);
            Marshal.FreeHGlobal((nint)from);
            Marshal.FreeHGlobal((nint)to);
        }
    }
}
```