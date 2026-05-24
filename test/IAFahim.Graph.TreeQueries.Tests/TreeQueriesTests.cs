namespace IAFahim.Graph.TreeQueries.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Graph.TreeQueries;

    public sealed unsafe class TreeQueriesTests
    {
        private static void AddEdge(int* head, int* to, int* next, ref int edgeCount, int u, int v)
        {
            to[edgeCount] = v;
            next[edgeCount] = head[u];
            head[u] = edgeCount++;

            to[edgeCount] = u;
            next[edgeCount] = head[v];
            head[v] = edgeCount++;
        }

        [Test]
        public void Centroid_Basic()
        {
            const int N = 5;
            int* head = stackalloc int[N];
            int* to = stackalloc int[2 * N];
            int* next = stackalloc int[2 * N];
            int edgeCount = 1;

            for (int i = 0; i < N; i++) head[i] = 0;

            // Edges: 0-1, 1-2, 1-3, 3-4
            AddEdge(head, to, next, ref edgeCount, 0, 1);
            AddEdge(head, to, next, ref edgeCount, 1, 2);
            AddEdge(head, to, next, ref edgeCount, 1, 3);
            AddEdge(head, to, next, ref edgeCount, 3, 4);

            int* centroids = stackalloc int[2];
            int centroidCount = 0;
            TreeCentroid.AllCentroids(N, head, to, next, centroids, ref centroidCount);

            // The centroid is node 1 (size of components if 1 is removed: 0: 1, 2: 1, 3: 2. Max size is 2 <= 5/2 = 2).
            Assert.AreEqual(1, centroidCount);
            Assert.AreEqual(1, centroids[0]);

            long* weights = stackalloc long[N];
            weights[0] = 1;
            weights[1] = 1;
            weights[2] = 10; // heavy demand on node 2
            weights[3] = 1;
            weights[4] = 1;

            int median = TreeCentroid.WeightedMedian(N, head, to, next, weights);
            Assert.AreEqual(2, median); // node 2 has 10/14 weight, so it is the weighted median
        }

        [Test]
        public void TreeHashing_Basic()
        {
            // Create two isomorphic trees
            // Tree 1: 0-1, 1-2 (line)
            const int N1 = 3;
            int* head1 = stackalloc int[N1];
            int* to1 = stackalloc int[2 * N1];
            int* next1 = stackalloc int[2 * N1];
            int ec1 = 1;
            for (int i = 0; i < N1; i++) head1[i] = 0;
            AddEdge(head1, to1, next1, ref ec1, 0, 1);
            AddEdge(head1, to1, next1, ref ec1, 1, 2);

            // Tree 2: 0-2, 2-1 (line, isomorphic)
            const int N2 = 3;
            int* head2 = stackalloc int[N2];
            int* to2 = stackalloc int[2 * N2];
            int* next2 = stackalloc int[2 * N2];
            int ec2 = 1;
            for (int i = 0; i < N2; i++) head2[i] = 0;
            AddEdge(head2, to2, next2, ref ec2, 0, 2);
            AddEdge(head2, to2, next2, ref ec2, 2, 1);

            ulong hash1 = TreeHashing.CanonicalHash(N1, head1, to1, next1);
            ulong hash2 = TreeHashing.CanonicalHash(N2, head2, to2, next2);
            Assert.AreEqual(hash1, hash2);

            // Automorphism count of 3-node line (0-1-2) rooted at centroid 1:
            // Centroid is 1. Children of 1 are 0 and 2. Both are subtrees of size 1 (same hash).
            // Auto count is Auto(0) * Auto(2) * 2! = 2.
            long autos = TreeHashing.AutomorphismCount(N1, head1, to1, next1, 1000000007);
            Assert.AreEqual(2, autos);

            // Tree edit distance: Line (0-1-2) to Line (0-1-2) should be 0.
            int dist = TreeHashing.TreeEditDistance(N1, head1, to1, next1, N2, head2, to2, next2);
            Assert.AreEqual(0, dist);

            // Embedding check: Tree 1 embeds into a larger tree
            // Tree 3: 0-1, 1-2, 1-3 (star-like)
            const int N3 = 4;
            int* head3 = stackalloc int[N3];
            int* to3 = stackalloc int[2 * N3];
            int* next3 = stackalloc int[2 * N3];
            int ec3 = 1;
            for (int i = 0; i < N3; i++) head3[i] = 0;
            AddEdge(head3, to3, next3, ref ec3, 0, 1);
            AddEdge(head3, to3, next3, ref ec3, 1, 2);
            AddEdge(head3, to3, next3, ref ec3, 1, 3);

            // Line (N1=3) embeds into star (N3=4) (0-1-2 is a path of size 3 in both)
            Assert.IsTrue(TreeHashing.EmbeddingCheck(N1, head1, to1, next1, N3, head3, to3, next3));
        }

        [Test]
        public void TreeDp_Basic()
        {
            const int N = 5;
            int* head = stackalloc int[N];
            int* to = stackalloc int[2 * N];
            int* next = stackalloc int[2 * N];
            int edgeCount = 1;

            for (int i = 0; i < N; i++) head[i] = 0;

            // Edges: 0-1, 1-2, 1-3, 3-4
            AddEdge(head, to, next, ref edgeCount, 0, 1);
            AddEdge(head, to, next, ref edgeCount, 1, 2);
            AddEdge(head, to, next, ref edgeCount, 1, 3);
            AddEdge(head, to, next, ref edgeCount, 3, 4);

            int minCover = TreeDp.MinVertexCover(N, head, to, next);
            Assert.AreEqual(2, minCover); // cover is {1, 3}

            int maxInd = TreeDp.MaxIndependentSet(N, head, to, next);
            Assert.AreEqual(3, maxInd); // independent set is {0, 2, 4}

            int minDom = TreeDp.DominatingSet(N, head, to, next);
            Assert.AreEqual(2, minDom); // dominating set is {1, 3}

            long* edgeWeights = stackalloc long[2 * N];
            for (int i = 0; i < 2 * N; i++) edgeWeights[i] = 1;

            long maxMatching = TreeDp.MatchingDp(N, head, to, next, edgeWeights);
            Assert.AreEqual(2, maxMatching); // matching is {0-1, 3-4}

            byte* isTerminal = stackalloc byte[N];
            for (int i = 0; i < N; i++) isTerminal[i] = 0;
            isTerminal[2] = 1;
            isTerminal[4] = 1;

            long steinerCost = TreeDp.SteinerTree(N, head, to, next, edgeWeights, isTerminal, 2);
            Assert.AreEqual(3, steinerCost); // edges in Steiner tree: 2-1, 1-3, 3-4 (total 3 edges, cost 3)
        }
    }
}
