namespace IAFahim.DS.Dsu.Tests
{
    using IAFahim.DS.Dsu;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class DsuTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            fixed (int* parent = new int[0])
            fixed (int* size = new int[0])
            {
                DsuInit.Run(parent, size, 0);
                Assert.True(true);
            }
        }

        [Fact]
        public void SingleElement_ParentIsSelf()
        {
            int* parent = stackalloc int[1];
            int* size = stackalloc int[1];
            DsuInit.Run(parent, size, 1);
            Assert.Equal(0, DsuFind.Run(parent, 0));
            Assert.Equal(1, DsuSize.Run(parent, size, 0));
        }

        [Fact]
        public void UnionFind_Basic()
        {
            const int n = 5;
            int* parent = stackalloc int[n];
            int* size = stackalloc int[n];
            DsuInit.Run(parent, size, n);

            DsuUnion.Run(parent, size, 0, 1);
            DsuUnion.Run(parent, size, 1, 2);
            Assert.True(DsuSame.Run(parent, 0, 2));
            Assert.Equal(3, DsuSize.Run(parent, size, 0));

            Assert.False(DsuSame.Run(parent, 0, 3));
            Assert.False(DsuSame.Run(parent, 2, 4));

            DsuUnion.Run(parent, size, 3, 4);
            Assert.True(DsuSame.Run(parent, 3, 4));

            DsuUnion.Run(parent, size, 2, 3);
            Assert.True(DsuSame.Run(parent, 0, 4));
            Assert.Equal(5, DsuSize.Run(parent, size, 0));
        }

        [Fact]
        public void SelfLoop_NoChange()
        {
            int* parent = stackalloc int[3];
            int* size = stackalloc int[3];
            DsuInit.Run(parent, size, 3);
            DsuUnion.Run(parent, size, 0, 0);
            Assert.Equal(0, DsuFind.Run(parent, 0));
        }
    }
}
