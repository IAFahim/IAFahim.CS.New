namespace IAFahim.String.FMIndex.Tests
{
    using IAFahim.String.FMIndex;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class FmBackwardSearchTests
    {
        [Test]
        public void BackwardSearch_MatchesSaCount_AndBrute()
        {
            int[] baseText = { 2, 1, 3, 1, 2, 1, 3, 2, 1 };
            int sigma = 5;
            int sentinel = 0;
            int len = baseText.Length + 1;
            int* text = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            int* sa = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            int* bwt = (int*)Marshal.AllocHGlobal(sizeof(int) * len);
            int* occ = (int*)Marshal.AllocHGlobal(sizeof(int) * sigma * (len + 1));
            int* c = (int*)Marshal.AllocHGlobal(sizeof(int) * (sigma + 1));
            try
            {
                for (int i = 0; i < baseText.Length; i++) text[i] = baseText[i];
                text[baseText.Length] = sentinel;
                NaiveSa(text, len, sa);

                FmBackwardSearch.Build(text, len, sigma, sa, bwt, occ, c);

                int[][] patterns =
                {
                    new[] { 1 }, new[] { 2 }, new[] { 3 }, new[] { 1, 2 },
                    new[] { 2, 1 }, new[] { 1, 3, 1 }, new[] { 2, 1, 3 },
                    new[] { 1, 2, 1 }, new[] { 3, 1, 2, 1 }, new[] { 4 }
                };
                foreach (int[] pat in patterns)
                {
                    fixed (int* pp = pat)
                    {
                        int fmCount = FmBackwardSearch.Count(occ, c, len, sigma, pp, pat.Length);
                        int saCount = FMIndex.Count(text, len, pp, pat.Length, sa);
                        int brute = BruteCount(baseText, pat);
                        Assert.AreEqual(brute, fmCount, $"FM count pat={Fmt(pat)}");
                        Assert.AreEqual(brute, saCount, $"SA count pat={Fmt(pat)}");
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)sa);
                Marshal.FreeHGlobal((nint)bwt);
                Marshal.FreeHGlobal((nint)occ);
                Marshal.FreeHGlobal((nint)c);
            }
        }

        private static void NaiveSa(int* text, int len, int* sa)
        {
            int[] idx = new int[len];
            for (int i = 0; i < len; i++) idx[i] = i;
            int[] arr = idx;
            Array.Sort(arr, (x, y) =>
            {
                int n = len;
                for (int k = 0; x + k < n && y + k < n; k++)
                {
                    if (text[x + k] != text[y + k]) return text[x + k] - text[y + k];
                }
                return (n - x) - (n - y);
            });
            for (int i = 0; i < len; i++) sa[i] = arr[i];
        }

        private static int BruteCount(int[] text, int[] pat)
        {
            int count = 0;
            for (int i = 0; i + pat.Length <= text.Length; i++)
            {
                bool ok = true;
                for (int j = 0; j < pat.Length; j++) if (text[i + j] != pat[j]) { ok = false; break; }
                if (ok) count++;
            }
            return count;
        }

        private static string Fmt(int[] a) => string.Join(",", a);
    }
}
