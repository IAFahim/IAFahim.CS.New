namespace IAFahim.String.Match
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class AhoCorasick
    {
        public struct State
        {
            public int Link;
            public int Out;
            public int ExitLink;
            public fixed int Next[256];
        }

        public static void Build(State* st, ref int size, int sigma)
        {
            size = 1;
            InitializeState(st, 0, sigma);
        }

        public static void AddPattern(State* st, ref int size, int sigma, int* pattern, int len, int patternId)
        {
            int v = 0;
            for (int i = 0; i < len; i++)
            {
                v = GetOrAddNextState(st, ref size, v, pattern[i], sigma);
            }
            st[v].Out = patternId;
        }

        private static int GetOrAddNextState(State* st, ref int size, int v, int c, int sigma)
        {
            int next = GetNext(st, v, c);
            if (next == -1)
            {
                next = size++;
                SetNext(st, v, c, next);
                InitializeState(st, next, sigma);
            }
            return next;
        }

        private static void InitializeState(State* st, int idx, int sigma)
        {
            st[idx].Link = 0;
            st[idx].Out = -1;
            st[idx].ExitLink = -1;
            for (int i = 0; i < sigma; i++)
            {
                st[idx].Next[i] = -1;
            }
        }

        public static void BuildLinks(State* st, int size, int sigma, int* queue)
        {
            int qh = 0, qt = 0;
            queue[qt++] = 0;
            while (qh < qt)
            {
                int v = queue[qh++];
                for (int c = 0; c < sigma; c++)
                {
                    int u = GetNext(st, v, c);
                    if (u == -1) continue;
                    int link = st[v].Link;
                    while (link != 0 && GetNext(st, link, c) == -1) link = st[link].Link;
                    st[u].Link = (v == 0) ? 0 : (GetNext(st, link, c) != -1 ? GetNext(st, link, c) : 0);
                    st[u].ExitLink = st[st[u].Link].Out != -1 ? st[u].Link : st[st[u].Link].ExitLink;
                    queue[qt++] = u;
                }
            }
        }

        public static int Search(State* st, int sigma, byte* text, int textLen, int* matches)
        {
            int v = 0, matchCount = 0;
            for (int i = 0; i < textLen; i++)
            {
                v = Transition(st, v, text[i]);
                if (st[v].Out != -1) matches[matchCount++] = st[v].Out;
                int exit = st[v].ExitLink;
                while (exit != -1)
                {
                    if (st[exit].Out != -1) matches[matchCount++] = st[exit].Out;
                    exit = st[exit].ExitLink;
                }
            }
            return matchCount;
        }

        private static int Transition(State* st, int v, int c)
        {
            while (v != 0 && GetNext(st, v, c) == -1) v = st[v].Link;
            return GetNext(st, v, c) != -1 ? GetNext(st, v, c) : 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNext(State* st, int v, int c)
        {
            return st[v].Next[c];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNext(State* st, int v, int c, int next)
        {
            st[v].Next[c] = next;
        }
    }
}
