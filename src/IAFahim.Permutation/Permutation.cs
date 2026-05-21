namespace IAFahim.Permutation
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ValidatePermutation
    {
        public static bool Run(int n, int* p)
        {
            bool* seen = stackalloc bool[n];
            for (int i = 0; i < n; i++) seen[i] = false;
            for (int i = 0; i < n; i++)
            {
                if (p[i] < 0 || p[i] >= n || seen[p[i]]) return false;
                seen[p[i]] = true;
            }
            return true;
        }
    }

    public static unsafe class InversePermutation
    {
        public static void Run(int n, int* p, int* inv)
        {
            for (int i = 0; i < n; i++) inv[p[i]] = i;
        }
    }

    public static unsafe class ComposePermutation
    {
        public static void Run(int n, int* a, int* b, int* result)
        {
            for (int i = 0; i < n; i++) result[i] = b[a[i]];
        }
    }

    public static unsafe class PermPower
    {
        public static void Run(int n, int* p, int* result, long k)
        {
            for (int i = 0; i < n; i++) result[i] = i;
            int* temp = stackalloc int[n];
            int* basePerm = stackalloc int[n];
            for (int i = 0; i < n; i++) basePerm[i] = p[i];
            while (k > 0)
            {
                if ((k & 1) == 1)
                {
                    for (int i = 0; i < n; i++) temp[i] = result[i];
                    for (int i = 0; i < n; i++) result[i] = basePerm[temp[i]];
                }
                for (int i = 0; i < n; i++) temp[i] = basePerm[i];
                for (int i = 0; i < n; i++) basePerm[i] = temp[temp[i]];
                k >>= 1;
            }
        }
    }

    public static unsafe class CycleDecomposition
    {
        public static int Run(int n, int* p, int* cycles, int* start, int* length)
        {
            bool* visited = stackalloc bool[n];
            for (int i = 0; i < n; i++) visited[i] = false;
            int cycleCount = 0;
            for (int i = 0; i < n; i++)
            {
                if (visited[i]) continue;
                int startIdx = cycleCount;
                cycles[cycleCount++] = i;
                int cur = p[i];
                while (cur != i)
                {
                    visited[cur] = true;
                    cycles[cycleCount++] = cur;
                    cur = p[cur];
                }
                length[startIdx] = cycleCount - startIdx;
                start[startIdx] = startIdx;
            }
            return cycleCount;
        }
    }

    public static unsafe class KthPermutation
    {
        public static void Run(int n, long k, int* result)
        {
            long* fact = stackalloc long[n + 1];
            fact[0] = 1;
            for (int i = 1; i <= n; i++) fact[i] = fact[i - 1] * i;
            bool* used = stackalloc bool[n];
            for (int i = 0; i < n; i++) used[i] = false;
            k--;
            for (int i = 0; i < n; i++)
            {
                int idx = (int)(k / fact[n - 1 - i]);
                int cnt = 0;
                for (int j = 0; j < n; j++)
                {
                    if (!used[j])
                    {
                        if (cnt == idx) { result[i] = j; used[j] = true; break; }
                        cnt++;
                    }
                }
                k %= fact[n - 1 - i];
            }
        }
    }

    public static unsafe class PermutationRank
    {
        public static long Run(int n, int* perm)
        {
            long* fact = stackalloc long[n + 1];
            fact[0] = 1;
            for (int i = 1; i <= n; i++) fact[i] = fact[i - 1] * i;
            bool* used = stackalloc bool[n];
            for (int i = 0; i < n; i++) used[i] = false;
            long rank = 0;
            for (int i = 0; i < n; i++)
            {
                int cnt = 0;
                for (int j = 0; j < perm[i]; j++)
                    if (!used[j]) cnt++;
                rank += cnt * fact[n - 1 - i];
                used[perm[i]] = true;
            }
            return rank + 1;
        }
    }

    public static unsafe class GrayRank
    {
        public static long Run(long n)
        {
            return n ^ (n >> 1);
        }
    }

    public static unsafe class GrayUnrank
    {
        public static long Run(long g)
        {
            long n = 0;
            while (g > 0)
            {
                n ^= g;
                g >>= 1;
            }
            return n;
        }
    }
}
