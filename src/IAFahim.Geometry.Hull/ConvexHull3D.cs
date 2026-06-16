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

            // outFaces is only written by CollectRemainingFaces at the very end, so during the
            // incremental build its storage is free to use as a transient per-round list of the
            // faces made visible by the current point. sizeof(Face) > sizeof(int), so it always
            // holds at least faceCount indices.
            int* visible = (int*)outFaces;

            for (int i = 0; i < n; i++)
            {
                if (i == p0 || i == p1 || i == p2 || i == p3) continue;
                if (ProcessPoint(i, xs, ys, zs, faces, ref faceCount, head, visible))
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

            double dx1 = xs[p1] - xs[p0], dy1 = ys[p1] - ys[p0], dz1 = zs[p1] - zs[p0];
            for (int i = p1 + 1; i < n; i++)
            {
                double dx2 = xs[i] - xs[p0], dy2 = ys[i] - ys[p0], dz2 = zs[i] - zs[p0];
                double cx = dy1 * dz2 - dz1 * dy2;
                double cy = dz1 * dx2 - dx1 * dz2;
                double cz = dx1 * dy2 - dy1 * dx2;
                if (cx * cx + cy * cy + cz * cz > 1e-15)
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
            // Neighbor slots follow the SetNeighbor convention: F0 = edge (B,C),
            // F1 = edge (C,A), F2 = edge (A,B). The face indices below are independent
            // of the p1/p2 swap above because they reference faces, not vertices.
            faces[0] = new Face { A = p0, B = p1, C = p2, F0 = 3, F1 = 1, F2 = 2 };
            faces[1] = new Face { A = p0, B = p2, C = p3, F0 = 3, F1 = 2, F2 = 0 };
            faces[2] = new Face { A = p0, B = p3, C = p1, F0 = 3, F1 = 0, F2 = 1 };
            faces[3] = new Face { A = p1, B = p3, C = p2, F0 = 1, F1 = 0, F2 = 2 };
            return 4;
        }

        private static bool ProcessPoint(int p, double* xs, double* ys, double* zs, Face* faces, ref int faceCount, int* head, int* visible)
        {
            int firstNew = faceCount;
            int visCount = 0;
            for (int f = 0; f < firstNew; f++)
                if (!faces[f].Deleted && Volume(xs, ys, zs, faces[f].A, faces[f].B, faces[f].C, p) < -1e-9)
                {
                    faces[f].Deleted = true;
                    visible[visCount++] = f;
                }
            if (visCount == 0) return false;

            // Build the horizon only from faces made visible by THIS point. Faces deleted in
            // earlier rounds remain flagged Deleted but must not be revisited, otherwise their
            // stale edges would spawn duplicate/unstitched faces.
            for (int k = 0; k < visCount; k++)
            {
                int f = visible[k];
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
                // The deleted face's forward edge is (u,v); the new face keeps that edge in the
                // same outward orientation as (A,B) = (u,v), with the new apex p as C.
                faces[newF] = new Face { A = u, B = v, C = p };
                // Surviving horizon face carries this shared edge reversed as (v,u); link it back.
                SetNeighbor(faces, neighbor, v, u, newF);
                // On the new face the shared horizon edge is (A,B) = (u,v) -> slot F2.
                faces[newF].F2 = neighbor;
                // Index this new face by its B vertex for sibling stitching in FixNeighbors.
                head[v] = newF;
            }
        }

        private static void FixNeighbors(Face* faces, int faceCount, int* head)
        {
            for (int f = 0; f < faceCount; f++)
            {
                // A genuine new face of this round is the unique face indexed by its own B vertex.
                if (faces[f].Deleted || head[faces[f].B] != f) continue;
                int nextF = head[faces[f].A];
                if (nextF != -1)
                {
                    // f shares directed edge (p, A) with the sibling new face nextF: that is f's
                    // (C,A) edge -> slot F1, and the sibling's (B,C) edge -> slot F0.
                    SetNeighbor(faces, f, faces[f].C, faces[f].A, nextF);
                    SetNeighbor(faces, nextF, faces[nextF].B, faces[nextF].C, f);
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
