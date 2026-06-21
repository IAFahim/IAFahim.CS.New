namespace IAFahim.DS.OrderedSet.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class OrderedSetTests
    {
        [Test]
        public void Empty_Insert_GrowsByOne()
        {
            int* ptr = stackalloc int[8];
            int len = OrderedSet.Insert(ptr, 0, 5);
            Assert.AreEqual(1, len);
            Assert.AreEqual(5, ptr[0]);
        }

        [Test]
        public void Insert_Duplicate_NoChange()
        {
            int* ptr = stackalloc int[8] { 1, 3, 5, 7, 0, 0, 0, 0 };
            int len = 4;
            len = OrderedSet.Insert(ptr, len, 3);
            Assert.AreEqual(4, len);
            Assert.AreEqual(new[] { 1, 3, 5, 7 }, AsArray(ptr, len));
        }

        [Test]
        public void Insert_BeginningMiddleEnd_StaysSorted()
        {
            int* ptr = stackalloc int[16] { 10, 20, 30, 40, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            int len = 4;
            len = OrderedSet.Insert(ptr, len, 5);
            len = OrderedSet.Insert(ptr, len, 25);
            len = OrderedSet.Insert(ptr, len, 50);
            Assert.AreEqual(7, len);
            Assert.AreEqual(new[] { 5, 10, 20, 25, 30, 40, 50 }, AsArray(ptr, len));
        }

        [Test]
        public void Erase_MissingKey_NoChange()
        {
            int* ptr = stackalloc int[8] { 10, 20, 30, 40, 0, 0, 0, 0 };
            int len = 4;
            len = OrderedSet.Erase(ptr, len, 25);
            Assert.AreEqual(4, len);
        }

        [Test]
        public void Erase_PresentKey_RemovesAndStaysSorted()
        {
            int* ptr = stackalloc int[8] { 10, 20, 30, 40, 0, 0, 0, 0 };
            int len = 4;
            len = OrderedSet.Erase(ptr, len, 20);
            Assert.AreEqual(3, len);
            Assert.AreEqual(new[] { 10, 30, 40 }, AsArray(ptr, len));
            len = OrderedSet.Erase(ptr, len, 10);
            Assert.AreEqual(new[] { 30, 40 }, AsArray(ptr, len));
            len = OrderedSet.Erase(ptr, len, 40);
            Assert.AreEqual(new[] { 30 }, AsArray(ptr, len));
        }

        [Test]
        public void Rank_CountsStrictlyLess()
        {
            int* ptr = stackalloc int[8] { 2, 4, 6, 8, 0, 0, 0, 0 };
            int len = 4;
            Assert.AreEqual(0, OrderedSet.Rank(ptr, len, 1));
            Assert.AreEqual(0, OrderedSet.Rank(ptr, len, 2));
            Assert.AreEqual(1, OrderedSet.Rank(ptr, len, 3));
            Assert.AreEqual(3, OrderedSet.Rank(ptr, len, 8));
            Assert.AreEqual(4, OrderedSet.Rank(ptr, len, 99));
        }

        [Test]
        public void Kth_ReturnsByPosition()
        {
            int* ptr = stackalloc int[8] { 2, 4, 6, 8, 0, 0, 0, 0 };
            int len = 4;
            Assert.AreEqual(2, OrderedSet.Kth(ptr, len, 0));
            Assert.AreEqual(4, OrderedSet.Kth(ptr, len, 1));
            Assert.AreEqual(8, OrderedSet.Kth(ptr, len, 3));
        }

        private static int[] AsArray(int* ptr, int len)
        {
            int[] arr = new int[len];
            for (int i = 0; i < len; i++) arr[i] = ptr[i];
            return arr;
        }
    }
}
