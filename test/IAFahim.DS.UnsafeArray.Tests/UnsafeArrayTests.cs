using IAFahim.DS.UnsafeArray;
using IAFahim.Sort.Insertion;

namespace IAFahim.DS.Tests
{
    using System;
    using Unity.Collections;
    using Xunit;

    public sealed unsafe class UnsafeArrayTests
    {
        [Fact]
        public void Constructor_AllocatesMemory()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(8, Allocator.Persistent);

            try
            {
                Assert.True(array.Ptr != null);
                Assert.Equal(8, array.Length);
                Assert.Equal(Allocator.Persistent, array.Allocator);
            }
            finally
            {
                array.Dispose();
            }
        }

        [Fact]
        public void Constructor_ZeroInitializes()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(4, Allocator.Persistent);

            try
            {
                for (int i = 0; i < array.Length; i++)
                    Assert.Equal(0, array.Ptr[i]);
            }
            finally
            {
                array.Dispose();
            }
        }

        [Fact]
        public void WriteAndRead_RoundTrips()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(4, Allocator.Persistent);

            try
            {
                array.Ptr[0] = 10;
                array.Ptr[1] = 20;
                array.Ptr[2] = 30;
                array.Ptr[3] = 40;

                Assert.Equal(10, array.Ptr[0]);
                Assert.Equal(20, array.Ptr[1]);
                Assert.Equal(30, array.Ptr[2]);
                Assert.Equal(40, array.Ptr[3]);
            }
            finally
            {
                array.Dispose();
            }
        }

        [Fact]
        public void Dispose_NullsPointer()
        {
            UnsafeArray<int> array = new UnsafeArray<int>(4, Allocator.Persistent);
            array.Dispose();

            Assert.True(array.Ptr == null);
            Assert.Equal(0, array.Length);
        }

        [Fact]
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

                Assert.Equal(1, array.Ptr[0]);
                Assert.Equal(2, array.Ptr[1]);
                Assert.Equal(3, array.Ptr[2]);
                Assert.Equal(4, array.Ptr[3]);
            }
            finally
            {
                array.Dispose();
            }
        }
    }
}