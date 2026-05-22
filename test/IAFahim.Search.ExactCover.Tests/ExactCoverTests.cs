namespace IAFahim.Search.ExactCover.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.Search;

    public sealed unsafe class ExactCoverTests
    {
        private static bool IsValidSudoku(int* grid)
        {
            for (int i = 0; i < 9; i++)
            {
                int rowMask = 0;
                int colMask = 0;
                int boxMask = 0;
                for (int j = 0; j < 9; j++)
                {
                    int valRow = grid[i * 9 + j];
                    if (valRow < 1 || valRow > 9 || (rowMask & (1 << valRow)) != 0)
                    {
                        return false;
                    }
                    rowMask |= (1 << valRow);

                    int valCol = grid[j * 9 + i];
                    if (valCol < 1 || valCol > 9 || (colMask & (1 << valCol)) != 0)
                    {
                        return false;
                    }
                    colMask |= (1 << valCol);

                    int br = (i / 3) * 3 + j / 3;
                    int bc = (i % 3) * 3 + j % 3;
                    int valBox = grid[br * 9 + bc];
                    if (valBox < 1 || valBox > 9 || (boxMask & (1 << valBox)) != 0)
                    {
                        return false;
                    }
                    boxMask |= (1 << valBox);
                }
            }
            return true;
        }

        [Fact]
        public void SolveDlx_SimpleMatrix_FindsSolution()
        {
            const int Rows = 4;
            const int Cols = 4;
            int* matrix = (int*)Marshal.AllocHGlobal(Rows * Cols * sizeof(int));
            int* solution = (int*)Marshal.AllocHGlobal(Rows * sizeof(int));
            int solutionSize = 0;
            try
            {
                matrix[0 * Cols + 0] = 1; matrix[0 * Cols + 1] = 0; matrix[0 * Cols + 2] = 1; matrix[0 * Cols + 3] = 0;
                matrix[1 * Cols + 0] = 1; matrix[1 * Cols + 1] = 0; matrix[1 * Cols + 2] = 0; matrix[1 * Cols + 3] = 1;
                matrix[2 * Cols + 0] = 0; matrix[2 * Cols + 1] = 1; matrix[2 * Cols + 2] = 0; matrix[2 * Cols + 3] = 1;
                matrix[3 * Cols + 0] = 0; matrix[3 * Cols + 1] = 1; matrix[3 * Cols + 2] = 1; matrix[3 * Cols + 3] = 0;

                bool solved = ExactCover.SolveDlx(matrix, Rows, Cols, solution, &solutionSize);
                Assert.True(solved);
                Assert.Equal(2, solutionSize);

                bool hasRow0 = false;
                bool hasRow2 = false;
                bool hasRow1 = false;
                bool hasRow3 = false;
                for (int i = 0; i < solutionSize; i++)
                {
                    if (solution[i] == 0) hasRow0 = true;
                    if (solution[i] == 2) hasRow2 = true;
                    if (solution[i] == 1) hasRow1 = true;
                    if (solution[i] == 3) hasRow3 = true;
                }
                Assert.True((hasRow0 && hasRow2) || (hasRow1 && hasRow3));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)matrix);
                Marshal.FreeHGlobal((nint)solution);
            }
        }

        [Fact]
        public void SolveSudokuDlx_SimpleSudoku_FindsSolution()
        {
            int* sudoku = (int*)Marshal.AllocHGlobal(81 * sizeof(int));
            try
            {
                int[] initial = new int[81]
                {
                    5, 3, 0, 0, 7, 0, 0, 0, 0,
                    6, 0, 0, 1, 9, 5, 0, 0, 0,
                    0, 9, 8, 0, 0, 0, 0, 6, 0,
                    8, 0, 0, 0, 6, 0, 0, 0, 3,
                    4, 0, 0, 8, 0, 3, 0, 0, 1,
                    7, 0, 0, 0, 2, 0, 0, 0, 6,
                    0, 6, 0, 0, 0, 0, 2, 8, 0,
                    0, 0, 0, 4, 1, 9, 0, 0, 5,
                    0, 0, 0, 0, 8, 0, 0, 7, 9
                };
                for (int i = 0; i < 81; i++)
                {
                    sudoku[i] = initial[i];
                }

                bool solved = ExactCover.SolveSudokuDlx(sudoku);
                Assert.True(solved);
                Assert.True(IsValidSudoku(sudoku));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sudoku);
            }
        }

        [Fact]
        public void SudokuBitmaskSolve_SimpleSudoku_FindsSolution()
        {
            int* sudoku = (int*)Marshal.AllocHGlobal(81 * sizeof(int));
            try
            {
                int[] initial = new int[81]
                {
                    5, 3, 0, 0, 7, 0, 0, 0, 0,
                    6, 0, 0, 1, 9, 5, 0, 0, 0,
                    0, 9, 8, 0, 0, 0, 0, 6, 0,
                    8, 0, 0, 0, 6, 0, 0, 0, 3,
                    4, 0, 0, 8, 0, 3, 0, 0, 1,
                    7, 0, 0, 0, 2, 0, 0, 0, 6,
                    0, 6, 0, 0, 0, 0, 2, 8, 0,
                    0, 0, 0, 4, 1, 9, 0, 0, 5,
                    0, 0, 0, 0, 8, 0, 0, 7, 9
                };
                for (int i = 0; i < 81; i++)
                {
                    sudoku[i] = initial[i];
                }

                bool solved = ExactCover.SudokuBitmaskSolve(sudoku);
                Assert.True(solved);
                Assert.True(IsValidSudoku(sudoku));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sudoku);
            }
        }

        [Fact]
        public void KillerSudokuSolve_SimpleKillerSudoku_FindsSolution()
        {
            int* sudoku = (int*)Marshal.AllocHGlobal(81 * sizeof(int));
            int* cageSums = (int*)Marshal.AllocHGlobal(9 * sizeof(int));
            int* cageIds = (int*)Marshal.AllocHGlobal(81 * sizeof(int));
            try
            {
                for (int i = 0; i < 81; i++)
                {
                    sudoku[i] = 0;
                    cageIds[i] = i / 9; // 9 cages, each is a row
                }
                for (int i = 0; i < 9; i++)
                {
                    cageSums[i] = 45; // Sum of numbers 1-9 is 45
                }

                bool solved = ExactCover.KillerSudokuSolve(sudoku, cageSums, cageIds, 9);
                Assert.True(solved);
                Assert.True(IsValidSudoku(sudoku));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)sudoku);
                Marshal.FreeHGlobal((nint)cageSums);
                Marshal.FreeHGlobal((nint)cageIds);
            }
        }

        [Fact]
        public void KenKenSolve_SimpleKenKen_FindsSolution()
        {
            const int N = 3;
            int* grid = (int*)Marshal.AllocHGlobal(N * N * sizeof(int));
            int* cageTargets = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* cageOps = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* cageIds = (int*)Marshal.AllocHGlobal(N * N * sizeof(int));
            try
            {
                for (int i = 0; i < N * N; i++)
                {
                    grid[i] = 0;
                }
                int[] ids = new int[9]
                {
                    0, 0, 1,
                    2, 2, 1,
                    3, 3, 3
                };
                int[] targets = new int[4] { 3, 3, 5, 6 };
                int[] ops = new int[4] { 0, 1, 0, 1 }; // 0: +, 1: *

                for (int i = 0; i < 9; i++)
                {
                    cageIds[i] = ids[i];
                }
                for (int i = 0; i < 4; i++)
                {
                    cageTargets[i] = targets[i];
                    cageOps[i] = ops[i];
                }

                bool solved = ExactCover.KenKenSolve(grid, N, cageTargets, cageOps, cageIds, 4);
                Assert.True(solved);

                for (int r = 0; r < N; r++)
                {
                    bool[] rowSeen = new bool[N + 1];
                    bool[] colSeen = new bool[N + 1];
                    for (int c = 0; c < N; c++)
                    {
                        int valRow = grid[r * N + c];
                        int valCol = grid[c * N + r];
                        Assert.True(valRow >= 1 && valRow <= N);
                        Assert.True(valCol >= 1 && valCol <= N);
                        Assert.False(rowSeen[valRow]);
                        Assert.False(colSeen[valCol]);
                        rowSeen[valRow] = true;
                        colSeen[valCol] = true;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)grid);
                Marshal.FreeHGlobal((nint)cageTargets);
                Marshal.FreeHGlobal((nint)cageOps);
                Marshal.FreeHGlobal((nint)cageIds);
            }
        }

        [Fact]
        public void SolvePolyominoTiling_SimpleTiling_FindsSolution()
        {
            const int W = 2;
            const int H = 2;
            int* grid = (int*)Marshal.AllocHGlobal(W * H * sizeof(int));
            int* variantPieceId = (int*)Marshal.AllocHGlobal(2 * sizeof(int));
            int* variantOffsets = (int*)Marshal.AllocHGlobal(2 * sizeof(int));
            int* variantLengths = (int*)Marshal.AllocHGlobal(2 * sizeof(int));
            int* variantX = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* variantY = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            try
            {
                variantPieceId[0] = 0;
                variantOffsets[0] = 0;
                variantLengths[0] = 2;

                variantPieceId[1] = 1;
                variantOffsets[1] = 2;
                variantLengths[1] = 2;

                variantX[0] = 0; variantY[0] = 0;
                variantX[1] = 1; variantY[1] = 0;

                variantX[2] = 0; variantY[2] = 0;
                variantX[3] = 1; variantY[3] = 0;

                bool solved = ExactCover.SolvePolyominoTiling(W, H, 2, 2, variantPieceId, variantOffsets, variantLengths, variantX, variantY, grid);
                Assert.True(solved);
                Assert.Equal(0, grid[0]);
                Assert.Equal(0, grid[1]);
                Assert.Equal(1, grid[2]);
                Assert.Equal(1, grid[3]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)grid);
                Marshal.FreeHGlobal((nint)variantPieceId);
                Marshal.FreeHGlobal((nint)variantOffsets);
                Marshal.FreeHGlobal((nint)variantLengths);
                Marshal.FreeHGlobal((nint)variantX);
                Marshal.FreeHGlobal((nint)variantY);
            }
        }

        [Fact]
        public void NQueens_CountAndSymmetry_Correct()
        {
            int* solution = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            try
            {
                bool solved = ExactCover.NQueensBitmask(4, solution);
                Assert.True(solved);

                int count4 = ExactCover.NQueensCount(4);
                Assert.Equal(2, count4);

                int count8 = ExactCover.NQueensCount(8);
                Assert.Equal(92, count8);

                int symCount4 = ExactCover.NQueensSymmetry(4);
                Assert.Equal(1, symCount4);

                int symCount8 = ExactCover.NQueensSymmetry(8);
                Assert.Equal(12, symCount8);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)solution);
            }
        }

        [Fact]
        public void MagicSquareSolve_Size3_FindsSolution()
        {
            const int N = 3;
            int* grid = (int*)Marshal.AllocHGlobal(N * N * sizeof(int));
            try
            {
                for (int i = 0; i < N * N; i++) grid[i] = 0;
                bool solved = ExactCover.MagicSquareSolve(N, grid);
                Assert.True(solved);

                int target = N * (N * N + 1) / 2;
                for (int r = 0; r < N; r++)
                {
                    int rowSum = 0;
                    int colSum = 0;
                    for (int c = 0; c < N; c++)
                    {
                        rowSum += grid[r * N + c];
                        colSum += grid[c * N + r];
                    }
                    Assert.Equal(target, rowSum);
                    Assert.Equal(target, colSum);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)grid);
            }
        }

        [Fact]
        public void CryptarithmSolve_SendMoreMoney_FindsSolution()
        {
            const int NumLetters = 8;
            const int NumWords = 2;
            int* wordLengths = (int*)Marshal.AllocHGlobal(NumWords * sizeof(int));
            int* wordLetters = (int*)Marshal.AllocHGlobal(8 * sizeof(int));
            int* wordOffsets = (int*)Marshal.AllocHGlobal(NumWords * sizeof(int));
            const int TargetLength = 5;
            int* targetLetters = (int*)Marshal.AllocHGlobal(TargetLength * sizeof(int));
            bool* isLeading = (bool*)Marshal.AllocHGlobal(NumLetters * sizeof(bool));
            int* resultDigits = (int*)Marshal.AllocHGlobal(NumLetters * sizeof(int));
            try
            {
                wordLengths[0] = 4;
                wordOffsets[0] = 0;
                wordLetters[0] = 0; // S
                wordLetters[1] = 1; // E
                wordLetters[2] = 2; // N
                wordLetters[3] = 3; // D

                wordLengths[1] = 4;
                wordOffsets[1] = 4;
                wordLetters[4] = 4; // M
                wordLetters[5] = 5; // O
                wordLetters[6] = 6; // R
                wordLetters[7] = 1; // E

                targetLetters[0] = 4; // M
                targetLetters[1] = 5; // O
                targetLetters[2] = 2; // N
                targetLetters[3] = 1; // E
                targetLetters[4] = 7; // Y

                for (int i = 0; i < NumLetters; i++)
                {
                    isLeading[i] = (i == 0 || i == 4);
                }

                bool solved = ExactCover.CryptarithmSolve(NumLetters, NumWords, wordLengths, wordLetters, wordOffsets, TargetLength, targetLetters, isLeading, resultDigits);
                Assert.True(solved);
                Assert.Equal(9, resultDigits[0]); // S = 9
                Assert.Equal(5, resultDigits[1]); // E = 5
                Assert.Equal(6, resultDigits[2]); // N = 6
                Assert.Equal(7, resultDigits[3]); // D = 7
                Assert.Equal(1, resultDigits[4]); // M = 1
                Assert.Equal(0, resultDigits[5]); // O = 0
                Assert.Equal(8, resultDigits[6]); // R = 8
                Assert.Equal(2, resultDigits[7]); // Y = 2
            }
            finally
            {
                Marshal.FreeHGlobal((nint)wordLengths);
                Marshal.FreeHGlobal((nint)wordLetters);
                Marshal.FreeHGlobal((nint)wordOffsets);
                Marshal.FreeHGlobal((nint)targetLetters);
                Marshal.FreeHGlobal((nint)isLeading);
                Marshal.FreeHGlobal((nint)resultDigits);
            }
        }

        [Fact]
        public void ConstraintPropagationAndAc3_SimpleCsp_FindsSolution()
        {
            const int NumVars = 2;
            const int MaxDomain = 3;
            int* domainSizes = (int*)Marshal.AllocHGlobal(NumVars * sizeof(int));
            bool* domains = (bool*)Marshal.AllocHGlobal(NumVars * MaxDomain * sizeof(bool));
            const int NumConstraints = 1;
            int* constVar1 = (int*)Marshal.AllocHGlobal(NumConstraints * sizeof(int));
            int* constVar2 = (int*)Marshal.AllocHGlobal(NumConstraints * sizeof(int));
            bool* relations = (bool*)Marshal.AllocHGlobal(NumConstraints * MaxDomain * MaxDomain * sizeof(bool));
            int* assignment = (int*)Marshal.AllocHGlobal(NumVars * sizeof(int));
            try
            {
                domainSizes[0] = 3;
                domainSizes[1] = 3;
                for (int i = 0; i < NumVars * MaxDomain; i++) domains[i] = true;

                constVar1[0] = 0;
                constVar2[0] = 1;

                for (int u = 0; u < 3; u++)
                {
                    for (int v = 0; v < 3; v++)
                    {
                        relations[0 * MaxDomain * MaxDomain + u * MaxDomain + v] = (u < v);
                    }
                }

                bool ac3Result = ExactCover.ArcConsistencyAc3(NumVars, domainSizes, domains, MaxDomain, NumConstraints, constVar1, constVar2, relations);
                Assert.True(ac3Result);
                Assert.False(domains[0 * MaxDomain + 2]);
                Assert.False(domains[1 * MaxDomain + 0]);

                bool cspResult = ExactCover.ConstraintPropagationSolve(NumVars, domainSizes, domains, MaxDomain, NumConstraints, constVar1, constVar2, relations, assignment);
                Assert.True(cspResult);
                Assert.True(assignment[0] < assignment[1]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)domainSizes);
                Marshal.FreeHGlobal((nint)domains);
                Marshal.FreeHGlobal((nint)constVar1);
                Marshal.FreeHGlobal((nint)constVar2);
                Marshal.FreeHGlobal((nint)relations);
                Marshal.FreeHGlobal((nint)assignment);
            }
        }
    }
}
