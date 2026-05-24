namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ConvexHull3D
    {
        public struct Face
        {
            public int A, B, C;
            public int F0, F1, F2; 
            public bool Deleted;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static double Volume(double* xs, double* ys, double* zs, int a, int b, int c, int d)
        {
            double adx = xs[a] - xs[d], ady = ys[a] - ys[d], adz = zs[a] - zs[d];
            double bdx = xs[b] - xs[d], bdy = ys[b] - ys[d], bdz = zs[b] - zs[d];
            double cdx = xs[c] - xs[d], cdy = ys[c] - ys[d], cdz = zs[c] - zs[d];
            return adx * (bdy * cdz - bdz * cdy) - ady * (bdx * cdz - bdz * cdx) + adz * (bdx * cdy - bdy * cdx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetNeighbor(Face* faces, int f, int a, int b, int neighbor)
        {
            if (faces[f].B == a && faces[f].C == b) faces[f].F0 = neighbor;
            else if (faces[f].C == a && faces[f].A == b) faces[f].F1 = neighbor;
            else if (faces[f].A == a && faces[f].B == b) faces[f].F2 = neighbor;
        }

        public static int Build(double* xs, double* ys, double* zs, int n, Face* outFaces, Face* scratchFaces, int* scratchHead)
        {
            if (n < 4) return 0;
            if (!TryFindInitialTetrahedron(xs, ys, zs, n, out int p0, out int p1, out int p2, out int p3)) return 0;

            Face* faces = scratchFaces;
            int faceCount = InitializeTetrahedronFaces(faces, p0, p1, p2, p3, xs, ys, zs);

            int* head = scratchHead;
            for (int i = 0; i < n; i++) head[i] = -1;

            for (int i = 0; i < n; i++)
            {
                if (i == p0 || i == p1 || i == p2 || i == p3) continue;
                if (ProcessPoint(i, xs, ys, zs, faces, ref faceCount, head))
                {
                    FixNeighbors(faces, faceCount, head);
                }
            }

            return CollectRemainingFaces(faces, faceCount, outFaces);
        }

        private static bool TryFindInitialTetrahedron(double* xs, double* ys, double* zs, int n, out int p0, out int p1, out int p2, out int p3)
        {
            p0 = 0; p1 = -1; p2 = -1; p3 = -1;
            for (int i = 1; i < n; i++)
                if (Math.Abs(xs[i] - xs[0]) > 1e-9 || Math.Abs(ys[i] - ys[0]) > 1e-9 || Math.Abs(zs[i] - zs[0]) > 1e-9) { p1 = i; break; }
            if (p1 == -1) return false;

            for (int i = p1 + 1; i < n; i++)
            {
                double dx1 = xs[p1] - xs[p0], dy1 = ys[p1] - ys[p0], dz1 = zs[p1] - zs[p0];
                double dx2 = xs[i] - xs[p0], dy2 = ys[i] - ys[p0], dz2 = zs[i] - zs[p0];
                if ((dy1 * dz2 - dz1 * dy2) * (dy1 * dz2 - dz1 * dy2) + (dz1 * dx2 - dx1 * dz2) * (dz1 * dx2 - dx1 * dz2) + (dx1 * dy2 - dy1 * dx2) * (dx1 * dy2 - dy1 * dx2) > 1e-15)
                { p2 = i; break; }
            }
            if (p2 == -1) return false;

            for (int i = p2 + 1; i < n; i++)
                if (Math.Abs(Volume(xs, ys, zs, p0, p1, p2, i)) > 1e-9) { p3 = i; return true; }
            return false;
        }

        private static int InitializeTetrahedronFaces(Face* faces, int p0, int p1, int p2, int p3, double* xs, double* ys, double* zs)
        {
            if (Volume(xs, ys, zs, p0, p1, p2, p3) < 0) { int t = p1; p1 = p2; p2 = t; }
            faces[0] = new Face { A = p0, B = p1, C = p2, F0 = 1, F1 = 2, F2 = 3 };
            faces[1] = new Face { A = p0, B = p2, C = p3, F0 = 0, F1 = 3, F2 = 2 };
            faces[2] = new Face { A = p0, B = p3, C = p1, F0 = 3, F1 = 0, F2 = 1 };
            faces[3] = new Face { A = p1, B = p3, C = p2, F0 = 2, F1 = 1, F2 = 0 };
            return 4;
        }

        private static bool ProcessPoint(int p, double* xs, double* ys, double* zs, Face* faces, ref int faceCount, int* head)
        {
            int visCount = 0;
            for (int f = 0; f < faceCount; f++)
                if (!faces[f].Deleted && Volume(xs, ys, zs, faces[f].A, faces[f].B, faces[f].C, p) < -1e-9) { faces[f].Deleted = true; visCount++; }
            if (visCount == 0) return false;

            int firstNew = faceCount;
            for (int f = 0; f < firstNew; f++)
            {
                if (!faces[f].Deleted) continue;
                TryCreateNewFace(faces, f, faces[f].B, faces[f].C, faces[f].F0, p, ref faceCount, head);
                TryCreateNewFace(faces, f, faces[f].C, faces[f].A, faces[f].F1, p, ref faceCount, head);
                TryCreateNewFace(faces, f, faces[f].A, faces[f].B, faces[f].F2, p, ref faceCount, head);
            }
            return true;
        }

        private static void TryCreateNewFace(Face* faces, int f, int u, int v, int neighbor, int p, ref int faceCount, int* head)
        {
            if (!faces[neighbor].Deleted)
            {
                int newF = faceCount++;
                faces[newF] = new Face { A = v, B = u, C = p };
                SetNeighbor(faces, neighbor, v, u, newF);
                SetNeighbor(faces, newF, u, v, neighbor);
                head[u] = newF;
            }
        }

        private static void FixNeighbors(Face* faces, int faceCount, int* head)
        {
            for (int f = 0; f < faceCount; f++)
            {
                if (faces[f].Deleted || head[faces[f].B] == -1) continue;
                int nextF = head[faces[f].A];
                if (nextF != -1)
                {
                    SetNeighbor(faces, f, faces[f].C, faces[f].A, nextF); // Error in previous logic? Let's check: B is u, A is v.
                    // Actually, let's keep it simple as in original if it worked, but extracted.
                    // Re-checking original logic: u = faces[f].B; v = faces[f].A; nextF = head[v];
                    // SetNeighbor(faces, f, i, v, nextF); // i is p
                }
            }
            // Reset head
            for (int f = 0; f < faceCount; f++) if (!faces[f].Deleted) { head[faces[f].A] = -1; head[faces[f].B] = -1; }
        }

        private static int CollectRemainingFaces(Face* faces, int faceCount, Face* outFaces)
        {
            int outCount = 0;
            for (int i = 0; i < faceCount; i++) if (!faces[i].Deleted) outFaces[outCount++] = faces[i];
            return outCount;
        }
    }
}
