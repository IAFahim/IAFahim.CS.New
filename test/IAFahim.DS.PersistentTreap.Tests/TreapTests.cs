namespace IAFahim.DS.PersistentTreap.Tests
{
    using IAFahim.DS.PersistentTreap;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class PersistentTreapTests
    {
        [Test]
        public void Insert_Basic()
        {
            const int maxNodes = 100;
            int* nodes = stackalloc int[maxNodes], left = stackalloc int[maxNodes], right = stackalloc int[maxNodes], prio = stackalloc int[maxNodes], size = stackalloc int[maxNodes];
            int allocCnt = 0;

            int root = PersistentTreapInsert.Run(nodes, left, right, prio, size, &allocCnt, 0, 5);
            root = PersistentTreapInsert.Run(nodes, left, right, prio, size, &allocCnt, root, 3);
            root = PersistentTreapInsert.Run(nodes, left, right, prio, size, &allocCnt, root, 7);

            Assert.IsTrue(PersistentTreapFind.Run(nodes, left, right, root, 3));
            Assert.IsTrue(PersistentTreapFind.Run(nodes, left, right, root, 5));
            Assert.IsTrue(PersistentTreapFind.Run(nodes, left, right, root, 7));
            Assert.IsFalse(PersistentTreapFind.Run(nodes, left, right, root, 4));
        }

        [Test]
        public void EmptyTree_NoCrash()
        {
            const int maxNodes = 10;
            int* nodes = stackalloc int[maxNodes], left = stackalloc int[maxNodes], right = stackalloc int[maxNodes], prio = stackalloc int[maxNodes], size = stackalloc int[maxNodes];
            int allocCnt = 0;
            Assert.IsFalse(PersistentTreapFind.Run(nodes, left, right, 0, 5));
            int root = PersistentTreapErase.Run(nodes, left, right, prio, size, &allocCnt, 0, 5);
            Assert.AreEqual(0, root);
        }

        [Test]
        public void MultipleInserts_AllFound()
        {
            const int maxNodes = 1000;
            int* nodes = stackalloc int[maxNodes], left = stackalloc int[maxNodes], right = stackalloc int[maxNodes], prio = stackalloc int[maxNodes], size = stackalloc int[maxNodes];
            int allocCnt = 0;
            int root = 0;
            for (int i = 0; i < 50; i += 2) root = PersistentTreapInsert.Run(nodes, left, right, prio, size, &allocCnt, root, i);
            for (int i = 0; i < 50; i += 2) Assert.IsTrue(PersistentTreapFind.Run(nodes, left, right, root, i));
            for (int i = 1; i < 50; i += 2) Assert.IsFalse(PersistentTreapFind.Run(nodes, left, right, root, i));
            PersistentTreapNode.Update(left, right, size, root);
            Assert.IsTrue(size[root] > 0);
        }

        [Test]
        public void Erase_RemovesValue()
        {
            const int maxNodes = 100;
            int* nodes = stackalloc int[maxNodes], left = stackalloc int[maxNodes], right = stackalloc int[maxNodes], prio = stackalloc int[maxNodes], size = stackalloc int[maxNodes];
            int allocCnt = 0;
            int root = 0;
            root = PersistentTreapInsert.Run(nodes, left, right, prio, size, &allocCnt, root, 1);
            root = PersistentTreapInsert.Run(nodes, left, right, prio, size, &allocCnt, root, 2);
            root = PersistentTreapErase.Run(nodes, left, right, prio, size, &allocCnt, root, 1);
            Assert.IsFalse(PersistentTreapFind.Run(nodes, left, right, root, 1));
            Assert.IsTrue(PersistentTreapFind.Run(nodes, left, right, root, 2));
        }
    }
}
