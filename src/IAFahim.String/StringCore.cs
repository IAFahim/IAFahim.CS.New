namespace IAFahim.String
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ManacherOdd
    {
        public static void Run(byte* s, int len, int* radii)
        {
            int l = 0, r = -1;
            for (int i = 0; i < len; i++)
            {
                int k = i > r ? 1 : Math.Min(radii[l + r - i], r - i + 1);
                while (i - k >= 0 && i + k < len && s[i - k] == s[i + k]) k++;
                radii[i] = k--;
                if (i + k > r)
                {
                    l = i - k;
                    r = i + k;
                }
            }
        }
    }

    public static unsafe class ManacherEven
    {
        public static void Run(byte* s, int len, int* radii)
        {
            int l = 0, r = -1;
            for (int i = 0; i < len; i++)
            {
                int k = i > r ? 0 : Math.Min(radii[l + r - i + 1], r - i + 1);
                while (i - k - 1 >= 0 && i + k < len && s[i - k - 1] == s[i + k]) k++;
                radii[i] = k--;
                if (i + k > r)
                {
                    l = i - k - 1;
                    r = i + k;
                }
            }
        }
    }

    public static unsafe class DuvalLyndon
    {
        public static int Run(byte* s, int len, int* starts, int* lengths)
        {
            int count = 0;
            int i = 0;
            while (i < len)
            {
                int j = i + 1, k = i;
                while (j < len && s[k] <= s[j])
                {
                    if (s[k] < s[j]) k = i;
                    else k++;
                    j++;
                }
                while (i <= k)
                {
                    starts[count] = i;
                    lengths[count] = j - k;
                    count++;
                    i += j - k;
                }
            }
            return count;
        }
    }

    public static unsafe class MinCyclicShift
    {
        public static int Run(byte* s, int len)
        {
            int i = 0, j = 1, k = 0;
            byte* doubled = stackalloc byte[len * 2];
            for (int x = 0; x < len * 2; x++) doubled[x] = s[x % len];
            while (i < len && j < len && k < len)
            {
                if (doubled[i + k] == doubled[j + k]) { k++; continue; }
                if (doubled[i + k] > doubled[j + k]) i += k + 1;
                else j += k + 1;
                if (i == j) j++;
                k = 0;
            }
            return i < j ? i : j;
        }
    }

    public static unsafe class RunLengthEncode
    {
        public static int Run(byte* s, int len, byte* values, int* counts)
        {
            if (len == 0) return 0;
            int count = 0;
            values[0] = s[0];
            counts[0] = 1;
            for (int i = 1; i < len; i++)
            {
                if (s[i] == values[count])
                    counts[count]++;
                else
                {
                    count++;
                    values[count] = s[i];
                    counts[count] = 1;
                }
            }
            return count + 1;
        }
    }

    public static unsafe class RunLengthDecode
    {
        public static int Run(byte* values, int* counts, int runCount, byte* dst)
        {
            int pos = 0;
            for (int i = 0; i < runCount; i++)
            {
                for (int j = 0; j < counts[i]; j++)
                    dst[pos++] = values[i];
            }
            return pos;
        }
    }

    public static unsafe class StringPeriod
    {
        public static int Run(byte* s, int len)
        {
            int* fail = stackalloc int[len];
            fail[0] = 0;
            for (int i = 1, j = 0; i < len; i++)
            {
                while (j > 0 && s[i] != s[j]) j = fail[j - 1];
                if (s[i] == s[j]) j++;
                fail[i] = j;
            }
            int p = len - fail[len - 1];
            return len % p == 0 ? p : len;
        }
    }

    public static unsafe class MinPeriod
    {
        public static int Run(byte* s, int len)
        {
            return StringPeriod.Run(s, len);
        }
    }

    public static unsafe class Borders
    {
        public static int Run(byte* s, int len, int* borders)
        {
            int* fail = stackalloc int[len];
            fail[0] = 0;
            for (int i = 1, j = 0; i < len; i++)
            {
                while (j > 0 && s[i] != s[j]) j = fail[j - 1];
                if (s[i] == s[j]) j++;
                fail[i] = j;
            }
            int count = 0;
            int b = fail[len - 1];
            while (b > 0)
            {
                borders[count++] = b;
                b = fail[b - 1];
            }
            return count;
        }
    }

    public static unsafe class CountOccurrences
    {
        public static int Run(byte* text, int textLen, byte* pattern, int patLen)
        {
            if (patLen == 0 || patLen > textLen) return 0;
            int count = 0;
            int* fail = stackalloc int[patLen];
            fail[0] = 0;
            for (int i = 1, j = 0; i < patLen; i++)
            {
                while (j > 0 && pattern[i] != pattern[j]) j = fail[j - 1];
                if (pattern[i] == pattern[j]) j++;
                fail[i] = j;
            }
            int k = 0;
            for (int i = 0; i < textLen; i++)
            {
                while (k > 0 && text[i] != pattern[k]) k = fail[k - 1];
                if (text[i] == pattern[k]) k++;
                if (k == patLen) { count++; k = fail[k - 1]; }
            }
            return count;
        }
    }
}