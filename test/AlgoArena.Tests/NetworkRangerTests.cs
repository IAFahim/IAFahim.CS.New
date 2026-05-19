namespace AlgoArena.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.Graph;
    using IAFahim.Graph.Tree;

    public sealed unsafe class NetworkRangerTests
    {
        [Fact]
        public void Dijkstra_SingleEdge()
        {
            int n = 2;
            int* head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* weight = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* edgeId = (int*)Marshal.AllocHGlobal(sizeof(int));
            long* dist = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) head[i] = 0;
                *edgeId = 0;

                AddWeightedEdge.Run(head, to, next, weight, edgeId, 0, 1, 5, edgeId);

                Dijkstra.Run(n, 0, head, to, next, weight, dist, parent);

                Assert.Equal(0, dist[0]);
                Assert.Equal(5, dist[1]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)next);
                Marshal.FreeHGlobal((nint)weight);
                Marshal.FreeHGlobal((nint)edgeId);
                Marshal.FreeHGlobal((nint)dist);
                Marshal.FreeHGlobal((nint)parent);
            }
        }

        [Fact]
        public void Dijkstra_MultiplePaths()
        {
            int n = 4;
            int maxEdges = n * 4;
            int* head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* weight = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* edgeId = (int*)Marshal.AllocHGlobal(sizeof(int));
            long* dist = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) head[i] = 0;
                *edgeId = 0;

                AddWeightedEdge.Run(head, to, next, weight, edgeId, 0, 1, 1, edgeId);
                AddWeightedEdge.Run(head, to, next, weight, edgeId, 1, 2, 1, edgeId);
                AddWeightedEdge.Run(head, to, next, weight, edgeId, 0, 2, 5, edgeId);
                AddWeightedEdge.Run(head, to, next, weight, edgeId, 2, 3, 1, edgeId);
                AddWeightedEdge.Run(head, to, next, weight, edgeId, 1, 3, 10, edgeId);

                Dijkstra.Run(n, 0, head, to, next, weight, dist, parent);

                Assert.Equal(0, dist[0]);
                Assert.Equal(1, dist[1]);
                Assert.Equal(2, dist[2]);
                Assert.Equal(3, dist[3]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)next);
                Marshal.FreeHGlobal((nint)weight);
                Marshal.FreeHGlobal((nint)edgeId);
                Marshal.FreeHGlobal((nint)dist);
                Marshal.FreeHGlobal((nint)parent);
            }
        }

        [Fact]
        public void Dijkstra_Unreachable()
        {
            int n = 3;
            int* head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(6 * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(6 * sizeof(int));
            int* weight = (int*)Marshal.AllocHGlobal(6 * sizeof(int));
            int* edgeId = (int*)Marshal.AllocHGlobal(sizeof(int));
            long* dist = (long*)Marshal.AllocHGlobal(n * sizeof(long));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) head[i] = 0;
                *edgeId = 0;

                AddWeightedEdge.Run(head, to, next, weight, edgeId, 0, 1, 2, edgeId);

                Dijkstra.Run(n, 0, head, to, next, weight, dist, parent);

                Assert.Equal(0, dist[0]);
                Assert.Equal(2, dist[1]);
                Assert.Equal(long.MaxValue, dist[2]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)next);
                Marshal.FreeHGlobal((nint)weight);
                Marshal.FreeHGlobal((nint)edgeId);
                Marshal.FreeHGlobal((nint)dist);
                Marshal.FreeHGlobal((nint)parent);
            }
        }

        [Fact]
        public void DijkstraRestorePath_Simple()
        {
            int n = 4;
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* path = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) parent[i] = -1;
                parent[0] = -1;
                parent[1] = 0;
                parent[2] = 1;
                parent[3] = 2;

                int len = DijkstraRestorePath.Run(parent, 3, path);

                Assert.Equal(4, len);
                Assert.Equal(0, path[0]);
                Assert.Equal(1, path[1]);
                Assert.Equal(2, path[2]);
                Assert.Equal(3, path[3]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)parent);
                Marshal.FreeHGlobal((nint)path);
            }
        }

        [Fact]
        public void DijkstraRestorePath_DirectEdge()
        {
            int n = 2;
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* path = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                parent[0] = -1;
                parent[1] = 0;

                int len = DijkstraRestorePath.Run(parent, 1, path);

                Assert.Equal(2, len);
                Assert.Equal(0, path[0]);
                Assert.Equal(1, path[1]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)parent);
                Marshal.FreeHGlobal((nint)path);
            }
        }

        [Fact]
        public void Bfs_SimpleGraph()
        {
            int n = 4;
            int maxEdges = n * 4;
            int* head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* weight = (int*)Marshal.AllocHGlobal(maxEdges * sizeof(int));
            int* edgeId = (int*)Marshal.AllocHGlobal(sizeof(int));
            int* dist = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) head[i] = 0;
                *edgeId = 0;

                AddWeightedEdge.Run(head, to, next, weight, edgeId, 0, 1, 1, edgeId);
                AddWeightedEdge.Run(head, to, next, weight, edgeId, 0, 2, 1, edgeId);
                AddWeightedEdge.Run(head, to, next, weight, edgeId, 1, 3, 1, edgeId);
                AddWeightedEdge.Run(head, to, next, weight, edgeId, 2, 3, 1, edgeId);

                Bfs.Run(n, 0, head, to, next, dist, parent);

                Assert.Equal(0, dist[0]);
                Assert.Equal(1, dist[1]);
                Assert.Equal(1, dist[2]);
                Assert.Equal(2, dist[3]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)next);
                Marshal.FreeHGlobal((nint)weight);
                Marshal.FreeHGlobal((nint)edgeId);
                Marshal.FreeHGlobal((nint)dist);
                Marshal.FreeHGlobal((nint)parent);
            }
        }

        [Fact]
        public void Bfs_DisconnectedNodes()
        {
            int n = 4;
            int* head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(8 * sizeof(int));
            int* next = (int*)Marshal.AllocHGlobal(8 * sizeof(int));
            int* weight = (int*)Marshal.AllocHGlobal(8 * sizeof(int));
            int* edgeId = (int*)Marshal.AllocHGlobal(sizeof(int));
            int* dist = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                for (int i = 0; i < n; i++) head[i] = 0;
                *edgeId = 0;

                AddWeightedEdge.Run(head, to, next, weight, edgeId, 0, 1, 1, edgeId);

                Bfs.Run(n, 0, head, to, next, dist, parent);

                Assert.Equal(0, dist[0]);
                Assert.Equal(1, dist[1]);
                Assert.Equal(-1, dist[2]);
                Assert.Equal(-1, dist[3]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)head);
                Marshal.FreeHGlobal((nint)to);
                Marshal.FreeHGlobal((nint)next);
                Marshal.FreeHGlobal((nint)weight);
                Marshal.FreeHGlobal((nint)edgeId);
                Marshal.FreeHGlobal((nint)dist);
                Marshal.FreeHGlobal((nint)parent);
            }
        }
    }
}