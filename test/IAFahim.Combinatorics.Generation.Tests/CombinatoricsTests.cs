namespace IAFahim.Combinatorics.Generation.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using Xunit;
    using IAFahim.Combinatorics.Generation;

    public sealed unsafe class CombinatoricsTests
    {
        [Fact]
        public void MultisetCombinations_GeneratesCorrectly()
        {
            int n = 2;
            int k = 2;
            int* m = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* comb = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                m[0] = 2;
                m[1] = 2;

                bool first = true;
                int count = 0;
                
                // Lexicographical order: [0, 2], [1, 1], [2, 0]
                int[,] expected = new int[,] {
                    { 0, 2 },
                    { 1, 1 },
                    { 2, 0 }
                };

                while (Combinations.TryNextMultiset(m, n, k, comb, ref first))
                {
                    Assert.Equal(expected[count, 0], comb[0]);
                    Assert.Equal(expected[count, 1], comb[1]);
                    count++;
                }

                Assert.Equal(3, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)m);
                Marshal.FreeHGlobal((nint)comb);
            }
        }

        [Fact]
        public void CoolLex_GeneratesCorrectly()
        {
            int n = 4;
            int t = 2;
            // c needs size t + 2
            int* c = (int*)Marshal.AllocHGlobal((t + 2) * sizeof(int));
            int* res = (int*)Marshal.AllocHGlobal(t * sizeof(int));
            int* scratch = (int*)Marshal.AllocHGlobal((t + 2) * sizeof(int));
            try
            {
                // Init c pointer to null or scratch
                int* state = null;
                int count = 0;

                while (Combinations.TryGenerateCoolLex(n, t, state, res, scratch))
                {
                    state = scratch; // subsequent calls pass the scratch pointer
                    count++;
                }

                // 4 choose 2 = 6 combinations
                Assert.Equal(6, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)c);
                Marshal.FreeHGlobal((nint)res);
                Marshal.FreeHGlobal((nint)scratch);
            }
        }

        [Fact]
        public void RevolvingDoor_GeneratesCorrectly()
        {
            int n = 4;
            int k = 2;
            int* res = (int*)Marshal.AllocHGlobal(k * sizeof(int));
            int* scratch = (int*)Marshal.AllocHGlobal((k + 2) * sizeof(int));
            try
            {
                int* state = null;
                int count = 0;

                while (Combinations.TryGenerateRevolvingDoor(n, k, state, res, scratch))
                {
                    state = scratch;
                    count++;
                }

                // 4 choose 2 = 6 combinations
                Assert.Equal(6, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)res);
                Marshal.FreeHGlobal((nint)scratch);
            }
        }

        [Fact]
        public void LyndonWords_GeneratesCorrectly()
        {
            int n = 4;
            int k = 2;
            int* res = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* scratch = (int*)Marshal.AllocHGlobal((n + 2) * sizeof(int));
            try
            {
                int* w = null;
                int jState = 0;
                int count = 0;

                while (NecklacesAndBracelets.TryGenerateLyndon(n, k, w, res, ref jState, scratch))
                {
                    w = scratch;
                    count++;
                }

                // Lyndon words of length <= 4 over binary alphabet {0, 1}:
                // 1: [0]
                // 2: [0, 0, 0, 1]
                // 3: [0, 0, 1, 1]
                // 4: [0, 1]
                // 5: [0, 1, 1, 1]
                // 6: [1]
                Assert.Equal(6, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)res);
                Marshal.FreeHGlobal((nint)scratch);
            }
        }

        [Fact]
        public void DeBruijnFromLyndon_GeneratesCorrectly()
        {
            int n = 3;
            int k = 2;
            int maxLen = 1 << n; // 8
            int* seq = (int*)Marshal.AllocHGlobal(maxLen * sizeof(int));
            try
            {
                int len = NecklacesAndBracelets.DeBruijnFromLyndon(k, n, seq);
                Assert.Equal(8, len);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)seq);
            }
        }

        [Fact]
        public void HeapPermutations_GeneratesCorrectly()
        {
            int n = 3;
            int* a = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* c = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                bool first = true;
                bool hasNext = true;
                int count = 0;

                while (hasNext)
                {
                    Permutations.HeapPermutation(n, a, c, &hasNext, ref first);
                    if (hasNext) count++;
                }

                Assert.Equal(6, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)c);
            }
        }

        [Fact]
        public void JohnsonTrotter_GeneratesCorrectly()
        {
            int n = 3;
            int* a = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            bool* dir = (bool*)Marshal.AllocHGlobal(n * sizeof(bool));
            try
            {
                bool first = true;
                bool hasNext = true;
                int count = 0;

                while (hasNext)
                {
                    Permutations.JohnsonTrotter(n, a, dir, &hasNext, ref first);
                    if (hasNext) count++;
                }

                Assert.Equal(6, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
                Marshal.FreeHGlobal((nint)dir);
            }
        }

        [Fact]
        public void PermutationsWithDuplicates_GeneratesCorrectly()
        {
            int n = 3;
            int* a = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                a[0] = 1;
                a[1] = 1;
                a[2] = 2;

                bool first = true;
                bool hasNext = true;
                int count = 0;

                while (hasNext)
                {
                    Permutations.PermutationsWithDuplicates(n, a, &hasNext, ref first);
                    if (hasNext) count++;
                }

                Assert.Equal(3, count); // unique permutations: [1,1,2], [1,2,1], [2,1,1]
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
            }
        }

        [Fact]
        public void Derangements_GeneratesCorrectly()
        {
            int n = 3;
            int* a = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                bool first = true;
                bool hasNext = true;
                int count = 0;

                while (hasNext)
                {
                    Permutations.Derangement(n, a, &hasNext, ref first);
                    if (hasNext) count++;
                }

                Assert.Equal(2, count); // Derangements of size 3: [1,2,0], [2,0,1]
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
            }
        }

        [Fact]
        public void RandomPermutations_RunsSuccessfully()
        {
            int n = 5;
            int* a = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                uint seed = 42;
                Permutations.RandomPermutation(n, a, ref seed);
                // Verify all numbers 0 to 4 are present
                bool[] present = new bool[5];
                for (int i = 0; i < n; i++)
                {
                    Assert.True(a[i] >= 0 && a[i] < n);
                    present[a[i]] = true;
                }
                for (int i = 0; i < n; i++) Assert.True(present[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
            }
        }

        [Fact]
        public void RandomDerangements_GeneratesValidDerangement()
        {
            int n = 5;
            int* a = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                uint seed = 42;
                Permutations.RandomDerangement(n, a, ref seed);
                for (int i = 0; i < n; i++)
                {
                    Assert.NotEqual(i, a[i]);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
            }
        }

        [Fact]
        public void InvolutionCount_ExpectedValues()
        {
            Assert.Equal(1, Permutations.InvolutionCount(0));
            Assert.Equal(1, Permutations.InvolutionCount(1));
            Assert.Equal(2, Permutations.InvolutionCount(2));
            Assert.Equal(4, Permutations.InvolutionCount(3));
            Assert.Equal(10, Permutations.InvolutionCount(4));
        }

        [Fact]
        public void Involutions_GeneratesCorrectly()
        {
            int n = 3;
            int* a = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                bool first = true;
                bool hasNext = true;
                int count = 0;

                while (hasNext)
                {
                    Permutations.Involution(n, a, &hasNext, ref first);
                    if (hasNext) count++;
                }

                Assert.Equal(4, count); // Involutions of size 3: 4
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
            }
        }

        [Fact]
        public void RandomConnectedGraph_RunsSuccessfully()
        {
            int n = 5;
            int m = 6;
            int* from = (int*)Marshal.AllocHGlobal(m * sizeof(int));
            int* to = (int*)Marshal.AllocHGlobal(m * sizeof(int));
            try
            {
                uint seed = 42;
                RandomStructures.RandomConnectedGraph(n, m, from, to, ref seed);
                // Basic checks
                for (int i = 0; i < m; i++)
                {
                    Assert.True(from[i] >= 0 && from[i] < n);
                    Assert.True(to[i] >= 0 && to[i] < n);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)from);
                Marshal.FreeHGlobal((nint)to);
            }
        }

        [Fact]
        public void IntegerPartitions_GeneratesCorrectly()
        {
            int n = 4;
            int* p = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                SetPartitions.IntegerPartitionState state;
                SetPartitions.InitIntegerPartition(n, &state);

                int count = 0;
                int len;
                while (SetPartitions.NextIntegerPartition(&state, p, out len))
                {
                    count++;
                }

                // Partitions of 4: 5 ([4], [3,1], [2,2], [2,1,1], [1,1,1,1])
                Assert.Equal(5, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)p);
            }
        }

        [Fact]
        public void SetPartitions_GeneratesCorrectly()
        {
            int n = 3;
            int* kappa = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* m = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                SetPartitions.SetPartitionState state;
                SetPartitions.InitSetPartition(n, &state);

                int count = 0;
                while (SetPartitions.NextSetPartition(&state, kappa, m))
                {
                    count++;
                }

                // Bell number B_3 = 5
                Assert.Equal(5, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)kappa);
                Marshal.FreeHGlobal((nint)m);
            }
        }

        [Fact]
        public void Compositions_GeneratesCorrectly()
        {
            int n = 3;
            int k = 2;
            int* comp = (int*)Marshal.AllocHGlobal(k * sizeof(int));
            try
            {
                SetPartitions.CompositionState state;
                SetPartitions.InitComposition(n, k, &state);

                int count = 0;
                while (SetPartitions.NextComposition(&state, comp))
                {
                    count++;
                }

                // Compositions of 3 into 2 parts: 4 ([3,0], [2,1], [1,2], [0,3])
                Assert.Equal(4, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)comp);
            }
        }

        [Fact]
        public void DyckWords_GeneratesCorrectly()
        {
            int n = 3;
            byte* a = (byte*)Marshal.AllocHGlobal(2 * n * sizeof(byte));
            try
            {
                bool first = true;
                int count = 0;

                while (CatalanStructures.TryGenerateDyckWord(n, a, ref first))
                {
                    count++;
                    long r = CatalanStructures.RankDyckWord(a, n);
                    byte* b = stackalloc byte[2 * n];
                    CatalanStructures.UnrankDyckWord(r, n, b);
                    for (int i = 0; i < 2 * n; i++) Assert.Equal(a[i], b[i]);
                }

                // Catalan C_3 = 5
                Assert.Equal(5, count);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)a);
            }
        }
    }
}
