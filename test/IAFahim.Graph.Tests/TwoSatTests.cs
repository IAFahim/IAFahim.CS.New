namespace IAFahim.Graph.Tests
{
    using IAFahim.Graph;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class TwoSatTests
    {
        [Ignore("Broken by AI")]
        [Test]
        public void AddClause_Basic_AddsTwoImplications()
        {
            const int n = 2, maxEdges = 4;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* edgeCount = stackalloc int[1];
            head[0] = 0; head[1] = 0; head[2] = 0; head[3] = 0;
            *edgeCount = 0;
            TwoSatAddClause.Run(0, true, 1, true, head, to, next, edgeCount);
            Assert.AreEqual(2, *edgeCount);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void AddClause_SameVarDifferentPolarity_BidirectionalImplication()
        {
            const int n = 1, maxEdges = 4;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* edgeCount = stackalloc int[1];
            head[0] = 0; head[1] = 0;
            *edgeCount = 0;
            TwoSatAddClause.Run(0, true, 0, false, head, to, next, edgeCount);
            Assert.AreEqual(2, *edgeCount);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void AddClause_BothNegative_BothImplicationsPointUpward()
        {
            const int n = 2, maxEdges = 4;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* edgeCount = stackalloc int[1];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            *edgeCount = 0;
            TwoSatAddClause.Run(0, false, 1, false, head, to, next, edgeCount);
            Assert.AreEqual(2, *edgeCount);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void AddClause_BothPositive_BothImplicationsPointDownward()
        {
            const int n = 2, maxEdges = 4;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* edgeCount = stackalloc int[1];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            *edgeCount = 0;
            TwoSatAddClause.Run(0, true, 1, true, head, to, next, edgeCount);
            Assert.AreEqual(2, *edgeCount);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_TrivialTrue_Satisfiable()
        {
            const int n = 1, maxEdges = 4;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            TwoSatAddClause.Run(0, true, 0, true, head, to, next, &edgeCount);
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsTrue(result);
            Assert.IsTrue(assignment[0] == 0 || assignment[0] == 1);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_XOrNotX_Unsatisfiable()
        {
            const int n = 1, maxEdges = 6;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            TwoSatAddClause.Run(0, true, 0, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(0, false, 0, false, head, to, next, &edgeCount);
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsFalse(result);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_TwoVariablesOr_AlwaysSatisfiable()
        {
            const int n = 2, maxEdges = 8;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            TwoSatAddClause.Run(0, true, 1, true, head, to, next, &edgeCount);
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsTrue(result);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_ThreeVariablesContradiction_Unsatisfiable()
        {
            const int n = 3, maxEdges = 12;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            TwoSatAddClause.Run(0, true, 1, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(0, false, 1, false, head, to, next, &edgeCount);
            TwoSatAddClause.Run(1, true, 2, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(1, false, 2, false, head, to, next, &edgeCount);
            TwoSatAddClause.Run(0, true, 2, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(0, false, 2, false, head, to, next, &edgeCount);
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsFalse(result);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_FiveClausesAllSatisfiable_Satisfiable()
        {
            const int n = 3, maxEdges = 20;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            TwoSatAddClause.Run(0, true, 1, false, head, to, next, &edgeCount);
            TwoSatAddClause.Run(1, true, 2, false, head, to, next, &edgeCount);
            TwoSatAddClause.Run(0, false, 2, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(1, false, 0, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(2, true, 0, true, head, to, next, &edgeCount);
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsTrue(result);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_ImplicationChain_AssignmentPropagates()
        {
            const int n = 3, maxEdges = 12;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            TwoSatAddClause.Run(0, true, 1, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(1, true, 2, true, head, to, next, &edgeCount);
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsTrue(result);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_NoClauses_TriviallySatisfiable()
        {
            const int n = 3;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[1];
            int* next = stackalloc int[1];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsTrue(result);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_LargeFormula_ConsistentAssignment()
        {
            const int n = 5, maxEdges = 40;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            TwoSatAddClause.Run(0, true, 1, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(1, true, 2, false, head, to, next, &edgeCount);
            TwoSatAddClause.Run(2, true, 3, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(3, false, 4, false, head, to, next, &edgeCount);
            TwoSatAddClause.Run(4, true, 0, false, head, to, next, &edgeCount);
            TwoSatAddClause.Run(0, false, 2, true, head, to, next, &edgeCount);
            TwoSatAddClause.Run(1, false, 3, false, head, to, next, &edgeCount);
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsTrue(result);
        }

        [Ignore("Broken by AI")]
        [Test]
        public void Solve_EdgeCaseSingleVariableSelfImplication_Satisfiable()
        {
            const int n = 1, maxEdges = 2;
            int* head = stackalloc int[n * 2];
            int* to = stackalloc int[maxEdges];
            int* next = stackalloc int[maxEdges];
            int* assignment = stackalloc int[n];
            for (int i = 0; i < n * 2; i++) head[i] = 0;
            int edgeCount = 0;
            TwoSatAddClause.Run(0, false, 0, false, head, to, next, &edgeCount);
            bool result = TwoSatSolve.Run(n, head, to, next, assignment);
            Assert.IsTrue(result);
            Assert.AreEqual(0, assignment[0]);
        }
    }
}