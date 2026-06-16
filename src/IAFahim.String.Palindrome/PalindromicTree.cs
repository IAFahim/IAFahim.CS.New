namespace IAFahim.String.Palindrome
{
    using System.Runtime.InteropServices;

    public static unsafe class PalindromicTree
    {
        private const int Alphabet = 256;
        private const int NoEdge = -1;

        public struct Node
        {
            public int Next0;
            public int Len;
            public int Link;
            public long Occ;
        }

        private static Node* _node;
        private static int* _next;
        private static int _size;
        private static int _last;
        private static int _len;

        public static void Build(byte* s, int len)
        {
            _len = len;
            int nodeCount = len + 3;
            _node = (Node*)Marshal.AllocHGlobal(sizeof(Node) * nodeCount);
            _next = (int*)Marshal.AllocHGlobal(sizeof(int) * nodeCount * Alphabet);
            int transitionCount = nodeCount * Alphabet;
            for (int i = 0; i < transitionCount; i++)
                _next[i] = NoEdge;
            _size = 2;
            _node[0].Len = -1;
            _node[0].Link = 0;
            _node[0].Next0 = NoEdge;
            _node[0].Occ = 0;
            _node[1].Len = 0;
            _node[1].Link = 0;
            _node[1].Next0 = NoEdge;
            _node[1].Occ = 0;
            _last = 1;
            for (int i = 0; i < len; i++)
                Extend(s, i);
        }

        private static void Extend(byte* s, int i)
        {
            Node* node = _node;
            int* next = _next;
            byte c = s[i];
            int posM1 = i - 1;

            int cur = _last;
            while (true)
            {
                int checkLen = node[cur].Len;
                if (posM1 - checkLen >= 0 && s[posM1 - checkLen] == c)
                    break;
                cur = node[cur].Link;
            }

            int existing = next[cur * Alphabet + c];
            if (existing != NoEdge)
            {
                node[existing].Occ++;
                _last = existing;
                return;
            }

            int state = _size++;
            node[state].Len = node[cur].Len + 2;
            node[state].Next0 = NoEdge;
            node[state].Occ = 1;
            if (node[state].Len == 1)
            {
                node[state].Link = 1;
            }
            else
            {
                int link = node[cur].Link;
                while (true)
                {
                    int checkLen = node[link].Len;
                    if (posM1 - checkLen >= 0 && s[posM1 - checkLen] == c)
                        break;
                    link = node[link].Link;
                }
                node[state].Link = next[link * Alphabet + c];
            }
            next[cur * Alphabet + c] = state;
            _last = state;
        }

        public static int DistinctCount() => _size - 2;
    }
}
