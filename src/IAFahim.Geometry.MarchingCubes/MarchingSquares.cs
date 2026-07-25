namespace IAFahim.Geometry.MarchingCubes
{
    using System.Runtime.CompilerServices;

    public static unsafe class MarchingSquares
    {
        // Isoline with linear edge interpolation. outSegs: (x0,y0,x1,y1) per segment.
        public static int Contour(
            float* values, int width, int height, float level,
            float* outSegs, int outCap)
        {
            if (width < 2 || height < 2 || outCap <= 0) return 0;
            int count = 0;
            for (int y = 0; y < height - 1; y++)
            {
                for (int x = 0; x < width - 1; x++)
                {
                    float v00 = values[y * width + x];
                    float v10 = values[y * width + x + 1];
                    float v01 = values[(y + 1) * width + x];
                    float v11 = values[(y + 1) * width + x + 1];
                    int code = 0;
                    if (v00 >= level) code |= 1;
                    if (v10 >= level) code |= 2;
                    if (v11 >= level) code |= 4;
                    if (v01 >= level) code |= 8;
                    if (code == 0 || code == 15) continue;

                    float ax, ay, bx, by, cx, cy, dx, dy;
                    LerpEdge(x, y, x + 1, y, v00, v10, level, out ax, out ay);
                    LerpEdge(x + 1, y, x + 1, y + 1, v10, v11, level, out bx, out by);
                    LerpEdge(x, y + 1, x + 1, y + 1, v01, v11, level, out cx, out cy);
                    LerpEdge(x, y, x, y + 1, v00, v01, level, out dx, out dy);
                    EmitCase(code, ax, ay, bx, by, cx, cy, dx, dy, outSegs, outCap, ref count);
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void LerpEdge(
            float x0, float y0, float x1, float y1, float v0, float v1, float level,
            out float px, out float py)
        {
            float denom = v1 - v0;
            float t = System.Math.Abs(denom) < 1e-20f ? 0.5f : (level - v0) / denom;
            if (t < 0f) t = 0f;
            if (t > 1f) t = 1f;
            px = x0 + t * (x1 - x0);
            py = y0 + t * (y1 - y0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Emit(
            float x0, float y0, float x1, float y1, float* outSegs, int outCap, ref int count)
        {
            if (count >= outCap) return;
            int o = count * 4;
            outSegs[o] = x0; outSegs[o + 1] = y0; outSegs[o + 2] = x1; outSegs[o + 3] = y1;
            count++;
        }

        private static void EmitCase(
            int code,
            float ax, float ay, float bx, float by, float cx, float cy, float dx, float dy,
            float* outSegs, int outCap, ref int count)
        {
            switch (code)
            {
                case 1: case 14: Emit(dx, dy, ax, ay, outSegs, outCap, ref count); break;
                case 2: case 13: Emit(ax, ay, bx, by, outSegs, outCap, ref count); break;
                case 3: case 12: Emit(dx, dy, bx, by, outSegs, outCap, ref count); break;
                case 4: case 11: Emit(bx, by, cx, cy, outSegs, outCap, ref count); break;
                case 5:
                    Emit(dx, dy, ax, ay, outSegs, outCap, ref count);
                    Emit(bx, by, cx, cy, outSegs, outCap, ref count);
                    break;
                case 6: case 9: Emit(ax, ay, cx, cy, outSegs, outCap, ref count); break;
                case 7: case 8: Emit(dx, dy, cx, cy, outSegs, outCap, ref count); break;
                case 10:
                    Emit(ax, ay, bx, by, outSegs, outCap, ref count);
                    Emit(dx, dy, cx, cy, outSegs, outCap, ref count);
                    break;
            }
        }
    }

    // Full 256-config single-cube Marching Cubes (Lorensen & Cline) with linear edge interp.
    // Tables: MarchingCubesTables (public-domain lineage via Paul Bourke / fogleman/mc).
    public static unsafe class MarchingCubes
    {
        // values[8] corners: 000,100,110,010,001,101,111,011
        // outTris: 9 floats per triangle (xyz*3). Returns triangle count.
        public static int PolygonizeCube(float* values, float level, float* outTris, int outCap)
        {
            int cubeIndex = 0;
            for (int i = 0; i < 8; i++)
                if (values[i] >= level) cubeIndex |= 1 << i;

            int edges = MarchingCubesTables.EdgeTable[cubeIndex];
            if (edges == 0) return 0;

            float* verts = stackalloc float[36];
            for (int e = 0; e < 12; e++)
            {
                if ((edges & (1 << e)) == 0) continue;
                int a = EdgeA(e);
                int b = EdgeB(e);
                float ax, ay, az, bx, by, bz;
                Corner(a, out ax, out ay, out az);
                Corner(b, out bx, out by, out bz);
                Lerp(ax, ay, az, values[a], bx, by, bz, values[b], level,
                    out verts[e * 3], out verts[e * 3 + 1], out verts[e * 3 + 2]);
            }

            int baseIdx = cubeIndex * 16;
            int triCount = 0;
            for (int i = 0; MarchingCubesTables.TriTable[baseIdx + i] != -1; i += 3)
            {
                if (triCount >= outCap) break;
                int e0 = MarchingCubesTables.TriTable[baseIdx + i];
                int e1 = MarchingCubesTables.TriTable[baseIdx + i + 1];
                int e2 = MarchingCubesTables.TriTable[baseIdx + i + 2];
                int o = triCount * 9;
                outTris[o] = verts[e0 * 3];
                outTris[o + 1] = verts[e0 * 3 + 1];
                outTris[o + 2] = verts[e0 * 3 + 2];
                outTris[o + 3] = verts[e1 * 3];
                outTris[o + 4] = verts[e1 * 3 + 1];
                outTris[o + 5] = verts[e1 * 3 + 2];
                outTris[o + 6] = verts[e2 * 3];
                outTris[o + 7] = verts[e2 * 3 + 1];
                outTris[o + 8] = verts[e2 * 3 + 2];
                triCount++;
            }
            return triCount;
        }

        // Expected triangle count for cubeIndex (from full table) — for tests / validation.
        public static int TriangleCount(int cubeIndex)
        {
            if ((uint)cubeIndex > 255u) return 0;
            if (MarchingCubesTables.EdgeTable[cubeIndex] == 0) return 0;
            int baseIdx = cubeIndex * 16;
            int n = 0;
            for (int i = 0; MarchingCubesTables.TriTable[baseIdx + i] != -1; i += 3) n++;
            return n;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Corner(int i, out float x, out float y, out float z)
        {
            x = (i == 1 || i == 2 || i == 5 || i == 6) ? 1f : 0f;
            y = (i == 2 || i == 3 || i == 6 || i == 7) ? 1f : 0f;
            z = (i >= 4) ? 1f : 0f;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EdgeA(int e)
        {
            switch (e)
            {
                case 0: return 0; case 1: return 1; case 2: return 2; case 3: return 3;
                case 4: return 4; case 5: return 5; case 6: return 6; case 7: return 7;
                case 8: return 0; case 9: return 1; case 10: return 2; default: return 3;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EdgeB(int e)
        {
            switch (e)
            {
                case 0: return 1; case 1: return 2; case 2: return 3; case 3: return 0;
                case 4: return 5; case 5: return 6; case 6: return 7; case 7: return 4;
                case 8: return 4; case 9: return 5; case 10: return 6; default: return 7;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Lerp(
            float x0, float y0, float z0, float v0,
            float x1, float y1, float z1, float v1,
            float level, out float x, out float y, out float z)
        {
            float denom = v1 - v0;
            float t = System.Math.Abs(denom) < 1e-20f ? 0.5f : (level - v0) / denom;
            if (t < 0f) t = 0f;
            if (t > 1f) t = 1f;
            x = x0 + t * (x1 - x0);
            y = y0 + t * (y1 - y0);
            z = z0 + t * (z1 - z0);
        }
    }
}
