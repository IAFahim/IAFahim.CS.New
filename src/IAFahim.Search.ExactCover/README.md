# IAFahim.Search.ExactCover

## Description
This package solves exact cover problems using dancing links and back-tracking, including grid placement games and queen puzzle counts.

## Complexity
Time complexity is exponential in the worst case but highly optimized via dancing links. Space complexity is O(Rows * Cols) for grid representations.

## API Signature
```csharp
namespace IAFahim.Search
{
    public static unsafe class ExactCover
    {
        public static bool SolveDlx(int* matrix, int rows, int cols, int* solution, int* solutionSize, int* L, int* R, int* U, int* D, int* C, int* RowIdx, int* colSize);
        public static bool SolveSudokuDlx(int* sudoku, int* L, int* R_dlx, int* U, int* D, int* C, int* RowIdx, int* colSize);
    }
}
```

## Usage Example
```csharp
using System;
using System.Runtime.InteropServices;
using IAFahim.Search;

public static unsafe class Program
{
    public static void Main()
    {
        int size = 81;
        int* sudoku = (int*)Marshal.AllocHGlobal(size * sizeof(int));
        int* temp = (int*)Marshal.AllocHGlobal(400 * sizeof(int));
        try
        {
            int i = 0;
            while (i < size)
            {
                sudoku[i] = 0;
                i = i + 1;
            }
            ExactCover.SolveSudokuDlx(sudoku, temp, temp, temp, temp, temp, temp, temp);
        }
        finally
        {
            Marshal.FreeHGlobal((IntPtr)sudoku);
            Marshal.FreeHGlobal((IntPtr)temp);
        }
    }
}
```