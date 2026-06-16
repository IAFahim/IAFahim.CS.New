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

        private const int HeapSortThreshold = 32;

        private static void SortUlong(ulong* arr, int len)
        {
            if (len > HeapSortThreshold) { HeapSortUlong(arr, len); return; }
            for (int i = 1; i < len; i++)
            {
                ulong key = arr[i];
                int j = i - 1;
                while (j >= 0 && arr[j] > key) { arr[j + 1] = arr[j]; j--; }
                arr[j + 1] = key;
            }
        }

        private static void HeapSortUlong(ulong* arr, int len)
        {
            for (int i = len / 2 - 1; i >= 0; i--) SiftDownUlong(arr, i, len);
            for (int end = len - 1; end > 0; end--)
            {
                ulong tmp = arr[0]; arr[0] = arr[end]; arr[end] = tmp;
                SiftDownUlong(arr, 0, end);
            }
        }

        private static void SiftDownUlong(ulong* arr, int root, int len)
        {
            while (true)
            {
                int child = 2 * root + 1;
                if (child >= len) break;
                if (child + 1 < len && arr[child + 1] > arr[child]) child++;
                if (arr[root] >= arr[child]) break;
                ulong tmp = arr[root]; arr[root] = arr[child]; arr[child] = tmp;
                root = child;
            }
        }

        public static ulong CanonicalHashRooted(int u, int p, int* head, int* to, int* next, ulong* subHash)
        {
            int childCount = CountChildren(u, p, head, to, next);
            if (childCount == 0) return subHash[u] = SplitMix64(1);

            ulong* childHashes = stackalloc ulong[childCount];
            CollectAndSortChildHashes(u, p, head, to, next, subHash, childHashes);

            ulong h = 1;
            for (int i = 0; i < childCount; i++) h = h * 1000003UL + SplitMix64(childHashes[i]);
            return subHash[u] = SplitMix64(h);
        }

        private static int CountChildren(int u, int p, int* head, int* to, int* next)
        {
            int count = 0;
            for (int e = head[u]; e != 0; e = next[e]) if (to[e] != p) count++;
            return count;
        }

        private static void CollectAndSortChildHashes(int u, int p, int* head, int* to, int* next, ulong* subHash, ulong* childHashes)
        {
            int idx = 0;
            for (int e = head[u]; e != 0; e = next[e])
            {
                if (to[e] != p) childHashes[idx++] = CanonicalHashRooted(to[e], u, head, to, next, subHash);
            }
            SortUlong(childHashes, idx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong CanonicalHash(int n, int* head, int* to, int* next)
        {
            int* centroids = stackalloc int[2]; int centroidCount = 0;
            TreeCentroid.AllCentroids(n, head, to, next, centroids, ref centroidCount);

            ulong* subHash = stackalloc ulong[n];
            if (centroidCount == 1) return CanonicalHashRooted(centroids[0], -1, head, to, next, subHash);
            
            ulong h1 = CanonicalHashRooted(centroids[0], -1, head, to, next, subHash);
            ulong h2 = CanonicalHashRooted(centroids[1], -1, head, to, next, subHash);
            return h1 < h2 ? h1 : h2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long AutomorphismCount(int n, int* head, int* to, int* next, long mod)
        {
            int* centroids = stackalloc int[2]; int centroidCount = 0;
            TreeCentroid.AllCentroids(n, head, to, next, centroids, ref centroidCount);

            ulong* subHash = stackalloc ulong[n];
            if (centroidCount == 1)
            {
                CanonicalHashRooted(centroids[0], -1, head, to, next, subHash);
                return AutomorphismCountRooted(centroids[0], -1, head, to, next, subHash, mod);
            }
            
            ulong h1 = CanonicalHashRooted(centroids[0], -1, head, to, next, subHash);
            ulong h2 = CanonicalHashRooted(centroids[1], -1, head, to, next, subHash);
            if (h1 != h2) return (AutomorphismCountRooted(centroids[0], -1, head, to, next, subHash, mod) * AutomorphismCountRooted(centroids[1], -1, head, to, next, subHash, mod)) % mod;
            return (AutomorphismCountRooted(centroids[0], centroids[1], head, to, next, subHash, mod) * AutomorphismCountRooted(centroids[1], centroids[0], head, to, next, subHash, mod) * 2) % mod;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool EmbeddingCheck(int n1, int* head1, int* to1, int* next1, int n2, int* head2, int* to2, int* next2)
        {
            // Try all possible roots for tree 1 and tree 2? No, that's O(N^2).
            // But for small trees it's fine.
            for (int r2 = 0; r2 < n2; r2++)
                if (EmbeddingCheckRooted(0, -1, r2, -1, head1, to1, next1, head2, to2, next2)) return true;
            return false;
        }

        public static long AutomorphismCountRooted(int u, int p, int* head, int* to, int* next, ulong* subHash, long mod)
        {
            int childCount = CountChildren(u, p, head, to, next);
            if (childCount == 0) return 1;

            ulong* childHashes = stackalloc ulong[childCount];
            int idx = 0; long ans = 1;
            for (int e = head[u]; e != 0; e = next[e])
            {
                int v = to[e];
                if (v != p) { childHashes[idx++] = subHash[v]; ans = (ans * AutomorphismCountRooted(v, u, head, to, next, subHash, mod)) % mod; }
            }
            SortUlong(childHashes, childCount);
            return MultiplyBySymmetryFactor(ans, childHashes, childCount, mod);
        }

        private static long MultiplyBySymmetryFactor(long ans, ulong* childHashes, int childCount, long mod)
        {
            int count = 1;
            for (int i = 1; i < childCount; i++)
            {
                if (childHashes[i] == childHashes[i - 1]) count++;
                else { for (int k = 1; k <= count; k++) ans = (ans * k) % mod; count = 1; }
            }
            for (int k = 1; k <= count; k++) ans = (ans * k) % mod;
            return ans;
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
                    if (match[v] < 0 || Kuhn(match[v], head, to, next, match, visited)) { match[v] = u; return true; }
                }
            }
            return false;
        }

        public static bool EmbeddingCheckRooted(int u1, int p1, int u2, int p2, int* head1, int* to1, int* next1, int* head2, int* to2, int* next2)
        {
            int cc1 = CountChildren(u1, p1, head1, to1, next1), cc2 = CountChildren(u2, p2, head2, to2, next2);
            if (cc1 == 0) return true;
            if (cc2 < cc1) return false;

            int* children1 = stackalloc int[cc1], children2 = stackalloc int[cc2];
            CollectChildren(u1, p1, head1, to1, next1, children1);
            CollectChildren(u2, p2, head2, to2, next2, children2);

            return CanMatchChildren(cc1, cc2, children1, children2, u1, u2, head1, to1, next1, head2, to2, next2);
        }

        private static void CollectChildren(int u, int p, int* head, int* to, int* next, int* children)
        {
            int idx = 0;
            for (int e = head[u]; e != 0; e = next[e]) if (to[e] != p) children[idx++] = to[e];
        }

        private static bool CanMatchChildren(int cc1, int cc2, int* children1, int* children2, int u1, int u2, int* head1, int* to1, int* next1, int* head2, int* to2, int* next2)
        {
            int* bHead = stackalloc int[cc1], bTo = stackalloc int[cc1 * cc2 + 1], bNext = stackalloc int[cc1 * cc2 + 1];
            int bEc = 1; for (int i = 0; i < cc1; i++) bHead[i] = 0;

            for (int i = 0; i < cc1; i++)
                for (int j = 0; j < cc2; j++)
                    if (EmbeddingCheckRooted(children1[i], u1, children2[j], u2, head1, to1, next1, head2, to2, next2))
                    {
                        bTo[bEc] = j; bNext[bEc] = bHead[i]; bHead[i] = bEc++;
                    }

            int* match = stackalloc int[cc2]; for (int j = 0; j < cc2; j++) match[j] = -1;
            int mc = 0; byte* vis = stackalloc byte[cc2];
            for (int i = 0; i < cc1; i++)
            {
                for (int j = 0; j < cc2; j++) vis[j] = 0;
                if (Kuhn(i, bHead, bTo, bNext, match, vis)) mc++;
            }
            return mc == cc1;
        }

        public static int TreeEditDistance(int n1, int* head1, int* to1, int* next1, int n2, int* head2, int* to2, int* next2)
        {
            int* po1 = stackalloc int[n1 + 1], lm1 = stackalloc int[n1 + 1], poi1 = stackalloc int[n1];
            int pt1 = 1; PostOrder(0, -1, head1, to1, next1, po1, ref pt1, lm1, poi1);

            int* po2 = stackalloc int[n2 + 1], lm2 = stackalloc int[n2 + 1], poi2 = stackalloc int[n2];
            int pt2 = 1; PostOrder(0, -1, head2, to2, next2, po2, ref pt2, lm2, poi2);

            byte* kr1 = stackalloc byte[n1 + 1], kr2 = stackalloc byte[n2 + 1];
            IdentifyKeyroots(n1, lm1, kr1); IdentifyKeyroots(n2, lm2, kr2);

            int* td = stackalloc int[(n1 + 1) * (n2 + 1)], fd = stackalloc int[(n1 + 1) * (n2 + 1)];
            for (int i = 1; i <= n1; i++)
                if (kr1[i] != 0)
                    for (int j = 1; j <= n2; j++)
                        if (kr2[j] != 0) UpdateForestDistances(i, j, n2, lm1, lm2, td, fd);

            return td[n1 * (n2 + 1) + n2];
        }

        private static void IdentifyKeyroots(int n, int* lm, byte* kr)
        {
            byte* seen = stackalloc byte[n + 1];
            for (int i = 0; i <= n; i++) seen[i] = 0;
            for (int i = n; i >= 1; i--)
            {
                int l = lm[i];
                if (seen[l] == 0) { kr[i] = 1; seen[l] = 1; }
                else kr[i] = 0;
            }
        }

        private static void UpdateForestDistances(int i, int j, int n2, int* lm1, int* lm2, int* td, int* fd)
        {
            int l1 = lm1[i], l2 = lm2[j], n2p = n2 + 1;
            for (int x = l1 - 1; x <= i; x++) for (int y = l2 - 1; y <= j; y++) fd[x * n2p + y] = 0;
            for (int x = l1; x <= i; x++) fd[x * n2p + (l2 - 1)] = fd[(x - 1) * n2p + (l2 - 1)] + 1;
            for (int y = l2; y <= j; y++) fd[(l1 - 1) * n2p + y] = fd[(l1 - 1) * n2p + (y - 1)] + 1;

            for (int x = l1; x <= i; x++)
            for (int y = l2; y <= j; y++)
            {
                if (lm1[x] == l1 && lm2[y] == l2)
                {
                    fd[x * n2p + y] = Math.Min(Math.Min(fd[(x - 1) * n2p + y] + 1, fd[x * n2p + (y - 1)] + 1), fd[(x - 1) * n2p + (y - 1)]);
                    td[x * n2p + y] = fd[x * n2p + y];
                }
                else fd[x * n2p + y] = Math.Min(Math.Min(fd[(x - 1) * n2p + y] + 1, fd[x * n2p + (y - 1)] + 1), fd[(lm1[x] - 1) * n2p + (lm2[y] - 1)] + td[x * n2p + y]);
            }
        }

        private static void PostOrder(int u, int p, int* head, int* to, int* next, int* po, ref int pt, int* lm, int* poi)
        {
            int fc = -1;
            for (int e = head[u]; e != 0; e = next[e])
                if (to[e] != p) { if (fc == -1) fc = to[e]; PostOrder(to[e], u, head, to, next, po, ref pt, lm, poi); }
            int cur = pt++; po[cur] = u; poi[u] = cur;
            lm[cur] = fc == -1 ? cur : lm[poi[fc]];
        }
    }
}
