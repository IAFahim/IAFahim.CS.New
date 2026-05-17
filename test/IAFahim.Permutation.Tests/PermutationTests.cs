namespace IAFahim.Permutation.Tests
{
    using Xunit;

    public sealed unsafe class GrayCodeTests
    {
        [Fact]
        public void ToGray_Zero() { Assert.Equal(0, GrayCode.ToGray(0)); }

        [Fact]
        public void ToGray_One() { Assert.Equal(1, GrayCode.ToGray(1)); }

        [Fact]
        public void ToGray_Two() { Assert.Equal(3, GrayCode.ToGray(2)); }

        [Fact]
        public void FromGray_Zero() { Assert.Equal(0, GrayCode.FromGray(0)); }

        [Fact]
        public void FromGray_One() { Assert.Equal(1, GrayCode.FromGray(1)); }

        [Fact]
        public void FromGray_Three() { Assert.Equal(2, GrayCode.FromGray(3)); }

        [Fact]
        public void RoundTrip()
        {
            for (int i = 0; i < 256; i++)
            {
                int g = GrayCode.ToGray(i);
                int n = GrayCode.FromGray(g);
                Assert.Equal(i, n);
            }
        }

        [Fact]
        public void Generate_Count()
        {
            int* dst = stackalloc int[4];
            GrayCode.Generate(dst, 2);
            Assert.Equal(0, dst[0]);
            Assert.Equal(1, dst[1]);
            Assert.Equal(3, dst[2]);
            Assert.Equal(2, dst[3]);
        }
    }

    public sealed unsafe class NextPermutationTests
    {
        [Fact]
        public void Simple()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            bool result = NextPermutation.Run(ptr, 3);
            Assert.True(result);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(3, ptr[1]);
            Assert.Equal(2, ptr[2]);
        }

        [Fact]
        public void LastPermutation()
        {
            int* ptr = stackalloc int[] { 3, 2, 1 };
            bool result = NextPermutation.Run(ptr, 3);
            Assert.False(result);
        }

        [Fact]
        public void Single() 
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 1;
            Assert.False(NextPermutation.Run<int>(ptr, 1));
        }

        [Fact]
        public void TwoElements()
        {
            int* ptr = stackalloc int[] { 1, 2 };
            bool result = NextPermutation.Run(ptr, 2);
            Assert.True(result);
            Assert.Equal(2, ptr[0]);
            Assert.Equal(1, ptr[1]);
        }
    }

    public sealed unsafe class PrevPermutationTests
    {
        [Fact]
        public void Simple()
        {
            int* ptr = stackalloc int[] { 1, 3, 2 };
            bool result = PrevPermutation.Run(ptr, 3);
            Assert.True(result);
            Assert.Equal(1, ptr[0]);
            Assert.Equal(2, ptr[1]);
            Assert.Equal(3, ptr[2]);
        }

        [Fact]
        public void FirstPermutation() 
        {
            int* ptr = stackalloc int[3];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3;
            Assert.False(PrevPermutation.Run<int>(ptr, 3));
        }
    }

    public sealed unsafe class CartesianProductTests
    {
        [Fact]
        public void Count()
        {
            int* sizes = stackalloc int[] { 2, 3 };
            Assert.Equal(6, CartesianProduct.ComputeCount(sizes, 2));
        }

        [Fact]
        public void GetAt()
        {
            int* sizes = stackalloc int[] { 2, 3 };
            int* dst = stackalloc int[2];
            CartesianProduct.GetAt(sizes, 2, 4, dst);
            Assert.Equal(1, dst[0]);
            Assert.Equal(1, dst[1]);
        }
    }
}