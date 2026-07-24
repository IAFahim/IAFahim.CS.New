namespace IAFahim.Graph.DAG.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class DagPathCoverTests
    {
        [Test]
        public void Empty_ReturnsZero()
        {
            int* match = stackalloc int[1];
            int* dist = stackalloc int[1];
            int* queue = stackalloc int[1];
            int* head = stackalloc int[1];
            head[0] = 0;
            Assert.AreEqual(0, DagMinimumPathCover.Run(head, null, null, match, dist, queue, 0));
        }

        [Test]
        public void Chain_NeedsOnePath()
        {
            const int N = 3;
            int* head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(8 * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(8 * sizeof(int));
            int* match = (int*)Marshal.AllocHGlobal(2 * N * sizeof(int));
            int* dist = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* queue = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) head[i] = 0;
                int e = 1;
                AddEdge(head, next, to, 0, 1, ref e);
                AddEdge(head, next, to, 1, 2, ref e);
                int cover = DagMinimumPathCover.Run(head, next, to, match, dist, queue, N);
                Assert.AreEqual(1, cover);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)next);
                Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)match);
                Marshal.FreeHGlobal((nint)dist);
                Marshal.FreeHGlobal((nint)queue);
            }
        }

        [Test]
        public void DisjointVertices_NeedsNPaths()
        {
            const int N = 4;
            int* head = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* match = (int*)Marshal.AllocHGlobal(2 * N * sizeof(int));
            int* dist = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            int* queue = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++) head[i] = 0;
                int cover = DagMinimumPathCover.Run(head, next, to, match, dist, queue, N);
                Assert.AreEqual(N, cover);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)next);
                Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)match);
                Marshal.FreeHGlobal((nint)dist);
                Marshal.FreeHGlobal((nint)queue);
            }
        }

        private static void AddEdge(int* head, int* next, int* to, int u, int v, ref int e)
        {
            to[e] = v;
            next[e] = head[u];
            head[u] = e;
            e++;
        }
    }

    public sealed unsafe class DagAntichainTests
    {
        [Test]
        public void TotalOrder_AntichainOne()
        {
            const int N = 3;
            bool* reach = stackalloc bool[N * N];
            int* matchRight = stackalloc int[N];
            bool* visited = stackalloc bool[N];
            for (int i = 0; i < N * N; i++) reach[i] = false;
            reach[0 * N + 1] = true;
            reach[0 * N + 2] = true;
            reach[1 * N + 2] = true;
            int len = DagLongestAntichain.Run(reach, matchRight, visited, N);
            Assert.AreEqual(1, len);
        }

        [Test]
        public void EmptyRelations_AntichainN()
        {
            const int N = 5;
            bool* reach = stackalloc bool[N * N];
            int* matchRight = stackalloc int[N];
            bool* visited = stackalloc bool[N];
            for (int i = 0; i < N * N; i++) reach[i] = false;
            Assert.AreEqual(N, DagLongestAntichain.Run(reach, matchRight, visited, N));
        }
    }
}
