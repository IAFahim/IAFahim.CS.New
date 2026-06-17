namespace IAFahim.String.SuffixAutomaton.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class KthSubstringTests
    {
        [Test]
        public void AbbDistinct_KthSubstrings()
        {
            const int N = 3;
            int maxStates = 2 * N + 2;
            int maxEdges = 3 * N + 2;
            int* text = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            SuffixAutomaton.State* st = (SuffixAutomaton.State*)Marshal.AllocHGlobal(maxStates * sizeof(SuffixAutomaton.State));
            SuffixAutomaton.Edge* e = (SuffixAutomaton.Edge*)Marshal.AllocHGlobal(maxEdges * sizeof(SuffixAutomaton.Edge));
            long* dp = (long*)Marshal.AllocHGlobal(maxStates * sizeof(long));
            int* outPtr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                text[0] = 'a'; text[1] = 'b'; text[2] = 'b';
                int size = 0, last = 0, edgeCount = 0;
                SuffixAutomaton.Build(text, N, st, e, ref size, ref last, ref edgeCount);

                AssertKth(st, e, size, dp, outPtr, 1, "a");
                AssertKth(st, e, size, dp, outPtr, 2, "ab");
                AssertKth(st, e, size, dp, outPtr, 3, "abb");
                AssertKth(st, e, size, dp, outPtr, 4, "b");
                AssertKth(st, e, size, dp, outPtr, 5, "bb");

                int outLen;
                Assert.IsFalse(KthSubstring.Find(st, e, size, 6, &outLen, outPtr, dp));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)st);
                Marshal.FreeHGlobal((nint)e);
                Marshal.FreeHGlobal((nint)dp);
                Marshal.FreeHGlobal((nint)outPtr);
            }
        }

        [Test]
        public void AbabClone_KthSubstrings()
        {
            const int N = 4;
            int maxStates = 2 * N + 2;
            int maxEdges = 3 * N + 2;
            int* text = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            SuffixAutomaton.State* st = (SuffixAutomaton.State*)Marshal.AllocHGlobal(maxStates * sizeof(SuffixAutomaton.State));
            SuffixAutomaton.Edge* e = (SuffixAutomaton.Edge*)Marshal.AllocHGlobal(maxEdges * sizeof(SuffixAutomaton.Edge));
            long* dp = (long*)Marshal.AllocHGlobal(maxStates * sizeof(long));
            int* outPtr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                text[0] = 'a'; text[1] = 'b'; text[2] = 'a'; text[3] = 'b';
                int size = 0, last = 0, edgeCount = 0;
                SuffixAutomaton.Build(text, N, st, e, ref size, ref last, ref edgeCount);

                AssertKth(st, e, size, dp, outPtr, 1, "a");
                AssertKth(st, e, size, dp, outPtr, 2, "ab");
                AssertKth(st, e, size, dp, outPtr, 3, "aba");
                AssertKth(st, e, size, dp, outPtr, 4, "abab");
                AssertKth(st, e, size, dp, outPtr, 5, "b");
                AssertKth(st, e, size, dp, outPtr, 6, "ba");
                AssertKth(st, e, size, dp, outPtr, 7, "bab");

                int outLen;
                Assert.IsFalse(KthSubstring.Find(st, e, size, 8, &outLen, outPtr, dp));
            }
            finally
            {
                Marshal.FreeHGlobal((nint)text);
                Marshal.FreeHGlobal((nint)st);
                Marshal.FreeHGlobal((nint)e);
                Marshal.FreeHGlobal((nint)dp);
                Marshal.FreeHGlobal((nint)outPtr);
            }
        }

        private static void AssertKth(SuffixAutomaton.State* st, SuffixAutomaton.Edge* e, int stateCount, long* dp, int* outPtr, long k, string expected)
        {
            int outLen;
            Assert.IsTrue(KthSubstring.Find(st, e, stateCount, k, &outLen, outPtr, dp), "find k=" + k);
            Assert.AreEqual(expected.Length, outLen, "len k=" + k);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual((int)expected[i], outPtr[i], "char k=" + k + " i=" + i);
        }
    }
}
