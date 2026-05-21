namespace IAFahim.String.Match.Tests
{
    using System;
    using Xunit;
    using IAFahim.String.Palindrome;

    public sealed unsafe class ManacherTests
    {
        [Fact]
        public void Odd_SingleCenter()
        {
            byte[] arr = new byte[] { 97 };
            fixed (byte* ptr = arr)
            {
                int* d = stackalloc int[1];
                Manacher.Odd(ptr, 1, d);
                Assert.Equal(1, d[0]);
            }
        }

        [Fact]
        public void Odd_Palindrome()
        {
            byte[] arr = new byte[] { 97, 98, 97 };
            fixed (byte* ptr = arr)
            {
                int* d = stackalloc int[3];
                Manacher.Odd(ptr, 3, d);
                Assert.Equal(1, d[0]);
                Assert.Equal(3, d[1]);
                Assert.Equal(1, d[2]);
            }
        }

        [Fact]
        public void Even_NoPalindrome()
        {
            byte[] arr = new byte[] { 97, 98, 99 };
            fixed (byte* ptr = arr)
            {
                int* d = stackalloc int[3];
                Manacher.Even(ptr, 3, d);
                Assert.Equal(0, d[0]);
                Assert.Equal(0, d[1]);
                Assert.Equal(0, d[2]);
            }
        }

        [Fact]
        public void Even_WithPalindrome()
        {
            byte[] arr = new byte[] { 97, 98, 98, 97 };
            fixed (byte* ptr = arr)
            {
                int* d = stackalloc int[4];
                Manacher.Even(ptr, 4, d);
                Assert.Equal(0, d[0]);
                Assert.Equal(1, d[1]);
                Assert.Equal(2, d[2]);
                Assert.Equal(1, d[3]);
            }
        }
    }
}
