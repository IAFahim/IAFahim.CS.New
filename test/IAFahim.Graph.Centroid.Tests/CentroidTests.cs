namespace IAFahim.Graph.Centroid.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class CentroidTests
    {
        // Convention-A tree builder: allocates head/to/next on heap, head zeroed (sentinel),
        // edges indexed from 1. Fills the out pointers; caller frees all three via try/finally.
        private static void BuildTree(int n, (int u, int v)[] edges, out int* head, out int* to, out int* next)
        {
            int maxE = edges.Length * 2;
            head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            to = (int*)Marshal.AllocHGlobal((maxE + 1) * sizeof(int));
            next = (int*)Marshal.AllocHGlobal((maxE + 1) * sizeof(int));
            for (int i = 0; i < n; i++) head[i] = 0;
            int ec = 1;
            foreach ((int u, int v) in edges)
            {
                int id1 = ec++; to[id1] = v; next[id1] = head[u]; head[u] = id1;
                int id2 = ec++; to[id2] = u; next[id2] = head[v]; head[v] = id2;
            }
        }

        [Test]
        public void PathGraph_CentroidIsMiddle()
        {
            const int N = 5;
            BuildTree(N, new[] { (0, 1), (1, 2), (2, 3), (3, 4) }, out int* head, out int* to, out int* next);
            try
            {
                int* sz = stackalloc int[N];
                int* centroid = stackalloc int[1];
                byte* removed = stackalloc byte[N];
                int c = CentroidDecomposition.Build(N, head, to, next, centroid, sz, removed);
                Assert.AreEqual(2, c);
            }
            finally { Free(head, to, next); }
        }

        [Test]
        public void StarGraph_CentroidIsCenter()
        {
            const int N = 5;
            BuildTree(N, new[] { (0, 1), (0, 2), (0, 3), (0, 4) }, out int* head, out int* to, out int* next);
            try
            {
                int* sz = stackalloc int[N];
                int* centroid = stackalloc int[1];
                byte* removed = stackalloc byte[N];
                int c = CentroidDecomposition.Build(N, head, to, next, centroid, sz, removed);
                Assert.AreEqual(0, c);
            }
            finally { Free(head, to, next); }
        }

        [Test]
        public void SingleNode_CentroidIsSelf()
        {
            const int N = 1;
            BuildTree(N, Array.Empty<(int, int)>(), out int* head, out int* to, out int* next);
            try
            {
                int* sz = stackalloc int[N];
                int* centroid = stackalloc int[1];
                byte* removed = stackalloc byte[N];
                int c = CentroidDecomposition.Build(N, head, to, next, centroid, sz, removed);
                Assert.AreEqual(0, c);
            }
            finally { Free(head, to, next); }
        }

        [Test]
        public void Decompose_AllNodesBecomeCentroidOnce()
        {
            const int N = 6;
            BuildTree(N, new[] { (0, 1), (1, 2), (1, 3), (0, 4), (4, 5) }, out int* head, out int* to, out int* next);
            try
            {
                int* sz = stackalloc int[N];
                int* centroids = stackalloc int[N];
                int centroidCount = 0;
                byte* removed = stackalloc byte[N];
                CentroidDecomposition.Decompose(N, head, to, next, 0, removed, sz, centroids, &centroidCount);
                Assert.AreEqual(N, centroidCount);
                bool[] seen = new bool[N];
                for (int i = 0; i < centroidCount; i++)
                {
                    Assert.IsTrue(centroids[i] >= 0 && centroids[i] < N);
                    Assert.IsFalse(seen[centroids[i]], $"node {centroids[i]} appeared twice");
                    seen[centroids[i]] = true;
                }
            }
            finally { Free(head, to, next); }
        }

        [Test]
        public void Decompose_PerfectBinaryTree_RootIsFirstCentroid()
        {
            const int N = 7;
            BuildTree(N, new[] { (0, 1), (0, 2), (1, 3), (1, 4), (2, 5), (2, 6) }, out int* head, out int* to, out int* next);
            try
            {
                int* sz = stackalloc int[N];
                int* centroids = stackalloc int[N];
                int centroidCount = 0;
                byte* removed = stackalloc byte[N];
                CentroidDecomposition.Decompose(N, head, to, next, 0, removed, sz, centroids, &centroidCount);
                Assert.AreEqual(N, centroidCount);
                Assert.AreEqual(0, centroids[0]);
            }
            finally { Free(head, to, next); }
        }

        private static void Free(int* head, int* to, int* next)
        {
            Marshal.FreeHGlobal((nint)head);
            Marshal.FreeHGlobal((nint)to);
            Marshal.FreeHGlobal((nint)next);
        }
    }
}
