namespace IAFahim.Pathfinding.Jps
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class JumpPointSearch
    {
        private const int Inf = 0x3f3f3f3f;

        // 4-connected pathfinding on binary grid (0 = walkable). Uses A* with
        // optional jump pruning along cardinal rays (Harabor & Grastien style
        // simplification): when a ray from u toward a neighbor is free, we jump
        // to the farthest forced/goal cell instead of stepping one by one.
        public static int FindPath(
            byte* grid, int width, int height,
            int sx, int sy, int gx, int gy,
            int* outPath, int outCap)
        {
            if (width <= 0 || height <= 0) return -1;
            if (!InBounds(sx, sy, width, height) || !InBounds(gx, gy, width, height)) return -1;
            if (Blocked(grid, width, sx, sy) || Blocked(grid, width, gx, gy)) return -1;
            if (sx == gx && sy == gy)
            {
                if (outCap < 1) return -1;
                outPath[0] = sx; outPath[1] = sy;
                return 1;
            }

            int n = width * height;
            int* gScore = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* parent = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            byte* closed = (byte*)Marshal.AllocHGlobal(n);
            int* heapN = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            int* heapF = (int*)Marshal.AllocHGlobal(n * sizeof(int));
            for (int i = 0; i < n; i++) { gScore[i] = Inf; parent[i] = -1; closed[i] = 0; }

            int start = sy * width + sx;
            int goal = gy * width + gx;
            gScore[start] = 0;
            int hs = 0;
            Push(heapN, heapF, ref hs, start, Heuristic(sx, sy, gx, gy));

            int found = -1;
            while (hs > 0)
            {
                int u = Pop(heapN, heapF, ref hs);
                if (closed[u] != 0) continue;
                closed[u] = 1;
                if (u == goal) { found = u; break; }

                int ux = u % width, uy = u / width;
                // Cardinal neighbors (A* base) — always safe.
                Relax(grid, width, height, u, ux, uy, ux + 1, uy, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
                Relax(grid, width, height, u, ux, uy, ux - 1, uy, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
                Relax(grid, width, height, u, ux, uy, ux, uy + 1, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
                Relax(grid, width, height, u, ux, uy, ux, uy - 1, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);

                // Jump successors along each ray (extra long-range edges).
                JumpRelax(grid, width, height, u, ux, uy, 1, 0, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
                JumpRelax(grid, width, height, u, ux, uy, -1, 0, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
                JumpRelax(grid, width, height, u, ux, uy, 0, 1, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
                JumpRelax(grid, width, height, u, ux, uy, 0, -1, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
            }

            int pathLen = -1;
            if (found >= 0)
            {
                int len = 0;
                for (int c = found; c >= 0; c = parent[c]) len++;
                if (len <= outCap)
                {
                    int idx = len - 1;
                    for (int c = found; c >= 0; c = parent[c])
                    {
                        outPath[idx * 2] = c % width;
                        outPath[idx * 2 + 1] = c / width;
                        idx--;
                    }
                    pathLen = len;
                }
            }

            Marshal.FreeHGlobal((nint)heapF);
            Marshal.FreeHGlobal((nint)heapN);
            Marshal.FreeHGlobal((nint)closed);
            Marshal.FreeHGlobal((nint)parent);
            Marshal.FreeHGlobal((nint)gScore);
            return pathLen;
        }

        private static void Relax(
            byte* grid, int w, int h, int u, int ux, int uy, int nx, int ny,
            int gx, int gy, int* gScore, int* parent, byte* closed,
            int* heapN, int* heapF, ref int hs)
        {
            if (!InBounds(nx, ny, w, h) || Blocked(grid, w, nx, ny)) return;
            int j = ny * w + nx;
            if (closed[j] != 0) return;
            int ng = gScore[u] + 1;
            if (ng < gScore[j])
            {
                gScore[j] = ng;
                parent[j] = u;
                Push(heapN, heapF, ref hs, j, ng + Heuristic(nx, ny, gx, gy));
            }
        }

        private static void JumpRelax(
            byte* grid, int w, int h, int u, int ux, int uy, int dx, int dy,
            int gx, int gy, int* gScore, int* parent, byte* closed,
            int* heapN, int* heapF, ref int hs)
        {
            int jx = ux, jy = uy;
            int steps = 0;
            while (true)
            {
                int nx = jx + dx, ny = jy + dy;
                if (!InBounds(nx, ny, w, h) || Blocked(grid, w, nx, ny)) return;
                jx = nx; jy = ny; steps++;
                if (jx == gx && jy == gy)
                {
                    Commit(u, jx, jy, w, steps, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
                    return;
                }
                if (HasForced(grid, w, h, jx, jy, dx, dy))
                {
                    Commit(u, jx, jy, w, steps, gx, gy, gScore, parent, closed, heapN, heapF, ref hs);
                    return;
                }
            }
        }

        private static void Commit(
            int u, int jx, int jy, int w, int steps, int gx, int gy,
            int* gScore, int* parent, byte* closed, int* heapN, int* heapF, ref int hs)
        {
            int j = jy * w + jx;
            if (closed[j] != 0) return;
            int ng = gScore[u] + steps;
            if (ng < gScore[j])
            {
                gScore[j] = ng;
                parent[j] = u;
                Push(heapN, heapF, ref hs, j, ng + Heuristic(jx, jy, gx, gy));
            }
        }

        private static bool HasForced(byte* grid, int w, int h, int x, int y, int dx, int dy)
        {
            if (dx != 0)
            {
                if (InBounds(x, y - 1, w, h) && !Blocked(grid, w, x, y - 1) &&
                    InBounds(x - dx, y - 1, w, h) && Blocked(grid, w, x - dx, y - 1)) return true;
                if (InBounds(x, y + 1, w, h) && !Blocked(grid, w, x, y + 1) &&
                    InBounds(x - dx, y + 1, w, h) && Blocked(grid, w, x - dx, y + 1)) return true;
            }
            else
            {
                if (InBounds(x - 1, y, w, h) && !Blocked(grid, w, x - 1, y) &&
                    InBounds(x - 1, y - dy, w, h) && Blocked(grid, w, x - 1, y - dy)) return true;
                if (InBounds(x + 1, y, w, h) && !Blocked(grid, w, x + 1, y) &&
                    InBounds(x + 1, y - dy, w, h) && Blocked(grid, w, x + 1, y - dy)) return true;
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool InBounds(int x, int y, int w, int h)
            => (uint)x < (uint)w && (uint)y < (uint)h;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool Blocked(byte* grid, int w, int x, int y)
            => grid[y * w + x] != 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int Heuristic(int x, int y, int gx, int gy)
            => Math.Abs(x - gx) + Math.Abs(y - gy);

        private static void Push(int* heapN, int* heapF, ref int hs, int node, int f)
        {
            int i = hs++;
            heapN[i] = node; heapF[i] = f;
            while (i > 0)
            {
                int p = (i - 1) >> 1;
                if (heapF[p] <= heapF[i]) break;
                int tn = heapN[p]; heapN[p] = heapN[i]; heapN[i] = tn;
                int tf = heapF[p]; heapF[p] = heapF[i]; heapF[i] = tf;
                i = p;
            }
        }

        private static int Pop(int* heapN, int* heapF, ref int hs)
        {
            int r = heapN[0];
            hs--;
            heapN[0] = heapN[hs]; heapF[0] = heapF[hs];
            int i = 0;
            while (true)
            {
                int l = i * 2 + 1, ri = l + 1, s = i;
                if (l < hs && heapF[l] < heapF[s]) s = l;
                if (ri < hs && heapF[ri] < heapF[s]) s = ri;
                if (s == i) break;
                int tn = heapN[s]; heapN[s] = heapN[i]; heapN[i] = tn;
                int tf = heapF[s]; heapF[s] = heapF[i]; heapF[i] = tf;
                i = s;
            }
            return r;
        }
    }
}
