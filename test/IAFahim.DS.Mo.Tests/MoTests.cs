namespace IAFahim.DS.Mo.Tests
{
    using IAFahim.DS.Mo;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class MoTests
    {
        [Test]
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
            // Parity-Mo order: blocks ascending; within a block, r
            // descending for even blocks and ascending for odd blocks.
            // l is NOT monotonic in Mo order.
            for (int i = 1; i < q; i++)
                Assert.IsTrue(block[i] >= block[i - 1], "blocks not ascending");
            for (int i = 1; i < q; i++)
            {
                if (block[i] != block[i - 1]) continue;
                if ((block[i] & 1) == 0)
                    Assert.IsTrue(r[i] <= r[i - 1], "even block r not descending");
                else
                    Assert.IsTrue(r[i] >= r[i - 1], "odd block r not ascending");
            }
        }

        [Test]
        public void MoAdd_Remove_Basic()
        {
            int* curL = stackalloc int[1];
            int* curR = stackalloc int[1];
            const int maxVal = 100;
            int* freq = (int*)Marshal.AllocHGlobal(maxVal * sizeof(int));
            try
            {
                for (int i = 0; i < maxVal; i++) freq[i] = 0;
                MoAdd.Run(freq, 5);
                MoAdd.Run(freq, 5);
                MoRemove.Run(freq, 5);
                Assert.AreEqual(1, freq[5]);
            }
            finally { Marshal.FreeHGlobal((nint)freq); }
        }

        [Test]
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
                    Assert.AreEqual(0, freq[i]);
            }
            finally { Marshal.FreeHGlobal((nint)freq); }
        }

        [Test]
        public void DistinctCount_AndAddRemoveInt()
        {
            int* freq = stackalloc int[10];
            for (int i = 0; i < 10; i++) freq[i] = 0;
            int distinct = 0;
            MoDistinctCounter.AddInt(freq, &distinct, 3);
            MoDistinctCounter.AddInt(freq, &distinct, 3);
            MoDistinctCounter.AddInt(freq, &distinct, 5);
            Assert.AreEqual(2, distinct);
            Assert.AreEqual(2, MoAnswer.DistinctCount(freq, 10));
            MoDistinctCounter.RemoveInt(freq, &distinct, 3);
            MoDistinctCounter.RemoveInt(freq, &distinct, 3);
            Assert.AreEqual(1, distinct);
        }

        [Test]
        public void MoWithUpdates_CallerOwnedFreq_NoCrash()
        {
            const int n = 3;
            int* arr = stackalloc int[] { 1, 2, 1 };
            Query3D* queries = stackalloc Query3D[1];
            queries[0] = new Query3D { L = 0, R = 2, T = 0, Id = 0 };
            Update* updates = stackalloc Update[1];
            updates[0] = new Update { Pos = 0, OldVal = 1, NewVal = 3 };
            int* ans = stackalloc int[1];
            int* freq = stackalloc int[16];
            for (int i = 0; i < 16; i++) freq[i] = 0;
            MoWithUpdates.Run(n, arr, 1, queries, 0, updates, ans, 2, freq);
            Assert.IsTrue(ans[0] >= 0);
        }
    }
}