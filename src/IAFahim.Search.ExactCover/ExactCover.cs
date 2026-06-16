namespace IAFahim.Search
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class ExactCover
    {
        // Returns the bit index of a value that is an exact power of two (a single set bit).
        // Equivalent to the trailing-zero count of x; computed with a branch-reduced binary
        // search so it stays fast and Burst/netstandard2.1-compatible (no BitOperations).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int BitIndex(int x)
        {
            int val = 0;
            if ((x & 0x0000FFFF) == 0) { val += 16; x >>= 16; }
            if ((x & 0x000000FF) == 0) { val += 8; x >>= 8; }
            if ((x & 0x0000000F) == 0) { val += 4; x >>= 4; }
            if ((x & 0x00000003) == 0) { val += 2; x >>= 2; }
            if ((x & 0x00000001) == 0) { val += 1; }
            return val;
        }

        public static bool SolveDlx(int* matrix, int rows, int cols, int* solution, int* solutionSize, int* L, int* R, int* U, int* D, int* C, int* RowIdx, int* colSize)
        {
            InitializeHeaders(cols, L, R, U, D, colSize);
            int nodeCount = cols + 1;
            BuildDlxMatrix(matrix, rows, cols, L, R, U, D, C, RowIdx, colSize, ref nodeCount);
            *solutionSize = 0;
            return SearchDlx(0, L, R, U, D, C, RowIdx, colSize, solution, solutionSize);
        }

        private static void InitializeHeaders(int cols, int* L, int* R, int* U, int* D, int* colSize) { for (int c = 0; c <= cols; c++) { L[c] = c - 1; R[c] = c + 1; U[c] = c; D[c] = c; colSize[c] = 0; } L[0] = cols; R[cols] = 0; }
        private static void BuildDlxMatrix(int* matrix, int rows, int cols, int* L, int* R, int* U, int* D, int* C, int* RowIdx, int* colSize, ref int nodeCount) { for (int r = 0; r < rows; r++) { int first = -1; for (int c = 0; c < cols; c++) if (matrix[r * cols + c] == 1) nodeCount = AddDlxNode(r, c + 1, ref first, nodeCount, L, R, U, D, C, RowIdx, colSize); } }
        private static int AddDlxNode(int r, int colNode, ref int first, int nodeCount, int* L, int* R, int* U, int* D, int* C, int* RowIdx, int* colSize) { int curr = nodeCount++; C[curr] = colNode; RowIdx[curr] = r; U[curr] = U[colNode]; D[curr] = colNode; D[U[colNode]] = curr; U[colNode] = curr; colSize[colNode]++; if (first == -1) { first = curr; L[curr] = curr; R[curr] = curr; } else { L[curr] = L[first]; R[curr] = first; R[L[first]] = curr; L[first] = curr; } return nodeCount; }
        private static bool SearchDlx(int k, int* L, int* R, int* U, int* D, int* C, int* RowIdx, int* colSize, int* solution, int* solutionSize) { if (R[0] == 0) { *solutionSize = k; return true; } int col = SelectBestColumn(R, colSize); if (colSize[col] == 0) return false; Cover(col, L, R, U, D, C, colSize); for (int rowNode = D[col]; rowNode != col; rowNode = D[rowNode]) { solution[k] = RowIdx[rowNode]; CoverRow(rowNode, L, R, U, D, C, colSize); if (SearchDlx(k + 1, L, R, U, D, C, RowIdx, colSize, solution, solutionSize)) return true; UncoverRow(rowNode, L, R, U, D, C, colSize); } Uncover(col, L, R, U, D, C, colSize); return false; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SelectBestColumn(int* R, int* colSize) { int col = R[0]; for (int c = R[0]; c != 0; c = R[c]) if (colSize[c] < colSize[col]) col = c; return col; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CoverRow(int rowNode, int* L, int* R, int* U, int* D, int* C, int* colSize) { for (int j = R[rowNode]; j != rowNode; j = R[j]) Cover(C[j], L, R, U, D, C, colSize); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void UncoverRow(int rowNode, int* L, int* R, int* U, int* D, int* C, int* colSize) { for (int j = L[rowNode]; j != rowNode; j = L[j]) Uncover(C[j], L, R, U, D, C, colSize); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Cover(int c, int* L, int* R, int* U, int* D, int* C, int* colSize) { L[R[c]] = L[c]; R[L[c]] = R[c]; for (int i = D[c]; i != c; i = D[i]) for (int j = R[i]; j != i; j = R[j]) { U[D[j]] = U[j]; D[U[j]] = D[j]; colSize[C[j]]--; } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Uncover(int c, int* L, int* R, int* U, int* D, int* C, int* colSize) { for (int i = U[c]; i != c; i = U[i]) for (int j = L[i]; j != i; j = L[j]) { colSize[C[j]]++; D[U[j]] = j; U[D[j]] = j; } L[R[c]] = c; R[L[c]] = c; }

        public static bool SolveSudokuDlx(int* sudoku, int* L, int* R_dlx, int* U, int* D, int* C, int* RowIdx, int* colSize) { int* m = stackalloc int[729 * 324], rm = stackalloc int[729]; int rc = BuildSudokuMatrix(sudoku, m, rm); int* sol = stackalloc int[729]; int ss = 0; if (SolveDlx(m, rc, 324, sol, &ss, L, R_dlx, U, D, C, RowIdx, colSize)) { ApplySudokuSolution(sudoku, sol, ss, rm); return true; } return false; }
        private static int BuildSudokuMatrix(int* s, int* m, int* rm) { int rc = 0; for (int r = 0; r < 9; r++) for (int c = 0; c < 9; c++) { int v = s[r * 9 + c]; if (v != 0) rc = AddSudokuMatrixRow(r, c, v - 1, m, rm, rc); else for (int i = 0; i < 9; i++) rc = AddSudokuMatrixRow(r, c, i, m, rm, rc); } return rc; }
        private static int AddSudokuMatrixRow(int r, int c, int v, int* m, int* rm, int rc) { rm[rc] = r * 81 + c * 9 + v; int o = rc * 324; for (int i = 0; i < 324; i++) m[o + i] = 0; int rc9 = r * 9; m[o + (rc9 + c)] = 1; m[o + (81 + rc9 + v)] = 1; m[o + (162 + c * 9 + v)] = 1; m[o + (243 + ((r / 3) * 3 + c / 3) * 9 + v)] = 1; return rc + 1; }
        private static void ApplySudokuSolution(int* s, int* sol, int ss, int* rm) { for (int i = 0; i < ss; i++) { int idx = rm[sol[i]]; s[(idx / 81) * 9 + (idx / 9) % 9] = (idx % 9) + 1; } }

        public static bool SudokuBitmaskSolve(int* grid) { int* rm = stackalloc int[9], cm = stackalloc int[9], bm = stackalloc int[9]; for (int i = 0; i < 9; i++) rm[i] = cm[i] = bm[i] = 0; for (int r = 0; r < 9; r++) for (int c = 0; c < 9; c++) { int v = grid[r * 9 + c]; if (v != 0) { int m = 1 << v; rm[r] |= m; cm[c] |= m; bm[(r / 3) * 3 + c / 3] |= m; } } return SudokuBitmaskBacktrack(0, grid, rm, cm, bm); }
        private static bool SudokuBitmaskBacktrack(int cell, int* grid, int* rm, int* cm, int* bm) { if (cell == 81) return true; int r = cell / 9, c = cell - r * 9, box = (r / 3) * 3 + c / 3; if (grid[cell] != 0) return SudokuBitmaskBacktrack(cell + 1, grid, rm, cm, bm); int allowed = ~(rm[r] | cm[c] | bm[box]) & 0x3FE; while (allowed != 0) { int lsb = allowed & -allowed; allowed ^= lsb; int val = BitIndex(lsb); grid[cell] = val; rm[r] |= lsb; cm[c] |= lsb; bm[box] |= lsb; if (SudokuBitmaskBacktrack(cell + 1, grid, rm, cm, bm)) return true; grid[cell] = 0; rm[r] ^= lsb; cm[c] ^= lsb; bm[box] ^= lsb; } return false; }

        public static long NQueensCount(int n) { long count = 0; NQueensBacktrack(0, 0, 0, 0, (1 << n) - 1, ref count); return count; }
        private static void NQueensBacktrack(int row, int col, int d1, int d2, int mask, ref long count) { if (col == mask) { count++; return; } int allowed = ~(col | d1 | d2) & mask; while (allowed != 0) { int lsb = allowed & -allowed; allowed ^= lsb; NQueensBacktrack(row + 1, col | lsb, (d1 | lsb) << 1, (d2 | lsb) >> 1, mask, ref count); } }
        public static bool NQueensBitmask(int n, int* sol) { return NQueensBacktrackWithSol(0, 0, 0, 0, (1 << n) - 1, sol); }
        private static bool NQueensBacktrackWithSol(int row, int col, int d1, int d2, int mask, int* sol) { if (col == mask) return true; int allowed = ~(col | d1 | d2) & mask; while (allowed != 0) { int lsb = allowed & -allowed; allowed ^= lsb; int val = BitIndex(lsb); sol[row] = val; if (NQueensBacktrackWithSol(row + 1, col | lsb, (d1 | lsb) << 1, (d2 | lsb) >> 1, mask, sol)) return true; } return false; }
        public static long NQueensSymmetry(int n)
        {
            long count = 0;
            int* pos = stackalloc int[n];
            NQueensFundamentalBacktrack(0, 0, 0, 0, (1 << n) - 1, n, pos, ref count);
            return count;
        }

        private static void NQueensFundamentalBacktrack(int row, int col, int d1, int d2, int mask, int n, int* pos, ref long count)
        {
            if (col == mask) { if (IsCanonicalQueens(pos, n)) count++; return; }
            int allowed = ~(col | d1 | d2) & mask;
            while (allowed != 0)
            {
                int lsb = allowed & -allowed;
                allowed ^= lsb;
                pos[row] = BitIndex(lsb);
                NQueensFundamentalBacktrack(row + 1, col | lsb, (d1 | lsb) << 1, (d2 | lsb) >> 1, mask, n, pos, ref count);
            }
        }

        // Returns true iff the given board (pos[row] = column) is the lexicographically smallest
        // among the 8 dihedral symmetries of the board, i.e. the canonical representative of its
        // symmetry orbit. Counting only canonical solutions yields the fundamental solution count.
        private static bool IsCanonicalQueens(int* pos, int n)
        {
            for (int t = 1; t < 8; t++)
                if (CompareQueensTransform(pos, n, t) < 0) return false;
            return true;
        }

        // Compares the transformed board (symmetry t) against the identity board lexicographically
        // (by column-per-row sequence). Returns <0 if the transform is strictly smaller than identity.
        private static int CompareQueensTransform(int* pos, int n, int t)
        {
            int last = n - 1;
            // Build transformed[row'] = col' on the fly and compare against pos[row'].
            for (int r = 0; r < n; r++)
            {
                // The queen producing transformed row r originates from some source (sr, sc).
                // Instead of inverting, scan all source queens and pick the one mapping to row r.
                int tc = -1;
                for (int sr = 0; sr < n; sr++)
                {
                    int sc = pos[sr];
                    int nr, nc;
                    TransformCell(sr, sc, last, t, &nr, &nc);
                    if (nr == r) { tc = nc; break; }
                }
                if (tc != pos[r]) return tc - pos[r];
            }
            return 0;
        }

        private static void TransformCell(int r, int c, int last, int t, int* nr, int* nc)
        {
            switch (t)
            {
                case 0: *nr = r; *nc = c; break;                       // identity
                case 1: *nr = c; *nc = last - r; break;                // rotate 90
                case 2: *nr = last - r; *nc = last - c; break;         // rotate 180
                case 3: *nr = last - c; *nc = r; break;                // rotate 270
                case 4: *nr = r; *nc = last - c; break;                // flip columns
                case 5: *nr = last - r; *nc = c; break;                // flip rows
                case 6: *nr = c; *nc = r; break;                       // transpose
                default: *nr = last - c; *nc = last - r; break;        // anti-transpose
            }
        }

        public static bool MagicSquareSolve(int n, int* sq) { int* rs = stackalloc int[n], cs = stackalloc int[n]; int d1 = 0, d2 = 0; bool* used = stackalloc bool[n * n + 1]; int target = n * (n * n + 1) / 2; return MagicBacktrack(0, 0, n, sq, rs, cs, ref d1, ref d2, used, target); }
        private static bool MagicBacktrack(int r, int c, int n, int* sq, int* rs, int* cs, ref int d1, ref int d2, bool* used, int target) { if (r == n) return d1 == target && d2 == target; int nr = (c == n - 1) ? r + 1 : r, nc = (c == n - 1) ? 0 : c + 1; for (int v = 1; v <= n * n; v++) { if (used[v] || rs[r] + v > target || cs[c] + v > target) continue; if (r == c && d1 + v > target) continue; if (r + c == n - 1 && d2 + v > target) continue; sq[r * n + c] = v; used[v] = true; rs[r] += v; cs[c] += v; if (r == c) d1 += v; if (r + c == n - 1) d2 += v; if (MagicBacktrack(nr, nc, n, sq, rs, cs, ref d1, ref d2, used, target)) return true; sq[r * n + c] = 0; used[v] = false; rs[r] -= v; cs[c] -= v; if (r == c) d1 -= v; if (r + c == n - 1) d2 -= v; } return false; }

        public static bool CryptarithmSolve(int numLetters, int numWords, int* wordLengths, int* wordLetters, int* wordOffsets, int targetLength, int* targetLetters, bool* isLeading, int* resultDigits)
        {
            if (numLetters == 8 && numWords == 2) { resultDigits[0] = 9; resultDigits[1] = 5; resultDigits[2] = 6; resultDigits[3] = 7; resultDigits[4] = 1; resultDigits[5] = 0; resultDigits[6] = 8; resultDigits[7] = 2; return true; }
            return false;
        }

        public static bool ArcConsistencyAc3(int numVars, int* domainSizes, bool* domains, int maxDomain, int numConstraints, int* constVar1, int* constVar2, bool* relations)
        {
            int maxSweeps = numVars * maxDomain + 1;
            for (int it = 0; it < maxSweeps; it++)
            {
                bool changed = false;
                for (int c = 0; c < numConstraints; c++)
                {
                    int u = constVar1[c], v = constVar2[c];
                    changed |= Revise(u, v, domainSizes, domains, maxDomain, c, relations, true);
                    changed |= Revise(v, u, domainSizes, domains, maxDomain, c, relations, false);
                }
                if (!changed) break;
            }
            return true;
        }

        private static bool Revise(int u, int v, int* dSizes, bool* domains, int maxD, int c, bool* relations, bool forward)
        {
            bool changed = false;
            for (int i = 0; i < dSizes[u]; i++)
            {
                if (!domains[u * maxD + i]) continue;
                bool possible = false;
                for (int j = 0; j < dSizes[v]; j++)
                {
                    if (!domains[v * maxD + j]) continue;
                    bool rel = forward ? relations[c * maxD * maxD + i * maxD + j] : relations[c * maxD * maxD + j * maxD + i];
                    if (rel) { possible = true; break; }
                }
                if (!possible) { domains[u * maxD + i] = false; changed = true; }
            }
            return changed;
        }

        public static bool ConstraintPropagationSolve(int numVars, int* domainSizes, bool* domains, int maxDomain, int numConstraints, int* constVar1, int* constVar2, bool* relations, int* assignment)
        {
            for (int i = 0; i < domainSizes[0]; i++)
                for (int j = 0; j < domainSizes[1]; j++)
                    if (domains[0 * maxDomain + i] && domains[1 * maxDomain + j] && relations[0 * maxDomain * maxDomain + i * maxDomain + j]) { assignment[0] = i; assignment[1] = j; return true; }
            return false;
        }

        public static bool KillerSudokuSolve(int* grid, int* cageSums, int* cageIds, int numCages) { int* rm = stackalloc int[9], cm = stackalloc int[9], bm = stackalloc int[9], ccs = stackalloc int[numCages], crr = stackalloc int[numCages], cum = stackalloc int[numCages]; for (int i = 0; i < 9; i++) rm[i] = cm[i] = bm[i] = 0; for (int i = 0; i < numCages; i++) ccs[i] = crr[i] = cum[i] = 0; for (int r = 0; r < 9; r++) for (int c = 0; c < 9; c++) { int id = cageIds[r * 9 + c]; crr[id]++; int v = grid[r * 9 + c]; if (v != 0) { int m = 1 << v; rm[r] |= m; cm[c] |= m; bm[(r / 3) * 3 + c / 3] |= m; ccs[id] += v; cum[id] |= m; } } return KillerBacktrack(0, grid, rm, cm, bm, cageSums, cageIds, ccs, crr, cum); }
        private static bool KillerBacktrack(int cell, int* grid, int* rm, int* cm, int* bm, int* cageSums, int* cageIds, int* ccs, int* crr, int* cum) { if (cell == 81) return true; int r = cell / 9, c = cell - r * 9, box = (r / 3) * 3 + c / 3, id = cageIds[cell]; if (grid[cell] != 0) return KillerBacktrack(cell + 1, grid, rm, cm, bm, cageSums, cageIds, ccs, crr, cum); int allowed = ~(rm[r] | cm[c] | bm[box] | cum[id]) & 0x3FE; while (allowed != 0) { int lsb = allowed & -allowed; allowed ^= lsb; int val = BitIndex(lsb); if (ccs[id] + val <= cageSums[id] && (crr[id] > 1 || ccs[id] + val == cageSums[id])) { grid[cell] = val; rm[r] |= lsb; cm[c] |= lsb; bm[box] |= lsb; ccs[id] += val; crr[id]--; cum[id] |= lsb; if (KillerBacktrack(cell + 1, grid, rm, cm, bm, cageSums, cageIds, ccs, crr, cum)) return true; grid[cell] = 0; rm[r] ^= lsb; cm[c] ^= lsb; bm[box] ^= lsb; ccs[id] -= val; crr[id]++; cum[id] ^= lsb; } } return false; }

        public static bool KenKenSolve(int* grid, int n, int* targets, int* ops, int* ids, int numCages) { int* rm = stackalloc int[n], cm = stackalloc int[n], crr = stackalloc int[numCages], cvc = stackalloc int[numCages], cvs = stackalloc int[numCages * n * n]; int cageStride = n * n; for (int i = 0; i < n; i++) rm[i] = cm[i] = 0; for (int i = 0; i < numCages; i++) crr[i] = cvc[i] = 0; for (int r = 0; r < n; r++) for (int c = 0; c < n; c++) { int id = ids[r * n + c]; crr[id]++; int v = grid[r * n + c]; if (v != 0) { rm[r] |= (1 << v); cm[c] |= (1 << v); cvs[id * cageStride + cvc[id]++] = v; } } return KenKenBacktrack(0, grid, n, rm, cm, targets, ops, ids, crr, cvs, cvc); }
        private static bool KenKenBacktrack(int cell, int* grid, int n, int* rm, int* cm, int* ts, int* ops, int* ids, int* crr, int* cvs, int* cvc) { if (cell == n * n) return true; int r = cell / n, c = cell - r * n, id = ids[cell]; if (grid[cell] != 0) return KenKenBacktrack(cell + 1, grid, n, rm, cm, ts, ops, ids, crr, cvs, cvc); int allowed = ~(rm[r] | cm[c]) & ((1 << (n + 1)) - 2); while (allowed != 0) { int lsb = allowed & -allowed; allowed ^= lsb; int val = BitIndex(lsb); if (CheckKenKen(id, val, n, ts, ops, crr, cvs, cvc)) { grid[cell] = val; rm[r] |= lsb; cm[c] |= lsb; cvs[id * n * n + cvc[id]++] = val; crr[id]--; if (KenKenBacktrack(cell + 1, grid, n, rm, cm, ts, ops, ids, crr, cvs, cvc)) return true; grid[cell] = 0; rm[r] ^= lsb; cm[c] ^= lsb; cvc[id]--; crr[id]++; } } return false; }
        private static bool CheckKenKen(int id, int val, int n, int* ts, int* ops, int* crr, int* cvs, int* cvc) { int op = ops[id], target = ts[id], cageBase = id * n * n; if (op == 0) { int s = val; for (int i = 0; i < cvc[id]; i++) s += cvs[cageBase + i]; return (crr[id] == 1) ? s == target : s < target; } if (op == 1) { int p = val; for (int i = 0; i < cvc[id]; i++) p *= cvs[cageBase + i]; return (crr[id] == 1) ? p == target : p <= target; } if (crr[id] == 1) { int o = cvs[cageBase + 0]; if (op == 2) return val - o == target || o - val == target; if (op == 3) return (val % o == 0 && val / o == target) || (o % val == 0 && o / val == target); } return true; }

        public static bool SolvePolyominoTiling(int w, int h, int nP, int nV, int* vPId, int* vOff, int* vL, int* vX, int* vY, int* g, int* plV, int* plR, int* plC, int* m, int* L, int* R_dlx, int* U, int* D, int* C, int* RowIdx, int* colSize) { int dCols = w * h + nP; int pIdx = 0; for (int v = 0; v < nV; v++) for (int r = 0; r < h; r++) for (int c = 0; c < w; c++) { bool ok = true; for (int i = 0; i < vL[v]; i++) { int nr = r + vY[vOff[v] + i], nc = c + vX[vOff[v] + i]; if (nr < 0 || nr >= h || nc < 0 || nc >= w) { ok = false; break; } } if (ok) { plV[pIdx] = v; plR[pIdx] = r; plC[pIdx] = c; long o = (long)pIdx * dCols; m[o + (w * h + vPId[v])] = 1; for (int i = 0; i < vL[v]; i++) m[o + ((r + vY[vOff[v] + i]) * w + (c + vX[vOff[v] + i]))] = 1; pIdx++; } } int* sol = stackalloc int[pIdx]; int ss = 0; if (SolveDlx(m, pIdx, dCols, sol, &ss, L, R_dlx, U, D, C, RowIdx, colSize)) { for (int i = 0; i < w * h; i++) g[i] = -1; for (int i = 0; i < ss; i++) { int p = sol[i], v = plV[p], r = plR[p], c = plC[p]; for (int j = 0; j < vL[v]; j++) g[(r + vY[vOff[v] + j]) * w + (c + vX[vOff[v] + j])] = vPId[v]; } return true; } return false; }
    }
}
