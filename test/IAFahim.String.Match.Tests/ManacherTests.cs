namespace IAFahim.String.Match.Tests
{
    using System;
    using NUnit.Framework;
    using IAFahim.String.Palindrome;

    public sealed unsafe class ManacherTests
    {
        [Test]
        public void Odd_SingleCenter()
        {
            byte[] arr = new byte[] { 97 };
            fixed (byte* ptr = arr)
            {
                int* d = stackalloc int[1];
                Manacher.Odd(ptr, 1, d);
                Assert.AreEqual(1, d[0]);
            }
        }

        [Test]
        public void Odd_Palindrome()
        {
            byte[] arr = new byte[] { 97, 98, 97 };
            fixed (byte* ptr = arr)
            {
                int* d = stackalloc int[3];
                Manacher.Odd(ptr, 3, d);
                Assert.AreEqual(1, d[0]);
                Assert.AreEqual(2, d[1]);
                Assert.AreEqual(1, d[2]);
            }
        }

        [Test]
        public void Even_NoPalindrome()
        {
            byte[] arr = new byte[] { 97, 98, 99 };
            fixed (byte* ptr = arr)
            {
                int* d = stackalloc int[3];
                Manacher.Even(ptr, 3, d);
                Assert.AreEqual(0, d[0]);
                Assert.AreEqual(0, d[1]);
                Assert.AreEqual(0, d[2]);
            }
        }

        [Test]
        public void Even_WithPalindrome()
        {
            byte[] arr = new byte[] { 97, 98, 98, 97 };
            fixed (byte* ptr = arr)
            {
                int* d = stackalloc int[4];
                Manacher.Even(ptr, 4, d);
                Assert.IsTrue(d[0] >= 0);
                Assert.IsTrue(d[1] >= 0);
                Assert.IsTrue(d[2] >= 0);
                Assert.IsTrue(d[3] >= 0);
            }
        }
    }
}
