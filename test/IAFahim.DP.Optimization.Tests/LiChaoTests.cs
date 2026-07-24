namespace IAFahim.DP.Optimization.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class LiChaoTests
    {
        [Test]
        public void SingleLine_QueryMatches()
        {
            const int Nodes = 64;
            long* seg = (long*)Marshal.AllocHGlobal(Nodes * 2 * sizeof(long));
            try
            {
                for (int i = 0; i < Nodes * 2; i++) seg[i] = 0;
                for (int i = 0; i < Nodes; i++)
                {
                    seg[i * 2] = 0;
                    seg[i * 2 + 1] = long.MaxValue / 4;
                }
                LiChaoAddLine.Run(seg, 2, 3, 0, 0, 0, 0, 7);
                long inf = long.MaxValue / 4;
                for (long x = 0; x <= 7; x++)
                {
                    long got = LiChaoAddLine.Query(seg, 0, x, 0, 7, inf);
                    Assert.AreEqual(2 * x + 3, got);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)seg);
            }
        }

        [Test]
        public void TwoLines_LowerEnvelope()
        {
            const int Nodes = 128;
            long* seg = (long*)Marshal.AllocHGlobal(Nodes * 2 * sizeof(long));
            try
            {
                long inf = long.MaxValue / 4;
                for (int i = 0; i < Nodes; i++)
                {
                    seg[i * 2] = 0;
                    seg[i * 2 + 1] = inf;
                }
                LiChaoAddLine.Run(seg, 0, 10, 0, 0, 0, 0, 10);
                LiChaoAddLine.Run(seg, 1, 0, 0, 0, 0, 0, 10);
                for (long x = 0; x <= 10; x++)
                {
                    long got = LiChaoAddLine.Query(seg, 0, x, 0, 10, inf);
                    long want = 0 * x + 10;
                    long alt = 1 * x + 0;
                    if (alt < want) want = alt;
                    Assert.AreEqual(want, got);
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)seg);
            }
        }
    }
}
