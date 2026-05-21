namespace IAFahim.String.FMIndex
{
    using System;
    using System.Runtime.InteropServices;

    public static unsafe class FMIndex
    {
        private static int* _occ;
        private static int* _text;
        private static int _len;
        private static int _sigma;

        public static void Build(int* text, int len, int sigma)
        {
            _len = len;
            _sigma = sigma;
            _text = text;
            _occ = (int*)Marshal.AllocHGlobal(sizeof(int) * (len + 1) * sigma);
            for (int c = 0; c < sigma; c++)
                _occ[c * (len + 1)] = 0;
            for (int i = 0; i < len; i++)
            {
                for (int c = 0; c < sigma; c++)
                    _occ[c * (len + 1) + i + 1] = _occ[c * (len + 1) + i];
                _occ[text[i] * (len + 1) + i + 1]++;
            }
        }

        public static int Count(int* pattern, int patLen, int* sa)
        {
            int l = 0, r = _len;
            while (l < r)
            {
                int mid = (l + r) >> 1;
                int pos = sa[mid];
                int cmpLen = Math.Min(patLen, _len - pos);
                if (CompareRange(_text, pos, pattern, 0, cmpLen) >= 0)
                    r = mid;
                else
                    l = mid + 1;
            }
            int start = l;
            l = 0; r = _len;
            while (l < r)
            {
                int mid = (l + r) >> 1;
                int pos = sa[mid];
                int cmpLen = Math.Min(patLen, _len - pos);
                if (CompareRange(_text, pos, pattern, 0, cmpLen) > 0)
                    r = mid;
                else
                    l = mid + 1;
            }
            return l - start;
        }

        public static void Locate(int* pattern, int patLen, int* sa, int* result, int* count)
        {
            int l = 0, r = _len;
            while (l < r)
            {
                int mid = (l + r) >> 1;
                int pos = sa[mid];
                int cmpLen = Math.Min(patLen, _len - pos);
                if (CompareRange(_text, pos, pattern, 0, cmpLen) >= 0)
                    r = mid;
                else
                    l = mid + 1;
            }
            int start = l;
            while (r < _len && (_occ[r + 1] - _occ[start]) < patLen) r++;
            *count = r - start;
            for (int i = start; i <= r; i++)
                result[i - start] = sa[i];
        }

        private static int CompareRange(int* a, int aOff, int* b, int bOff, int len)
        {
            for (int i = 0; i < len; i++)
                if (a[aOff + i] != b[bOff + i])
                    return a[aOff + i] - b[bOff + i];
            return 0;
        }

        public static void Dispose()
        {
            if (_occ != null)
            {
                Marshal.FreeHGlobal((nint)_occ);
                _occ = null;
            }
        }
    }
}
