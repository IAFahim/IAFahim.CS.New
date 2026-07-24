namespace IAFahim.DP.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class Knapsack01Tests
    {
        [Test]
        public void Empty_Zero()
        {
            long* dp = stackalloc long[1];
            dp[0] = 0;
            Assert.AreEqual(0, Knapsack01.Run(0, 0, null, null, dp));
        }

        [Test]
        public void Classic_Instance()
        {
            const int N = 3;
            const long Cap = 5;
            long* w = stackalloc long[N];
            long* v = stackalloc long[N];
            w[0] = 2; w[1] = 3; w[2] = 4;
            v[0] = 3; v[1] = 4; v[2] = 5;
            int cols = (int)Cap + 1;
            long* dp = (long*)Marshal.AllocHGlobal((N + 1) * cols * sizeof(long));
            try
            {
                for (int i = 0; i < (N + 1) * cols; i++) dp[i] = 0;
                long ans = Knapsack01.Run(N, Cap, w, v, dp);
                Assert.AreEqual(7, ans);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)dp);
            }
        }
    }
}
