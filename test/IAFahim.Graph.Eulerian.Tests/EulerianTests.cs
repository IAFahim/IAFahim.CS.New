namespace IAFahim.Graph.Eulerian.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class EulerianTests
    {
        // Builds undirected graph with Convention-A edges starting at index 2 (even), so the
        // XOR-pair trick `e ^ 1` used by EulerianPathUndirected correctly pairs (2k, 2k+1).
        // head zeroed (sentinel). out params freed by caller via try/finally.
        private static void BuildUndirected(int n, (int u, int v)[] edges, out int* head, out int* to, out int* next)
        {
            int maxE = edges.Length * 2;
            head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            to = (int*)Marshal.AllocHGlobal((maxE + 2) * sizeof(int));
            next = (int*)Marshal.AllocHGlobal((maxE + 2) * sizeof(int));
            for (int i = 0; i < n; i++) head[i] = 0;
            int ec = 2; // even start for XOR pairing
            foreach ((int u, int v) in edges)
            {
                int id1 = ec++; to[id1] = v; next[id1] = head[u]; head[u] = id1;
                int id2 = ec++; to[id2] = u; next[id2] = head[v]; head[v] = id2;
            }
        }

        // Builds directed graph; no pairing constraint.
        private static void BuildDirected(int n, (int u, int v)[] edges, out int* head, out int* to, out int* next)
        {
            int maxE = edges.Length;
            head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            to = (int*)Marshal.AllocHGlobal((maxE + 1) * sizeof(int));
            next = (int*)Marshal.AllocHGlobal((maxE + 1) * sizeof(int));
            for (int i = 0; i < n; i++) head[i] = 0;
            int ec = 1;
            foreach ((int u, int v) in edges)
            {
                int id = ec++; to[id] = v; next[id] = head[u]; head[u] = id;
            }
        }

        private static void Free(int* head, int* to, int* next)
        {
            Marshal.FreeHGlobal((nint)head);
            Marshal.FreeHGlobal((nint)to);
            Marshal.FreeHGlobal((nint)next);
        }

        [Test]
        public void Undirected_Triangle_EulerianCircuit()
        {
            const int N = 3;
            BuildUndirected(N, new[] { (0, 1), (1, 2), (2, 0) }, out int* head, out int* to, out int* next);
            try
            {
                int* path = stackalloc int[8];
                int len = EulerianPathUndirected.Run(N, head, to, next, 0, path);
                // Eulerian circuit uses all 3 edges => visits 4 nodes (start repeats at end).
                Assert.AreEqual(4, len);
                Assert.AreEqual(path[0], path[len - 1]);
                // All three edges {01, 12, 20} must appear in the walk.
                Assert.IsTrue(UsesAllTriangleEdges(path, len));
            }
            finally { Free(head, to, next); }
        }

        private static bool UsesAllTriangleEdges(int* path, int len)
        {
            bool ab = false, bc = false, ca = false;
            for (int i = 0; i + 1 < len; i++)
            {
                int a = path[i], b = path[i + 1];
                if ((a == 0 && b == 1) || (a == 1 && b == 0)) ab = true;
                if ((a == 1 && b == 2) || (a == 2 && b == 1)) bc = true;
                if ((a == 2 && b == 0) || (a == 0 && b == 2)) ca = true;
            }
            return ab && bc && ca;
        }

        [Test]
        public void Undirected_Square_EulerianCircuit()
        {
            // 0-1-2-3-0 (4-cycle). All degrees even.
            const int N = 4;
            BuildUndirected(N, new[] { (0, 1), (1, 2), (2, 3), (3, 0) }, out int* head, out int* to, out int* next);
            try
            {
                int* path = stackalloc int[10];
                int len = EulerianPathUndirected.Run(N, head, to, next, 0, path);
                Assert.AreEqual(5, len);
                Assert.AreEqual(path[0], path[len - 1]);
            }
            finally { Free(head, to, next); }
        }

        [Test]
        public void Undirected_NoEulerianPath_TooManyOdd()
        {
            // Star: center 0 connected to 1,2,3. 3 odd-degree nodes => no Eulerian path.
            const int N = 4;
            BuildUndirected(N, new[] { (0, 1), (0, 2), (0, 3) }, out int* head, out int* to, out int* next);
            try
            {
                int* path = stackalloc int[10];
                int len = EulerianPathUndirected.Run(N, head, to, next, 0, path);
                Assert.AreEqual(0, len);
            }
            finally { Free(head, to, next); }
        }

        [Test]
        public void Directed_SimplePath_TrailFromSource()
        {
            // 0->1->2. Eulerian trail from 0 (outdeg=1) to 2 (indeg=1).
            const int N = 3;
            BuildDirected(N, new[] { (0, 1), (1, 2) }, out int* head, out int* to, out int* next);
            try
            {
                int* path = stackalloc int[8];
                int len = EulerianPathDirected.Run(N, head, to, next, 0, path);
                Assert.AreEqual(3, len);
                Assert.AreEqual(0, path[0]);
                Assert.AreEqual(2, path[len - 1]);
            }
            finally { Free(head, to, next); }
        }

        [Test]
        public void Directed_Cycle_EulerianCircuit()
        {
            // 0->1->2->0. Balanced degrees => Eulerian circuit.
            const int N = 3;
            BuildDirected(N, new[] { (0, 1), (1, 2), (2, 0) }, out int* head, out int* to, out int* next);
            try
            {
                int* path = stackalloc int[8];
                int len = EulerianPathDirected.Run(N, head, to, next, 0, path);
                Assert.AreEqual(4, len);
                Assert.AreEqual(path[0], path[len - 1]);
            }
            finally { Free(head, to, next); }
        }
    }
}
