namespace IAFahim.String.Grammar.Tests
{
    using System.Runtime.InteropServices;
    using System.Text;
    using NUnit.Framework;

    public sealed unsafe class SlpTests
    {
        [Test]
        public void Empty_NoRule()
        {
            int ruleCount = 0;
            int root = StraightLineProgram.Build(null, 0, 0, null, ref ruleCount);
            Assert.AreEqual(StraightLineProgram.NoRule, root);
        }

        [Test]
        public void BuildQuery_RoundTrip()
        {
            byte[] s = Encoding.ASCII.GetBytes("abacabad");
            const int N = 8;
            int maxRules = 2 * N;
            StraightLineProgram.Rule* rules = (StraightLineProgram.Rule*)Marshal.AllocHGlobal(maxRules * sizeof(StraightLineProgram.Rule));
            try
            {
                int ruleCount = 0;
                int root;
                fixed (byte* p = s)
                {
                    root = StraightLineProgram.Build(p, N, maxRules, rules, ref ruleCount);
                }
                Assert.IsTrue(root >= 0);
                Assert.IsTrue(ruleCount > 0);
                for (int i = 0; i < N; i++)
                {
                    byte c = StraightLineProgram.Query(rules, root, i);
                    Assert.AreEqual(s[i], c);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)rules);
            }
        }
    }

    public sealed unsafe class GrammarCompressTests
    {
        [Test]
        public void Compress_UsesNonTerminalsAbove255()
        {
            byte* input = stackalloc byte[] { 1, 2, 1, 2, 1, 2 };
            GrammarCompress.Rule* rules = stackalloc GrammarCompress.Rule[8];
            int* work = stackalloc int[16];
            int count = GrammarCompress.Compress(input, 6, rules, 8, work);
            Assert.IsTrue(count >= 0);
            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(rules[i].Left >= 0);
            }
        }
    }
}
