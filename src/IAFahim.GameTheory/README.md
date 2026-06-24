# IAFahim.GameTheory

## Description
A collection of game theory algorithms. Includes Grundy value derivation on directed graphs, Nim sum solvers, minimax search with alpha-beta pruning, and game dynamic programming utilities.

## Complexity
O(V + E) for Grundy derivations on DAGs, O(N) for Nim sums, and O(B^D) for Minimax where B is branching factor and D is search depth.

## API Signature
```csharp
public static unsafe class GrundyDAG
{
    public static int Run(int n, int* to, int* grundy, int* indeg, int* queue)
}
public static unsafe class NimSum
{
    public static long Run(int n, long* piles)
}
public static unsafe class Minimax
{
    public static long Run(int depth, bool isMax, long alpha, long beta, long* gameState, int player)
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.GameTheory;

public static unsafe class Example
{
    public static void Run()
    {
        long* piles = (long*)Marshal.AllocHGlobal(5 * sizeof(long));
        try
        {
            piles[0] = 3;
            piles[1] = 4;
            piles[2] = 5;
            long nim = NimSum.Run(3, piles);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)piles);
        }
    }
}
```