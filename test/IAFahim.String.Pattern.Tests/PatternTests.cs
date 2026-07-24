namespace IAFahim.String.Pattern.Tests
{
    using System.Runtime.InteropServices;
    using System.Text;
    using NUnit.Framework;

    public sealed unsafe class AhoPersistentTests
    {
        [Test]
        public void InsertQuery_FindsPattern()
        {
            const int Sigma = 26;
            const int MaxNodes = 256;
            byte[] pat = Encoding.ASCII.GetBytes("ab");
            byte[] text = Encoding.ASCII.GetBytes("xabxab");
            byte** patterns = (byte**)Marshal.AllocHGlobal(8 * sizeof(nint));
            int* lengths = stackalloc int[8];
            int* roots = stackalloc int[32];
            int* nexts = (int*)Marshal.AllocHGlobal(MaxNodes * Sigma * sizeof(int));
            int* fails = (int*)Marshal.AllocHGlobal(MaxNodes * sizeof(int));
            int* counts = (int*)Marshal.AllocHGlobal(MaxNodes * sizeof(int));
            try
            {
                for (int i = 0; i < MaxNodes * Sigma; i++) nexts[i] = 0;
                for (int i = 0; i < MaxNodes; i++) { fails[i] = 0; counts[i] = 0; }
                int numPatterns = 0, nodeCount = 0, activeMask = 0;
                fixed (byte* pp = pat)
                fixed (byte* pt = text)
                {
                    AhoPersistentBuild.Insert(pp, 2, patterns, lengths, ref numPatterns,
                        roots, nexts, fails, counts, ref nodeCount, ref activeMask, Sigma, (byte)'a');
                    long hits = AhoPersistentQuery.Run(pt, 6, roots, activeMask, nexts, counts, Sigma, (byte)'a');
                    Assert.IsTrue(hits >= 2);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)patterns);
                Marshal.FreeHGlobal((nint)nexts);
                Marshal.FreeHGlobal((nint)fails);
                Marshal.FreeHGlobal((nint)counts);
            }
        }
    }
}
