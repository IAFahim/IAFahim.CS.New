namespace IAFahim.String.Tests
{
    using IAFahim.String.Pattern;
    using System;
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class AhoPersistentTests
    {
        [Fact]
        public void AhoPersistent_BuildAndQuery()
        {
            int maxNodes = 1000;
            int numPatterns = 0;
            int nodeCount = 0;
            int activeMask = 0;
            
            byte** patterns = (byte**)Marshal.AllocHGlobal(32 * sizeof(byte*));
            int* lengths = (int*)Marshal.AllocHGlobal(32 * sizeof(int));
            int* roots = (int*)Marshal.AllocHGlobal(32 * sizeof(int));
            int* nexts = (int*)Marshal.AllocHGlobal(maxNodes * 26 * sizeof(int));
            int* fails = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));
            int* counts = (int*)Marshal.AllocHGlobal(maxNodes * sizeof(int));

            try
            {
                // Init arrays to 0
                for (int i = 0; i < maxNodes * 26; i++) nexts[i] = 0;
                for (int i = 0; i < maxNodes; i++) fails[i] = counts[i] = 0;
                for (int i = 0; i < 32; i++) roots[i] = 0;

                // "he"
                byte* p1 = stackalloc byte[2];
                p1[0] = (byte)'h'; p1[1] = (byte)'e';
                AhoPersistentBuild.Insert(p1, 2, patterns, lengths, ref numPatterns, roots, nexts, fails, counts, ref nodeCount, ref activeMask);

                // "she"
                byte* p2 = stackalloc byte[3];
                p2[0] = (byte)'s'; p2[1] = (byte)'h'; p2[2] = (byte)'e';
                AhoPersistentBuild.Insert(p2, 3, patterns, lengths, ref numPatterns, roots, nexts, fails, counts, ref nodeCount, ref activeMask);

                // "his"
                byte* p3 = stackalloc byte[3];
                p3[0] = (byte)'h'; p3[1] = (byte)'i'; p3[2] = (byte)'s';
                AhoPersistentBuild.Insert(p3, 3, patterns, lengths, ref numPatterns, roots, nexts, fails, counts, ref nodeCount, ref activeMask);

                // "hers"
                byte* p4 = stackalloc byte[4];
                p4[0] = (byte)'h'; p4[1] = (byte)'e'; p4[2] = (byte)'r'; p4[3] = (byte)'s';
                AhoPersistentBuild.Insert(p4, 4, patterns, lengths, ref numPatterns, roots, nexts, fails, counts, ref nodeCount, ref activeMask);

                // Query "ushers"
                byte* text = stackalloc byte[6];
                text[0] = (byte)'u'; text[1] = (byte)'s'; text[2] = (byte)'h'; text[3] = (byte)'e'; text[4] = (byte)'r'; text[5] = (byte)'s';
                
                long matches = AhoPersistentQuery.Run(text, 6, roots, activeMask, nexts, counts);
                
                // Matches in "ushers" from dictionary {"he", "she", "his", "hers"}:
                // "she", "he", "hers" => 3 matches
                Assert.Equal(3L, matches);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)patterns);
                Marshal.FreeHGlobal((nint)lengths);
                Marshal.FreeHGlobal((nint)roots);
                Marshal.FreeHGlobal((nint)nexts);
                Marshal.FreeHGlobal((nint)fails);
                Marshal.FreeHGlobal((nint)counts);
            }
        }
    }
}
