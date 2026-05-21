namespace IAFahim.DS.Treap.Tests
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class TreapVariantTests
    {
        private static TreapImplicitNode* AllocNode(long val, int priority)
        {
            TreapImplicitNode* n = (TreapImplicitNode*)Marshal.AllocHGlobal(sizeof(TreapImplicitNode));
            n->Priority = priority;
            n->Size = 1;
            n->Value = val;
            n->Sum = val;
            n->Lazy = 0;
            n->HasLazy = false;
            n->Rev = false;
            n->Left = null;
            n->Right = null;
            return n;
        }

        [Fact]
        public void TreapImplicit_EmptyRange_NoOp()
        {
            TreapImplicitNode* root = null;
            long sum = TreapImplicit.QueryRange(ref root, 0, 0);
            Assert.Equal(0L, sum);
        }

        [Fact]
        public void TreapImplicit_RangeAdd_And_Sum()
        {
            TreapImplicitNode* root = null;
            TreapImplicitNode* n1 = AllocNode(1, 100);
            TreapImplicitNode* n2 = AllocNode(2, 80);
            TreapImplicitNode* n3 = AllocNode(3, 60);
            root = TreapImplicit.Merge(root, n1);
            root = TreapImplicit.Merge(root, n2);
            root = TreapImplicit.Merge(root, n3);

            try
            {
                TreapImplicit.AddRange(ref root, 0, 1, 10);
                long sum = TreapImplicit.QueryRange(ref root, 0, 2);
                Assert.Equal(26L, sum);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
                Marshal.FreeHGlobal((nint)n3);
            }
        }

        [Fact]
        public void TreapImplicit_Reverse()
        {
            TreapImplicitNode* root = null;
            TreapImplicitNode* n1 = AllocNode(1, 100);
            TreapImplicitNode* n2 = AllocNode(2, 80);
            TreapImplicitNode* n3 = AllocNode(3, 60);
            root = TreapImplicit.Merge(root, n1);
            root = TreapImplicit.Merge(root, n2);
            root = TreapImplicit.Merge(root, n3);

            try
            {
                TreapImplicit.ReverseRange(ref root, 0, 2);
                long total = TreapImplicit.QueryRange(ref root, 0, 2);
                Assert.Equal(6L, total);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
                Marshal.FreeHGlobal((nint)n3);
            }
        }

        private static TreapRevNode* AllocRevNode(long val, int priority)
        {
            TreapRevNode* n = (TreapRevNode*)Marshal.AllocHGlobal(sizeof(TreapRevNode));
            n->Priority = priority;
            n->Size = 1;
            n->Value = val;
            n->Sum = val;
            n->Rev = false;
            n->Left = null;
            n->Right = null;
            return n;
        }

        [Fact]
        public void TreapRangeReverse_Basic()
        {
            TreapRevNode* root = null;
            TreapRevNode* n1 = AllocRevNode(1, 100);
            TreapRevNode* n2 = AllocRevNode(2, 80);
            TreapRevNode* n3 = AllocRevNode(3, 60);
            root = TreapRangeReverse.Merge(root, n1);
            root = TreapRangeReverse.Merge(root, n2);
            root = TreapRangeReverse.Merge(root, n3);

            try
            {
                TreapRangeReverse.Reverse(ref root, 0, 2);
                Assert.Equal(3, root->Size);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
                Marshal.FreeHGlobal((nint)n3);
            }
        }

        [Fact]
        public void TreapRangeRotate_Basic()
        {
            TreapRevNode* root = null;
            TreapRevNode* n1 = AllocRevNode(1, 100);
            TreapRevNode* n2 = AllocRevNode(2, 80);
            TreapRevNode* n3 = AllocRevNode(3, 60);
            TreapRevNode* n4 = AllocRevNode(4, 40);
            root = TreapRangeReverse.Merge(root, n1);
            root = TreapRangeReverse.Merge(root, n2);
            root = TreapRangeReverse.Merge(root, n3);
            root = TreapRangeReverse.Merge(root, n4);

            try
            {
                TreapRangeRotate.Rotate(ref root, 0, 3, 2);
                Assert.Equal(4, root->Size);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
                Marshal.FreeHGlobal((nint)n3);
                Marshal.FreeHGlobal((nint)n4);
            }
        }

        private static TreapMinNode* AllocMinNode(long val, int priority)
        {
            TreapMinNode* n = (TreapMinNode*)Marshal.AllocHGlobal(sizeof(TreapMinNode));
            n->Priority = priority;
            n->Size = 1;
            n->Value = val;
            n->Min = val;
            n->LazyAssign = 0;
            n->HasAssign = false;
            n->Left = null;
            n->Right = null;
            return n;
        }

        [Fact]
        public void TreapRangeMin_Basic()
        {
            TreapMinNode* root = null;
            TreapMinNode* n1 = AllocMinNode(5, 100);
            TreapMinNode* n2 = AllocMinNode(3, 80);
            TreapMinNode* n3 = AllocMinNode(7, 60);
            root = TreapRangeMin.Merge(root, n1);
            root = TreapRangeMin.Merge(root, n2);
            root = TreapRangeMin.Merge(root, n3);

            try
            {
                long m = TreapRangeMin.QueryMin(ref root, 0, 2);
                Assert.Equal(3L, m);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
                Marshal.FreeHGlobal((nint)n3);
            }
        }

        private static TreapSumNode* AllocSumNode(long val, int priority)
        {
            TreapSumNode* n = (TreapSumNode*)Marshal.AllocHGlobal(sizeof(TreapSumNode));
            n->Priority = priority;
            n->Size = 1;
            n->Value = val;
            n->Sum = val;
            n->LazyAdd = 0;
            n->Left = null;
            n->Right = null;
            return n;
        }

        [Fact]
        public void TreapRangeSum_Basic()
        {
            TreapSumNode* root = null;
            TreapSumNode* n1 = AllocSumNode(1, 100);
            TreapSumNode* n2 = AllocSumNode(2, 80);
            TreapSumNode* n3 = AllocSumNode(3, 60);
            root = TreapRangeSum.Merge(root, n1);
            root = TreapRangeSum.Merge(root, n2);
            root = TreapRangeSum.Merge(root, n3);

            try
            {
                TreapRangeSum.AddRange(ref root, 1, 2, 10);
                long s = TreapRangeSum.QuerySum(ref root, 0, 2);
                Assert.Equal(26L, s);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
                Marshal.FreeHGlobal((nint)n3);
            }
        }

        private static TreapAffineNode* AllocAffineNode(long val, int priority)
        {
            TreapAffineNode* n = (TreapAffineNode*)Marshal.AllocHGlobal(sizeof(TreapAffineNode));
            n->Priority = priority;
            n->Size = 1;
            n->Value = val;
            n->Sum = val;
            n->LazyA = 1;
            n->LazyB = 0;
            n->HasLazy = false;
            n->Left = null;
            n->Right = null;
            return n;
        }

        [Fact]
        public void TreapAffineRange_Basic()
        {
            TreapAffineNode* root = null;
            TreapAffineNode* n1 = AllocAffineNode(1, 100);
            TreapAffineNode* n2 = AllocAffineNode(2, 80);
            TreapAffineNode* n3 = AllocAffineNode(3, 60);
            root = TreapAffineRange.Merge(root, n1);
            root = TreapAffineRange.Merge(root, n2);
            root = TreapAffineRange.Merge(root, n3);

            try
            {
                TreapAffineRange.AffineRange(ref root, 0, 2, 2, 1);
                long s = TreapAffineRange.QuerySum(ref root, 0, 2);
                Assert.Equal(15L, s);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
                Marshal.FreeHGlobal((nint)n3);
            }
        }
    }
}
