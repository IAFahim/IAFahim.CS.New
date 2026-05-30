namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SuffixAutomaton
    {
        public struct State
        {
            public int Link;
            public int Len;
            public int Head;
        }

        public struct Edge
        {
            public int To;
            public int Char;
            public int Next;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNext(State* st, Edge* e, int v, int c)
        {
            for (int edge = st[v].Head; edge != -1; edge = e[edge].Next)
                if (e[edge].Char == c) return e[edge].To;
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNext(State* st, Edge* e, ref int edgeCount, int v, int c, int to)
        {
            e[edgeCount] = new Edge { Char = c, To = to, Next = st[v].Head };
            st[v].Head = edgeCount++;
        }

        public static void Build(int* ptr, int len, State* st, Edge* e, ref int size, ref int last, ref int edgeCount)
        {
            size = 1; edgeCount = 0;
            last = 0;
            st[0].Len = 0; st[0].Link = -1; st[0].Head = -1;
            for (int i = 0; i < len; i++) Extend(ptr[i], st, e, ref size, ref last, ref edgeCount);
        }

        private static void Extend(int c, State* st, Edge* e, ref int size, ref int last, ref int edgeCount)
        {
            int cur = size++;
            st[cur].Len = st[last].Len + 1;
            st[cur].Link = 0;
            st[cur].Head = -1;

            int p = last;
            while (p != -1 && GetNext(st, e, p, c) == -1)
            {
                SetNext(st, e, ref edgeCount, p, c, cur);
                p = st[p].Link;
            }

            if (p == -1)
            {
                st[cur].Link = 0;
            }
            else
            {
                int q = GetNext(st, e, p, c);
                if (st[p].Len + 1 == st[q].Len)
                {
                    st[cur].Link = q;
                }
                else
                {
                    int clone = size++;
                    st[clone].Len = st[p].Len + 1;
                    st[clone].Head = st[q].Head;
                    st[clone].Link = st[q].Link;

                    while (p != -1 && GetNext(st, e, p, c) == q)
                    {
                        SetNext(st, e, ref edgeCount, p, c, clone);
                        p = st[p].Link;
                    }

                    st[q].Link = st[cur].Link = clone;
                }
            }
            last = cur;
        }

        public static int Length(State* st, int v) => st[v].Len;

        public static int Link(State* st, int v) => st[v].Link;

        public static int Transition(State* st, Edge* e, int v, int c) => GetNext(st, e, v, c);

        public static bool Contains(State* st, Edge* e, int v, int c) => GetNext(st, e, v, c) != -1;

        public static void EnumerateTransitions(State* st, Edge* e, int v, int* chars, int* targets, int* count)
        {
            int cnt = 0;
            for (int edge = st[v].Head; edge != -1; edge = e[edge].Next)
            {
                chars[cnt] = e[edge].Char;
                targets[cnt] = e[edge].To;
                cnt++;
            }
            *count = cnt;
        }
    }
}
