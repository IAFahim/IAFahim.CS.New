namespace IAFahim.Geometry.Arrangement
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class PointLocationBuild
    {
        public static int Run(int* xs, int* ys, int n, int* grid, int gridSize)
        {
            int minX = xs[0], maxX = xs[0], minY = ys[0], maxY = ys[0];
            for (int i = 1; i < n; i++)
            {
                if (xs[i] < minX) minX = xs[i];
                if (xs[i] > maxX) maxX = xs[i];
                if (ys[i] < minY) minY = ys[i];
                if (ys[i] > maxY) maxY = ys[i];
            }
            int cellW = (maxX - minX) / gridSize + 1;
            int cellH = (maxY - minY) / gridSize + 1;
            for (int i = 0; i < n; i++)
            {
                int cx = (xs[i] - minX) / cellW;
                int cy = (ys[i] - minY) / cellH;
                grid[cy * gridSize + cx]++;
            }
            return gridSize * gridSize;
        }

        public static void BuildKdTree(long* points, int* tree, int node, int l, int r, int depth)
        {
            if (l > r) return;
            int mid = (l + r) >> 1;
            int axis = depth & 1;
            for (int i = l; i <= r; i++)
            {
                int m = i;
                for (int j = i + 1; j <= r; j++)
                {
                    if (axis == 0 ? points[j * 2] < points[m * 2] : points[j * 2 + 1] < points[m * 2 + 1])
                        m = j;
                }
                if (m != i)
                {
                    long tx = points[i * 2], ty = points[i * 2 + 1];
                    points[i * 2] = points[m * 2]; points[i * 2 + 1] = points[m * 2 + 1];
                    points[m * 2] = tx; points[m * 2 + 1] = ty;
                }
            }
            tree[node] = mid;
            BuildKdTree(points, tree, node * 2, l, mid - 1, depth + 1);
            BuildKdTree(points, tree, node * 2 + 1, mid + 1, r, depth + 1);
        }
    }

    public static unsafe class PointLocationQuery
    {
        public static int Run(int* grid, int gridSize, int minX, int minY, int cellW, int cellH, int px, int py)
        {
            int cx = (px - minX) / cellW;
            int cy = (py - minY) / cellH;
            if (cx < 0 || cx >= gridSize || cy < 0 || cy >= gridSize) return -1;
            return grid[cy * gridSize + cx];
        }

        public static int QueryKdTree(long* points, int* tree, int node, int depth, long px, long py)
        {
            if (node == 0) return -1;
            int idx = tree[node];
            int axis = depth & 1;
            long val = axis == 0 ? points[idx * 2] : points[idx * 2 + 1];
            int child = px < val ? node * 2 : node * 2 + 1;
            int best = idx;
            long dist = (points[idx * 2] - px) * (points[idx * 2] - px) + (points[idx * 2 + 1] - py) * (points[idx * 2 + 1] - py);
            int next = QueryKdTree(points, tree, child, depth + 1, px, py);
            if (next >= 0)
            {
                long d = (points[next * 2] - px) * (points[next * 2] - px) + (points[next * 2 + 1] - py) * (points[next * 2 + 1] - py);
                if (d < dist) { best = next; dist = d; }
            }
            long diff = axis == 0 ? px - val : py - val;
            int other = axis == 0 ? node * 2 + 1 : node * 2;
            if (diff * diff <= dist)
            {
                int cand = QueryKdTree(points, tree, other, depth + 1, px, py);
                if (cand >= 0)
                {
                    long d = (points[cand * 2] - px) * (points[cand * 2] - px) + (points[cand * 2 + 1] - py) * (points[cand * 2 + 1] - py);
                    if (d < dist) best = cand;
                }
            }
            return best;
        }
    }

    public static unsafe class VerticalDecomposition
    {
        public static int Run(int* xs, int* ys, int n, int* outX, int* outY)
        {
            int* order = stackalloc int[n];
            for (int i = 0; i < n; i++) order[i] = i;
            for (int i = 1; i < n; i++)
            {
                int key = order[i], j = i - 1;
                while (j >= 0 && xs[order[j]] > xs[key]) { order[j + 1] = order[j]; j--; }
                order[j + 1] = key;
            }
            int idx = 0;
            for (int i = 0; i < n; i++)
            {
                outX[idx] = xs[order[i]];
                outY[idx++] = ys[order[i]];
            }
            return idx;
        }
    }

    public static unsafe class TrapezoidalMapBuild
    {
        public static int Run(int* segX1, int* segY1, int* segX2, int* segY2, int n, int* trapX1, int* trapY1, int* trapX2, int* trapY2)
        {
            int m = 0;
            for (int i = 0; i < n; i++)
            {
                int x1 = segX1[i], y1 = segY1[i], x2 = segX2[i], y2 = segY2[i];
                int loX = x1 < x2 ? x1 : x2;
                int hiX = x1 < x2 ? x2 : x1;
                trapX1[m] = loX;
                trapY1[m] = y1;
                trapX2[m] = hiX;
                trapY2[m] = y2;
                m++;
                trapX1[m] = loX;
                trapY1[m] = y2;
                trapX2[m] = hiX;
                trapY2[m] = y1;
                m++;
            }
            return m;
        }
    }

    public static unsafe class TrapezoidalMapQuery
    {
        public static int Run(int* trapX1, int* trapY1, int* trapX2, int* trapY2, int n, int px, int py)
        {
            for (int i = 0; i < n; i++)
            {
                if (trapX1[i] <= px && px <= trapX2[i])
                {
                    int yLow = trapY1[i] < trapY2[i] ? trapY1[i] : trapY2[i];
                    int yHigh = trapY1[i] < trapY2[i] ? trapY2[i] : trapY1[i];
                    if (yLow <= py && py <= yHigh) return i;
                }
            }
            return -1;
        }
    }

    public static unsafe class ArrangementBuild
    {
        public static int Run(int* segX1, int* segY1, int* segX2, int* segY2, int n, int* outX, int* outY, int* outAdj)
        {
            int m = 0;
            for (int i = 0; i < n; i++)
            {
                outX[m] = segX1[i];
                outY[m] = segY1[i];
                outAdj[m] = i + 1;
                m++;
                outX[m] = segX2[i];
                outY[m] = segY2[i];
                outAdj[m] = i + 1;
                m++;
            }
            return m;
        }
    }

    public static unsafe class ArrangementFaces
    {
        public static int Run(int n, int* head, int* to, int* next, int* visited, int* outFace)
        {
            int faceCount = 0;
            for (int i = 0; i < n; i++)
            {
                for (int e = head[i]; e != 0; e = next[e])
                {
                    if (visited[e] == 0)
                    {
                        visited[e] = 1;
                        visited[e ^ 1] = 1;
                        outFace[faceCount++] = i;
                        break;
                    }
                }
            }
            return faceCount;
        }
    }

    public static unsafe class PolygonBooleanUnion
    {
        public static int Run(int* x1, int* y1, int* x2, int* y2, int n, int* outX, int* outY)
        {
            int count = 0;
            for (int i = 0; i < n; i++)
            {
                int xLo = Math.Min(x1[i], x2[i]);
                int xHi = Math.Max(x1[i], x2[i]);
                int yLo = Math.Min(y1[i], y2[i]);
                int yHi = Math.Max(y1[i], y2[i]);
                outX[count] = xLo; outY[count] = yLo; count++;
                outX[count] = xLo; outY[count] = yHi; count++;
                outX[count] = xHi; outY[count] = yLo; count++;
                outX[count] = xHi; outY[count] = yHi; count++;
            }
            return count;
        }
    }

    public static unsafe class PolygonBooleanIntersection
    {
        public static int Run(int* x1, int* y1, int* x2, int* y2, int n, int* outX, int* outY)
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    int l = Math.Max(x1[i], x1[j]);
                    int r = Math.Min(x2[i], x2[j]);
                    int b = Math.Max(y1[i], y1[j]);
                    int t = Math.Min(y2[i], y2[j]);
                    if (l < r && b < t)
                    {
                        outX[0] = l; outY[0] = b;
                        outX[1] = l; outY[1] = t;
                        outX[2] = r; outY[2] = b;
                        outX[3] = r; outY[3] = t;
                        return 4;
                    }
                }
            }
            return 0;
        }
    }

    public static unsafe class PolygonBooleanDifference
    {
        public static int Run(int* x1, int* y1, int* x2, int* y2, int* outX, int* outY)
        {
            outX[0] = x1[0]; outY[0] = y1[0];
            outX[1] = x1[1]; outY[1] = y1[1];
            outX[2] = x1[0]; outY[2] = y1[1];
            outX[3] = x1[1]; outY[3] = y1[0];
            return 4;
        }
    }

    public static unsafe class PolygonBooleanXor
    {
        public static int Run(int* x1, int* y1, int* x2, int* y2, int* outX, int* outY)
        {
            outX[0] = Math.Min(x1[0], x2[0]);
            outY[0] = Math.Min(y1[0], y2[0]);
            outX[1] = Math.Min(x1[0], x2[0]);
            outY[1] = Math.Max(y1[1], y2[1]);
            outX[2] = Math.Max(x1[1], x2[1]);
            outY[2] = Math.Min(y1[0], y2[0]);
            outX[3] = Math.Max(x1[1], x2[1]);
            outY[3] = Math.Max(y1[1], y2[1]);
            return 4;
        }
    }
}