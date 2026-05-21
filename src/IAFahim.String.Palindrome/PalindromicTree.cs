namespace IAFahim.String.Palindrome
{
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
    using System;

    public static unsafe class PalindromicTree
    {
        public struct Node
        {
            public int Next0;
            public int Len;
            public int Link;
            public long Occ;
        }

        private static Node* _node;
        private static int _size;
        private static int _last;
        private static int _len;

        public static void Build(byte* s, int len)
        {
            _len = len;
            _node = (Node*)Marshal.AllocHGlobal(sizeof(Node) * (len + 3));
            _size = 2;
            _node[0].Len = -1;
            _node[0].Link = 0;
            _node[0].Next0 = -1;
            _node[0].Occ = 0;
            _node[1].Len = 0;
            _node[1].Link = 0;
            _node[1].Next0 = -1;
            _node[1].Occ = 0;
            _last = 1;
            for (int i = 0; i < len; i++)
                Extend(s[i]);
        }

        private static void Extend(byte c)
        {
            int cur = _size++;
            _node[cur].Len = _node[_last].Len + 2;
            _node[cur].Next0 = -1;
            _node[cur].Occ = 1;
            int p = _last;
            while (p >= 0 && GetNext(p, c) == -1)
            {
                SetNext(p, c, cur);
                p = _node[p].Link;
            }
            if (p == -1)
            {
                _node[cur].Link = 1;
            }
            else
            {
                int q = GetNext(p, c);
                if (_node[p].Len + 2 == _node[q].Len)
                {
                    _node[cur].Link = q;
                }
                else
                {
                    int clone = _size++;
                    _node[clone] = _node[q];
                    _node[clone].Len = _node[p].Len + 2;
                    while (p >= 0 && GetNext(p, c) == q)
                    {
                        SetNext(p, c, clone);
                        p = _node[p].Link;
                    }
                    _node[q].Link = _node[cur].Link = clone;
                }
            }
            _last = cur;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNext(int v, int c)
        {
            var ptr = ((IntPtr)_node + v * sizeof(Node) + sizeof(int));
            return ((int*)ptr)[c];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNext(int v, int c, int next)
        {
            var ptr = ((IntPtr)_node + v * sizeof(Node) + sizeof(int));
            ((int*)ptr)[c] = next;
        }

        public static int DistinctCount() => _size - 2;
    }
}
