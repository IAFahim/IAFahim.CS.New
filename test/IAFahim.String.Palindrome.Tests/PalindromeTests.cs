namespace IAFahim.String.Palindrome.Tests
{
    using System.Runtime.InteropServices;
    using System.Text;
    using NUnit.Framework;

    public sealed unsafe class ManacherTests
    {
        [Test]
        public void Empty_NoOp()
        {
            Manacher.Odd(null, 0, null);
            Manacher.Even(null, 0, null);
        }

        [Test]
        public void Odd_SingleCenter()
        {
            byte[] s = Encoding.ASCII.GetBytes("aba");
            int* d = stackalloc int[3];
            fixed (byte* p = s)
            {
                Manacher.Odd(p, 3, d);
            }
            Assert.AreEqual(1, d[0]);
            Assert.AreEqual(2, d[1]);
            Assert.AreEqual(1, d[2]);
        }

        [Test]
        public void Even_DoubleCenter()
        {
            byte[] s = Encoding.ASCII.GetBytes("abba");
            int* d = stackalloc int[4];
            fixed (byte* p = s)
            {
                Manacher.Even(p, 4, d);
            }
            Assert.AreEqual(0, d[0]);
            Assert.AreEqual(0, d[1]);
            Assert.AreEqual(2, d[2]);
            Assert.AreEqual(0, d[3]);
        }

        [Test]
        public void Odd_AllSame_FullRadii()
        {
            byte[] s = Encoding.ASCII.GetBytes("aaaa");
            int* d = stackalloc int[4];
            fixed (byte* p = s)
            {
                Manacher.Odd(p, 4, d);
            }
            Assert.AreEqual(1, d[0]);
            Assert.AreEqual(2, d[1]);
            Assert.AreEqual(2, d[2]);
            Assert.AreEqual(1, d[3]);
        }
    }

    public sealed unsafe class PalindromicTreeTests
    {
        [Test]
        public void DistinctCount_Simple()
        {
            byte[] s = Encoding.ASCII.GetBytes("aaa");
            fixed (byte* p = s)
            {
                PalindromicTree.Build(p, 3);
                Assert.AreEqual(3, PalindromicTree.DistinctCount());
            }
        }
    }
}
