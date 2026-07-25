namespace IAFahim.String.Parse.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class EarleyTests
    {
        [Test]
        public void SToT_Terminals_AcceptsAndRejects()
        {
            int[] rules = { 256, 257, -1, 257, 97, -1, 257, 98, -1 };
            int* rl = MakeRules(rules);
            try
            {
                ExpectAccept("a", rl, 3, 0);
                ExpectAccept("b", rl, 3, 0);
                ExpectReject("c", rl, 3, 0);
                ExpectReject("ab", rl, 3, 0);
                ExpectReject("", rl, 3, 0);
            }
            finally { Marshal.FreeHGlobal((nint)rl); }
        }

        [Test]
        public void EpsilonProduction_AcceptsEmptyOnly()
        {
            int[] rules = { 256, 257, -1, 257, -1, 0 };
            int* rl = MakeRules(rules);
            try
            {
                ExpectAccept("", rl, 2, 0);
                ExpectReject("a", rl, 2, 0);
            }
            finally { Marshal.FreeHGlobal((nint)rl); }
        }

        [Test]
        public void StartRuleOnly_DoesNotAcceptSiblingLanguage()
        {
            int[] rules = { 256, 97, -1, 256, 98, -1 };
            int* rl = MakeRules(rules);
            try
            {
                ExpectAccept("a", rl, 2, 0);
                ExpectReject("b", rl, 2, 0);
            }
            finally { Marshal.FreeHGlobal((nint)rl); }
        }

        private static byte* MakeInput(string s, out int len)
        {
            len = s.Length;
            byte* p = (byte*)Marshal.AllocHGlobal(len > 0 ? len : 1);
            for (int i = 0; i < len; i++) p[i] = (byte)s[i];
            return p;
        }

        private static int* MakeRules(int[] r)
        {
            int* p = (int*)Marshal.AllocHGlobal(r.Length * sizeof(int));
            for (int i = 0; i < r.Length; i++) p[i] = r[i];
            return p;
        }

        private static void ExpectAccept(string input, int* rules, int ruleCount, int startRule)
        {
            byte* inp = MakeInput(input, out int len);
            try { Assert.IsTrue(Earley.Parse(inp, len, rules, ruleCount, startRule), "expected accept: '" + input + "'"); }
            finally { Marshal.FreeHGlobal((nint)inp); }
        }

        private static void ExpectReject(string input, int* rules, int ruleCount, int startRule)
        {
            byte* inp = MakeInput(input, out int len);
            try { Assert.IsFalse(Earley.Parse(inp, len, rules, ruleCount, startRule), "expected reject: '" + input + "'"); }
            finally { Marshal.FreeHGlobal((nint)inp); }
        }
    }

    public sealed unsafe class CykTests
    {
        [Test]
        public void Parse_EmptyInput_ReturnsFalse()
        {
            int* terminals = stackalloc int[1] { 97 };
            int* productions = stackalloc int[3] { 0, 0, 0 };
            Assert.IsFalse(Cyk.Parse(terminals, productions, 0, 0, null, 0));
        }

        [Test]
        public void Parse_SingleTerminal_AcceptsMatchingByte()
        {
            // terminals[p] = byte for production p assigning symbol productions[p]
            int* terminals = stackalloc int[1] { 97 };
            int* productions = stackalloc int[1] { 0 };
            byte* input = stackalloc byte[1] { 97 };
            Assert.IsTrue(Cyk.Parse(terminals, productions, 1, 0, input, 1));
            byte* bad = stackalloc byte[1] { 98 };
            Assert.IsFalse(Cyk.Parse(terminals, productions, 1, 0, bad, 1));
        }
    }

    public sealed unsafe class SuffixOracleTests
    {
        [Test]
        public void Build_Contains_FindsSubstrings()
        {
            byte* text = stackalloc byte[4] { (byte)'a', (byte)'b', (byte)'c', (byte)'a' };
            SuffixOracle.Build(text, 4, 256);
            byte* pat = stackalloc byte[2] { (byte)'b', (byte)'c' };
            Assert.IsTrue(SuffixOracle.Contains(pat, 2));
            byte* miss = stackalloc byte[2] { (byte)'c', (byte)'b' };
            Assert.IsFalse(SuffixOracle.Contains(miss, 2));
        }
    }
}
