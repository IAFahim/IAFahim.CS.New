namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class SuffixAutomaton
    {
        public struct State { public int Next0, Link, Len; }

        public static void Build(int* ptr, int len, State* st, ref int size, ref int last)
        {
            size = 1; last = 0; st[0].Len = 0; st[0].Link = -1; st[0].Next0 = -1;
            for (int i = 0; i < len; i++) Extend(ptr[i], st, ref size, ref last);
        }

        private static void Extend(int c, State* st, ref int size, ref int last)
        {
            int cur = size++; st[cur].Len = st[last].Len + 1; st[cur].Next0 = -1;
            int p = FindLinkWithNoNext(last, c, st, cur);
            if (p == -1) { st[cur].Link = 0; }
            else
            {
                int q = GetNext(st, p, c);
                if (st[p].Len + 1 == st[q].Len) { st[cur].Link = q; }
                else { int clone = CloneState(p, q, st, ref size, c, cur); st[q].Link = st[cur].Link = clone; }
            }
            last = cur;
        }

        private static int FindLinkWithNoNext(int last, int c, State* st, int cur)
        {
            int p = last;
            while (p != -1 && GetNext(st, p, c) == -1) { SetNext(st, p, c, cur); p = st[p].Link; }
            return p;
        }

        private static int CloneState(int p, int q, State* st, ref int size, int c, int cur)
        {
            int clone = size++; st[clone] = st[q]; st[clone].Len = st[p].Len + 1;
            while (p != -1 && GetNext(st, p, c) == q) { SetNext(st, p, c, clone); p = st[p].Link; }
            return clone;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNext(State* st, int state, int c) => ((int*)((IntPtr)st + state * sizeof(State) + sizeof(int)))[c];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNext(State* st, int state, int c, int next) => ((int*)((IntPtr)st + state * sizeof(State) + sizeof(int)))[c] = next;
    }
}
