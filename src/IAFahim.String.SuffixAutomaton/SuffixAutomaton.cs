namespace IAFahim.String.SuffixAutomaton
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class SuffixAutomaton
    {
        public struct State
        {
            public int Next0;
            public int Link;
            public int Len;
        }

        private static int _size;
        private static int _last;
        private static State* _st;

        public static void Build(int* ptr, int len)
        {
            _size = 1;
            _last = 0;
            _st = (State*)Marshal.AllocHGlobal(sizeof(State) * len * 2);
            _st[0].Len = 0;
            _st[0].Link = -1;
            _st[0].Next0 = -1;
            for (int i = 0; i < len; i++)
                Extend(ptr[i]);
        }

        private static void Extend(int c)
        {
            int cur = _size++;
            _st[cur].Len = _st[_last].Len + 1;
            _st[cur].Next0 = -1;
            int p = _last;
            while (p != -1 && GetNext(p, c) == -1)
            {
                SetNext(p, c, cur);
                p = _st[p].Link;
            }
            if (p == -1)
            {
                _st[cur].Link = 0;
            }
            else
            {
                int q = GetNext(p, c);
                if (_st[p].Len + 1 == _st[q].Len)
                {
                    _st[cur].Link = q;
                }
                else
                {
                    int clone = _size++;
                    _st[clone] = _st[q];
                    _st[clone].Len = _st[p].Len + 1;
                    while (p != -1 && GetNext(p, c) == q)
                    {
                        SetNext(p, c, clone);
                        p = _st[p].Link;
                    }
                    _st[q].Link = _st[cur].Link = clone;
                }
            }
            _last = cur;
        }

        private static int GetNext(int state, int c)
        {
            return ((int*)((IntPtr)_st + state * sizeof(State) + sizeof(int)))[c];
        }

        private static void SetNext(int state, int c, int next)
        {
            ((int*)((IntPtr)_st + state * sizeof(State) + sizeof(int)))[c] = next;
        }
    }
}
