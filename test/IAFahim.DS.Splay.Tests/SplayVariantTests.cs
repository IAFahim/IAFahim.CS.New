namespace IAFahim.DS.Splay.Tests
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class SplayVariantTests
    {
        private static SplayRevNode* AllocNode(int key, int priority)
        {
            SplayRevNode* n = (SplayRevNode*)Marshal.AllocHGlobal(sizeof(SplayRevNode));
            n->Key = key;
            n->Size = 1;
            n->Sum = key;
            n->Rev = false;
            n->Parent = null;
            n->Left = null;
            n->Right = null;
            return n;
        }

        private static SplayRevNode* BuildChain(int n)
        {
            SplayRevNode* root = null;
            for (int i = 1; i <= n; i++)
            {
                SplayRevNode* node = AllocNode(i, 0);
                if (root == null) { root = node; continue; }
                SplayRevNode* cur = root;
                while (cur->Right != null) cur = cur->Right;
                cur->Right = node;
                node->Parent = cur;
                SplayRangeReverse.Update(cur);
            }
            SplayRangeReverse.Update(root);
            return root;
        }

        [Fact]
        public void SplayRangeReverse_EmptyRoot_NoOp()
        {
            SplayRevNode* root = null;
        }

        [Fact]
        public void SplayRangeQuery_SingleElement()
        {
            SplayRevNode* node = AllocNode(42, 0);
            SplayRevNode* root = node;
            try
            {
                long sum = SplayRangeQuery.QuerySum(&root, 0, 0);
                Assert.Equal(42L, sum);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)node);
            }
        }

        [Fact]
        public void SplayRangeQuery_MultipleElements()
        {
            SplayRevNode* n1 = AllocNode(10, 0);
            SplayRevNode* n2 = AllocNode(5, 0);
            n1->Right = n2;
            n2->Parent = n1;
            SplayRangeReverse.Update(n1);
            SplayRevNode* root = n1;

            try
            {
                long sum = SplayRangeQuery.QuerySum(&root, 0, 0);
                Assert.Equal(10L, sum);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
            }
        }
    }
}
