namespace IAFahim.DS.WaveletMatrix.Tests
{
    using IAFahim.DS.WaveletMatrix;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class WaveletMatrixAdvancedTests
    {
        private const int N = 64;
        private const int MaxVal = 100;
        private const int Log = 7;

        private int[] _data;

        [SetUp]
        public void Setup()
        {
            Random rng = new Random(123);
            _data = new int[N];
            for (int i = 0; i < N; i++) _data[i] = rng.Next(0, MaxVal);
        }

        private void Build(int* data, int n, int* bitmaps, int* ranks, int* mids)
        {
            WaveletMatrixBuild.Run(data, n, MaxVal, bitmaps, ranks, mids, Log);
        }

        private static void HeapSortInt(int* a, int len)
        {
            for (int i = (len >> 1) - 1; i >= 0; i--) Sift(a, i, len);
            for (int i = len - 1; i > 0; i--) { int t = a[0]; a[0] = a[i]; a[i] = t; Sift(a, 0, i); }
        }

        private static void Sift(int* a, int i, int len)
        {
            int half = len >> 1;
            while (i < half)
            {
                int child = (i << 1) + 1, right = child + 1;
                if (right < len && a[right] > a[child]) child = right;
                if (a[child] <= a[i]) break;
                int t = a[i]; a[i] = a[child]; a[child] = t;
                i = child;
            }
        }

        [Test]
        public void Quantile_MatchesBrute()
        {
            int* data = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            int stride = N + 1;
            int* bitmaps = (int*)Marshal.AllocHGlobal(sizeof(int) * 2 * stride);
            int* ranks = (int*)Marshal.AllocHGlobal(sizeof(int) * Log * stride);
            int* mids = (int*)Marshal.AllocHGlobal(sizeof(int) * Log);
            int* sorted = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            try
            {
                for (int i = 0; i < N; i++) data[i] = _data[i];
                Build(data, N, bitmaps, ranks, mids);
                for (int t = 0; t < 50; t++)
                {
                    int l = new Random(1000 + t).Next(0, N);
                    int r = new Random(2000 + t).Next(l, N);
                    int len = r - l + 1;
                    for (int i = 0; i < len; i++) sorted[i] = _data[l + i];
                    HeapSortInt(sorted, len);
                    for (int k = 1; k <= len; k++)
                    {
                        int fast = WaveletMatrixQuantile.Run(bitmaps, ranks, mids, l, r, k, Log);
                        Assert.AreEqual(sorted[k - 1], fast, $"quantile l={l} r={r} k={k}");
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)data);
                Marshal.FreeHGlobal((nint)bitmaps);
                Marshal.FreeHGlobal((nint)ranks);
                Marshal.FreeHGlobal((nint)mids);
                Marshal.FreeHGlobal((nint)sorted);
            }
        }

        [Test]
        public void RectangleCount_PrevNext_Intersect_MatchBrute()
        {
            int* data = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            int stride = N + 1;
            int* bitmaps = (int*)Marshal.AllocHGlobal(sizeof(int) * 2 * stride);
            int* ranks = (int*)Marshal.AllocHGlobal(sizeof(int) * Log * stride);
            int* mids = (int*)Marshal.AllocHGlobal(sizeof(int) * Log);
            try
            {
                for (int i = 0; i < N; i++) data[i] = _data[i];
                Build(data, N, bitmaps, ranks, mids);

                int l = 5, r = 40;
                int vLo = 20, vHi = 70;
                int bruteCnt = 0;
                for (int i = l; i <= r; i++) if (_data[i] >= vLo && _data[i] < vHi) bruteCnt++;
                Assert.AreEqual(bruteCnt, WaveletMatrixRectangleCount.Run(bitmaps, ranks, mids, l, r, vLo, vHi, Log), "rect count");

                int prevBrute = int.MinValue;
                for (int i = l; i <= r; i++) if (_data[i] < 55 && _data[i] > prevBrute) prevBrute = _data[i];
                Assert.AreEqual(prevBrute, WaveletMatrixPrevValue.Run(bitmaps, ranks, mids, l, r, 55, Log), "prev<55");

                int nextBrute = int.MaxValue;
                for (int i = l; i <= r; i++) if (_data[i] > 30 && _data[i] < nextBrute) nextBrute = _data[i];
                Assert.AreEqual(nextBrute, WaveletMatrixNextValue.Run(bitmaps, ranks, mids, l, r, 30, Log), "next>30");

                int l2 = 10, r2 = 50;
                int bruteIntersect = 0;
                bool[] seen = new bool[MaxVal + 1];
                for (int i = l; i <= r; i++) seen[_data[i]] = true;
                for (int i = l2; i <= r2; i++) if (_data[i] < seen.Length && seen[_data[i]]) { bruteIntersect++; seen[_data[i]] = false; }
                Assert.AreEqual(bruteIntersect, WaveletMatrixIntersect.Run(bitmaps, ranks, mids, l, r, l2, r2, Log), "intersect distinct count");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)data);
                Marshal.FreeHGlobal((nint)bitmaps);
                Marshal.FreeHGlobal((nint)ranks);
                Marshal.FreeHGlobal((nint)mids);
            }
        }

        [Test]
        public void RectangleSum_MatchesBrute()
        {
            int* data = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            int stride = N + 1;
            int* bitmaps = (int*)Marshal.AllocHGlobal(sizeof(int) * 2 * stride);
            int* ranks = (int*)Marshal.AllocHGlobal(sizeof(int) * Log * stride);
            int* mids = (int*)Marshal.AllocHGlobal(sizeof(int) * Log);
            long* valSums = (long*)Marshal.AllocHGlobal(sizeof(long) * Log * stride);
            try
            {
                for (int i = 0; i < N; i++) data[i] = _data[i];
                WaveletMatrixBuildSums.Run(data, N, bitmaps, ranks, mids, valSums, Log);
                int l = 3, r = 50, vLo = 10, vHi = 80;
                long brute = 0;
                for (int i = l; i <= r; i++) if (_data[i] >= vLo && _data[i] < vHi) brute += _data[i];
                Assert.AreEqual(brute, WaveletMatrixRectangleSum.Run(bitmaps, ranks, mids, valSums, l, r, vLo, vHi, Log), "rect sum");
            }
            finally
            {
                Marshal.FreeHGlobal((nint)data);
                Marshal.FreeHGlobal((nint)bitmaps);
                Marshal.FreeHGlobal((nint)ranks);
                Marshal.FreeHGlobal((nint)mids);
                Marshal.FreeHGlobal((nint)valSums);
            }
        }

        [Test]
        public void SuccinctRankSelect_RoundTrip()
        {
            const int Bits = 200;
            int words = (Bits + 63) / 64;
            ulong* bits = (ulong*)Marshal.AllocHGlobal(sizeof(ulong) * words);
            long* prefix = (long*)Marshal.AllocHGlobal(sizeof(long) * (words + 1));
            Random rng = new Random(999);
            try
            {
                int totalOnes = 0;
                for (int w = 0; w < words; w++)
                {
                    ulong x = 0;
                    for (int b = 0; b < 64; b++) if (rng.NextDouble() < 0.4) { x |= 1UL << b; totalOnes++; }
                    bits[w] = x;
                }
                SuccinctWaveletBuild.Run(bits, words, prefix);

                for (int i = 0; i <= Bits; i++)
                {
                    int brute = 0;
                    for (int j = 0; j < i; j++)
                        if ((bits[j >> 6] & (1UL << (j & 63))) != 0) brute++;
                    Assert.AreEqual(brute, SuccinctWaveletRank.Run(bits, prefix, i), $"rank({i})");
                }

                int k = 1;
                for (int i = 0; i < Bits && k <= totalOnes; i++)
                {
                    if ((bits[i >> 6] & (1UL << (i & 63))) != 0)
                    {
                        Assert.AreEqual(i, SuccinctWaveletSelect.Run(bits, prefix, words, k), $"select({k})");
                        k++;
                    }
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)bits);
                Marshal.FreeHGlobal((nint)prefix);
            }
        }
    }
}
