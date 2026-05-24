namespace IAFahim.Graph.DAG
{
    using System.Runtime.CompilerServices;

    public static unsafe class KthTopologicalOrder
    {
        public static bool Run(int* adjMask, int n, long* dp, long k, int* order)
        {
            int maxMask = 1 << n;
            for (int i = 0; i < maxMask; i++) dp[i] = 0;
            dp[0] = 1;

            for (int m = 0; m < maxMask; m++) if (dp[m] != 0) UpdateDpForMask(m, n, adjMask, dp);

            if (k > dp[maxMask - 1] || k <= 0) return false;

            int curM = maxMask - 1;
            for (int s = n - 1; s >= 0; s--)
            {
                int nextV = FindKthNode(curM, n, adjMask, dp, ref k);
                order[s] = nextV; curM ^= (1 << nextV);
            }
            return true;
        }

        private static void UpdateDpForMask(int m, int n, int* adjMask, long* dp)
        {
            for (int i = 0; i < n; i++)
                if ((m & (1 << i)) == 0 && (adjMask[i] & m) == adjMask[i]) dp[m | (1 << i)] += dp[m];
        }

        private static int FindKthNode(int m, int n, int* adjMask, long* dp, ref long k)
        {
            for (int i = n - 1; i >= 0; i--)
            {
                if ((m & (1 << i)) != 0 && (adjMask[i] & (m ^ (1 << i))) == adjMask[i])
                {
                    long cnt = dp[m ^ (1 << i)];
                    if (k <= cnt) return i;
                    k -= cnt;
                }
            }
            return -1;
        }
    }
}
