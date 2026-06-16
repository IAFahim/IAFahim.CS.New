namespace IAFahim.String.Palindrome
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class DynamicPalindromicTree
    {
        private const int Alphabet = 256;
        private const int NoEdge = -1;

        private static int _last;
        private static int _size;
        private static PalindromicTree.Node* _node;
        private static int* _next;
        private static byte* _s;
        private static int _pos;

        public static void Init(int maxLen)
        {
            int nodeCount = maxLen + 3;
            _node = (PalindromicTree.Node*)Marshal.AllocHGlobal(sizeof(PalindromicTree.Node) * nodeCount);
            _next = (int*)Marshal.AllocHGlobal(sizeof(int) * nodeCount * Alphabet);
            _s = (byte*)Marshal.AllocHGlobal(maxLen);
            int transitionCount = nodeCount * Alphabet;
            for (int i = 0; i < transitionCount; i++)
                _next[i] = NoEdge;
            _size = 2;
            _last = 1;
            _pos = 0;
            _node[0].Len = -1;
            _node[0].Link = 0;
            _node[0].Next0 = NoEdge;
            _node[0].Occ = 0;
            _node[1].Len = 0;
            _node[1].Link = 0;
            _node[1].Next0 = NoEdge;
            _node[1].Occ = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetNext(int v, int c)
        {
            return _next[v * Alphabet + c];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNext(int v, int c, int next)
        {
            _next[v * Alphabet + c] = next;
        }

        public static int Add(byte c)
        {
            _s[_pos] = c;
            int posM1 = _pos - 1;
            int cur = _last;
            while (true)
            {
                int checkLen = _node[cur].Len;
                if (posM1 - checkLen >= 0 && _s[posM1 - checkLen] == c)
                    break;
                cur = _node[cur].Link;
            }

            int existing = GetNext(cur, c);
            if (existing != NoEdge)
            {
                _node[existing].Occ++;
                _last = existing;
                _pos++;
                return _size - 2;
            }

            int state = _size++;
            _node[state].Len = _node[cur].Len + 2;
            _node[state].Next0 = NoEdge;
            _node[state].Occ = 1;
            if (_node[state].Len == 1)
            {
                _node[state].Link = 1;
            }
            else
            {
                int link = _node[cur].Link;
                while (true)
                {
                    int checkLen = _node[link].Len;
                    if (posM1 - checkLen >= 0 && _s[posM1 - checkLen] == c)
                        break;
                    link = _node[link].Link;
                }
                _node[state].Link = GetNext(link, c);
            }
            SetNext(cur, c, state);
            _last = state;
            _pos++;
            return _size - 2;
        }
    }
}
