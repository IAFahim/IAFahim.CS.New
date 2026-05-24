namespace IAFahim.Graph.DynamicTrees.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Graph.DynamicTrees;

    public sealed unsafe class DynamicTreesTests
    {
        [Test]
        public void LinkCutTree_Basic()
        {
            const int N = 10;
            LctNode* nodes = stackalloc LctNode[N];
            LinkCutTree.Init(nodes, N);

            // Set initial values
            for (int i = 0; i < N; i++)
            {
                nodes[i].Val = i + 1;
                LinkCutTree.PushUp(nodes, i);
            }

            // Initially disconnected
            for (int i = 0; i < N; i++)
            {
                Assert.AreEqual(i, LinkCutTree.FindRoot(nodes, i));
            }

            // Link: 0-1, 1-2, 3-4
            LinkCutTree.Link(nodes, 0, 1);
            LinkCutTree.Link(nodes, 1, 2);
            LinkCutTree.Link(nodes, 3, 4);

            Assert.AreEqual(LinkCutTree.FindRoot(nodes, 0), LinkCutTree.FindRoot(nodes, 2));
            Assert.AreNotEqual(LinkCutTree.FindRoot(nodes, 0), LinkCutTree.FindRoot(nodes, 3));

            // Path query: path between 0 and 2 should have values 1, 2, 3
            // PathMin = 1, PathMax = 3, PathSum = 6
            Assert.AreEqual(1, LinkCutTree.PathMin(nodes, 0, 2));
            Assert.AreEqual(3, LinkCutTree.PathMax(nodes, 0, 2));

            // PathAdd: add 10 to path 0-2
            LinkCutTree.PathAdd(nodes, 0, 2, 10);
            Assert.AreEqual(11, LinkCutTree.PathMin(nodes, 0, 2));
            Assert.AreEqual(13, LinkCutTree.PathMax(nodes, 0, 2));

            // Cut: 1-2
            LinkCutTree.Cut(nodes, 1, 2);
            Assert.AreNotEqual(LinkCutTree.FindRoot(nodes, 0), LinkCutTree.FindRoot(nodes, 2));
        }

        [Test]
        public void EulerTourTree_Basic()
        {
            const int N = 10;
            // ETT needs vertex nodes (N) and directed edge nodes (2 * (N - 1))
            // Total nodes: N + 2 * N = 3 * N
            const int totalNodes = 3 * N;
            EttNode* nodes = stackalloc EttNode[totalNodes];
            EulerTourTree.Init(nodes, totalNodes);

            // Set initial values on vertex nodes
            for (int i = 0; i < N; i++)
            {
                nodes[i].Val = i + 1;
                EulerTourTree.PushUp(nodes, i);
            }

            uint randState = 42;

            // Link: 0-1 (edge node indices N and N+1)
            // Link: 1-2 (edge node indices N+2 and N+3)
            EulerTourTree.Link(nodes, 0, 1, N, N + 1, ref randState);
            EulerTourTree.Link(nodes, 1, 2, N + 2, N + 3, ref randState);

            Assert.IsTrue(EulerTourTree.Connected(nodes, 0, 2));
            Assert.IsFalse(EulerTourTree.Connected(nodes, 0, 3));

            // Subtree queries
            // Since we rerooted and linked, let's query the subtree of 1
            // Root the tree at 0
            EulerTourTree.Reroot(nodes, 0, ref randState);
            // Now 0 is the root. Subtree of 1 contains 1 and 2.
            // Values of 1 and 2 are 2 and 3. Sum = 5.
            Assert.AreEqual(5, EulerTourTree.SubtreeQuery(nodes, 1));

            // Cut 1-2
            EulerTourTree.Cut(nodes, N + 2, N + 3, ref randState);
            Assert.IsFalse(EulerTourTree.Connected(nodes, 1, 2));
        }

        [Test]
        public void TopTree_Basic()
        {
            const int N = 5;
            TopTreeNode* nodes = stackalloc TopTreeNode[N];
            TopTree.Init(nodes, N);

            for (int i = 0; i < N; i++)
            {
                nodes[i].Val = i + 1;
                TopTree.PushUp(nodes, i);
            }

            // Link: 0-1, 1-2
            TopTree.Link(nodes, 0, 1);
            TopTree.Link(nodes, 1, 2);

            Assert.AreEqual(1 + 2 + 3, TopTree.PathQuery(nodes, 0, 2));

            // Cut 1-2
            TopTree.Cut(nodes, 1, 2);
            Assert.AreEqual(1 + 2, TopTree.PathQuery(nodes, 0, 1));
        }
    }
}
