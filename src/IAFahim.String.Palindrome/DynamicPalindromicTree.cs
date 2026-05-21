namespace IAFahim.String.Palindrome
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class DynamicPalindromicTree
    {
        private static int _last;
        private static int _size;
        private static PalindromicTree.Node* _node;
        private static byte* _s;
        private static int _pos;

        public static void Init(int maxLen)
        {
            _node = (PalindromicTree.Node*)Marshal.AllocHGlobal(sizeof(PalindromicTree.Node) * (maxLen + 3));
            _s = (byte*)Marshal.AllocHGlobal(maxLen);
            _size = 2;
            _last = 1;
            _pos = 0;
            _node[0].Len = -1;
            _node[0].Link = 0;
            _node[0].Next0 = -1;
            _node[0].Occ = 0;
            _node[1].Len = 0;
            _node[1].Link = 0;
            _node[1].Next0 = -1;
            _node[1].Occ = 0;
        }

        public static int Add(byte c)
        {
            _s[_pos] = c;
            int cur = _last;
            while (true)
            {
                int checkLen = _node[cur].Len;
                if (_pos - 1 - checkLen >= 0 && _s[_pos - 1 - checkLen] == c)
                    break;
                cur = (int)_node[cur].Link;
            }
            if (((int*)((IntPtr)_node + cur * sizeof(PalindromicTree.Node) + sizeof(int)))[c] != -1)
            {
                int next = ((int*)((IntPtr)_node + cur * sizeof(PalindromicTree.Node) + sizeof(int)))[c];
                _node[next].Occ++;
                _last = next;
                _pos++;
                return _size - 2;
            }
            int state = _size++;
            _node[state].Len = _node[cur].Len + 2;
            _node[state].Next0 = -1;
            _node[state].Occ = 1;
            if (_node[state].Len == 1)
            {
                _node[state].Link = 1;
            }
            else
            {
                int link = (int)_node[cur].Link;
                while (true)
                {
                    int checkLen = _node[link].Len;
                    if (_pos - 1 - checkLen >= 0 && _s[_pos - 1 - checkLen] == c)
                        break;
                    link = (int)_node[link].Link;
                }
                _node[state].Link = ((int*)((IntPtr)_node + link * sizeof(PalindromicTree.Node) + sizeof(int)))[c];
            }
            ((int*)((IntPtr)_node + cur * sizeof(PalindromicTree.Node) + sizeof(int)))[c] = state;
            _last = state;
            _pos++;
            return _size - 2;
        }
    }
}
