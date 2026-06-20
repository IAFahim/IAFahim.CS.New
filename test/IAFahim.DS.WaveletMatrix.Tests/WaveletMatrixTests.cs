namespace IAFahim.DS.WaveletMatrix.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class WaveletMatrixTests
    {
        // Reference k-th smallest via sort + index. Used to verify WaveletMatrixKth over a
        // full range of k for many distributions.
        private static int KthSmallestReference(int* data, int l, int r, int k)
        {
            int len = r - l + 1;
            int* tmp = (int*)Marshal.AllocHGlobal(len * sizeof(int));
            try
            {
                for (int i = 0; i < len; i++) tmp[i] = data[l + i];
                for (int i = 1; i < len; i++)
                {
                    int key = tmp[i], j = i - 1;
                    while (j >= 0 && tmp[j] > key) { tmp[j + 1] = tmp[j]; j--; }
                    tmp[j + 1] = key;
                }
                return tmp[k];
            }
            finally { Marshal.FreeHGlobal((nint)tmp); }
        }

        private static void VerifyRange(int* data, int n, int maxVal, int log, int* bitmaps, int* ranks, int* mids, int l, int r)
        {
            for (int k = 0; k <= r - l; k++)
            {
                int expected = KthSmallestReference(data, l, r, k);
                int actual = WaveletMatrixKth.Run(bitmaps, ranks, mids, l, r, k, log);
                Assert.AreEqual(expected, actual, $"k={k} over [{l},{r}]");
            }
        }

        [Test]
        public void Empty_NoOp()
        {
            int* data = stackalloc int[0];
            int* bitmaps = stackalloc int[2];
            int* ranks = stackalloc int[2];
            int* mids = stackalloc int[1];
            Assert.DoesNotThrow(() => WaveletMatrixBuild.Run(data, 0, 0, bitmaps, ranks, mids, 1));
        }

        [Test]
        public void SingleElement_ReturnsIt()
        {
            const int N = 1;
            int* data = stackalloc int[N] { 5 };
            int log = 3;
            int levels = log * (N + 1);
            int* bitmaps = stackalloc int[levels];
            int* ranks = stackalloc int[levels];
            int* mids = stackalloc int[log];
            WaveletMatrixBuild.Run(data, N, 7, bitmaps, ranks, mids, log);
            Assert.AreEqual(5, WaveletMatrixKth.Run(bitmaps, ranks, mids, 0, 0, 0, log));
        }

        [Test]
        public void PowerOfTwo_AllDistinct_AllKth()
        {
            const int N = 8;
            int* data = stackalloc int[N] { 13, 7, 2, 11, 5, 0, 15, 9 };
            int maxVal = 15, log = 4;
            int levels = log * (N + 1);
            int* bitmaps = stackalloc int[levels];
            int* ranks = stackalloc int[levels];
            int* mids = stackalloc int[log];
            WaveletMatrixBuild.Run(data, N, maxVal, bitmaps, ranks, mids, log);
            VerifyRange(data, N, maxVal, log, bitmaps, ranks, mids, 0, N - 1);
        }

        [Test]
        public void Duplicates_AllEqual()
        {
            const int N = 6;
            int* data = stackalloc int[N] { 4, 4, 4, 4, 4, 4 };
            int maxVal = 4, log = 3;
            int levels = log * (N + 1);
            int* bitmaps = stackalloc int[levels];
            int* ranks = stackalloc int[levels];
            int* mids = stackalloc int[log];
            WaveletMatrixBuild.Run(data, N, maxVal, bitmaps, ranks, mids, log);
            for (int k = 0; k < N; k++)
                Assert.AreEqual(4, WaveletMatrixKth.Run(bitmaps, ranks, mids, 0, N - 1, k, log));
        }

        [Test]
        public void Reversed_AllKth()
        {
            const int N = 8;
            int* data = stackalloc int[N] { 7, 6, 5, 4, 3, 2, 1, 0 };
            int maxVal = 7, log = 3;
            int levels = log * (N + 1);
            int* bitmaps = stackalloc int[levels];
            int* ranks = stackalloc int[levels];
            int* mids = stackalloc int[log];
            WaveletMatrixBuild.Run(data, N, maxVal, bitmaps, ranks, mids, log);
            VerifyRange(data, N, maxVal, log, bitmaps, ranks, mids, 0, N - 1);
        }

        [Test]
        public void SubrangeQueries_MatchReference()
        {
            const int N = 16;
            int* data = stackalloc int[N] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5, 8, 9, 7, 9, 3 };
            int maxVal = 9, log = 4;
            int levels = log * (N + 1);
            int* bitmaps = stackalloc int[levels];
            int* ranks = stackalloc int[levels];
            int* mids = stackalloc int[log];
            WaveletMatrixBuild.Run(data, N, maxVal, bitmaps, ranks, mids, log);
            int[,] ranges = { { 0, 3 }, { 4, 9 }, { 2, 14 }, { 7, 7 }, { 10, 15 }, { 1, 8 } };
            for (int i = 0; i < ranges.GetLength(0); i++)
                VerifyRange(data, N, maxVal, log, bitmaps, ranks, mids, ranges[i, 0], ranges[i, 1]);
        }

        [Test]
        public void LargeRandom_AllPositionsAndKth()
        {
            const int N = 256;
            int maxVal = 255, log = 8;
            int* data = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int levels = log * (N + 1);
            int* bitmaps = (int*)Marshal.AllocHGlobal(levels * sizeof(int));
            int* ranks = (int*)Marshal.AllocHGlobal(levels * sizeof(int));
            int* mids = (int*)Marshal.AllocHGlobal(log * sizeof(int));
            try
            {
                Random rng = new Random(42);
                for (int i = 0; i < N; i++) data[i] = rng.Next(maxVal + 1);
                WaveletMatrixBuild.Run(data, N, maxVal, bitmaps, ranks, mids, log);
                for (int trial = 0; trial < 50; trial++)
                {
                    int l = rng.Next(N);
                    int r = rng.Next(l, N - 1);
                    int k = rng.Next(r - l + 1);
                    int expected = KthSmallestReference(data, l, r, k);
                    int actual = WaveletMatrixKth.Run(bitmaps, ranks, mids, l, r, k, log);
                    Assert.AreEqual(expected, actual, $"trial {trial}: k={k} over [{l},{r}]");
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)data);
                Marshal.FreeHGlobal((nint)bitmaps);
                Marshal.FreeHGlobal((nint)ranks);
                Marshal.FreeHGlobal((nint)mids);
            }
        }
    }
}
