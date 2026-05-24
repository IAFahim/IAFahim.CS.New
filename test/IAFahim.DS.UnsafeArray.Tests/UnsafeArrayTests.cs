using IAFahim.DS.UnsafeArray;
using IAFahim.Sort.Insertion;

namespace IAFahim.DS.Tests
{
    using System;
    using Unity.Collections;
    using NUnit.Framework;

    public sealed unsafe class UnsafeArrayTests
    {
        [Test]
        public void Constructor_AllocatesMemory()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(8, Allocator.Persistent);

            try
            {
                Assert.IsTrue(array.Ptr != null);
                Assert.AreEqual(8, array.Length);
                Assert.AreEqual(Allocator.Persistent, array.Allocator);
            }
            finally
            {
                array.Dispose();
            }
        }

        [Test]
        public void Constructor_ZeroInitializes()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(4, Allocator.Persistent);

            try
            {
                for (int i = 0; i < array.Length; i++)
                    Assert.AreEqual(0, array.Ptr[i]);
            }
            finally
            {
                array.Dispose();
            }
        }

        [Test]
        public void WriteAndRead_RoundTrips()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(4, Allocator.Persistent);

            try
            {
                array.Ptr[0] = 10;
                array.Ptr[1] = 20;
                array.Ptr[2] = 30;
                array.Ptr[3] = 40;

                Assert.AreEqual(10, array.Ptr[0]);
                Assert.AreEqual(20, array.Ptr[1]);
                Assert.AreEqual(30, array.Ptr[2]);
                Assert.AreEqual(40, array.Ptr[3]);
            }
            finally
            {
                array.Dispose();
            }
        }

        [Test]
        public void Dispose_NullsPointer()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(4, Allocator.Persistent);
            array.Dispose();

            Assert.IsTrue(array.Ptr == null);
            Assert.AreEqual(0, array.Length);
        }

        [Test]
        public void SortIntegration_WorksWithRawPointer()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(4, Allocator.Persistent);

            try
            {
                array.Ptr[0] = 4;
                array.Ptr[1] = 2;
                array.Ptr[2] = 3;
                array.Ptr[3] = 1;

                Insertion.Run(array.Ptr, array.Length);

                Assert.AreEqual(1, array.Ptr[0]);
                Assert.AreEqual(2, array.Ptr[1]);
                Assert.AreEqual(3, array.Ptr[2]);
                Assert.AreEqual(4, array.Ptr[3]);
            }
            finally
            {
                array.Dispose();
            }
        }
    }
}