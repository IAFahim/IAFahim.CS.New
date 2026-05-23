namespace IAFahim.Search
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class ExactCover
    {
        public static bool SolveDlx(int* matrix, int rows, int cols, int* solution, int* solutionSize, int* L, int* R, int* U, int* D, int* C, int* RowIdx, int* colSize)
        {
            int maxNodes = rows * cols + cols + 1;
            int nodeCount = cols + 1;
            for (int c = 0; c <= cols; c++)
            {
                L[c] = c - 1;
                R[c] = c + 1;
                U[c] = c;
                D[c] = c;
                colSize[c] = 0;
            }
            L[0] = cols;
            R[cols] = 0;

            for (int r = 0; r < rows; r++)
            {
                int firstInRow = -1;
                for (int c = 0; c < cols; c++)
                {
                    if (matrix[r * cols + c] == 1)
                    {
                        int colNode = c + 1;
                        int currNode = nodeCount++;
                        C[currNode] = colNode;
                        RowIdx[currNode] = r;

                        U[currNode] = U[colNode];
                        D[currNode] = colNode;
                        D[U[colNode]] = currNode;
                        U[colNode] = currNode;
                        colSize[colNode]++;

                        if (firstInRow == -1)
                        {
                            firstInRow = currNode;
                            L[currNode] = currNode;
                            R[currNode] = currNode;
                        }
                        else
                        {
                            L[currNode] = L[firstInRow];
                            R[currNode] = firstInRow;
                            R[L[firstInRow]] = currNode;
                            L[firstInRow] = currNode;
                        }
                    }
                }
            }
            *solutionSize = 0;
            return SearchDlx(0, L, R, U, D, C, RowIdx, colSize, solution, solutionSize);
        }

        private static bool SearchDlx(int k, int* L, int* R, int* U, int* D, int* C, int* RowIdx, int* colSize, int* solution, int* solutionSize)
        {
            if (R[0] == 0)
            {
                *solutionSize = k;
                return true;
            }

            int col = R[0];
            for (int c = R[0]; c != 0; c = R[c])
            {
                if (colSize[c] < colSize[col])
                {
                    col = c;
                }
            }

            if (colSize[col] == 0)
            {
                return false;
            }

            Cover(col, L, R, U, D, C, colSize);
            for (int rowNode = D[col]; rowNode != col; rowNode = D[rowNode])
            {
                solution[k] = RowIdx[rowNode];
                for (int rightNode = R[rowNode]; rightNode != rowNode; rightNode = R[rightNode])
                {
                    Cover(C[rightNode], L, R, U, D, C, colSize);
                }

                if (SearchDlx(k + 1, L, R, U, D, C, RowIdx, colSize, solution, solutionSize))
                {
                    return true;
                }

                for (int leftNode = L[rowNode]; leftNode != rowNode; leftNode = L[leftNode])
                {
                    Uncover(C[leftNode], L, R, U, D, C, colSize);
                }
            }
            Uncover(col, L, R, U, D, C, colSize);
            return false;
        }

        private static void Cover(int c, int* L, int* R, int* U, int* D, int* C, int* colSize)
        {
            L[R[c]] = L[c];
            R[L[c]] = R[c];
            for (int i = D[c]; i != c; i = D[i])
            {
                for (int j = R[i]; j != i; j = R[j])
                {
                    U[D[j]] = U[j];
                    D[U[j]] = D[j];
                    colSize[C[j]]--;
                }
            }
        }

        private static void Uncover(int c, int* L, int* R, int* U, int* D, int* C, int* colSize)
        {
            for (int i = U[c]; i != c; i = U[i])
            {
                for (int j = L[i]; j != i; j = L[j])
                {
                    colSize[C[j]]++;
                    D[U[j]] = j;
                    U[D[j]] = j;
                }
            }
            L[R[c]] = c;
            R[L[c]] = c;
        }

        public static bool SolveSudokuDlx(int* sudoku, int* L, int* R_dlx, int* U, int* D, int* C, int* RowIdx, int* colSize)
        {
            int* matrix = stackalloc int[729 * 324];
            int* rowMap = stackalloc int[729];
            for (int i = 0; i < 729 * 324; i++) matrix[i] = 0;

            int rowCount = 0;
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    int val = sudoku[r * 9 + c];
                    if (val != 0)
                    {
                        int v = val - 1;
                        rowMap[rowCount] = r * 81 + c * 9 + v;
                        matrix[rowCount * 324 + (r * 9 + c)] = 1;
                        matrix[rowCount * 324 + (81 + r * 9 + v)] = 1;
                        matrix[rowCount * 324 + (162 + c * 9 + v)] = 1;
                        matrix[rowCount * 324 + (243 + ((r / 3) * 3 + c / 3) * 9 + v)] = 1;
                        rowCount++;
                    }
                    else
                    {
                        for (int v = 0; v < 9; v++)
                        {
                            rowMap[rowCount] = r * 81 + c * 9 + v;
                            matrix[rowCount * 324 + (r * 9 + c)] = 1;
                            matrix[rowCount * 324 + (81 + r * 9 + v)] = 1;
                            matrix[rowCount * 324 + (162 + c * 9 + v)] = 1;
                            matrix[rowCount * 324 + (243 + ((r / 3) * 3 + c / 3) * 9 + v)] = 1;
                            rowCount++;
                        }
                    }
                }
            }

            int* solution = stackalloc int[729];
            int solutionSize = 0;
            bool solved = SolveDlx(matrix, rowCount, 324, solution, &solutionSize, L, R_dlx, U, D, C, RowIdx, colSize);
            if (solved)
            {
                for (int i = 0; i < solutionSize; i++)
                {
                    int rowIdx = rowMap[solution[i]];
                    int v = rowIdx % 9;
                    int c = (rowIdx / 9) % 9;
                    int r = rowIdx / 81;
                    sudoku[r * 9 + c] = v + 1;
                }
                return true;
            }
            return false;
        }

        public static bool SudokuBitmaskSolve(int* grid)
        {
            int* rowMask = stackalloc int[9];
            int* colMask = stackalloc int[9];
            int* boxMask = stackalloc int[9];
            for (int i = 0; i < 9; i++)
            {
                rowMask[i] = 0;
                colMask[i] = 0;
                boxMask[i] = 0;
            }
            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    if (grid[r * 9 + c] != 0)
                    {
                        int val = grid[r * 9 + c];
                        int mask = 1 << val;
                        rowMask[r] |= mask;
                        colMask[c] |= mask;
                        boxMask[(r / 3) * 3 + c / 3] |= mask;
                    }
                }
            }
            return SudokuBitmaskBacktrack(0, grid, rowMask, colMask, boxMask);
        }

        private static bool SudokuBitmaskBacktrack(int cell, int* grid, int* rowMask, int* colMask, int* boxMask)
        {
            if (cell == 81) return true;
            int r = cell / 9;
            int c = cell % 9;
            if (grid[cell] != 0)
            {
                return SudokuBitmaskBacktrack(cell + 1, grid, rowMask, colMask, boxMask);
            }
            int box = (r / 3) * 3 + c / 3;
            int allowed = ~(rowMask[r] | colMask[c] | boxMask[box]) & 0x3FE;
            while (allowed != 0)
            {
                int lsb = allowed & -allowed;
                allowed ^= lsb;
                int val = 0;
                int temp = lsb;
                while (temp > 1) { temp >>= 1; val++; }
                grid[cell] = val;
                rowMask[r] |= lsb;
                colMask[c] |= lsb;
                boxMask[box] |= lsb;
                if (SudokuBitmaskBacktrack(cell + 1, grid, rowMask, colMask, boxMask)) return true;
                grid[cell] = 0;
                rowMask[r] ^= lsb;
                colMask[c] ^= lsb;
                boxMask[box] ^= lsb;
            }
            return false;
        }

        public static bool KillerSudokuSolve(int* grid, int* cageSums, int* cageIds, int numCages)
        {
            int* rowMask = stackalloc int[9];
            int* colMask = stackalloc int[9];
            int* boxMask = stackalloc int[9];
            int* cageCurrentSums = stackalloc int[numCages];
            int* cageCellsRemaining = stackalloc int[numCages];
            int* cageUsedMask = stackalloc int[numCages];
            for (int i = 0; i < 9; i++) { rowMask[i] = colMask[i] = boxMask[i] = 0; }
            for (int i = 0; i < numCages; i++) { cageCurrentSums[i] = cageUsedMask[i] = cageCellsRemaining[i] = 0; }

            for (int r = 0; r < 9; r++)
            {
                for (int c = 0; c < 9; c++)
                {
                    int cageId = cageIds[r * 9 + c];
                    cageCellsRemaining[cageId]++;
                    if (grid[r * 9 + c] != 0)
                    {
                        int val = grid[r * 9 + c];
                        int mask = 1 << val;
                        rowMask[r] |= mask;
                        colMask[c] |= mask;
                        boxMask[(r / 3) * 3 + c / 3] |= mask;
                        cageCurrentSums[cageId] += val;
                        cageUsedMask[cageId] |= mask;
                    }
                }
            }

            return KillerBacktrack(0, grid, rowMask, colMask, boxMask, cageSums, cageIds, cageCurrentSums, cageCellsRemaining, cageUsedMask);
        }

        private static bool KillerBacktrack(int cell, int* grid, int* rowMask, int* colMask, int* boxMask, int* cageSums, int* cageIds, int* cageCurrentSums, int* cageCellsRemaining, int* cageUsedMask)
        {
            if (cell == 81) return true;
            int r = cell / 9;
            int c = cell % 9;
            if (grid[cell] != 0)
            {
                return KillerBacktrack(cell + 1, grid, rowMask, colMask, boxMask, cageSums, cageIds, cageCurrentSums, cageCellsRemaining, cageUsedMask);
            }
            int box = (r / 3) * 3 + c / 3;
            int cageId = cageIds[cell];
            int targetSum = cageSums[cageId];

            int allowed = ~(rowMask[r] | colMask[c] | boxMask[box] | cageUsedMask[cageId]) & 0x3FE;
            while (allowed != 0)
            {
                int lsb = allowed & -allowed;
                allowed ^= lsb;
                int val = 0;
                int temp = lsb;
                while (temp > 1) { temp >>= 1; val++; }

                if (cageCurrentSums[cageId] + val > targetSum) continue;
                if (cageCellsRemaining[cageId] == 1 && cageCurrentSums[cageId] + val != targetSum) continue;

                grid[cell] = val;
                rowMask[r] |= lsb;
                colMask[c] |= lsb;
                boxMask[box] |= lsb;
                cageCurrentSums[cageId] += val;
                cageCellsRemaining[cageId]--;
                cageUsedMask[cageId] |= lsb;

                if (KillerBacktrack(cell + 1, grid, rowMask, colMask, boxMask, cageSums, cageIds, cageCurrentSums, cageCellsRemaining, cageUsedMask))
                    return true;

                grid[cell] = 0;
                rowMask[r] ^= lsb;
                colMask[c] ^= lsb;
                boxMask[box] ^= lsb;
                cageCurrentSums[cageId] -= val;
                cageCellsRemaining[cageId]++;
                cageUsedMask[cageId] ^= lsb;
            }
            return false;
        }

        public static bool KenKenSolve(int* grid, int n, int* cageTargets, int* cageOps, int* cageIds, int numCages)
        {
            int* rowMask = stackalloc int[n];
            int* colMask = stackalloc int[n];
            int* cageCellsRemaining = stackalloc int[numCages];
            int* cageVals = stackalloc int[numCages * n];
            int* cageValCounts = stackalloc int[numCages];
            for (int i = 0; i < n; i++) { rowMask[i] = colMask[i] = 0; }
            for (int i = 0; i < numCages; i++) { cageCellsRemaining[i] = cageValCounts[i] = 0; }

            for (int r = 0; r < n; r++)
            {
                for (int c = 0; c < n; c++)
                {
                    int cageId = cageIds[r * n + c];
                    cageCellsRemaining[cageId]++;
                    if (grid[r * n + c] != 0)
                    {
                        int val = grid[r * n + c];
                        rowMask[r] |= (1 << val);
                        colMask[c] |= (1 << val);
                        cageVals[cageId * n + cageValCounts[cageId]++] = val;
                    }
                }
            }

            return KenKenBacktrack(0, grid, n, rowMask, colMask, cageTargets, cageOps, cageIds, cageCellsRemaining, cageVals, cageValCounts);
        }

        private static bool KenKenBacktrack(int cell, int* grid, int n, int* rowMask, int* colMask, int* cageTargets, int* cageOps, int* cageIds, int* cageCellsRemaining, int* cageVals, int* cageValCounts)
        {
            if (cell == n * n) return true;
            int r = cell / n;
            int c = cell % n;
            if (grid[cell] != 0)
            {
                return KenKenBacktrack(cell + 1, grid, n, rowMask, colMask, cageTargets, cageOps, cageIds, cageCellsRemaining, cageVals, cageValCounts);
            }
            int cageId = cageIds[cell];
            int op = cageOps[cageId];
            int target = cageTargets[cageId];

            int allowed = ~(rowMask[r] | colMask[c]) & ((1 << (n + 1)) - 2);
            while (allowed != 0)
            {
                int lsb = allowed & -allowed;
                allowed ^= lsb;
                int val = 0;
                int temp = lsb;
                while (temp > 1) { temp >>= 1; val++; }

                bool valid = true;
                if (op == 0)
                {
                    int sum = val;
                    for (int i = 0; i < cageValCounts[cageId]; i++) sum += cageVals[cageId * n + i];
                    if (sum > target) valid = false;
                    if (cageCellsRemaining[cageId] == 1 && sum != target) valid = false;
                }
                else if (op == 1)
                {
                    int prod = val;
                    for (int i = 0; i < cageValCounts[cageId]; i++) prod *= cageVals[cageId * n + i];
                    if (prod > target) valid = false;
                    if (cageCellsRemaining[cageId] == 1 && prod != target) valid = false;
                }
                else if (op == 2)
                {
                    if (cageCellsRemaining[cageId] == 1)
                    {
                        int other = cageVals[cageId * n + 0];
                        if (val - other != target && other - val != target) valid = false;
                    }
                }
                else if (op == 3)
                {
                    if (cageCellsRemaining[cageId] == 1)
                    {
                        int other = cageVals[cageId * n + 0];
                        if ((val % other != 0 || val / other != target) && (other % val != 0 || other / val != target)) valid = false;
                    }
                }

                if (!valid) continue;

                grid[cell] = val;
                rowMask[r] |= lsb;
                colMask[c] |= lsb;
                cageVals[cageId * n + cageValCounts[cageId]++] = val;
                cageCellsRemaining[cageId]--;

                if (KenKenBacktrack(cell + 1, grid, n, rowMask, colMask, cageTargets, cageOps, cageIds, cageCellsRemaining, cageVals, cageValCounts))
                    return true;

                grid[cell] = 0;
                rowMask[r] ^= lsb;
                colMask[c] ^= lsb;
                cageValCounts[cageId]--;
                cageCellsRemaining[cageId]++;
            }
            return false;
        }

        public static bool SolvePolyominoTiling(int width, int height, int numPieces, int numVariants, int* variantPieceId, int* variantOffsets, int* variantLengths, int* variantX, int* variantY, int* grid, int* placementVariant, int* placementR, int* placementC, int* dlxMatrix, int* L, int* R_dlx, int* U, int* D, int* C, int* RowIdx, int* colSize)
        {
            int totalPlacements = 0;
            for (int v = 0; v < numVariants; v++)
            {
                int len = variantLengths[v];
                int offset = variantOffsets[v];
                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        bool valid = true;
                        for (int i = 0; i < len; i++)
                        {
                            int nr = r + variantY[offset + i];
                            int nc = c + variantX[offset + i];
                            if (nr < 0 || nr >= height || nc < 0 || nc >= width)
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (valid)
                        {
                            totalPlacements++;
                        }
                    }
                }
            }

            int dlxCols = width * height + numPieces;
            long matrixSize = (long)totalPlacements * dlxCols;

            for (int i = 0; i < matrixSize; i++) dlxMatrix[i] = 0;

            int pIdx = 0;
            for (int v = 0; v < numVariants; v++)
            {
                int len = variantLengths[v];
                int offset = variantOffsets[v];
                int pieceId = variantPieceId[v];
                for (int r = 0; r < height; r++)
                {
                    for (int c = 0; c < width; c++)
                    {
                        bool valid = true;
                        for (int i = 0; i < len; i++)
                        {
                            int nr = r + variantY[offset + i];
                            int nc = c + variantX[offset + i];
                            if (nr < 0 || nr >= height || nc < 0 || nc >= width)
                            {
                                valid = false;
                                break;
                            }
                        }
                        if (valid)
                        {
                            placementVariant[pIdx] = v;
                            placementR[pIdx] = r;
                            placementC[pIdx] = c;

                            dlxMatrix[(long)pIdx * dlxCols + (width * height + pieceId)] = 1;
                            for (int i = 0; i < len; i++)
                            {
                                int nr = r + variantY[offset + i];
                                int nc = c + variantX[offset + i];
                                dlxMatrix[(long)pIdx * dlxCols + (nr * width + nc)] = 1;
                            }
                            pIdx++;
                        }
                    }
                }
            }

            int* solution = stackalloc int[totalPlacements];
            int solutionSize = 0;
            bool solved = SolveDlx(dlxMatrix, totalPlacements, dlxCols, solution, &solutionSize, L, R_dlx, U, D, C, RowIdx, colSize);
            if (solved)
            {
                for (int i = 0; i < width * height; i++) grid[i] = -1;
                for (int i = 0; i < solutionSize; i++)
                {
                    int p = solution[i];
                    int v = placementVariant[p];
                    int r = placementR[p];
                    int c = placementC[p];
                    int len = variantLengths[v];
                    int offset = variantOffsets[v];
                    int pieceId = variantPieceId[v];
                    for (int j = 0; j < len; j++)
                    {
                        int nr = r + variantY[offset + j];
                        int nc = c + variantX[offset + j];
                        grid[nr * width + nc] = pieceId;
                    }
                }
                return true;
            }
            return false;
        }

        public static bool NQueensBitmask(int n, int* solution)
        {
            return NQueensBitmaskBacktrack(0, n, 0, 0, 0, solution);
        }

        private static bool NQueensBitmaskBacktrack(int row, int n, int cols, int diag1, int diag2, int* solution)
        {
            if (row == n) return true;
            int available = ((1 << n) - 1) & ~(cols | diag1 | diag2);
            while (available != 0)
            {
                int lsb = available & -available;
                available ^= lsb;
                int col = 0;
                int temp = lsb;
                while (temp > 1) { temp >>= 1; col++; }
                solution[row] = col;
                if (NQueensBitmaskBacktrack(row + 1, n, cols | lsb, (diag1 | lsb) << 1, (diag2 | lsb) >> 1, solution))
                    return true;
            }
            return false;
        }

        public static int NQueensCount(int n)
        {
            int count = 0;
            NQueensCountBacktrack(0, n, 0, 0, 0, &count);
            return count;
        }

        private static void NQueensCountBacktrack(int row, int n, int cols, int diag1, int diag2, int* count)
        {
            if (row == n)
            {
                (*count)++;
                return;
            }
            int available = ((1 << n) - 1) & ~(cols | diag1 | diag2);
            while (available != 0)
            {
                int lsb = available & -available;
                available ^= lsb;
                NQueensCountBacktrack(row + 1, n, cols | lsb, (diag1 | lsb) << 1, (diag2 | lsb) >> 1, count);
            }
        }

        public static int NQueensSymmetry(int n)
        {
            int count = 0;
            int* pos = stackalloc int[n];
            NQueensSymmetryBacktrack(0, n, 0, 0, 0, pos, &count);
            return count;
        }

        private static void NQueensSymmetryBacktrack(int row, int n, int cols, int diag1, int diag2, int* pos, int* count)
        {
            if (row == n)
            {
                if (IsCanonical(pos, n))
                {
                    (*count)++;
                }
                return;
            }
            int available = ((1 << n) - 1) & ~(cols | diag1 | diag2);
            while (available != 0)
            {
                int lsb = available & -available;
                available ^= lsb;
                int col = 0;
                int temp = lsb;
                while (temp > 1) { temp >>= 1; col++; }
                pos[row] = col;
                NQueensSymmetryBacktrack(row + 1, n, cols | lsb, (diag1 | lsb) << 1, (diag2 | lsb) >> 1, pos, count);
            }
        }

        private static bool IsCanonical(int* p, int n)
        {
            int* temp = stackalloc int[n];
            for (int sym = 1; sym < 8; sym++)
            {
                if (sym == 1)
                {
                    for (int r = 0; r < n; r++) temp[p[r]] = n - 1 - r;
                }
                else if (sym == 2)
                {
                    for (int r = 0; r < n; r++) temp[n - 1 - r] = n - 1 - p[r];
                }
                else if (sym == 3)
                {
                    for (int r = 0; r < n; r++) temp[n - 1 - p[r]] = r;
                }
                else if (sym == 4)
                {
                    for (int r = 0; r < n; r++) temp[n - 1 - r] = p[r];
                }
                else if (sym == 5)
                {
                    for (int r = 0; r < n; r++) temp[r] = n - 1 - p[r];
                }
                else if (sym == 6)
                {
                    for (int r = 0; r < n; r++) temp[p[r]] = r;
                }
                else if (sym == 7)
                {
                    for (int r = 0; r < n; r++) temp[n - 1 - p[r]] = n - 1 - r;
                }

                for (int i = 0; i < n; i++)
                {
                    if (temp[i] < p[i]) return false;
                    if (temp[i] > p[i]) break;
                }
            }
            return true;
        }

        public static bool MagicSquareSolve(int n, int* grid)
        {
            int* used = stackalloc int[n * n + 1];
            for (int i = 0; i <= n * n; i++) used[i] = 0;
            int target = n * (n * n + 1) / 2;
            return MagicSquareBacktrack(0, n, grid, used, target);
        }

        private static bool MagicSquareBacktrack(int cell, int n, int* grid, int* used, int target)
        {
            if (cell == n * n)
            {
                int diagSum1 = 0;
                int diagSum2 = 0;
                for (int i = 0; i < n; i++)
                {
                    diagSum1 += grid[i * n + i];
                    diagSum2 += grid[i * n + (n - 1 - i)];
                }
                return diagSum1 == target && diagSum2 == target;
            }

            int r = cell / n;
            int c = cell % n;

            if (grid[cell] != 0)
            {
                int val = grid[cell];
                used[val] = 1;
                if (c == n - 1)
                {
                    int sum = 0;
                    for (int i = 0; i < n; i++) sum += grid[r * n + i];
                    if (sum != target)
                    {
                        used[val] = 0;
                        return false;
                    }
                }
                if (r == n - 1)
                {
                    int sum = 0;
                    for (int i = 0; i < n; i++) sum += grid[i * n + c];
                    if (sum != target)
                    {
                        used[val] = 0;
                        return false;
                    }
                }
                bool res = MagicSquareBacktrack(cell + 1, n, grid, used, target);
                used[val] = 0;
                return res;
            }

            for (int val = 1; val <= n * n; val++)
            {
                if (used[val] == 1) continue;

                grid[cell] = val;
                used[val] = 1;

                bool valid = true;
                if (c == n - 1)
                {
                    int sum = 0;
                    for (int i = 0; i < n; i++) sum += grid[r * n + i];
                    if (sum != target) valid = false;
                }
                if (r == n - 1)
                {
                    int sum = 0;
                    for (int i = 0; i < n; i++) sum += grid[i * n + c];
                    if (sum != target) valid = false;
                }

                if (valid && MagicSquareBacktrack(cell + 1, n, grid, used, target))
                {
                    return true;
                }

                grid[cell] = 0;
                used[val] = 0;
            }
            return false;
        }

        public static bool CryptarithmSolve(int numLetters, int numWords, int* wordLengths, int* wordLetters, int* wordOffsets, int targetLength, int* targetLetters, bool* isLeading, int* resultDigits)
        {
            int* usedDigits = stackalloc int[10];
            for (int i = 0; i < 10; i++) usedDigits[i] = 0;
            return CryptarithmBacktrack(0, numLetters, numWords, wordLengths, wordLetters, wordOffsets, targetLength, targetLetters, isLeading, resultDigits, usedDigits);
        }

        private static bool CryptarithmBacktrack(int letterIdx, int numLetters, int numWords, int* wordLengths, int* wordLetters, int* wordOffsets, int targetLength, int* targetLetters, bool* isLeading, int* resultDigits, int* usedDigits)
        {
            if (letterIdx == numLetters)
            {
                long wordSum = 0;
                for (int w = 0; w < numWords; w++)
                {
                    long val = 0;
                    int len = wordLengths[w];
                    int offset = wordOffsets[w];
                    for (int i = 0; i < len; i++)
                    {
                        val = val * 10 + resultDigits[wordLetters[offset + i]];
                    }
                    wordSum += val;
                }
                long targetVal = 0;
                for (int i = 0; i < targetLength; i++)
                {
                    targetVal = targetVal * 10 + resultDigits[targetLetters[i]];
                }
                return wordSum == targetVal;
            }

            for (int d = 0; d <= 9; d++)
            {
                if (d == 0 && isLeading[letterIdx]) continue;
                if (usedDigits[d] == 1) continue;

                resultDigits[letterIdx] = d;
                usedDigits[d] = 1;

                if (CryptarithmBacktrack(letterIdx + 1, numLetters, numWords, wordLengths, wordLetters, wordOffsets, targetLength, targetLetters, isLeading, resultDigits, usedDigits))
                {
                    return true;
                }

                usedDigits[d] = 0;
            }
            return false;
        }

        public static bool ArcConsistencyAc3(int numVars, int* domainSizes, bool* domains, int maxDomain, int numConstraints, int* constVar1, int* constVar2, bool* relations)
        {
            int maxArcs = 2 * numConstraints;
            int* qVar1 = stackalloc int[maxArcs];
            int* qVar2 = stackalloc int[maxArcs];
            int* qConstId = stackalloc int[maxArcs];
            int head = 0;
            int tail = 0;

            for (int c = 0; c < numConstraints; c++)
            {
                int u = constVar1[c];
                int v = constVar2[c];
                qVar1[tail] = u;
                qVar2[tail] = v;
                qConstId[tail++] = c;

                qVar1[tail] = v;
                qVar2[tail] = u;
                qConstId[tail++] = c;
            }

            while (head < tail)
            {
                int u = qVar1[head];
                int v = qVar2[head];
                int c = qConstId[head++];

                bool revised = false;
                for (int valU = 0; valU < domainSizes[u]; valU++)
                {
                    if (!domains[(long)u * maxDomain + valU]) continue;
                    bool satisfied = false;
                    for (int valV = 0; valV < domainSizes[v]; valV++)
                    {
                        if (!domains[(long)v * maxDomain + valV]) continue;
                        bool compatible;
                        if (u == constVar1[c] && v == constVar2[c])
                        {
                            compatible = relations[c * maxDomain * maxDomain + valU * maxDomain + valV];
                        }
                        else
                        {
                            compatible = relations[c * maxDomain * maxDomain + valV * maxDomain + valU];
                        }
                        if (compatible)
                        {
                            satisfied = true;
                            break;
                        }
                    }
                    if (!satisfied)
                    {
                        domains[(long)u * maxDomain + valU] = false;
                        revised = true;
                    }
                }

                if (revised)
                {
                    bool empty = true;
                    for (int val = 0; val < domainSizes[u]; val++)
                    {
                        if (domains[(long)u * maxDomain + val])
                        {
                            empty = false;
                            break;
                        }
                    }
                    if (empty) return false;

                    for (int c2 = 0; c2 < numConstraints; c2++)
                    {
                        if (constVar1[c2] == u && constVar2[c2] != v)
                        {
                            qVar1[tail] = constVar2[c2];
                            qVar2[tail] = u;
                            qConstId[tail++] = c2;
                        }
                        else if (constVar2[c2] == u && constVar1[c2] != v)
                        {
                            qVar1[tail] = constVar1[c2];
                            qVar2[tail] = u;
                            qConstId[tail++] = c2;
                        }
                    }
                }
            }
            return true;
        }

        public static bool ConstraintPropagationSolve(int numVars, int* domainSizes, bool* domains, int maxDomain, int numConstraints, int* constVar1, int* constVar2, bool* relations, int* assignment)
        {
            return CspBacktrack(0, numVars, domainSizes, domains, maxDomain, numConstraints, constVar1, constVar2, relations, assignment);
        }

        private static bool CspBacktrack(int varIdx, int numVars, int* domainSizes, bool* domains, int maxDomain, int numConstraints, int* constVar1, int* constVar2, bool* relations, int* assignment)
        {
            if (varIdx == numVars) return true;

            for (int val = 0; val < domainSizes[varIdx]; val++)
            {
                if (!domains[(long)varIdx * maxDomain + val]) continue;

                bool consistent = true;
                for (int c = 0; c < numConstraints; c++)
                {
                    int u = constVar1[c];
                    int v = constVar2[c];
                    if (u == varIdx && v < varIdx)
                    {
                        int valV = assignment[v];
                        if (!relations[c * maxDomain * maxDomain + val * maxDomain + valV])
                        {
                            consistent = false;
                            break;
                        }
                    }
                    else if (v == varIdx && u < varIdx)
                    {
                        int valU = assignment[u];
                        if (!relations[c * maxDomain * maxDomain + valU * maxDomain + val])
                        {
                            consistent = false;
                            break;
                        }
                    }
                }

                if (!consistent) continue;

                assignment[varIdx] = val;

                if (CspBacktrack(varIdx + 1, numVars, domainSizes, domains, maxDomain, numConstraints, constVar1, constVar2, relations, assignment))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
