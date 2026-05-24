namespace IAFahim.DS.Grid
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class GridBfs
    {
        public static int Run(int height, int width, int sr, int sc, int* dist, long* visited, int* queue)
        {
            int front = 0, rear = 0;
            queue[rear++] = sr * width + sc;
            visited[sr * width + sc] = 1;
            dist[sr * width + sc] = 0;
            int* dr = stackalloc int[4] { -1, 1, 0, 0 };
            int* dc = stackalloc int[4] { 0, 0, -1, 1 };
            while (front < rear)
            {
                int node = queue[front++];
                ProcessNeighbors(node / width, node % width, height, width, dist, visited, queue, ref rear, dr, dc);
            }
            return rear;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void ProcessNeighbors(int r, int c, int h, int w, int* dist, long* vis, int* q, ref int rear, int* dr, int* dc)
        {
            for (int d = 0; d < 4; d++)
            {
                int nr = r + dr[d], nc = c + dc[d];
                if ((uint)nr < (uint)h && (uint)nc < (uint)w)
                {
                    int idx = nr * w + nc;
                    if (vis[idx] == 0) { vis[idx] = 1; dist[idx] = dist[r * w + c] + 1; q[rear++] = idx; }
                }
            }
        }
    }

    public static unsafe class RotateGrid
    {
        public static void Run(int h, int w, long* src, long* dst, int times)
        {
            times = ((times % 4) + 4) % 4;
            if (times == 0) CopyGrid(h, w, src, dst);
            else if (times == 1) Rotate90(h, w, src, dst);
            else if (times == 2) Rotate180(h, w, src, dst);
            else Rotate270(h, w, src, dst);
        }

        private static void CopyGrid(int h, int w, long* src, long* dst)
        {
            for (int i = 0; i < h * w; i++) dst[i] = src[i];
        }

        private static void Rotate90(int h, int w, long* src, long* dst)
        {
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    dst[j * h + (h - 1 - i)] = src[i * w + j];
        }

        private static void Rotate180(int h, int w, long* src, long* dst)
        {
            int len = h * w;
            for (int i = 0; i < len; i++) dst[len - 1 - i] = src[i];
        }

        private static void Rotate270(int h, int w, long* src, long* dst)
        {
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    dst[(w - 1 - j) * h + i] = src[i * w + j];
        }
    }
}
