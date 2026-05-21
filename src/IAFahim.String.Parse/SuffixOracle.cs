namespace IAFahim.String.Parse
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class SuffixOracle
    {
        private static int* _link;
        private static int* _len;
        private static int* _trans;
        private static int _size;
        private static int _last;
        private static int _sigma;

        public static void Build(byte* text, int len, int sigma)
        {
            _sigma = sigma;
            _size = 2;
            _last = 1;
            int states = len * 2 + 2;
            _link = (int*)Marshal.AllocHGlobal(sizeof(int) * states);
            _len = (int*)Marshal.AllocHGlobal(sizeof(int) * states);
            _trans = (int*)Marshal.AllocHGlobal(sizeof(int) * states * sigma);
            for (int i = 0; i < states * sigma; i++) _trans[i] = -1;
            _link[0] = -1; _len[0] = -1;
            _link[1] = 0; _len[1] = 0;
            for (int i = 0; i < len; i++)
                Extend(text[i]);
        }

        private static void Extend(byte c)
        {
            int cur = _size++;
            _len[cur] = _len[_last] + 1;
            int p = _last;
            while (p != -1 && _trans[p * _sigma + c] == -1)
            {
                _trans[p * _sigma + c] = cur;
                p = _link[p];
            }
            if (p == -1)
            {
                _link[cur] = 1;
            }
            else
            {
                int q = _trans[p * _sigma + c];
                if (_len[p] + 1 == _len[q])
                    _link[cur] = q;
                else
                {
                    int clone = _size++;
                    _len[clone] = _len[p] + 1;
                    _link[clone] = _link[q];
                    for (int i = 0; i < _sigma; i++)
                        _trans[clone * _sigma + i] = _trans[q * _sigma + i];
                    while (p != -1 && _trans[p * _sigma + c] == q)
                    {
                        _trans[p * _sigma + c] = clone;
                        p = _link[p];
                    }
                    _link[q] = _link[cur] = clone;
                }
            }
            _last = cur;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Contains(byte* pattern, int patLen)
        {
            int v = 1;
            for (int i = 0; i < patLen; i++)
            {
                v = _trans[v * _sigma + pattern[i]];
                if (v == -1) return false;
            }
            return true;
        }
    }
}
