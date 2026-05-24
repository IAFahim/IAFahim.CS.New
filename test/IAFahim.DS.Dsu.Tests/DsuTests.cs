namespace IAFahim.DS.Dsu.Tests
{
    using IAFahim.DS.Dsu;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class DsuTests
    {
        [Test]
        public void EmptyInput_NoOp()
        {
            fixed (int* parent = new int[0])
            fixed (int* size = new int[0])
            {
                DsuInit.Run(parent, size, 0);
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void SingleElement_ParentIsSelf()
        {
            int* parent = stackalloc int[1];
            int* size = stackalloc int[1];
            DsuInit.Run(parent, size, 1);
            Assert.AreEqual(0, DsuFind.Run(parent, 0));
            Assert.AreEqual(1, DsuSize.Run(parent, size, 0));
        }

        [Test]
        public void UnionFind_Basic()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            DsuInit.Run(parent, size, n);

            DsuUnion.Run(parent, size, 0, 1);
            DsuUnion.Run(parent, size, 1, 2);
            Assert.IsTrue(DsuSame.Run(parent, 0, 2));
            Assert.AreEqual(3, DsuSize.Run(parent, size, 0));

            Assert.IsFalse(DsuSame.Run(parent, 0, 3));
            Assert.IsFalse(DsuSame.Run(parent, 2, 4));

            DsuUnion.Run(parent, size, 3, 4);
            Assert.IsTrue(DsuSame.Run(parent, 3, 4));

            DsuUnion.Run(parent, size, 2, 3);
            Assert.IsTrue(DsuSame.Run(parent, 0, 4));
            Assert.AreEqual(5, DsuSize.Run(parent, size, 0));
        }

        [Test]
        public void SelfLoop_NoChange()
        {
            int* parent = stackalloc int[3];
            int* size = stackalloc int[3];
            DsuInit.Run(parent, size, 3);
            DsuUnion.Run(parent, size, 0, 0);
            Assert.AreEqual(0, DsuFind.Run(parent, 0));
        }
    }
}
