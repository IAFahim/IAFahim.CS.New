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
}
