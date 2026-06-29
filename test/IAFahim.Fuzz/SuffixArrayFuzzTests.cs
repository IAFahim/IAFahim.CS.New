namespace IAFahim.Fuzz
{
    using IAFahim.String.SuffixArray;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SuffixArrayFuzzTests
    {
        [Test]
        public void SuffixArray_SmallAlphabetMatchesBruteOracle_Fuzz()
        {
            RunSuffixArrayFuzz(2000, 7777, 96, 4);
        }

        [Test]
        public void SuffixArray_FullByteAlphabetMatchesBruteOracle_Fuzz()
        {
            RunSuffixArrayFuzz(50, 8888, 256, 256);
        }

        private static void RunSuffixArrayFuzz(int iterations, int seed, int maxLength, int alphabetSize)
        {
            const int ByteAlphabetSize = 256;
            Random rng = new Random(seed);
            for (int it = 0; it < iterations; it++)
            {
                int len = rng.Next(0, maxLength + 1);
                int intLength = len > 0 ? len : 1;
                int rankLength = len > 0 ? len * 2 : 1;
                int countLength = len > ByteAlphabetSize ? len : ByteAlphabetSize;
                byte* text = (byte*)Marshal.AllocHGlobal(intLength);
                int* sa = (int*)Marshal.AllocHGlobal(sizeof(int) * intLength);
                int* rank = (int*)Marshal.AllocHGlobal(sizeof(int) * rankLength);
                int* tmpSa = (int*)Marshal.AllocHGlobal(sizeof(int) * intLength);
                int* count = (int*)Marshal.AllocHGlobal(sizeof(int) * countLength);
                int* tmpRank = (int*)Marshal.AllocHGlobal(sizeof(int) * intLength);
                int* expected = (int*)Marshal.AllocHGlobal(sizeof(int) * intLength);
                try
                {
                    for (int i = 0; i < len; i++) text[i] = (byte)rng.Next(0, alphabetSize);

                    SuffixArray.Build(text, len, sa, rank, tmpSa, count, tmpRank);
                    BuildNaiveSuffixArray(text, len, expected);

                    for (int i = 0; i < len; i++)
                        Assert.AreEqual(expected[i], sa[i], $"SuffixArray mismatch it={it} len={len} idx={i} seed={seed}");
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)text);
                    Marshal.FreeHGlobal((nint)sa);
                    Marshal.FreeHGlobal((nint)rank);
                    Marshal.FreeHGlobal((nint)tmpSa);
                    Marshal.FreeHGlobal((nint)count);
                    Marshal.FreeHGlobal((nint)tmpRank);
                    Marshal.FreeHGlobal((nint)expected);
                }
            }
        }

        private static void BuildNaiveSuffixArray(byte* text, int len, int* sa)
        {
            for (int i = 0; i < len; i++) sa[i] = i;

            for (int i = 1; i < len; i++)
            {
                int value = sa[i];
                int j = i - 1;
                while (j >= 0 && CompareSuffixes(text, len, value, sa[j]) < 0)
                {
                    sa[j + 1] = sa[j];
                    j--;
                }
                sa[j + 1] = value;
            }
        }

        private static int CompareSuffixes(byte* text, int len, int left, int right)
        {
            int i = left;
            int j = right;
            while (i < len && j < len)
            {
                int diff = text[i] - text[j];
                if (diff != 0) return diff;
                i++;
                j++;
            }
            if (i == len && j == len) return 0;
            return i == len ? -1 : 1;
        }
    }
}
