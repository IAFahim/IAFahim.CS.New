namespace IAFahim.String.Match
{
using System.Runtime.InteropServices;
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class AhoCorasick
    {
        public struct State
        {
            public int Next0;
            public int Link;
            public int Out;
        }

        public static void Build(State* st, ref int size, int sigma)
        {
            size = 1;
            st[0].Next0 = -1;
            st[0].Link = 0;
            st[0].Out = -1;
        }

        public static void AddPattern(State* st, ref int size, int sigma, int* pattern, int len, int patternId)
        {
            int v = 0;
            for (int i = 0; i < len; i++)
            {
                int c = pattern[i];
                int next = GetNext(st, v, c);
                if (next == -1)
                {
                    next = size++;
                    SetNext(st, v, c, next);
                    st[next].Next0 = -1;
                    st[next].Link = 0;
                    st[next].Out = -1;
                }
                v = next;
            }
            st[v].Out = patternId;
        }

        public static void BuildLinks(State* st, int size, int sigma, int* queue)
        {
            int head = 0, tail = 0;
            queue[tail++] = 0;
            while (head < tail)
            {
                int v = queue[head++];
                for (int c = 0; c < sigma; c++)
                {
                    int u = GetNext(st, v, c);
                    if (u == -1) continue;
                    int link = st[v].Link;
                    while (link != 0 && GetNext(st, link, c) == -1)
                        link = st[link].Link;
                    if (v == 0 || GetNext(st, v, c) == u)
                        st[u].Link = 0;
                    else
                        st[u].Link = GetNext(st, link, c);
                    if (st[st[u].Link].Out != -1)
                        st[u].Out = st[st[u].Link].Out;
                    queue[tail++] = u;
                }
            }
        }

        public static int Search(State* st, int sigma, byte* text, int textLen, int* matches)
        {
            int v = 0, matchCount = 0;
            for (int i = 0; i < textLen; i++)
            {
                int c = text[i];
                while (v != 0 && GetNext(st, v, c) == -1)
                    v = st[v].Link;
                if (GetNext(st, v, c) != -1)
                    v = GetNext(st, v, c);
                if (st[v].Out != -1)
                    matches[matchCount++] = st[v].Out;
            }
            return matchCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNext(State* st, int v, int c)
        {
            var ptr = ((IntPtr)st + v * sizeof(State) + sizeof(int));
            return ((int*)ptr)[c];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNext(State* st, int v, int c, int next)
        {
            var ptr = ((IntPtr)st + v * sizeof(State) + sizeof(int));
            ((int*)ptr)[c] = next;
        }
    }
}
