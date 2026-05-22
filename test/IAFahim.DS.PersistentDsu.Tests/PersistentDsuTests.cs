namespace IAFahim.DS.PersistentDsu.Tests
{
    using IAFahim.DS.PersistentDsu;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class PersistentDsuTests
    {
        [Fact]
        public void Init_Basic()
        {
            const int n = 10;
            int* parent = stackalloc int[n * 20];
            PersistentDsuInit.Run(parent, n);
            for (int i = 0; i < n; i++)
            {
                Assert.Equal(i, parent[i]);
            }
        }

        [Fact]
        public void Union_SameComponent()
        {
            const int maxNodes = 50;
            int* parent = stackalloc int[maxNodes];
            int* size = stackalloc int[maxNodes];
            int* leftChild = stackalloc int[maxNodes];
            int* rightChild = stackalloc int[maxNodes];
            int allocCnt = 0;
            int prevRoot = 0;

            int newRoot = PersistentDsuUnion.Run(parent, size, leftChild, rightChild, &prevRoot, 0, 1, &allocCnt);
            prevRoot = newRoot;

            int ra = PersistentDsuFind.Run(parent, leftChild, rightChild, newRoot, 0);
            int rb = PersistentDsuFind.Run(parent, leftChild, rightChild, newRoot, 1);
            Assert.Equal(ra, rb);
        }

        [Fact]
        public void MultipleUnionsInSameVersion()
        {
            const int maxNodes = 50;
            int* parent = stackalloc int[maxNodes];
            int* size = stackalloc int[maxNodes];
            int* leftChild = stackalloc int[maxNodes];
            int* rightChild = stackalloc int[maxNodes];
            int allocCnt = 0;
            int prevRoot = 0;

            PersistentDsuUnion.Run(parent, size, leftChild, rightChild, &prevRoot, 0, 1, &allocCnt);
            PersistentDsuUnion.Run(parent, size, leftChild, rightChild, &prevRoot, 1, 2, &allocCnt);

            int ra = PersistentDsuFind.Run(parent, leftChild, rightChild, prevRoot, 0);
            int rb = PersistentDsuFind.Run(parent, leftChild, rightChild, prevRoot, 2);
            Assert.Equal(ra, rb);
        }

        [Fact]
        public void EmptyInit_NoOp()
        {
            int* parent = stackalloc int[0];
            PersistentDsuInit.Run(parent, 0);
        }
    }
}