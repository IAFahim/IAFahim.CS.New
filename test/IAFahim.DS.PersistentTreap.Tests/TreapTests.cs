namespace IAFahim.DS.PersistentTreap.Tests
{
    using IAFahim.DS.PersistentTreap;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class PersistentTreapTests
    {
        [Fact]
        public void Insert_Basic()
        {
            const int maxNodes = 100;
            int* nodes = stackalloc int[maxNodes];
            int* left = stackalloc int[maxNodes];
            int* right = stackalloc int[maxNodes];
            int* prio = stackalloc int[maxNodes];
            int* size = stackalloc int[maxNodes];
            int allocCnt = 0;

            int root = PersistentTreapInsert.Run(nodes, left, right, prio, &allocCnt, 0, 5);
            root = PersistentTreapInsert.Run(nodes, left, right, prio, &allocCnt, root, 3);
            root = PersistentTreapInsert.Run(nodes, left, right, prio, &allocCnt, root, 7);

            Assert.True(PersistentTreapFind.Run(nodes, left, right, root, 3));
            Assert.True(PersistentTreapFind.Run(nodes, left, right, root, 5));
            Assert.True(PersistentTreapFind.Run(nodes, left, right, root, 7));
            Assert.False(PersistentTreapFind.Run(nodes, left, right, root, 4));
        }

        [Fact]
        public void Erase_Basic()
        {
            const int maxNodes = 100;
            int* nodes = stackalloc int[maxNodes];
            int* left = stackalloc int[maxNodes];
            int* right = stackalloc int[maxNodes];
            int* prio = stackalloc int[maxNodes];
            int* size = stackalloc int[maxNodes];
            int allocCnt = 0;

            int root = PersistentTreapInsert.Run(nodes, left, right, prio, &allocCnt, 0, 10);
            root = PersistentTreapInsert.Run(nodes, left, right, prio, &allocCnt, root, 20);
            root = PersistentTreapInsert.Run(nodes, left, right, prio, &allocCnt, root, 30);

            root = PersistentTreapErase.Run(nodes, left, right, root, 20);

            Assert.True(PersistentTreapFind.Run(nodes, left, right, root, 10));
            Assert.False(PersistentTreapFind.Run(nodes, left, right, root, 20));
            Assert.True(PersistentTreapFind.Run(nodes, left, right, root, 30));
        }

        [Fact]
        public void EmptyTree_NoCrash()
        {
            const int maxNodes = 10;
            int* nodes = stackalloc int[maxNodes];
            int* left = stackalloc int[maxNodes];
            int* right = stackalloc int[maxNodes];
            int* prio = stackalloc int[maxNodes];
            int* size = stackalloc int[maxNodes];
            int allocCnt = 0;

            Assert.False(PersistentTreapFind.Run(nodes, left, right, 0, 5));
            int root = PersistentTreapErase.Run(nodes, left, right, 0, 5);
            Assert.Equal(0, root);
        }

        [Fact]
        public void MultipleInserts_AllFound()
        {
            const int maxNodes = 200;
            int* nodes = stackalloc int[maxNodes];
            int* left = stackalloc int[maxNodes];
            int* right = stackalloc int[maxNodes];
            int* prio = stackalloc int[maxNodes];
            int* size = stackalloc int[maxNodes];
            int allocCnt = 0;

            int root = 0;
            for (int i = 0; i < 50; i += 2)
            {
                root = PersistentTreapInsert.Run(nodes, left, right, prio, &allocCnt, root, i);
            }

            for (int i = 0; i < 50; i += 2)
            {
                Assert.True(PersistentTreapFind.Run(nodes, left, right, root, i));
            }
            for (int i = 1; i < 50; i += 2)
            {
                Assert.False(PersistentTreapFind.Run(nodes, left, right, root, i));
            }
        }
    }
}