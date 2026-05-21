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
            int levelSize = 1;
            int levelIdx = 0;
            while (front < rear)
            {
                int node = queue[front++];
                int r = node / width;
                int c = node % width;
                levelIdx++;
                if (levelIdx == levelSize)
                {
                    levelSize = rear - front;
                    levelIdx = 0;
                }
                for (int d = 0; d < 4; d++)
                {
                    int nr = r + dr[d];
                    int nc = c + dc[d];
                    if ((uint)nr < (uint)height && (uint)nc < (uint)width)
                    {
                        int idx = nr * width + nc;
                        if (visited[idx] == 0)
                        {
                            visited[idx] = 1;
                            dist[idx] = dist[r * width + c] + 1;
                            queue[rear++] = idx;
                        }
                    }
                }
            }
            return rear;
        }
    }

    public static unsafe class FloodFill
    {
        public static int Run(int height, int width, int sr, int sc, long target, long replacement, long* grid, int* stack, int maxStack)
        {
            int top = 0;
            stack[top++] = sr * width + sc;
            int count = 0;
            while (top > 0)
            {
                if (top >= maxStack)
                {
                    top--;
                    int node = stack[top];
                    int r = node / width;
                    int c = node % width;
                    if ((uint)r >= (uint)height || (uint)c >= (uint)width) continue;
                    int idx = r * width + c;
                    if (grid[idx] != target) continue;
                    grid[idx] = replacement;
                    count++;
                    if (top + 4 > maxStack) continue;
                    stack[top++] = (r - 1) * width + c;
                    stack[top++] = (r + 1) * width + c;
                    stack[top++] = r * width + (c - 1);
                    stack[top++] = r * width + (c + 1);
                }
                else
                {
                    int node = stack[--top];
                    int r = node / width;
                    int c = node % width;
                    if ((uint)r >= (uint)height || (uint)c >= (uint)width) continue;
                    int idx = r * width + c;
                    if (grid[idx] != target) continue;
                    grid[idx] = replacement;
                    count++;
                    if (top + 4 > maxStack) continue;
                    stack[top++] = (r - 1) * width + c;
                    stack[top++] = (r + 1) * width + c;
                    stack[top++] = r * width + (c - 1);
                    stack[top++] = r * width + (c + 1);
                }
            }
            return count;
        }
    }

    public static unsafe class IsInsideGrid
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Run(int r, int c, int height, int width)
        {
            return (uint)r < (uint)height && (uint)c < (uint)width;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RunFlat(int idx, int height, int width)
        {
            return (uint)idx < (uint)(height * width);
        }
    }

    public static unsafe class RotateGrid
    {
        public static void Run(int height, int width, long* src, long* dst, int times)
        {
            times = ((times % 4) + 4) % 4;
            if (times == 0)
            {
                for (int i = 0; i < height * width; i++) dst[i] = src[i];
                return;
            }
            int len = height * width;
            long* temp = stackalloc long[len];
            long* cur = src;
            long* next = dst;
            int h = height, w = width;
            for (int r = 0; r < times; r++)
            {
                for (int i = 0; i < h; i++)
                    for (int j = 0; j < w; j++)
                        next[j * h + (h - 1 - i)] = cur[i * w + j];
                for (int i = 0; i < len; i++) temp[i] = cur[i];
                long* swap = cur;
                cur = next;
                next = temp;
                int tmp = h;
                h = w;
                w = tmp;
            }
            if (cur != dst)
                for (int i = 0; i < len; i++) dst[i] = cur[i];
        }
    }

    public static unsafe class TransposeGrid
    {
        public static void Run(int height, int width, long* src, long* dst)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    dst[j * height + i] = src[i * width + j];
        }
    }

    public static unsafe class Prefix2D
    {
        public static void Build(int height, int width, long* grid, long* prefix)
        {
            for (int i = 0; i < height; i++)
            {
                long rowSum = 0;
                for (int j = 0; j < width; j++)
                {
                    rowSum += grid[i * width + j];
                    prefix[i * width + j] = rowSum;
                    if (i > 0) prefix[i * width + j] += prefix[(i - 1) * width + j];
                }
            }
        }

        public static long Query(int r1, int c1, int r2, int c2, long* prefix, int height, int width)
        {
            long res = prefix[r2 * width + c2];
            if (r1 > 0) res -= prefix[(r1 - 1) * width + c2];
            if (c1 > 0) res -= prefix[r2 * width + (c1 - 1)];
            if (r1 > 0 && c1 > 0) res += prefix[(r1 - 1) * width + (c1 - 1)];
            return res;
        }
    }
}