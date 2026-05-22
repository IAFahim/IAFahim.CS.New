namespace IAFahim.Geometry.Hull
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

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

        public static int Build(double* xs, double* ys, double* zs, int n, Face* outFaces)
        {
            if (n < 4) return 0;
            Face* faces = (Face*)Marshal.AllocHGlobal(n * 4 * sizeof(Face));
            int faceCount = 0;
            try
            {
                int p0 = 0, p1 = 1, p2 = 2, p3 = 3;
                bool found = false;
                for (int i = 1; i < n; i++)
                {
                    if (Math.Abs(xs[i] - xs[0]) > 1e-9 || Math.Abs(ys[i] - ys[0]) > 1e-9 || Math.Abs(zs[i] - zs[0]) > 1e-9)
                    {
                        p1 = i; break;
                    }
                }
                for (int i = p1 + 1; i < n; i++)
                {
                    double dx1 = xs[p1] - xs[p0], dy1 = ys[p1] - ys[p0], dz1 = zs[p1] - zs[p0];
                    double dx2 = xs[i] - xs[p0], dy2 = ys[i] - ys[p0], dz2 = zs[i] - zs[p0];
                    double cx = dy1 * dz2 - dz1 * dy2;
                    double cy = dz1 * dx2 - dx1 * dz2;
                    double cz = dx1 * dy2 - dy1 * dx2;
                    if (cx * cx + cy * cy + cz * cz > 1e-15)
                    {
                        p2 = i; break;
                    }
                }
                for (int i = p2 + 1; i < n; i++)
                {
                    if (Math.Abs(Volume(xs, ys, zs, p0, p1, p2, i)) > 1e-9)
                    {
                        p3 = i; found = true; break;
                    }
                }
                if (!found) return 0;

                if (Volume(xs, ys, zs, p0, p1, p2, p3) < 0)
                {
                    int t = p1; p1 = p2; p2 = t;
                }

                faces[0] = new Face { A = p0, B = p1, C = p2, F0 = 1, F1 = 2, F2 = 3 };
                faces[1] = new Face { A = p0, B = p2, C = p3, F0 = 0, F1 = 3, F2 = 2 };
                faces[2] = new Face { A = p0, B = p3, C = p1, F0 = 3, F1 = 0, F2 = 1 };
                faces[3] = new Face { A = p1, B = p3, C = p2, F0 = 2, F1 = 1, F2 = 0 };
                faceCount = 4;

                int* head = (int*)Marshal.AllocHGlobal(n * sizeof(int));
                for (int i = 0; i < n; i++) head[i] = -1;

                for (int i = 0; i < n; i++)
                {
                    if (i == p0 || i == p1 || i == p2 || i == p3) continue;

                    int visCount = 0;
                    for (int f = 0; f < faceCount; f++)
                    {
                        if (faces[f].Deleted) continue;
                        if (Volume(xs, ys, zs, faces[f].A, faces[f].B, faces[f].C, i) < -1e-9)
                        {
                            faces[f].Deleted = true;
                            visCount++;
                        }
                    }
                    if (visCount == 0) continue;

                    int firstNew = faceCount;
                    for (int f = 0; f < firstNew; f++)
                    {
                        if (!faces[f].Deleted) continue;
                        
                        // Edge 1: B -> C (Neighbor F0)
                        int u = faces[f].B;
                        int v = faces[f].C;
                        int neighbor = faces[f].F0;
                        if (!faces[neighbor].Deleted)
                        {
                            int newF = faceCount++;
                            faces[newF] = new Face { A = v, B = u, C = i };
                            SetNeighbor(faces, neighbor, v, u, newF);
                            SetNeighbor(faces, newF, u, v, neighbor);
                            head[u] = newF;
                        }

                        // Edge 2: C -> A (Neighbor F1)
                        u = faces[f].C;
                        v = faces[f].A;
                        neighbor = faces[f].F1;
                        if (!faces[neighbor].Deleted)
                        {
                            int newF = faceCount++;
                            faces[newF] = new Face { A = v, B = u, C = i };
                            SetNeighbor(faces, neighbor, v, u, newF);
                            SetNeighbor(faces, newF, u, v, neighbor);
                            head[u] = newF;
                        }

                        // Edge 3: A -> B (Neighbor F2)
                        u = faces[f].A;
                        v = faces[f].B;
                        neighbor = faces[f].F2;
                        if (!faces[neighbor].Deleted)
                        {
                            int newF = faceCount++;
                            faces[newF] = new Face { A = v, B = u, C = i };
                            SetNeighbor(faces, neighbor, v, u, newF);
                            SetNeighbor(faces, newF, u, v, neighbor);
                            head[u] = newF;
                        }
                    }

                    for (int f = firstNew; f < faceCount; f++)
                    {
                        int u = faces[f].B; // C is i
                        int v = faces[f].A;
                        int nextF = head[v];
                        if (nextF != -1)
                        {
                            SetNeighbor(faces, f, i, v, nextF);
                            SetNeighbor(faces, nextF, v, i, f);
                        }
                    }
                }
                Marshal.FreeHGlobal((nint)head);

                int outCount = 0;
                for (int i = 0; i < faceCount; i++)
                {
                    if (!faces[i].Deleted)
                    {
                        outFaces[outCount++] = faces[i];
                    }
                }
                return outCount;
            }
            finally
            {
                Marshal.FreeHGlobal((nint)faces);
            }
        }
    }
}
