namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

    public unsafe struct LyndonWordEnumerator
    {
        private int _n, _k; private bool _first;
        public LyndonWordEnumerator(int n, int k) { _n = n; _k = k; _first = true; }
        public bool MoveNext(int* w, int* res, out int resLen)
        {
            resLen = 0; if (_first) { for (int i = 0; i <= _n; i++) w[i] = 0; _first = false; _j = 1; }
            while (_j > 0)
            {
                int curJ = _j;
                int nextJ = _n; while (nextJ > 0 && w[nextJ] == _k - 1) nextJ--;
                bool emit = curJ == _n;
                if (emit) { resLen = _n; for (int i = 0; i < _n; i++) res[i] = w[i + 1]; }
                if (nextJ > 0) { w[nextJ]++; for (int m = nextJ + 1; m <= _n; m++) w[m] = w[m - nextJ]; _j = nextJ; }
                else _j = 0;
                if (emit) return true;
            }
            return false;
        }
        private int _j;
    }

    public static unsafe class NecklacesAndBracelets
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int DeBruijnFromLyndon(int k, int n, int* outSeq)
        {
            int* w = stackalloc int[n + 1]; for (int i = 0; i <= n; i++) w[i] = 0;
            int j = 1, pos = 0;
            while (j > 0)
            {
                if (n % j == 0) for (int i = 1; i <= j; i++) outSeq[pos++] = w[i];
                int nextJ = n; while (nextJ > 0 && w[nextJ] == k - 1) nextJ--;
                if (nextJ > 0) { w[nextJ]++; for (int m = nextJ + 1; m <= n; m++) w[m] = w[m - nextJ]; j = nextJ; }
                else j = 0;
            }
            return pos;
        }

        public static long NecklaceRank(int* necklace, int n, int k) => 0;
        public static bool NecklaceUnrank(long rank, int n, int k, int* outObj) => false;
        public static long BraceletRank(int* bracelet, int n, int k) => 0;
        public static bool BraceletUnrank(long rank, int n, int k, int* outObj) => false;
        public static long LyndonWordRank(int* word, int n, int k) => 0;
        public static bool LyndonWordUnrank(long rank, int n, int k, int* outObj) => false;
    }
