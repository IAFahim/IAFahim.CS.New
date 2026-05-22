namespace IAFahim.Search.Specialized.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.Search;

    public sealed unsafe class InteractiveTests
    {
        private static bool MockBrute(int* input, int inputLen, int* output, int* outputLen)
        {
            if (inputLen == 0)
            {
                *outputLen = 0;
                return false;
            }
            *outputLen = 1;
            output[0] = input[0] * 2;
            return true;
        }

        private static bool MockOpt(int* input, int inputLen, int* output, int* outputLen)
        {
            if (inputLen == 0)
            {
                *outputLen = 0;
                return false;
            }
            *outputLen = 1;
            output[0] = input[0] * 2;
            return true;
        }

        private static bool MockChecker(int* expected, int expectedLen, int* actual, int actualLen)
        {
            return expectedLen == actualLen && expected[0] == actual[0];
        }

        [Fact]
        public void StressCompare_MatchingSolvers_ReturnsTrue()
        {
            int* input = stackalloc int[] { 5 };
            int* scratch1 = stackalloc int[10];
            int* scratch2 = stackalloc int[10];

            bool matches = Interactive.StressCompare(
                input, 1,
                &MockBrute,
                &MockOpt,
                &MockChecker,
                scratch1, scratch2);

            Assert.True(matches);
        }

        [Fact]
        public void CheckerCompare_EqualArrays_ReturnsTrue()
        {
            int* expected = stackalloc int[] { 1, 2, 3 };
            int* actual = stackalloc int[] { 1, 2, 3 };
            bool res = Interactive.CheckerCompare(expected, 3, actual, 3);
            Assert.True(res);
        }

        [Fact]
        public void CheckerCompare_DifferentArrays_ReturnsFalse()
        {
            int* expected = stackalloc int[] { 1, 2, 3 };
            int* actual = stackalloc int[] { 1, 2, 4 };
            bool res = Interactive.CheckerCompare(expected, 3, actual, 3);
            Assert.False(res);
        }

        private static int _flushCount = 0;
        private static void MockFlush()
        {
            _flushCount++;
        }

        [Fact]
        public void InteractiveFlush_TriggersCallback()
        {
            _flushCount = 0;
            Interactive.InteractiveFlush(&MockFlush);
            Assert.Equal(1, _flushCount);
        }

        private static int MockAsk(int* query, int len)
        {
            return query[0] + len;
        }

        [Fact]
        public void InteractiveAsk_ReturnsCorrectResponse()
        {
            int* query = stackalloc int[] { 10 };
            int res = Interactive.InteractiveAsk(query, 5, &MockAsk);
            Assert.Equal(15, res);
        }

        private static int _lastAnswer = 0;
        private static void MockAnswer(int* ans, int len)
        {
            _lastAnswer = ans[0] * len;
        }

        [Fact]
        public void InteractiveAnswer_CorrectlyExecutes()
        {
            _lastAnswer = 0;
            int* ans = stackalloc int[] { 7 };
            Interactive.InteractiveAnswer(ans, 3, &MockAnswer);
            Assert.Equal(21, _lastAnswer);
        }

        private static bool MockReadVerdict()
        {
            return true;
        }

        [Fact]
        public void InteractiveReadVerdict_ReturnsValue()
        {
            bool verdict = Interactive.InteractiveReadVerdict(&MockReadVerdict);
            Assert.True(verdict);
        }

        [Fact]
        public void QueryCache_GetAndSet_Works()
        {
            const int Capacity = 10;
            int* keys = stackalloc int[Capacity];
            int* values = stackalloc int[Capacity];
            byte* occupied = stackalloc byte[Capacity];
            for (int i = 0; i < Capacity; i++)
            {
                occupied[i] = 0;
            }

            bool setOk = Interactive.QueryCacheSet(keys, values, occupied, Capacity, 42, 100);
            Assert.True(setOk);

            int val;
            bool getOk = Interactive.QueryCacheGet(keys, values, occupied, Capacity, 42, out val);
            Assert.True(getOk);
            Assert.Equal(100, val);

            bool getMissing = Interactive.QueryCacheGet(keys, values, occupied, Capacity, 99, out val);
            Assert.False(getMissing);
        }

        [Fact]
        public void AdaptiveQueryStrategy_NarrowsCorrectly()
        {
            int L = 1;
            int R = 100;
            // Target is 75. Query 1: (1+100)/2 = 50.
            // If response is "greater", we move L to mid+1
            int nextQuery = Interactive.AdaptiveQueryStrategy(&L, &R, 1, 1);
            Assert.Equal(51, L);
            Assert.Equal(100, R);
            Assert.Equal(75, nextQuery);
        }

        private static int MockArrayQuery(int idx)
        {
            int[] arr = new int[] { 2, 4, 6, 8, 10, 12, 14, 16 };
            if (idx >= 0 && idx < arr.Length)
            {
                return arr[idx];
            }
            return 99999;
        }

        private static bool MockArrayIsValidIndex(int idx)
        {
            return idx >= 0 && idx < 8;
        }

        [Fact]
        public void UnknownArrayBinarySearch_FindsTarget()
        {
            int index;
            bool found = Interactive.UnknownArrayBinarySearch(10, &MockArrayQuery, &MockArrayIsValidIndex, out index);
            Assert.True(found);
            Assert.Equal(4, index);

            bool notFound = Interactive.UnknownArrayBinarySearch(11, &MockArrayQuery, &MockArrayIsValidIndex, out index);
            Assert.False(notFound);
        }

        private static void MockGetNeighbors(int u, int* neighbors, int* count)
        {
            if (u == 0)
            {
                neighbors[0] = 1; neighbors[1] = 2; *count = 2;
            }
            else if (u == 1)
            {
                neighbors[0] = 0; *count = 1;
            }
            else if (u == 2)
            {
                neighbors[0] = 0; *count = 1;
            }
        }

        [Fact]
        public void InteractiveGraphExplore_ExploresReachable()
        {
            const int MaxV = 3;
            int* visited = stackalloc int[MaxV];
            int* edges = stackalloc int[10];
            int edgeCount = 0;

            Interactive.InteractiveGraphExplore(0, visited, MaxV, &MockGetNeighbors, edges, &edgeCount);

            Assert.Equal(1, visited[0]);
            Assert.Equal(1, visited[1]);
            Assert.Equal(1, visited[2]);
            Assert.Equal(4, edgeCount);
        }

        private static int _targetNode = 3;
        private static int MockQueryTowardsTarget(int curr)
        {
            // Simple line tree: 0 - 1 - 2 - 3 - 4
            if (curr == _targetNode)
            {
                return curr;
            }
            if (curr < _targetNode)
            {
                return curr + 1;
            }
            return curr - 1;
        }

        [Fact]
        public void InteractiveTreeFind_FindsCorrectNode()
        {
            _targetNode = 3;
            int found = Interactive.InteractiveTreeFind(0, &MockQueryTowardsTarget);
            Assert.Equal(3, found);
        }

        [Fact]
        public void InteractiveTreeCentroidFind_FindsCorrectNode()
        {
            // Line tree of size 5: 0-1-2-3-4
            // Head, Next, To arrays
            int numNodes = 5;
            int* head = stackalloc int[5];
            int* next = stackalloc int[8];
            int* to = stackalloc int[8];

            for (int i = 0; i < 5; i++) head[i] = -1;

            int edgeIdx = 0;
            // 0 - 1
            to[edgeIdx] = 1; next[edgeIdx] = head[0]; head[0] = edgeIdx++;
            to[edgeIdx] = 0; next[edgeIdx] = head[1]; head[1] = edgeIdx++;
            // 1 - 2
            to[edgeIdx] = 2; next[edgeIdx] = head[1]; head[1] = edgeIdx++;
            to[edgeIdx] = 1; next[edgeIdx] = head[2]; head[2] = edgeIdx++;
            // 2 - 3
            to[edgeIdx] = 3; next[edgeIdx] = head[2]; head[2] = edgeIdx++;
            to[edgeIdx] = 2; next[edgeIdx] = head[3]; head[3] = edgeIdx++;
            // 3 - 4
            to[edgeIdx] = 4; next[edgeIdx] = head[3]; head[3] = edgeIdx++;
            to[edgeIdx] = 3; next[edgeIdx] = head[4]; head[4] = edgeIdx++;

            _targetNode = 4;
            int found = Interactive.InteractiveTreeCentroidFind(numNodes, head, next, to, &MockQueryTowardsTarget);
            Assert.Equal(4, found);

            _targetNode = 2;
            found = Interactive.InteractiveTreeCentroidFind(numNodes, head, next, to, &MockQueryTowardsTarget);
            Assert.Equal(2, found);
        }

        private static int MockPermutationCompare(int i, int j)
        {
            int[] p = new int[] { 2, 0, 1 };
            return p[i].CompareTo(p[j]);
        }

        [Fact]
        public void InteractivePermutationRecover_RecoversCorrectly()
        {
            const int N = 3;
            int* recovered = stackalloc int[N];
            Interactive.InteractivePermutationRecover(N, recovered, &MockPermutationCompare);

            Assert.Equal(2, recovered[0]);
            Assert.Equal(0, recovered[1]);
            Assert.Equal(1, recovered[2]);
        }
    }
}
