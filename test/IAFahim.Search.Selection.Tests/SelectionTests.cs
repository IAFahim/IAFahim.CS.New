namespace IAFahim.Search.Selection.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class SelectionTests
    {
        [Test]
        public void SelectTopK_Empty_NoOp()
        {
            int* ptr = null;
            Selection.SelectTopK(ptr, 0, 0);
        }

        [Test]
        public void SelectTopK_K0_NoOp()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 3; ptr[2] = 7; ptr[3] = 1; ptr[4] = 9;
            Selection.SelectTopK(ptr, 5, 0);
            Assert.AreEqual(5, ptr[0]);
        }

        [Test]
        public void SelectTopK_KEqualsN_NoOp()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 3; ptr[2] = 7; ptr[3] = 1; ptr[4] = 9;
            Selection.SelectTopK(ptr, 5, 5);
            Assert.AreEqual(5, ptr[0]);
        }

        [Test]
        public void SelectTopK_K1_ReturnsMin()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 3; ptr[2] = 7; ptr[3] = 1; ptr[4] = 9;
            Selection.SelectTopK(ptr, 5, 1);
            Assert.AreEqual(1, ptr[0]);
        }

        [Test]
        public void SelectTopK_K2_ReturnsTop2()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 3; ptr[2] = 7; ptr[3] = 1; ptr[4] = 9;
            Selection.SelectTopK(ptr, 5, 2);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(3, ptr[1]);
        }

        [Test]
        public void SelectTopK_K4_ReturnsTop4()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 3; ptr[2] = 7; ptr[3] = 1; ptr[4] = 9;
            Selection.SelectTopK(ptr, 5, 4);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(3, ptr[1]);
            Assert.AreEqual(5, ptr[2]);
            Assert.AreEqual(7, ptr[3]);
            Assert.AreEqual(9, ptr[4]);
        }

        [Test]
        public void TryGetKth_InvalidK_ReturnsFalse()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 3; ptr[2] = 7; ptr[3] = 1; ptr[4] = 9;
            int result;
            Assert.IsFalse(Selection.TryGetKth(ptr, 5, 5, out result));
            Assert.IsFalse(Selection.TryGetKth(ptr, 5, 10, out result));
        }

        [Test]
        public void TryGetKth_ValidK_ReturnsTrue()
        {
            int* ptr = stackalloc int[5];
            ptr[0] = 5; ptr[1] = 3; ptr[2] = 7; ptr[3] = 1; ptr[4] = 9;
            int result;
            Assert.IsTrue(Selection.TryGetKth(ptr, 5, 1, out result));
            Assert.AreEqual(3, result);
        }

        [Test]
        public void MedianIndex_OddLength_ReturnsMiddle()
        {
            Assert.AreEqual(2, Selection.MedianIndex(5));
        }

        [Test]
        public void MedianIndex_EvenLength_ReturnsLeftMiddle()
        {
            Assert.AreEqual(2, Selection.MedianIndex(6));
        }

        [Test]
        public void MedianMaintain_EvenCount_NoOverflow()
        {
            long* a = stackalloc long[] { long.MaxValue - 2, long.MaxValue - 4, long.MaxValue - 6 };
            long* res = stackalloc long[3];
            long last = MedianMaintain.Run(3, a, res);
            Assert.AreEqual(a[0], res[0]);
            Assert.IsTrue(res[1] > 0);
            Assert.IsTrue(last > 0);
        }

        [Test]
        public void MedianMaintain_SimpleSequence()
        {
            long* a = stackalloc long[] { 5, 1, 3, 4, 2 };
            long* res = stackalloc long[5];
            MedianMaintain.Run(5, a, res);
            Assert.AreEqual(5, res[0]);
            Assert.AreEqual(3, res[1]);
            Assert.AreEqual(3, res[2]);
            Assert.AreEqual(3, res[3]);
            Assert.AreEqual(3, res[4]);
        }

        [Test]
        public void TopK_Run_Largest()
        {
            long* a = stackalloc long[] { 1, 9, 3, 7 };
            long* res = stackalloc long[2];
            int k = TopK.Run(4, a, 2, res);
            Assert.AreEqual(2, k);
            Assert.IsTrue(res[0] >= res[1]);
            Assert.IsTrue(res[0] == 9 || res[1] == 9);
        }

        [Test]
        public void SharedPartition_AndInsertionSort()
        {
            int* ptr = stackalloc int[] { 4, 1, 3, 2 };
            int p = SelectionShared.Partition(ptr, 0, 3);
            Assert.IsTrue(p >= 0 && p <= 3);
            long* lp = stackalloc long[] { 4, 1, 3, 2 };
            int pl = SelectionShared.PartitionLong(lp, 0, 3);
            Assert.IsTrue(pl >= 0 && pl <= 3);
            int* s = stackalloc int[] { 3, 1, 2 };
            SelectionShared.InsertionSort(s, 3);
            Assert.AreEqual(1, s[0]);
            Assert.AreEqual(2, s[1]);
            Assert.AreEqual(3, s[2]);
        }

        [Test]
        public void MedianIndex_Zero_ReturnsZero()
        {
            Assert.AreEqual(0, Selection.MedianIndex(0));
        }

        [Test]
        public void SelectTopK_AllIdentical_NoChange()
        {
            int* ptr = stackalloc int[5];
            for (int i = 0; i < 5; i++) ptr[i] = 7;
            Selection.SelectTopK(ptr, 5, 2);
            for (int i = 0; i < 5; i++) Assert.AreEqual(7, ptr[i]);
        }
    }
}