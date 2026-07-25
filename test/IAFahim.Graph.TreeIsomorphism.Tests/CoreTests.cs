namespace IAFahim.Graph.TreeIsomorphism.Tests
{
    using System;
    using NUnit.Framework;

    public sealed unsafe class OrderedTreeEditDistanceTests
    {
        [Test]
        public void IdenticalChain_DistanceZero()
        {
            int* p1 = stackalloc int[3] { -1, 0, 1 };
            int* p2 = stackalloc int[3] { -1, 0, 1 };
            Assert.AreEqual(0, OrderedTreeEditDistance.Run(p1, p2, 3, 3));
        }

        [Test]
        public void EmptyVsOne_DistanceOne()
        {
            int* p = stackalloc int[1] { -1 };
            Assert.AreEqual(1, OrderedTreeEditDistance.Run(null, p, 0, 1));
        }
    }

    public sealed unsafe class UnorderedTreeEditDistanceTests
    {
        [Test]
        public void RunConstrained_Identical_Zero()
        {
            int* p1 = stackalloc int[3] { -1, 0, 1 };
            int* p2 = stackalloc int[3] { -1, 0, 1 };
            Assert.AreEqual(0, UnorderedTreeEditDistance.RunConstrained(p1, 3, p2, 3));
        }

        [Test]
        public void RunConstrained_EmptyVsTwo_DistanceTwo()
        {
            int* p = stackalloc int[2] { -1, 0 };
            Assert.AreEqual(2, UnorderedTreeEditDistance.RunConstrained(null, 0, p, 2));
        }

        [Test]
        public void Run_Unconstrained_ThrowsNi()
        {
            int* p = stackalloc int[1] { -1 };
            Assert.Throws<NotImplementedException>(() => UnorderedTreeEditDistance.Run(p, 1, p, 1));
        }
    }
}
