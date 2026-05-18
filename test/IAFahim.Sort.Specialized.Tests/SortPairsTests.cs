namespace IAFahim.Sort.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class SortPairsTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            SortPairs.Run(null, 0);
        }

        [Fact]
        public void SingleElement_Unchanged()
        {
            var val = new SortPairs.Pair { Key = 1, Value = 100 };
            SortPairs.Run(&val, 1);
            Assert.Equal(1, val.Key);
            Assert.Equal(100, val.Value);
        }

        [Fact]
        public void AlreadySorted_Unchanged()
        {
            var ptr = (SortPairs.Pair*)Marshal.AllocHGlobal(3 * sizeof(SortPairs.Pair));
            try
            {
                ptr[0] = new SortPairs.Pair { Key = 1, Value = 10 };
                ptr[1] = new SortPairs.Pair { Key = 2, Value = 20 };
                ptr[2] = new SortPairs.Pair { Key = 3, Value = 30 };
                SortPairs.Run(ptr, 3);
                Assert.Equal(1, ptr[0].Key);
                Assert.Equal(2, ptr[1].Key);
                Assert.Equal(3, ptr[2].Key);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Fact]
        public void Reversed_Sorts()
        {
            var ptr = (SortPairs.Pair*)Marshal.AllocHGlobal(3 * sizeof(SortPairs.Pair));
            try
            {
                ptr[0] = new SortPairs.Pair { Key = 3, Value = 30 };
                ptr[1] = new SortPairs.Pair { Key = 2, Value = 20 };
                ptr[2] = new SortPairs.Pair { Key = 1, Value = 10 };
                SortPairs.Run(ptr, 3);
                Assert.Equal(1, ptr[0].Key);
                Assert.Equal(2, ptr[1].Key);
                Assert.Equal(3, ptr[2].Key);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }

        [Fact]
        public void SameKeyPreservesValue()
        {
            var ptr = (SortPairs.Pair*)Marshal.AllocHGlobal(3 * sizeof(SortPairs.Pair));
            try
            {
                ptr[0] = new SortPairs.Pair { Key = 5, Value = 50 };
                ptr[1] = new SortPairs.Pair { Key = 3, Value = 30 };
                ptr[2] = new SortPairs.Pair { Key = 5, Value = 51 };
                SortPairs.Run(ptr, 3);
                Assert.Equal(3, ptr[0].Key);
                Assert.Equal(30, ptr[0].Value);
                Assert.Equal(5, ptr[1].Key);
                Assert.Equal(5, ptr[2].Key);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}