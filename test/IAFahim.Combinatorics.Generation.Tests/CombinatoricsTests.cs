namespace IAFahim.Combinatorics.Generation.Tests
{
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using IAFahim.Combinatorics.Generation;

    public sealed unsafe class CombinatoricsTests
    {
        [Test]
        public void MultisetCombinations_GeneratesCorrectly()
        {
            int n = 2, k = 2;
            int* m = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* comb = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            try
            {
                m[0] = 2; m[1] = 2;
                bool first = true; int count = 0;
                int[,] expected = new int[,] { { 2, 0 }, { 1, 1 }, { 0, 2 } };
                while (Combinations.TryNextMultiset(m, n, k, comb, ref first))
                {
                    Assert.AreEqual(expected[count, 0], comb[0]);
                    Assert.AreEqual(expected[count, 1], comb[1]);
                    count++;
                }
                Assert.AreEqual(3, count);
            }
            finally { Marshal.FreeHGlobal((nint)m); Marshal.FreeHGlobal((nint)comb); }
        }

        [Test]
        public void CoolLex_GeneratesCorrectly()
        {
            int n = 4, t = 2;
            int* c = stackalloc int[t + 2];
            int* res = stackalloc int[t];
            Combinations.CoolLexEnumerator en = new Combinations.CoolLexEnumerator(n, t);
            int count = 0;
            while (en.MoveNext(c, res)) count++;
            Assert.AreEqual(6, count);
        }

        [Test]
        public void RevolvingDoor_GeneratesCorrectly()
        {
            int n = 4, k = 2;
            int* c = stackalloc int[k + 2];
            int* res = stackalloc int[k];
            Combinations.RevolvingDoorEnumerator en = new Combinations.RevolvingDoorEnumerator(n, k);
            int count = 0;
            while (en.MoveNext(c, res)) count++;
            Assert.AreEqual(6, count);
        }

        [Test]
        public void LyndonWords_GeneratesCorrectly()
        {
            int n = 4, k = 2;
            int* w = stackalloc int[n + 1];
            int* res = stackalloc int[n];
            LyndonWordEnumerator en = new LyndonWordEnumerator(n, k);
            int count = 0;
            while (en.MoveNext(w, res, out int resLen)) count++;
            Assert.AreEqual(6, count);
        }

        [Test]
        public void DeBruijnFromLyndon_GeneratesCorrectly()
        {
            int n = 3, k = 2;
            int* seq = stackalloc int[8];
            int len = NecklacesAndBracelets.DeBruijnFromLyndon(k, n, seq);
            Assert.AreEqual(8, len);
        }

        [Test]
        public void HeapPermutations_GeneratesCorrectly()
        {
            int n = 3;
            int* a = stackalloc int[n];
            int* c = stackalloc int[n];
            HeapPermutationEnumerator en = new HeapPermutationEnumerator(n);
            int count = 0;
            while (en.MoveNext(a, c)) count++;
            Assert.AreEqual(6, count);
        }

        [Test]
        public void JohnsonTrotter_GeneratesCorrectly()
        {
            int n = 3;
            int* a = stackalloc int[n];
            byte* dir = stackalloc byte[n];
            JohnsonTrotterEnumerator en = new JohnsonTrotterEnumerator(n);
            int count = 0;
            while (en.MoveNext(a, dir)) count++;
            Assert.AreEqual(6, count);
        }

        [Test]
        public void PermutationsWithDuplicates_GeneratesCorrectly()
        {
            int n = 3;
            int* a = stackalloc int[n];
            a[0] = 1; a[1] = 1; a[2] = 2;
            int count = 0;
            do { count++; } while (Permutations.NextPermutation(a, n));
            Assert.AreEqual(3, count);
        }

        [Test]
        public void Derangements_GeneratesCorrectly()
        {
            int n = 3;
            int* a = stackalloc int[n];
            for (int i = 0; i < n; i++) a[i] = i;
            int count = 0;
            while (Permutations.NextDerangement(a, n)) count++;
            Assert.AreEqual(2, count);
        }

        [Test]
        public void RandomPermutations_RunsSuccessfully()
        {
            int n = 5;
            int* a = stackalloc int[n];
            uint seed = 42;
            Permutations.RandomPermutation(n, a, ref seed);
            bool[] present = new bool[5];
            for (int i = 0; i < n; i++) { Assert.IsTrue(a[i] >= 0 && a[i] < n); present[a[i]] = true; }
            for (int i = 0; i < n; i++) Assert.IsTrue(present[i]);
        }

        [Test]
        public void RandomDerangements_GeneratesValidDerangement()
        {
            int n = 5;
            int* a = stackalloc int[n];
            try
            {
                uint seed = 42;
                Permutations.RandomDerangement(n, a, ref seed);
                for (int i = 0; i < n; i++)
                {
                    Assert.AreNotEqual(i, a[i]);
                }
            }
            finally
            {
            }
        }

        [Test]
        public void InvolutionCount_ExpectedValues()
        {
            Assert.AreEqual(1, Permutations.InvolutionCount(0));
            Assert.AreEqual(1, Permutations.InvolutionCount(1));
            Assert.AreEqual(2, Permutations.InvolutionCount(2));
            Assert.AreEqual(4, Permutations.InvolutionCount(3));
            Assert.AreEqual(10, Permutations.InvolutionCount(4));
        }

        [Test]
        public void Involutions_GeneratesCorrectly()
        {
            int n = 3;
            int* a = stackalloc int[n];
            for (int i = 0; i < n; i++) a[i] = i;
            int count = 0;
            do { if (Permutations.IsInvolution(n, a)) count++; } while (Permutations.NextPermutation(a, n));
            Assert.AreEqual(4, count);
        }

        [Test]
        public void IntegerPartitions_GeneratesCorrectly()
        {
            int n = 4;
            int* p = stackalloc int[n];
            IntegerPartitionEnumerator en = new IntegerPartitionEnumerator(n);
            int count = 0;
            while (en.MoveNext(p, out int len)) count++;
            Assert.AreEqual(5, count);
        }

        [Test]
        public void SetPartitions_GeneratesCorrectly()
        {
            int n = 3;
            int* kappa = stackalloc int[n], m = stackalloc int[n];
            SetPartitions.SetPartitionState state;
            SetPartitions.InitSetPartition(n, &state);
            int count = 0;
            while (SetPartitions.NextSetPartition(&state, kappa, m)) count++;
            Assert.AreEqual(5, count);
        }

        [Test]
        public void Compositions_GeneratesCorrectly()
        {
            int n = 3, k = 2;
            int* comp = stackalloc int[k];
            SetPartitions.CompositionState state;
            SetPartitions.InitComposition(n, k, &state);
            int count = 0;
            while (SetPartitions.NextComposition(&state, comp)) count++;
            Assert.AreEqual(4, count);
        }

        [Test]
        public void DyckWords_GeneratesCorrectly()
        {
            int n = 3;
            byte* a = stackalloc byte[2 * n];
            bool first = true; int count = 0;
            while (CatalanStructures.TryGenerateDyckWord(n, a, ref first))
            {
                count++;
                long r = CatalanStructures.RankDyckWord(a, n);
                byte* b = stackalloc byte[2 * n];
                CatalanStructures.UnrankDyckWord(r, n, b);
                for (int i = 0; i < 2 * n; i++) Assert.AreEqual(a[i], b[i]);
            }
            Assert.AreEqual(5, count);
        }
    }
}
