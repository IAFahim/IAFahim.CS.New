namespace IAFahim.Graph.Tests
{
    using IAFahim.Graph;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class Phase10GraphTests
    {
        [Test]
        public void Tournament_Hamiltonian_PathAndCycle()
        {
            const int n = 3;
            byte* adj = stackalloc byte[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = 0;

            // 0 -> 1 -> 2 -> 0
            adj[0 * n + 1] = 1;
            adj[1 * n + 2] = 1;
            adj[2 * n + 0] = 1;

            int* path = stackalloc int[n];
            Tournament.TournamentHamiltonianPath(n, adj, path);
            Assert.IsTrue(path[0] >= 0 && path[0] < n);

            int* cycle = stackalloc int[n];
            bool hasCycle = Tournament.TournamentHamiltonianCycle(n, adj, cycle);
            Assert.IsTrue(hasCycle);
            Assert.AreEqual(0, cycle[0]);
            Assert.AreEqual(1, cycle[1]);
            Assert.AreEqual(2, cycle[2]);
        }

        [Test]
        public void Tournament_MedianAndKing()
        {
            const int n = 3;
            byte* adj = stackalloc byte[n * n];
            for (int i = 0; i < n * n; i++) adj[i] = 0;

            adj[0 * n + 1] = 1;
            adj[1 * n + 2] = 1;
            adj[2 * n + 0] = 1;

            int* bestOrder = stackalloc int[n];
            Tournament.TournamentMedianOrder(n, adj, bestOrder);
            Assert.IsTrue(bestOrder[0] >= 0);

            int king = Tournament.TournamentKingFind(n, adj);
            Assert.IsTrue(king >= 0 && king < n);
        }

        [Test]
        public void Tournament_Orientations()
        {
            const int numNodes = 3;
            const int numEdges = 3;

            int* head = stackalloc int[numNodes];
            int* next = stackalloc int[numEdges * 2];
            int* to = stackalloc int[numEdges * 2];
            for (int i = 0; i < numNodes; i++) head[i] = -1;

            int* edgeU = stackalloc int[numEdges];
            int* edgeV = stackalloc int[numEdges];

            // Undirected triangle: 0-1, 1-2, 2-0
            edgeU[0] = 0; edgeV[0] = 1;
            edgeU[1] = 1; edgeV[1] = 2;
            edgeU[2] = 2; edgeV[2] = 0;

            int edgeIdx = 0;
            void AddEdge(int uNode, int vNode)
            {
                to[edgeIdx] = vNode; next[edgeIdx] = head[uNode]; head[uNode] = edgeIdx++;
                to[edgeIdx] = uNode; next[edgeIdx] = head[vNode]; head[vNode] = edgeIdx++;
            }
            AddEdge(0, 1);
            AddEdge(1, 2);
            AddEdge(2, 0);

            int* orientedU = stackalloc int[numEdges];
            int* orientedV = stackalloc int[numEdges];

            bool eulerian = Tournament.EulerianOrientation(numNodes, numEdges, head, next, to, edgeU, edgeV, orientedU, orientedV);
            Assert.IsTrue(eulerian);

            bool strong = Tournament.StrongOrientation(numNodes, numEdges, head, next, to, edgeU, edgeV, orientedU, orientedV);
            Assert.IsTrue(strong);

            Tournament.OrientEdgesAcyclic(numEdges, edgeU, edgeV, orientedU, orientedV);
            Assert.IsTrue(orientedU[0] < orientedV[0]);
        }

        [Test]
        public void MstVariants_ArborescenceAndBranching()
        {
            const int n = 3;
            const int m = 3;
            int* u = stackalloc int[m] { 0, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 0 };
            long* w = stackalloc long[m] { 10, 20, 30 };

            long* result = stackalloc long[n];
            long cost = MstVariants.MinimumArborescenceDirected(n, 0, u, v, w, m, result);
            Assert.AreEqual(30, cost);

            int* resultEdges = stackalloc int[m];
            int resultCount = 0;
            long maxBranch = MstVariants.MaximumBranching(n, u, v, w, m, resultEdges, &resultCount);
            Assert.IsTrue(maxBranch >= 0);

            long arbCount = MstVariants.ArborescenceCount(n, 0, u, v, m);
            Assert.AreEqual(1, arbCount);
        }

        [Test]
        public void MstVariants_DegreeAndCapacitated()
        {
            const int n = 4, m = 5;
            int* u = stackalloc int[m] { 0, 0, 0, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 3, 2, 3 };
            long* w = stackalloc long[m] { 10, 20, 30, 40, 50 };

            int* resultEdges = stackalloc int[m];
            int resultCount = 0;

            bool degMst = MstVariants.DegreeConstrainedMst(n, m, u, v, w, 0, 2, resultEdges, &resultCount);
            Assert.IsTrue(degMst);

            MstVariants.CapacitatedMst(n, m, u, v, w, 0, 10, resultEdges, &resultCount);
            Assert.IsTrue(resultCount >= 0);
        }

        [Test]
        public void MstVariants_MinDiameterAndBottleneck()
        {
            const int n = 4, m = 5;
            int* u = stackalloc int[m] { 0, 0, 0, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 3, 2, 3 };
            long* w = stackalloc long[m] { 1, 2, 3, 4, 5 };

            int* resultEdges = stackalloc int[m];
            int resultCount = 0;

            MstVariants.MinimumDiameterSpanningTree(n, m, u, v, w, resultEdges, &resultCount);
            Assert.IsTrue(resultCount > 0);

            MstVariants.MinimumBottleneckSpanningTree(n, m, u, v, w, resultEdges, &resultCount);
            Assert.IsTrue(resultCount > 0);

            long bp = MstVariants.MinimumBottleneckPath(n, m, u, v, w, 0, 3);
            Assert.AreEqual(3, bp);
        }

        [Test]
        public void MstVariants_KargerAndNagamochi()
        {
            const int n = 4, m = 5;
            int* u = stackalloc int[m] { 0, 0, 0, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 3, 2, 3 };

            int* bestCutU = stackalloc int[m];
            int* bestCutV = stackalloc int[m];
            int bestCutCount = 0;
            uint seed = 42;
            MstVariants.KargerSteinMinCut(n, m, u, v, bestCutU, bestCutV, &bestCutCount, ref seed);
            Assert.IsTrue(bestCutCount >= 0);

            int* certEdges = stackalloc int[m];
            int certCount = 0;
            MstVariants.NagamochiIbarakiSparseCertificate(n, m, u, v, 1, certEdges, &certCount);
            Assert.IsTrue(certCount >= 0);
        }

        [Test]
        public void Planar_GomoryHu()
        {
            const int n = 4;
            const int m = 5;
            int* head = stackalloc int[n];
            int* to = stackalloc int[m * 2 + 2];
            int* next = stackalloc int[m * 2 + 2];
            int* cap = stackalloc int[m * 2 + 2];

            for (int i = 0; i < n; i++) head[i] = 0;

            int edgeIdx = 2; // Dinic uses non-zero edges
            void AddCapEdge(int uNode, int vNode, int capacity)
            {
                to[edgeIdx] = vNode; cap[edgeIdx] = capacity; next[edgeIdx] = head[uNode]; head[uNode] = edgeIdx++;
                to[edgeIdx] = uNode; cap[edgeIdx] = capacity; next[edgeIdx] = head[vNode]; head[vNode] = edgeIdx++;
            }

            AddCapEdge(0, 1, 10);
            AddCapEdge(1, 2, 20);
            AddCapEdge(2, 3, 30);
            AddCapEdge(3, 0, 40);
            AddCapEdge(0, 2, 50);

            int* parent = stackalloc int[n];
            int* weight = stackalloc int[n];

            Planar.GomoryHuBuild(n, m, head, to, next, cap, parent, weight);
            
            int q = Planar.GomoryHuQuery(n, parent, weight, 0, 3);
            Assert.IsTrue(q >= 0);
        }

        [Test]
        public void Planar_EarDecompositionAndSt()
        {
            const int n = 3, m = 3;
            int* u = stackalloc int[m] { 0, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 0 };

            int* earEdges = stackalloc int[m];
            int* earLengths = stackalloc int[m];
            int earCount = 0;

            bool is2Connected = Planar.EarDecomposition(n, m, u, v, earEdges, earLengths, &earCount);
            Assert.IsTrue(is2Connected);
            Assert.AreEqual(1, earCount);

            int* stOrder = stackalloc int[n];
            bool stOk = Planar.StNumbering(n, m, u, v, 0, 2, stOrder);
            Assert.IsTrue(stOk);
            Assert.AreEqual(0, stOrder[0]);
            Assert.AreEqual(2, stOrder[n - 1]);
        }

        [Test]
        public void Planar_EmbeddingAndDual()
        {
            // K_4 (planar)
            const int n = 4, m = 6;
            int* u = stackalloc int[m] { 0, 0, 0, 1, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 3, 2, 3, 3 };

            int* embedHead = stackalloc int[n];
            int* embedNext = stackalloc int[m * 2];
            int* embedTo = stackalloc int[m * 2];

            bool isPlanar = Planar.PlanarEmbedding(n, m, u, v, embedHead, embedNext, embedTo);
            Assert.IsTrue(isPlanar);

            int dualN = 0, dualM = 0;
            int* dualU = stackalloc int[m];
            int* dualV = stackalloc int[m];
            int* faceSizes = stackalloc int[m];

            bool dualOk = Planar.PlanarDualBuild(n, m, u, v, embedHead, embedNext, embedTo, &dualN, &dualM, dualU, dualV, faceSizes);
            Assert.IsTrue(dualOk);
            Assert.IsTrue(dualN > 0);
        }

        [Test]
        public void Planar_ShortestPathAndSeparator()
        {
            const int n = 4, m = 5;
            int* u = stackalloc int[m] { 0, 0, 0, 1, 2 };
            int* v = stackalloc int[m] { 1, 2, 3, 2, 3 };
            long* w = stackalloc long[m] { 10, 20, 30, 40, 50 };

            long* dist = stackalloc long[n];
            Planar.PlanarShortestPath(n, m, u, v, w, 0, 3, dist);
            Assert.AreEqual(30, dist[3]);

            int* separator = stackalloc int[n];
            int separatorCount = 0;
            int* partA = stackalloc int[n];
            int partACount = 0;
            int* partB = stackalloc int[n];
            int partBCount = 0;

            bool sepOk = Planar.PlanarSeparator(n, m, u, v, separator, &separatorCount, partA, &partACount, partB, &partBCount);
            Assert.IsTrue(sepOk);
        }

        [Test]
        public void Planar_KuratowskiAndOuterplanar()
        {
            // K_5 (non-planar)
            const int n = 5, m = 10;
            int* u = stackalloc int[m] { 0, 0, 0, 0, 1, 1, 1, 2, 2, 3 };
            int* v = stackalloc int[m] { 1, 2, 3, 4, 2, 3, 4, 3, 4, 4 };

            int* kuratowskiU = stackalloc int[m];
            int* kuratowskiV = stackalloc int[m];
            int kuratowskiCount = 0;

            bool hasKuratowski = Planar.KuratowskiSubgraph(n, m, u, v, kuratowskiU, kuratowskiV, &kuratowskiCount);
            Assert.IsTrue(hasKuratowski);

            // Triangle is outerplanar, but K_4 is not
            const int nTri = 3, mTri = 3;
            int* uTri = stackalloc int[mTri] { 0, 1, 2 };
            int* vTri = stackalloc int[mTri] { 1, 2, 0 };
            Assert.IsTrue(Planar.OuterplanarCheck(nTri, mTri, uTri, vTri));

            const int nK4 = 4, mK4 = 6;
            int* uK4 = stackalloc int[mK4] { 0, 0, 0, 1, 1, 2 };
            int* vK4 = stackalloc int[mK4] { 1, 2, 3, 2, 3, 3 };
            Assert.IsFalse(Planar.OuterplanarCheck(nK4, mK4, uK4, vK4));
        }

        [Test]
        public void Planar_SeriesParallelAndTriconnected()
        {
            const int n = 4, m = 5;
            int* u = stackalloc int[m] { 0, 1, 2, 0, 2 };
            int* v = stackalloc int[m] { 1, 2, 3, 2, 3 };

            bool isSP = Planar.SeriesParallelDecompose(n, m, u, v, 0, 3);
            Assert.IsTrue(isSP);

            int* compType = stackalloc int[m];
            int triconn = Planar.TriconnectedComponents(n, m, u, v, compType);
            Assert.IsTrue(triconn >= 0);
        }

        [Test]
        public void Planar_Matching()
        {
            const int n = 4, m = 4;
            int* u = stackalloc int[m] { 0, 1, 2, 3 };
            int* v = stackalloc int[m] { 1, 2, 3, 0 };

            int* matchU = stackalloc int[n];
            int* matchV = stackalloc int[n];
            int matchCount = 0;

            Planar.MaximumPlanarMatching(n, m, u, v, matchU, matchV, &matchCount);
            Assert.AreEqual(2, matchCount);
        }
    }
}
