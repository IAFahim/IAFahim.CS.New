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
        public static void Run(int n, int* p, int* inv) { for (int i = 0; i < n; i++) inv[p[i]] = i; }
    }

    public static unsafe class ComposePermutation
    {
        public static void Run(int n, int* a, int* b, int* result) { for (int i = 0; i < n; i++) result[i] = b[a[i]]; }
    }

    public static unsafe class PermPower
    {
        public static void Run(int n, int* p, int* result, long k)
        {
            InitializeIdentity(n, result);
            int* basePerm = stackalloc int[n]; Buffer.MemoryCopy(p, basePerm, n * sizeof(int), n * sizeof(int));
            int* temp = stackalloc int[n];
            while (k > 0)
            {
                if ((k & 1) == 1) ComposeInto(n, result, basePerm, temp);
                if (k > 1) ComposeInto(n, basePerm, basePerm, temp);
                k >>= 1;
            }
        }

        private static void InitializeIdentity(int n, int* res) { for (int i = 0; i < n; i++) res[i] = i; }
        private static void ComposeInto(int n, int* a, int* b, int* tmp)
        {
            for (int i = 0; i < n; i++) tmp[i] = b[a[i]];
            Buffer.MemoryCopy(tmp, a, n * sizeof(int), n * sizeof(int));
        }
    }

    public static unsafe class CycleDecomposition
    {
        public static int Run(int n, int* p, int* cycles, int* start, int* length)
        {
            bool* visited = stackalloc bool[n];
            for (int i = 0; i < n; i++) visited[i] = false;
            int cycleCount = 0, currentPos = 0;
            for (int i = 0; i < n; i++)
            {
                if (visited[i]) continue;
                int startIdx = currentPos;
                start[cycleCount] = startIdx;
                int cur = i;
                while (!visited[cur])
                {
                    visited[cur] = true;
                    cycles[currentPos++] = cur;
                    cur = p[cur];
                }
                length[cycleCount++] = currentPos - startIdx;
            }
            return cycleCount;
        }
    }

    public static unsafe class KthPermutation
    {
        public static void Run(int n, long k, int* result)
        {
            long* fact = stackalloc long[n + 1]; ComputeFactorials(n, fact);
            bool* used = stackalloc bool[n]; for (int i = 0; i < n; i++) used[i] = false;
            k--;
            for (int i = 0; i < n; i++)
            {
                int digit = (int)(k / fact[n - 1 - i]);
                result[i] = ExtractNthUnused(n, digit, used);
                k %= fact[n - 1 - i];
            }
        }

        private static void ComputeFactorials(int n, long* fact)
        {
            fact[0] = 1; for (int i = 1; i <= n; i++) fact[i] = fact[i - 1] * i;
        }

        private static int ExtractNthUnused(int n, int targetIdx, bool* used)
        {
            int cnt = 0;
            for (int j = 0; j < n; j++)
                if (!used[j]) { if (cnt == targetIdx) { used[j] = true; return j; } cnt++; }
            return -1;
        }
    }

    public static unsafe class PermutationRank
    {
        public static long Run(int n, int* perm)
        {
            long* fact = stackalloc long[n + 1]; fact[0] = 1;
            for (int i = 1; i <= n; i++) fact[i] = fact[i - 1] * i;
            bool* used = stackalloc bool[n]; for (int i = 0; i < n; i++) used[i] = false;
            long rank = 0;
            for (int i = 0; i < n; i++)
            {
                int cnt = 0; for (int j = 0; j < perm[i]; j++) if (!used[j]) cnt++;
                rank += cnt * fact[n - 1 - i];
                used[perm[i]] = true;
            }
            return rank + 1;
        }
    }

    public static unsafe class GrayRank
    {
        public static long Run(long n) => n ^ (n >> 1);
    }

    public static unsafe class GrayUnrank
    {
        public static long Run(long g) { long n = 0; while (g > 0) { n ^= g; g >>= 1; } return n; }
    }
}
