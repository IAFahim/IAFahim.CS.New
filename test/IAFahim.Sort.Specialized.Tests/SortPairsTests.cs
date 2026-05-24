namespace IAFahim.Sort.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class SortPairsTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            SortPairs.Run(null, 0);
        }

        [Test]
        public void SingleElement_Unchanged()
        {
            var val = new SortPairs.Pair { Key = 1, Value = 100 };
            SortPairs.Run(&val, 1);
            Assert.AreEqual(1, val.Key);
            Assert.AreEqual(100, val.Value);
        }

        [Test]
        public void AlreadySorted_Unchanged()
        {
            var ptr = (SortPairs.Pair*)Marshal.AllocHGlobal(3 * sizeof(SortPairs.Pair));
            try
            {
                ptr[0] = new SortPairs.Pair { Key = 1, Value = 10 };
                ptr[1] = new SortPairs.Pair { Key = 2, Value = 20 };
                ptr[2] = new SortPairs.Pair { Key = 3, Value = 30 };
                SortPairs.Run(ptr, 3);
                Assert.AreEqual(1, ptr[0].Key);
                Assert.AreEqual(2, ptr[1].Key);
                Assert.AreEqual(3, ptr[2].Key);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void Reversed_Sorts()
        {
            var ptr = (SortPairs.Pair*)Marshal.AllocHGlobal(3 * sizeof(SortPairs.Pair));
            try
            {
                ptr[0] = new SortPairs.Pair { Key = 3, Value = 30 };
                ptr[1] = new SortPairs.Pair { Key = 2, Value = 20 };
                ptr[2] = new SortPairs.Pair { Key = 1, Value = 10 };
                SortPairs.Run(ptr, 3);
                Assert.AreEqual(1, ptr[0].Key);
                Assert.AreEqual(2, ptr[1].Key);
                Assert.AreEqual(3, ptr[2].Key);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Test]
        public void SameKeyPreservesValue()
        {
            var ptr = (SortPairs.Pair*)Marshal.AllocHGlobal(3 * sizeof(SortPairs.Pair));
            try
            {
                ptr[0] = new SortPairs.Pair { Key = 5, Value = 50 };
                ptr[1] = new SortPairs.Pair { Key = 3, Value = 30 };
                ptr[2] = new SortPairs.Pair { Key = 5, Value = 51 };
                SortPairs.Run(ptr, 3);
                Assert.AreEqual(3, ptr[0].Key);
                Assert.AreEqual(30, ptr[0].Value);
                Assert.AreEqual(5, ptr[1].Key);
                Assert.AreEqual(5, ptr[2].Key);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}