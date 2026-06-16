namespace IAFahim.Optimization.Games
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class MinCostFlow
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long MinCostCirculation(int n, int m, int* from, int* to, long* cap, long* cost)
        {
            long totalCost = 0;
            for (int iter = 0; iter < m; iter++)
            {
                if (cap[iter] > 0)
                    totalCost += cost[iter] * cap[iter];
            }
            return totalCost;
        }

        // Chu-Liu/Edmonds minimum-cost arborescence rooted at `root`.
        // Returns the total weight of the cheapest spanning arborescence (every
        // non-root vertex reachable from root via the chosen in-edges), or
        // long.MaxValue when no arborescence exists. Input edge arrays are not
        // modified; all working state lives in stackalloc scratch.
        public static long MinCostArborescence(int n, int* from, int* to, long* w, int m, int root)
        {
            const long Inf = long.MaxValue;

            if (n <= 0) return 0;

            // Working edge set (mutated across contraction rounds). Each contraction
            // round can collapse at least one cycle, so the number of active vertices
            // strictly decreases; at most n - 1 rounds occur. Edge count never grows.
            long* curW = stackalloc long[m];
            int* curFrom = stackalloc int[m];
            int* curTo = stackalloc int[m];
            for (int e = 0; e < m; e++)
            {
                curW[e] = w[e];
                curFrom[e] = from[e];
                curTo[e] = to[e];
            }

            // Per-round scratch sized for the maximum vertex count (never exceeds n).
            long* minIn = stackalloc long[n];   // weight of chosen min in-edge per vertex
            int* fromVertex = stackalloc int[n]; // source vertex of that chosen edge
            int* id = stackalloc int[n];        // cycle/SCC id assignment (relabel target)
            int* visited = stackalloc int[n];   // cycle-detection marker
            int* onStack = stackalloc int[n];   // cycle-detection marker

            int vertexCount = n;
            int curRoot = root;
            long total = 0;

            while (true)
            {
                // 1. Choose the cheapest incoming edge for every non-root vertex.
                for (int v = 0; v < vertexCount; v++) minIn[v] = Inf;
                for (int e = 0; e < m; e++)
                {
                    int u = curFrom[e];
                    int v = curTo[e];
                    if (u == v) continue; // self-loops can never belong to an arborescence
                    if (v == curRoot) continue;
                    if (curW[e] < minIn[v])
                    {
                        minIn[v] = curW[e];
                        fromVertex[v] = u;
                    }
                }

                // Any non-root vertex without an incoming edge => no arborescence.
                for (int v = 0; v < vertexCount; v++)
                {
                    if (v == curRoot) continue;
                    if (minIn[v] == Inf) return Inf;
                }

                // 2. Detect cycles among the chosen in-edges and assign new ids.
                for (int v = 0; v < vertexCount; v++) { id[v] = -1; visited[v] = -1; }

                int newCount = 0;
                bool hasCycle = false;
                for (int v = 0; v < vertexCount; v++)
                {
                    // Walk back along chosen in-edges marking the current trace, until
                    // hitting the root, an already-finished vertex, or our own trace.
                    int u = v;
                    while (u != curRoot && visited[u] == -1 && id[u] == -1)
                    {
                        visited[u] = v;
                        u = fromVertex[u];
                    }
                    // If we re-entered the trace started in this iteration, it's a cycle.
                    if (u != curRoot && visited[u] == v && id[u] == -1)
                    {
                        hasCycle = true;
                        int x = u;
                        do
                        {
                            id[x] = newCount;
                            x = fromVertex[x];
                        } while (x != u);
                        newCount++;
                    }
                }

                // 3. No cycle: the chosen in-edges form the arborescence. Sum & finish.
                if (!hasCycle)
                {
                    for (int v = 0; v < vertexCount; v++)
                    {
                        if (v == curRoot) continue;
                        total += minIn[v];
                    }
                    return total;
                }

                // `inCycle[v]` is true exactly for vertices that were contracted (their
                // id was set during cycle detection). Assign fresh singleton ids to the
                // rest, and accumulate the in-edge weights of cycle members into total.
                // `onStack` is reused here as the inCycle flag (1 = cycle member).
                for (int v = 0; v < vertexCount; v++)
                {
                    if (id[v] == -1)
                    {
                        onStack[v] = 0;
                        id[v] = newCount++;
                    }
                    else
                    {
                        onStack[v] = 1;
                        total += minIn[v];
                    }
                }

                // 4. Reweight & relabel edges for the contracted graph.
                //    An edge into a cycle member loses that member's chosen in-weight.
                int newRoot = id[curRoot];
                int newM = 0;
                for (int e = 0; e < m; e++)
                {
                    int u = curFrom[e];
                    int v = curTo[e];
                    int nu = id[u];
                    int nv = id[v];
                    if (nu == nv) continue; // edge inside a contracted node: drop it
                    long nw = curW[e];
                    if (onStack[v] == 1) nw -= minIn[v]; // v is a contracted cycle member
                    curFrom[newM] = nu;
                    curTo[newM] = nv;
                    curW[newM] = nw;
                    newM++;
                }

                m = newM;
                vertexCount = newCount;
                curRoot = newRoot;
            }
        }

        // Karp's minimum mean cycle algorithm.
        // Returns floor of the minimum cycle mean (min over all directed cycles of
        // totalWeight/edgeCount). Returns long.MaxValue when the graph has no cycle.
        // NOTE: this returns the floored integer mean; the true mean equals bestNum/bestDen.
        public static long MinMeanCycle(int n, int* from, int* to, long* w, int m)
        {
            const long Inf = long.MaxValue;

            if (n <= 0 || m <= 0) return Inf;

            // d[k * n + v] = min cost of a walk using exactly k edges ending at v,
            // starting from a virtual source linked to every vertex with cost 0.
            // We need levels 0..n inclusive, i.e. (n + 1) rows of n vertices.
            int rows = n + 1;
            long* d = stackalloc long[rows * n];

            for (int k = 0; k <= n; k++)
            {
                long* dk = d + k * n;
                if (k == 0)
                {
                    for (int v = 0; v < n; v++) dk[v] = 0;
                    continue;
                }
                for (int v = 0; v < n; v++) dk[v] = Inf;
            }

            for (int k = 1; k <= n; k++)
            {
                long* prev = d + (k - 1) * n;
                long* cur = d + k * n;
                for (int e = 0; e < m; e++)
                {
                    int u = from[e];
                    if (prev[u] == Inf) continue;
                    long cand = prev[u] + w[e];
                    int v = to[e];
                    if (cand < cur[v]) cur[v] = cand;
                }
            }

            // Min over v of max over k in [0, n-1] of (d[n][v] - d[k][v]) / (n - k),
            // compared exactly via cross-multiplication with positive denominators.
            long* dn = d + n * n;
            bool found = false;
            long bestNum = 0;
            long bestDen = 1;
            for (int v = 0; v < n; v++)
            {
                if (dn[v] == Inf) continue;

                // Inner maximum (worst k) for this vertex, as a fraction maxNum/maxDen.
                bool haveMax = false;
                long maxNum = 0;
                long maxDen = 1;
                for (int k = 0; k < n; k++)
                {
                    long dkv = d[k * n + v];
                    if (dkv == Inf) continue;
                    long num = dn[v] - dkv;
                    long den = n - k; // strictly positive since k < n
                    // Compare num/den against maxNum/maxDen: num/den > maxNum/maxDen
                    // <=> num * maxDen > maxNum * den (all denominators positive).
                    if (!haveMax || CompareProducts(num, maxDen, maxNum, den) > 0)
                    {
                        haveMax = true;
                        maxNum = num;
                        maxDen = den;
                    }
                }

                if (!haveMax) continue;

                // Outer minimum: maxNum/maxDen < bestNum/bestDen
                // <=> maxNum * bestDen < bestNum * maxDen (denominators positive).
                if (!found || CompareProducts(maxNum, bestDen, bestNum, maxDen) < 0)
                {
                    found = true;
                    bestNum = maxNum;
                    bestDen = maxDen;
                }
            }

            if (!found) return Inf;

            // Floor division of bestNum / bestDen (bestDen > 0).
            long q = bestNum / bestDen;
            if (bestNum % bestDen != 0 && bestNum < 0) q--;
            return q;
        }

        // Returns the sign (-1, 0, +1) of the signed product comparison (a*b) - (c*d),
        // computed exactly in 128-bit width so it never overflows long. netstandard2.1
        // has no System.Int128, so each product is built from a 64x64 -> 128 unsigned
        // multiply (high:low) plus a separately tracked sign, then the two 128-bit
        // magnitudes are compared.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareProducts(long a, long b, long c, long d)
        {
            // Magnitudes of the two products as 128-bit values (hiL:loL) and (hiR:loR),
            // with signs sL and sR in {-1, +1} (treating a zero magnitude as +1 sign).
            ulong loL, hiL;
            int sL = SignedMul128(a, b, out hiL, out loL);
            ulong loR, hiR;
            int sR = SignedMul128(c, d, out hiR, out loR);

            bool zeroL = hiL == 0UL && loL == 0UL;
            bool zeroR = hiR == 0UL && loR == 0UL;
            if (zeroL) sL = 0;
            if (zeroR) sR = 0;

            if (sL != sR) return sL < sR ? -1 : 1;

            // Same sign (or both zero): compare magnitudes, then apply the common sign.
            int mag = CompareU128(hiL, loL, hiR, loR);
            if (mag == 0) return 0;
            // sL == sR here; if both zero we already returned 0 (mag would be 0).
            return sL >= 0 ? mag : -mag;
        }

        // Computes the magnitude of x*y as a 128-bit unsigned value (hi:lo) and returns
        // the sign of the product (+1 or -1; sign is meaningless when the value is zero).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int SignedMul128(long x, long y, out ulong hi, out ulong lo)
        {
            int sign = 1;
            ulong ux;
            if (x < 0) { sign = -sign; ux = (ulong)(-(x + 1)) + 1UL; }
            else ux = (ulong)x;
            ulong uy;
            if (y < 0) { sign = -sign; uy = (ulong)(-(y + 1)) + 1UL; }
            else uy = (ulong)y;

            Mul64(ux, uy, out hi, out lo);
            return sign;
        }

        // 64x64 -> 128 unsigned multiply: (hi:lo) = x * y.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Mul64(ulong x, ulong y, out ulong hi, out ulong lo)
        {
            const ulong Mask32 = 0xFFFFFFFFUL;
            ulong xLo = x & Mask32;
            ulong xHi = x >> 32;
            ulong yLo = y & Mask32;
            ulong yHi = y >> 32;

            ulong ll = xLo * yLo;
            ulong lh = xLo * yHi;
            ulong hl = xHi * yLo;
            ulong hh = xHi * yHi;

            ulong cross = (ll >> 32) + (lh & Mask32) + (hl & Mask32);
            lo = (ll & Mask32) | (cross << 32);
            hi = hh + (lh >> 32) + (hl >> 32) + (cross >> 32);
        }

        // Compares two 128-bit unsigned values (-1, 0, +1).
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int CompareU128(ulong hiL, ulong loL, ulong hiR, ulong loR)
        {
            if (hiL != hiR) return hiL < hiR ? -1 : 1;
            if (loL != loR) return loL < loR ? -1 : 1;
            return 0;
        }
    }
}
