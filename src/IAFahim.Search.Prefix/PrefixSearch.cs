using System;
using System.Runtime.CompilerServices;

namespace IAFahim.Search.Prefix
{
    public static unsafe class PrefixSearch
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int LongestCommonPrefix(byte* a, byte* b, int maxLen)
        {
            int i = 0;
            while (i < maxLen && a[i] == b[i])
            {
                i++;
            }
            return i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Match(byte* text, int textLen, byte* pattern, int patLen)
        {
            if (patLen > textLen)
            {
                return false;
            }
            for (int i = 0; i <= textLen - patLen; i++)
            {
                bool ok = true;
                for (int j = 0; j < patLen; j++)
                {
                    if (text[i + j] != pattern[j])
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FindFirst(byte* text, int textLen, byte* pattern, int patLen)
        {
            if (patLen > textLen || patLen == 0)
            {
                return -1;
            }
            for (int i = 0; i <= textLen - patLen; i++)
            {
                bool ok = true;
                for (int j = 0; j < patLen; j++)
                {
                    if (text[i + j] != pattern[j])
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    return i;
                }
            }
            return -1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CountOccurrences(byte* text, int textLen, byte* pattern, int patLen)
        {
            if (patLen > textLen || patLen == 0)
            {
                return 0;
            }
            int count = 0;
            for (int i = 0; i <= textLen - patLen; i++)
            {
                bool ok = true;
                for (int j = 0; j < patLen; j++)
                {
                    if (text[i + j] != pattern[j])
                    {
                        ok = false;
                        break;
                    }
                }
                if (ok)
                {
                    count++;
                }
            }
            return count;
        }
    }
}