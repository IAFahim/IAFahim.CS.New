namespace IAFahim.Search.LIS.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class LisTests
    {
        [Test]
        public void Empty_Zero()
        {
            Assert.AreEqual(0, Lis.Run(null, 0, null));
        }

        [Test]
        public void Single_One()
        {
            int v = 7;
            Assert.AreEqual(1, Lis.Run(&v, 1, null));
        }

        [Test]
        public void Increasing_FullLength()
        {
            const int N = 8;
            int* ptr = stackalloc int[N];
            for (int i = 0; i < N; i++) ptr[i] = i;
            Assert.AreEqual(N, Lis.Run(ptr, N, null));
        }

        [Test]
        public void Decreasing_LengthOne()
        {
            const int N = 6;
            int* ptr = stackalloc int[N];
            for (int i = 0; i < N; i++) ptr[i] = N - i;
            Assert.AreEqual(1, Lis.Run(ptr, N, null));
        }

        [Test]
        public void Mixed_KnownLengthAndIncreasingIndices()
        {
            int* ptr = stackalloc int[7];
            ptr[0] = 3; ptr[1] = 1; ptr[2] = 4; ptr[3] = 1; ptr[4] = 5; ptr[5] = 9; ptr[6] = 2;
            int* result = stackalloc int[7];
            int len = Lis.Run(ptr, 7, result);
            Assert.AreEqual(4, len);
            for (int i = 1; i < len; i++)
            {
                Assert.IsTrue(result[i] > result[i - 1]);
                Assert.IsTrue(ptr[result[i]] > ptr[result[i - 1]]);
            }
        }
    }
}
