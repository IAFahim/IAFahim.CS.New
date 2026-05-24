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
                v = GetOrAddNextState(st, ref size, v, pattern[i]);
            }
            st[v].Out = patternId;
        }

        private static int GetOrAddNextState(State* st, ref int size, int v, int c)
        {
            int next = GetNext(st, v, c);
            if (next == -1)
            {
                next = size++;
                SetNext(st, v, c, next);
                InitializeState(st, next);
            }
            return next;
        }

        private static void InitializeState(State* st, int idx)
        {
            st[idx].Next0 = -1; st[idx].Link = 0; st[idx].Out = -1;
        }

        public static void BuildLinks(State* st, int size, int sigma, int* queue)
        {
            int qh = 0, qt = 0;
            InitializeLinkQueue(st, sigma, queue, ref qt);
            while (qh < qt)
            {
                int v = queue[qh++];
                UpdateChildLinks(st, v, sigma, queue, ref qt);
            }
        }

        private static void InitializeLinkQueue(State* st, int sigma, int* queue, ref int qt)
        {
            queue[qt++] = 0;
        }

        private static void UpdateChildLinks(State* st, int v, int sigma, int* queue, ref int qt)
        {
            for (int c = 0; c < sigma; c++)
            {
                int u = GetNext(st, v, c);
                if (u == -1) continue;
                int link = st[v].Link;
                while (link != 0 && GetNext(st, link, c) == -1) link = st[link].Link;
                st[u].Link = (v == 0 || GetNext(st, v, c) == u) ? 0 : GetNext(st, link, c);
                if (st[st[u].Link].Out != -1) st[u].Out = st[st[u].Link].Out;
                queue[qt++] = u;
            }
        }

        public static int Search(State* st, int sigma, byte* text, int textLen, int* matches)
        {
            int v = 0, matchCount = 0;
            for (int i = 0; i < textLen; i++)
            {
                v = Transition(st, v, text[i]);
                if (st[v].Out != -1) matches[matchCount++] = st[v].Out;
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
