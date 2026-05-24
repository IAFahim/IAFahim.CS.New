namespace IAFahim.Permutation.Tests
{
    using NUnit.Framework;

    public sealed unsafe class GrayCodeTests
    {
        [Test]
        public void ToGray_Zero() { Assert.AreEqual(0, GrayCode.ToGray(0)); }

        [Test]
        public void ToGray_One() { Assert.AreEqual(1, GrayCode.ToGray(1)); }

        [Test]
        public void ToGray_Two() { Assert.AreEqual(3, GrayCode.ToGray(2)); }

        [Test]
        public void FromGray_Zero() { Assert.AreEqual(0, GrayCode.FromGray(0)); }

        [Test]
        public void FromGray_One() { Assert.AreEqual(1, GrayCode.FromGray(1)); }

        [Test]
        public void FromGray_Three() { Assert.AreEqual(2, GrayCode.FromGray(3)); }

        [Test]
        public void RoundTrip()
        {
            for (int i = 0; i < 256; i++)
            {
                int g = GrayCode.ToGray(i);
                int n = GrayCode.FromGray(g);
                Assert.AreEqual(i, n);
            }
        }

        [Test]
        public void Generate_Count()
        {
            int* dst = stackalloc int[4];
            GrayCode.Generate(dst, 2);
            Assert.AreEqual(0, dst[0]);
            Assert.AreEqual(1, dst[1]);
            Assert.AreEqual(3, dst[2]);
            Assert.AreEqual(2, dst[3]);
        }
    }

    public sealed unsafe class NextPermutationTests
    {
        [Test]
        public void Simple()
        {
            int* ptr = stackalloc int[] { 1, 2, 3 };
            bool result = NextPermutation.Run(ptr, 3);
            Assert.IsTrue(result);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(3, ptr[1]);
            Assert.AreEqual(2, ptr[2]);
        }

        [Test]
        public void LastPermutation()
        {
            int* ptr = stackalloc int[] { 3, 2, 1 };
            bool result = NextPermutation.Run(ptr, 3);
            Assert.IsFalse(result);
        }

        [Test]
        public void Single() 
        {
            int* ptr = stackalloc int[1];
            ptr[0] = 1;
            Assert.IsFalse(NextPermutation.Run<int>(ptr, 1));
        }

        [Test]
        public void TwoElements()
        {
            int* ptr = stackalloc int[] { 1, 2 };
            bool result = NextPermutation.Run(ptr, 2);
            Assert.IsTrue(result);
            Assert.AreEqual(2, ptr[0]);
            Assert.AreEqual(1, ptr[1]);
        }
    }

    public sealed unsafe class PrevPermutationTests
    {
        [Test]
        public void Simple()
        {
            int* ptr = stackalloc int[] { 1, 3, 2 };
            bool result = PrevPermutation.Run(ptr, 3);
            Assert.IsTrue(result);
            Assert.AreEqual(1, ptr[0]);
            Assert.AreEqual(2, ptr[1]);
            Assert.AreEqual(3, ptr[2]);
        }

        [Test]
        public void FirstPermutation() 
        {
            int* ptr = stackalloc int[3];
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3;
            Assert.IsFalse(PrevPermutation.Run<int>(ptr, 3));
        }
    }

    public sealed unsafe class CartesianProductTests
    {
        [Test]
        public void Count()
        {
            int* sizes = stackalloc int[] { 2, 3 };
            Assert.AreEqual(6, CartesianProduct.ComputeCount(sizes, 2));
        }

        [Test]
        public void GetAt()
        {
            int* sizes = stackalloc int[] { 2, 3 };
            int* dst = stackalloc int[2];
            CartesianProduct.GetAt(sizes, 2, 4, dst);
            Assert.AreEqual(1, dst[0]);
            Assert.AreEqual(1, dst[1]);
        }
    }
}