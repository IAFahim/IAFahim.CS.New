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

        private static State* _st;
        private static int _size;
        private static int _sigma;

        public static void Build(int sigma, int initialCapacity)
        {
            _sigma = sigma;
            _size = 1;
            _st = (State*)Marshal.AllocHGlobal(sizeof(State) * initialCapacity);
            _st[0].Next0 = -1;
            _st[0].Link = 0;
            _st[0].Out = -1;
        }

        public static void AddPattern(int* pattern, int len, int patternId)
        {
            int v = 0;
            for (int i = 0; i < len; i++)
            {
                int c = pattern[i];
                int next = GetNext(v, c);
                if (next == -1)
                {
                    next = _size++;
                    SetNext(v, c, next);
                    _st[next].Next0 = -1;
                    _st[next].Link = 0;
                    _st[next].Out = -1;
                }
                v = next;
            }
            _st[v].Out = patternId;
        }

        public static void BuildLinks()
        {
            int* queue = (int*)Marshal.AllocHGlobal(sizeof(int) * _size);
            int head = 0, tail = 0;
            queue[tail++] = 0;
            while (head < tail)
            {
                int v = queue[head++];
                for (int c = 0; c < _sigma; c++)
                {
                    int u = GetNext(v, c);
                    if (u == -1) continue;
                    int link = _st[v].Link;
                    while (link != 0 && GetNext(link, c) == -1)
                        link = _st[link].Link;
                    if (v == 0 || GetNext(v, c) == u)
                        _st[u].Link = 0;
                    else
                        _st[u].Link = GetNext(link, c);
                    if (_st[_st[u].Link].Out != -1)
                        _st[u].Out = _st[_st[u].Link].Out;
                    queue[tail++] = u;
                }
            }
            Marshal.FreeHGlobal((nint)queue);
        }

        public static int Search(byte* text, int textLen, int* matches)
        {
            int v = 0, matchCount = 0;
            for (int i = 0; i < textLen; i++)
            {
                int c = text[i];
                while (v != 0 && GetNext(v, c) == -1)
                    v = _st[v].Link;
                if (GetNext(v, c) != -1)
                    v = GetNext(v, c);
                if (_st[v].Out != -1)
                    matches[matchCount++] = _st[v].Out;
            }
            return matchCount;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNext(int v, int c)
        {
            var ptr = ((IntPtr)_st + v * sizeof(State) + sizeof(int));
            return ((int*)ptr)[c];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNext(int v, int c, int next)
        {
            var ptr = ((IntPtr)_st + v * sizeof(State) + sizeof(int));
            ((int*)ptr)[c] = next;
        }
    }
}
