namespace IAFahim.DS.SegmentTree
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KineticTournamentBuild
    {
        public static void Run(long* a, long* b, int n, int size, long* ta, long* tb, long* curT, long t0)
        {
            *curT = t0;
            for (int i = 0; i < n; i++) { ta[size + i] = a[i]; tb[size + i] = b[i]; }
            for (int i = size + n; i < size + size; i++) { ta[i] = 0L; tb[i] = long.MaxValue; }
            for (int i = size - 1; i >= 1; i--) CombineAt(ta, tb, i, t0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void CombineAt(long* ta, long* tb, int i, long t)
        {
            int l = i << 1;
            int r = l | 1;
            long vl = ta[l] * t + tb[l];
            long vr = ta[r] * t + tb[r];
            if (vl <= vr) { ta[i] = ta[l]; tb[i] = tb[l]; }
            else { ta[i] = ta[r]; tb[i] = tb[r]; }
        }
    }

    public static unsafe class KineticSetTime
    {
        public static void Run(long* ta, long* tb, int size, long* curT, long newT)
        {
            *curT = newT;
            for (int i = size - 1; i >= 1; i--) KineticTournamentBuild.CombineAt(ta, tb, i, newT);
        }
    }

    public static unsafe class KineticTournamentUpdate
    {
        public static void Run(int size, long* ta, long* tb, long* curT, int idx, long a, long b)
        {
            long t = *curT;
            int p = size + idx;
            ta[p] = a; tb[p] = b;
            for (p >>= 1; p >= 1; p >>= 1) KineticTournamentBuild.CombineAt(ta, tb, p, t);
        }
    }

    public static unsafe class KineticTournamentWinner
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Run(long* ta, long* tb, long* curT)
        {
            return ta[1] * (*curT) + tb[1];
        }
    }

    public static unsafe class KineticSegmentTreeBuild
    {
        public static void Run(long* a, long* b, int n, int size, long* ta, long* tb, long* curT, long t0)
        {
            KineticTournamentBuild.Run(a, b, n, size, ta, tb, curT, t0);
        }
    }

    public static unsafe class KineticSegmentTreeQuery
    {
        public static long Run(int size, long* ta, long* tb, long* curT, int l, int r)
        {
            long t = *curT;
            long best = long.MaxValue;
            int lo = l + size;
            int hi = r + size;
            while (lo <= hi)
            {
                if ((lo & 1) == 1)
                {
                    long v = ta[lo] * t + tb[lo];
                    if (v < best) best = v;
                    lo++;
                }
                if ((hi & 1) == 0)
                {
                    long v = ta[hi] * t + tb[hi];
                    if (v < best) best = v;
                    hi--;
                }
                lo >>= 1; hi >>= 1;
            }
            return best;
        }
    }
}
