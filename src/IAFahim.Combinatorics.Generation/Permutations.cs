namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

    public unsafe struct HeapPermutationEnumerator
    {
        private int _n, _i;
        private bool _first;

        public HeapPermutationEnumerator(int n) { _n = n; _i = 1; _first = true; }

        public bool MoveNext(int* a, int* c)
        {
            if (_first)
            {
                for (int i = 0; i < _n; i++) { a[i] = i; c[i] = 0; }
                _first = false; return true;
            }
            while (_i < _n)
            {
                if (c[_i] < _i)
                {
                    if (_i % 2 == 0) { int t = a[0]; a[0] = a[_i]; a[_i] = t; }
                    else { int t = a[c[_i]]; a[c[_i]] = a[_i]; a[_i] = t; }
                    c[_i]++;
                    _i = 1;
                    return true;
                }
                else
                {
                    c[_i] = 0;
                    _i++;
                }
            }
            return false;
        }
    }

    public unsafe struct JohnsonTrotterEnumerator
    {
        private int _n;
        private bool _first;

        public JohnsonTrotterEnumerator(int n) { _n = n; _first = true; }

        public bool MoveNext(int* a, byte* dir)
        {
            if (_first)
            {
                for (int i = 0; i < _n; i++) { a[i] = i; dir[i] = 0; }
                _first = false; return true;
            }
            int mobileIdx = -1, mobileVal = -1;
            for (int i = 0; i < _n; i++)
            {
                if (dir[a[i]] == 0 && i > 0 && a[i] > a[i - 1])
                    if (a[i] > mobileVal) { mobileVal = a[i]; mobileIdx = i; }
                if (dir[a[i]] == 1 && i < _n - 1 && a[i] > a[i + 1])
                    if (a[i] > mobileVal) { mobileVal = a[i]; mobileIdx = i; }
            }
            if (mobileIdx == -1) return false;
            int mobileValActual = a[mobileIdx];
            int swapIdx = dir[mobileValActual] == 1 ? mobileIdx + 1 : mobileIdx - 1;
            int t = a[mobileIdx]; a[mobileIdx] = a[swapIdx]; a[swapIdx] = t;
            for (int i = 0; i < _n; i++)
                if (a[i] > mobileValActual) dir[a[i]] ^= 1;
            return true;
        }
    }

    public static unsafe class Permutations
    {
        public static bool NextDerangement(int* a, int n)
        {
            while (NextPermutation(a, n))
            {
                bool ok = true;
                for (int i = 0; i < n; i++) if (a[i] == i) { ok = false; break; }
                if (ok) return true;
            }
            return false;
        }

        public static bool NextPermutation(int* ptr, int len)
        {
            int i = len - 2;
            while (i >= 0 && ptr[i] >= ptr[i + 1]) i--;
            if (i < 0) return false;
            int j = len - 1;
            while (ptr[j] <= ptr[i]) j--;
            int tmp = ptr[i]; ptr[i] = ptr[j]; ptr[j] = tmp;
            int lo = i + 1, hi = len - 1;
            while (lo < hi) { tmp = ptr[lo]; ptr[lo] = ptr[hi]; ptr[hi] = tmp; lo++; hi--; }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomPermutation(int n, int* a, ref uint seed)
        {
            for (int i = 0; i < n; i++) a[i] = i;
            for (int i = n - 1; i > 0; i--)
            {
                seed ^= seed << 13; seed ^= seed >> 17; seed ^= seed << 5;
                int j = (int)(seed % (uint)(i + 1));
                int tmp = a[i]; a[i] = a[j]; a[j] = tmp;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RandomDerangement(int n, int* a, ref uint seed)
        {
            RandomPermutation(n, a, ref seed);
            for (int i = 0; i < n; i++)
            {
                if (a[i] == i) { int j = (i + 1) % n; int tmp = a[i]; a[i] = a[j]; a[j] = tmp; }
            }
        }

        public static long InvolutionCount(int n)
        {
            if (n <= 1) return 1;
            long* dp = stackalloc long[n + 1];
            dp[0] = 1; dp[1] = 1;
            for (int i = 2; i <= n; i++) dp[i] = dp[i - 1] + (i - 1) * dp[i - 2];
            return dp[n];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInvolution(int n, int* a)
        {
            for (int k = 0; k < n; k++) if (a[a[k]] != k) return false;
            return true;
        }
    }
