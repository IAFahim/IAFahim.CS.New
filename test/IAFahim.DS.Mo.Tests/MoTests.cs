namespace IAFahim.DS.Mo.Tests
{
    using IAFahim.DS.Mo;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class MoTests
    {
        [Fact]
        public void MoSort_Basic()
        {
            const int q = 5;
            int* l = stackalloc int[q];
            int* r = stackalloc int[q];
            int* block = stackalloc int[q];
            int* queries = stackalloc int[q];
            l[0] = 0; r[0] = 3;
            l[1] = 1; r[1] = 5;
            l[2] = 2; r[2] = 4;
            l[3] = 0; r[3] = 2;
            l[4] = 3; r[4] = 6;
            for (int i = 0; i < q; i++) { block[i] = l[i] / 2; queries[i] = i; }
            MoSort.Run(queries, l, r, block, q, 2);
            Assert.True(l[0] <= l[1]);
        }

        [Fact]
        public void MoAdd_Remove_Basic()
        {
            int* curL = stackalloc int[1];
            int* curR = stackalloc int[1];
            const int maxVal = 100;
            int* freq = (int*)Marshal.AllocHGlobal(maxVal * sizeof(int));
            try
            {
                for (int i = 0; i < maxVal; i++) freq[i] = 0;
                MoAdd.Run(curL, curR, freq, 5);
                MoAdd.Run(curL, curR, freq, 5);
                MoRemove.Run(curL, curR, freq, 5);
                Assert.Equal(1, freq[5]);
            }
            finally { Marshal.FreeHGlobal((nint)freq); }
        }

        [Fact]
        public void MoRollback_ResetsState()
        {
            const int maxVal = 100;
            int* freq = (int*)Marshal.AllocHGlobal(maxVal * sizeof(int));
            try
            {
                for (int i = 0; i < maxVal; i++) freq[i] = 0;
                freq[3] = 5;
                freq[7] = 10;
                MoRollback.Run(freq, maxVal);
                for (int i = 0; i < maxVal; i++)
                    Assert.Equal(0, freq[i]);
            }
            finally { Marshal.FreeHGlobal((nint)freq); }
        }
    }
}