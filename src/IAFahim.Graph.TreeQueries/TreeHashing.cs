namespace IAFahim.Graph.TreeQueries
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TreeHashing
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong SplitMix64(ulong x)
        {
            x ^= x >> 30;
            x *= 0xbf58476d1ce4e5b9UL;
            x ^= x >> 27;
            x *= 0x94d049bb133111ebUL;
            x ^= x >> 31;
            return x;
        }

        private static void SortUlong(ulong* arr, int len)
        {
            for (int i = 1; i < len; i++)
            {
                ulong key = arr[i];
                int j = i - 1;
                while (j >= 0 && arr[j] > key)
                {
                    arr[j + 1] = arr[j];
                    j--;
                }
                arr[j + 1] = key;
            }
        }

        public static ulong CanonicalHashRooted(
            int u, int p,
            int* head, int* to, int* next,
            ulong* subHash)
        {
            int childCount = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p) childCount++;
            }

            if (childCount == 0)
            {
                subHash[u] = SplitMix64(1);
                return subHash[u];
            }

            ulong* childHashes = stackalloc ulong[childCount];
            int idx = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    childHashes[idx++] = CanonicalHashRooted(v, u, head, to, next, subHash);
                }
            }

            SortUlong(childHashes, childCount);

            ulong h = 1;
            for (int i = 0; i < childCount; i++)
            {
                h = h * 1000003UL + SplitMix64(childHashes[i]);
            }
            subHash[u] = SplitMix64(h);
            return subHash[u];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong CanonicalHash(
            int n, int* head, int* to, int* next)
        {
            int* centroids = stackalloc int[2];
            int centroidCount = 0;
            TreeCentroid.AllCentroids(n, head, to, next, centroids, ref centroidCount);

            ulong* subHash = stackalloc ulong[n];
            if (centroidCount == 1)
            {
                return CanonicalHashRooted(centroids[0], -1, head, to, next, subHash);
            }
            else
            {
                ulong h1 = CanonicalHashRooted(centroids[0], -1, head, to, next, subHash);
                ulong h2 = CanonicalHashRooted(centroids[1], -1, head, to, next, subHash);
                return h1 < h2 ? h1 : h2;
            }
        }

        public static long AutomorphismCountRooted(
            int u, int p,
            int* head, int* to, int* next,
            ulong* subHash, long mod)
        {
            int childCount = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p) childCount++;
            }

            if (childCount == 0) return 1;

            ulong* childHashes = stackalloc ulong[childCount];
            int idx = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    childHashes[idx++] = subHash[v];
                }
            }

            SortUlong(childHashes, childCount);

            long ans = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    long subAns = AutomorphismCountRooted(v, u, head, to, next, subHash, mod);
                    ans = (ans * subAns) % mod;
                }
            }

            int count = 1;
            for (int i = 1; i < childCount; i++)
            {
                if (childHashes[i] == childHashes[i - 1])
                {
                    count++;
                }
                else
                {
                    for (int k = 1; k <= count; k++) ans = (ans * k) % mod;
                    count = 1;
                }
            }
            for (int k = 1; k <= count; k++) ans = (ans * k) % mod;

            return ans;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AutomorphismCount(
            int n, int* head, int* to, int* next, long mod)
        {
            int* centroids = stackalloc int[2];
            int centroidCount = 0;
            TreeCentroid.AllCentroids(n, head, to, next, centroids, ref centroidCount);

            ulong* subHash = stackalloc ulong[n];
            if (centroidCount == 1)
            {
                CanonicalHashRooted(centroids[0], -1, head, to, next, subHash);
                return AutomorphismCountRooted(centroids[0], -1, head, to, next, subHash, mod);
            }
            else
            {
                int c1 = centroids[0];
                int c2 = centroids[1];
                ulong h1 = CanonicalHashRooted(c1, -1, head, to, next, subHash);
                ulong h2 = CanonicalHashRooted(c2, -1, head, to, next, subHash);

                long ans1 = AutomorphismCountRooted(c1, -1, head, to, next, subHash, mod);

                if (h1 == h2)
                {
                    return (ans1 * 2) % mod;
                }
                return ans1;
            }
        }

        // --- EMBEDDING CHECK ---
        private static bool Kuhn(int u, int* head, int* to, int* next, int* match, byte* visited)
        {
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (visited[v] == 0)
                {
                    visited[v] = 1;
                    if (match[v] < 0 || Kuhn(match[v], head, to, next, match, visited))
                    {
                        match[v] = u;
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool EmbeddingCheckRooted(
            int u1, int p1, int u2, int p2,
            int* head1, int* to1, int* next1,
            int* head2, int* to2, int* next2)
        {
            int childCount1 = 0;
            for (int e = head1[u1]; e != 0; e = next1[e])
            {
                int v = to1[e];
                if (v != p1) childCount1++;
            }

            int childCount2 = 0;
            for (int e = head2[u2]; e != 0; e = next2[e])
            {
                int v = to2[e];
                if (v != p2) childCount2++;
            }

            if (childCount1 == 0) return true;
            if (childCount2 < childCount1) return false;

            int* children1 = stackalloc int[childCount1];
            int idx1 = 0;
            for (int e = head1[u1]; e != 0; e = next1[e])
            {
                int v = to1[e];
                if (v != p1) children1[idx1++] = v;
            }

            int* children2 = stackalloc int[childCount2];
            int idx2 = 0;
            for (int e = head2[u2]; e != 0; e = next2[e])
            {
                int v = to2[e];
                if (v != p2) children2[idx2++] = v;
            }

            int* bHead = stackalloc int[childCount1];
            int* bTo = stackalloc int[childCount1 * childCount2 + 1];
            int* bNext = stackalloc int[childCount1 * childCount2 + 1];
            int bEdgeCount = 1;

            for (int i = 0; i < childCount1; i++) bHead[i] = 0;

            for (int i = 0; i < childCount1; i++)
            {
                for (int j = 0; j < childCount2; j++)
                {
                    if (EmbeddingCheckRooted(children1[i], u1, children2[j], u2, head1, to1, next1, head2, to2, next2))
                    {
                        bTo[bEdgeCount] = j;
                        bNext[bEdgeCount] = bHead[i];
                        bHead[i] = bEdgeCount++;
                    }
                }
            }

            int* match = stackalloc int[childCount2];
            for (int j = 0; j < childCount2; j++) match[j] = -1;

            int matchCount = 0;
            byte* visited = stackalloc byte[childCount2];

            for (int i = 0; i < childCount1; i++)
            {
                for (int j = 0; j < childCount2; j++) visited[j] = 0;
                if (Kuhn(i, bHead, bTo, bNext, match, visited))
                {
                    matchCount++;
                }
            }

            return matchCount == childCount1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EmbeddingCheck(
            int n1, int* head1, int* to1, int* next1,
            int n2, int* head2, int* to2, int* next2)
        {
            for (int r2 = 0; r2 < n2; r2++)
            {
                if (EmbeddingCheckRooted(0, -1, r2, -1, head1, to1, next1, head2, to2, next2))
                {
                    return true;
                }
            }
            return false;
        }

        // --- ZHANG-SHASHA TREE EDIT DISTANCE ---
        private static void PostOrder(
            int u, int p,
            int* head, int* to, int* next,
            int* postorder, ref int pTimer,
            int* leftmost, int* postorderIdx)
        {
            int firstChild = -1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p)
                {
                    if (firstChild == -1) firstChild = v;
                    PostOrder(v, u, head, to, next, postorder, ref pTimer, leftmost, postorderIdx);
                }
            }

            int myIdx = pTimer++;
            postorder[myIdx] = u;
            postorderIdx[u] = myIdx;

            if (firstChild == -1)
            {
                leftmost[myIdx] = myIdx;
            }
            else
            {
                leftmost[myIdx] = leftmost[postorderIdx[firstChild]];
            }
        }

        public static int TreeEditDistance(
            int n1, int* head1, int* to1, int* next1,
            int n2, int* head2, int* to2, int* next2)
        {
            int* postorder1 = stackalloc int[n1 + 1];
            int* leftmost1 = stackalloc int[n1 + 1];
            int* postorderIdx1 = stackalloc int[n1];
            int pTimer1 = 1;
            PostOrder(0, -1, head1, to1, next1, postorder1, ref pTimer1, leftmost1, postorderIdx1);

            int* postorder2 = stackalloc int[n2 + 1];
            int* leftmost2 = stackalloc int[n2 + 1];
            int* postorderIdx2 = stackalloc int[n2];
            int pTimer2 = 1;
            PostOrder(0, -1, head2, to2, next2, postorder2, ref pTimer2, leftmost2, postorderIdx2);

            byte* isKeyroot1 = stackalloc byte[n1 + 1];
            for (int i = 1; i <= n1; i++) isKeyroot1[i] = 1;
            for (int i = 1; i <= n1; i++)
            {
                int lm = leftmost1[i];
                for (int j = i + 1; j <= n1; j++)
                {
                    if (leftmost1[j] == lm)
                    {
                        isKeyroot1[i] = 0;
                        break;
                    }
                }
            }
            isKeyroot1[n1] = 1;

            byte* isKeyroot2 = stackalloc byte[n2 + 1];
            for (int i = 1; i <= n2; i++) isKeyroot2[i] = 1;
            for (int i = 1; i <= n2; i++)
            {
                int lm = leftmost2[i];
                for (int j = i + 1; j <= n2; j++)
                {
                    if (leftmost2[j] == lm)
                    {
                        isKeyroot2[i] = 0;
                        break;
                    }
                }
            }
            isKeyroot2[n2] = 1;

            int* td = stackalloc int[(n1 + 1) * (n2 + 1)];
            int* fd = stackalloc int[(n1 + 1) * (n2 + 1)];

            for (int i = 1; i <= n1; i++)
            {
                if (isKeyroot1[i] != 0)
                {
                    for (int j = 1; j <= n2; j++)
                    {
                        if (isKeyroot2[j] != 0)
                        {
                            int l1 = leftmost1[i];
                            int l2 = leftmost2[j];

                            for (int x = l1 - 1; x <= i; x++)
                            {
                                for (int y = l2 - 1; y <= j; y++)
                                {
                                    fd[x * (n2 + 1) + y] = 0;
                                }
                            }

                            for (int x = l1; x <= i; x++)
                            {
                                fd[x * (n2 + 1) + (l2 - 1)] = fd[(x - 1) * (n2 + 1) + (l2 - 1)] + 1;
                            }
                            for (int y = l2; y <= j; y++)
                            {
                                fd[(l1 - 1) * (n2 + 1) + y] = fd[(l1 - 1) * (n2 + 1) + (y - 1)] + 1;
                            }

                            for (int x = l1; x <= i; x++)
                            {
                                for (int y = l2; y <= j; y++)
                                {
                                    if (leftmost1[x] == l1 && leftmost2[y] == l2)
                                    {
                                        int cost = 0; // unlabeled tree edit distance: node replacement cost is 0
                                        int valDel = fd[(x - 1) * (n2 + 1) + y] + 1;
                                        int valIns = fd[x * (n2 + 1) + (y - 1)] + 1;
                                        int valRepl = fd[(x - 1) * (n2 + 1) + (y - 1)] + cost;
                                        fd[x * (n2 + 1) + y] = Math.Min(Math.Min(valDel, valIns), valRepl);
                                        td[x * (n2 + 1) + y] = fd[x * (n2 + 1) + y];
                                    }
                                    else
                                    {
                                        int valDel = fd[(x - 1) * (n2 + 1) + y] + 1;
                                        int valIns = fd[x * (n2 + 1) + (y - 1)] + 1;
                                        int valTree = fd[(leftmost1[x] - 1) * (n2 + 1) + (leftmost2[y] - 1)] + td[x * (n2 + 1) + y];
                                        fd[x * (n2 + 1) + y] = Math.Min(Math.Min(valDel, valIns), valTree);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return td[n1 * (n2 + 1) + n2];
        }
    }
}
