namespace IAFahim.DS.Rope.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class RopeTests
    {
        private static RopeNode* AllocNode(byte val, int priority)
        {
            RopeNode* n = (RopeNode*)Marshal.AllocHGlobal(sizeof(RopeNode));
            n->Size = 1;
            n->Priority = priority;
            n->Value = val;
            n->Left = null;
            n->Right = null;
            return n;
        }

        [Test]
        public void RopeInsert_EmptyTree_SingleNode()
        {
            RopeNode* root = null;
            RopeNode* node = AllocNode((byte)'A', 100);
            try
            {
                root = RopeInsert.Run(root, 0, node);
                Assert.AreEqual(1, root->Size);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)node);
            }
        }

        [Test]
        public void RopeErase_SingleElement_BecomesEmpty()
        {
            RopeNode* root = null;
            RopeNode* node = AllocNode((byte)'A', 100);
            root = RopeInsert.Run(root, 0, node);
            root = RopeErase.Run(root, 0, 1);
            Assert.IsTrue(root == null);
            Marshal.FreeHGlobal((nint)node);
        }

        [Test]
        public void RopeSubstring_Basic()
        {
            RopeNode* root = null;
            RopeNode* n1 = AllocNode((byte)'H', 100);
            RopeNode* n2 = AllocNode((byte)'i', 80);
            root = RopeInsert.Run(root, 0, n1);
            root = RopeInsert.Run(root, 1, n2);

            byte* buf = stackalloc byte[10];
            root = RopeSubstring.Run(root, 0, 2, buf, out int count);

            try
            {
                Assert.AreEqual(2, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)n1);
                Marshal.FreeHGlobal((nint)n2);
            }
        }
    }
}
