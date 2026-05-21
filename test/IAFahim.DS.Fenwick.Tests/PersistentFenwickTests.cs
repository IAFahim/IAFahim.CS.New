namespace IAFahim.DS.Fenwick.Tests
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class PersistentFenwickTests
    {
        [Fact]
        public void PersistentFenwick_EmptyInput_NoOp()
        {
            int* lc = stackalloc int[100];
            int* rc = stackalloc int[100];
            int* sum = stackalloc int[100];
            int alloc = 0;
            for (int i = 0; i < 100; i++) { lc[i] = rc[i] = sum[i] = 0; }

            int q = PersistentFenwickQuery.Run(lc, rc, sum, 0, 0, 9, 0, 9);
            Assert.Equal(0, q);
        }

        [Fact]
        public void PersistentFenwick_UpdateAndQuery()
        {
            const int maxNodes = 300;
            int* lc = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* rc = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* sum = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int alloc = 0;

            try
            {
                for (int i = 0; i < maxNodes; i++) { lc[i] = rc[i] = sum[i] = 0; }

                int root1 = PersistentFenwickUpdate.Run(lc, rc, sum, &alloc, 0, 0, 9, 3, 1);
                int root2 = PersistentFenwickUpdate.Run(lc, rc, sum, &alloc, root1, 0, 9, 7, 1);

                int q1 = PersistentFenwickQuery.Run(lc, rc, sum, root1, 0, 9, 3, 3);
                int q2 = PersistentFenwickQuery.Run(lc, rc, sum, root2, 0, 9, 3, 7);

                Assert.Equal(1, q1);
                Assert.Equal(2, q2);

                int q1old = PersistentFenwickQuery.Run(lc, rc, sum, root1, 0, 9, 7, 7);
                Assert.Equal(0, q1old);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)lc);
                Marshal.FreeHGlobal((nint)rc);
                Marshal.FreeHGlobal((nint)sum);
            }
        }
    }
}
