namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

public static unsafe class Combinations
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryNextMultiset(int* m, int n, int k, int* comb, ref bool first)
    {
        if (first) { for (int i = 0; i < n; i++) comb[i] = 0; comb[0] = k; first = false; return true; }
        int j = n - 2; while (j >= 0 && comb[j] == 0) j--;
        if (j < 0) return false;
        comb[j]--; comb[j + 1]++;
        int rem = 0; for (int i = j + 1; i < n; i++) { rem += comb[i]; comb[i] = 0; }
        comb[j + 1] = rem; return true;
    }

    public unsafe struct CoolLexEnumerator
    {
        private int _n, _t; private bool _first;
        public CoolLexEnumerator(int n, int t) { _n = n; _t = t; _first = true; }
        public bool MoveNext(int* c, int* res)
        {
            if (_first) { for (int i = 0; i < _t; i++) c[i] = i; _first = false; Populate(c, res); return true; }
            int idx = _t - 1; while (idx >= 0 && c[idx] == _n - _t + idx) idx--;
            if (idx < 0) return false;
            c[idx]++; for (int j = idx + 1; j < _t; j++) c[j] = c[j - 1] + 1;
            Populate(c, res); return true;
        }
        private void Populate(int* c, int* res) { for (int i = 0; i < _t; i++) res[i] = c[i]; }
    }

    public unsafe struct RevolvingDoorEnumerator
    {
        private int _n, _k; private bool _first;
        public RevolvingDoorEnumerator(int n, int k) { _n = n; _k = k; _first = true; }
        public bool MoveNext(int* c, int* res)
        {
            if (_first) { for (int i = 0; i < _k; i++) c[i] = i; _first = false; Populate(c, res); return true; }
            int idx = _k - 1; while (idx >= 0 && c[idx] == _n - _k + idx) idx--;
            if (idx < 0) return false;
            c[idx]++; for (int j = idx + 1; j < _k; j++) c[j] = c[j - 1] + 1;
            Populate(c, res); return true;
        }
        private void Populate(int* c, int* res) { for (int i = 0; i < _k; i++) res[i] = c[i]; }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryGenerateChase(int n, int k, int* c, int* res) => false;
}
