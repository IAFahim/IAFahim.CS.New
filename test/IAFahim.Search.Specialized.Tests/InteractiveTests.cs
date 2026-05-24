namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Search;

    public sealed unsafe class InteractiveTests
    {
        private static bool MockBrute(int* input, int inputLen, int* output, int* outputLen) { if (inputLen == 0) { *outputLen = 0; return false; } *outputLen = 1; output[0] = input[0] * 2; return true; }
        private static bool MockOpt(int* input, int inputLen, int* output, int* outputLen) { if (inputLen == 0) { *outputLen = 0; return false; } *outputLen = 1; output[0] = input[0] * 2; return true; }
        private static bool MockChecker(int* expected, int expectedLen, int* actual, int actualLen) { return expectedLen == actualLen && expected[0] == actual[0]; }

        [Test]
        public void StressCompare_MatchingSolvers_ReturnsTrue()
        {
            int* input = stackalloc int[] { 5 }; int* scratch1 = stackalloc int[10]; int* scratch2 = stackalloc int[10];
            bool matches = Interactive.StressCompare(input, 1, &MockBrute, &MockOpt, &MockChecker, scratch1, scratch2);
            Assert.IsTrue(matches);
        }

        [Test]
        public void CheckerCompare_EqualArrays_ReturnsTrue()
        {
            int* expected = stackalloc int[] { 1, 2, 3 }, actual = stackalloc int[] { 1, 2, 3 };
            Assert.IsTrue(Interactive.CheckerCompare(expected, 3, actual, 3));
        }

        [Test]
        public void CheckerCompare_DifferentArrays_ReturnsFalse()
        {
            int* expected = stackalloc int[] { 1, 2, 3 }, actual = stackalloc int[] { 1, 2, 4 };
            Assert.IsFalse(Interactive.CheckerCompare(expected, 3, actual, 3));
        }

        [Test]
        public void InteractiveTreeCentroidFind_FindsCorrectNode()
        {
            int numNodes = 5; int* head = stackalloc int[5]; int* next = stackalloc int[8], to = stackalloc int[8];
            for (int i = 0; i < 5; i++) head[i] = -1;
            int edgeIdx = 0;
            to[edgeIdx] = 1; next[edgeIdx] = head[0]; head[0] = edgeIdx++;
            to[edgeIdx] = 0; next[edgeIdx] = head[1]; head[1] = edgeIdx++;
            to[edgeIdx] = 2; next[edgeIdx] = head[1]; head[1] = edgeIdx++;
            to[edgeIdx] = 1; next[edgeIdx] = head[2]; head[2] = edgeIdx++;
            to[edgeIdx] = 3; next[edgeIdx] = head[2]; head[2] = edgeIdx++;
            to[edgeIdx] = 2; next[edgeIdx] = head[3]; head[3] = edgeIdx++;
            to[edgeIdx] = 4; next[edgeIdx] = head[3]; head[3] = edgeIdx++;
            to[edgeIdx] = 3; next[edgeIdx] = head[4]; head[4] = edgeIdx++;

            _targetNode = 4;
            int found = Interactive.InteractiveTreeCentroidFind(numNodes, head, next, to, &MockQueryTowardsTarget);
            Assert.AreEqual(4, found);
        }

        private static int _targetNode = 3;
        private static int MockQueryTowardsTarget(int curr)
        {
            if (curr == _targetNode) return curr;
            return curr < _targetNode ? curr + 1 : curr - 1;
        }
    }
}
