namespace IAFahim.DS.Fenwick.Tests
{
    using IAFahim.DS.Fenwick;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class FenwickUpperBoundTests
    {
        [Test]
        public void UpperBoundInt64_Definition()
        {
            // Upper bound: first index where prefix > target
            const int n = 5;
            long* bit = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            try
            {
                for (int i = 0; i <= n; i++) bit[i] = 0;

                // Values [1, 2, 3, 4, 5] -> prefix: [1, 3, 6, 10, 15]
                for (int i = 0; i < n; i++)
                    Fenwick.AddInt64(bit, n, i, i + 1);

                // target=0: first prefix > 0 is 1 at index 0
                Assert.AreEqual(0, Fenwick.UpperBoundInt64(bit, n, 0));
                // target=1: first prefix > 1 is 3 at index 1
                Assert.AreEqual(1, Fenwick.UpperBoundInt64(bit, n, 1));
                // target=2: first prefix > 2 is 3 at index 1
                Assert.AreEqual(1, Fenwick.UpperBoundInt64(bit, n, 2));
                // target=5: first prefix > 5 is 6 at index 2
                Assert.AreEqual(2, Fenwick.UpperBoundInt64(bit, n, 5));
                // target=14: first prefix > 14 is 15 at index 4
                Assert.AreEqual(4, Fenwick.UpperBoundInt64(bit, n, 14));
            }
            finally { Marshal.FreeHGlobal((nint)bit); }
        }

        [Test]
        public void UpperBoundInt64_AllPrefixes()
        {
            const int n = 5;
            long* bit = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            try
            {
                for (int i = 0; i <= n; i++) bit[i] = 0;

                for (int i = 0; i < n; i++)
                    Fenwick.AddInt64(bit, n, i, i + 1);

                // For each prefix value p, upper_bound(p-1) should return the index of p
                // prefix[0]=1: upper(0)=0
                // prefix[1]=3: upper(2)=1
                // prefix[2]=6: upper(5)=2
                // prefix[3]=10: upper(9)=3
                // prefix[4]=15: upper(14)=4
                Assert.AreEqual(0, Fenwick.UpperBoundInt64(bit, n, 0));
                Assert.AreEqual(1, Fenwick.UpperBoundInt64(bit, n, 2));
                Assert.AreEqual(2, Fenwick.UpperBoundInt64(bit, n, 5));
                Assert.AreEqual(3, Fenwick.UpperBoundInt64(bit, n, 9));
                Assert.AreEqual(4, Fenwick.UpperBoundInt64(bit, n, 14));
            }
            finally { Marshal.FreeHGlobal((nint)bit); }
        }

        [Test]
        public void UpperBoundInt64_ExceedsTotal()
        {
            const int n = 5;
            long* bit = (long*)Marshal.AllocHGlobal((n + 1) * sizeof(long));
            try
            {
                for (int i = 0; i <= n; i++) bit[i] = 0;

                for (int i = 0; i < n; i++)
                    Fenwick.AddInt64(bit, n, i, i + 1);

                // Total sum = 15, so upper_bound(15) = n = 5 (no prefix exceeds 15)
                Assert.AreEqual(n, Fenwick.UpperBoundInt64(bit, n, 15));
                Assert.AreEqual(n, Fenwick.UpperBoundInt64(bit, n, 100));
            }
            finally { Marshal.FreeHGlobal((nint)bit); }
        }
    }
}
