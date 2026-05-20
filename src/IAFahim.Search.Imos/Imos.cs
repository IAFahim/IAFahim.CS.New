namespace IAFahim.Search.Imos
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ImosRectangle
    {
        public static void Add(int height, int width, long* diff, int r1, int c1, int r2, int c2, long val)
        {
            if ((uint)r1 >= (uint)height || (uint)c1 >= (uint)width) return;
            if (r2 > height) r2 = height;
            if (c2 > width) c2 = width;
            diff[r1 * width + c1] += val;
            if (r2 < height && c2 < width) diff[r2 * width + c2] += val;
            if (c2 < width) diff[r1 * width + c2] -= val;
            if (r2 < height) diff[r2 * width + c1] -= val;
        }

        public static void Build(int height, int width, long* diff, long* res)
        {
            for (int i = 0; i < height; i++)
            {
                long rowAcc = 0;
                for (int j = 0; j < width; j++)
                {
                    rowAcc += diff[i * width + j];
                    res[i * width + j] = rowAcc;
                    if (i > 0) res[i * width + j] += res[(i - 1) * width + j];
                }
            }
        }
    }

    public static unsafe class LargestRectangleHistogram
    {
        public static long Run(int n, long* h, long* res)
        {
            long maxArea = 0;
            int* stack = stackalloc int[n];
            int top = 0;
            for (int i = 0; i <= n; i++)
            {
                long curHeight = (i < n) ? h[i] : 0;
                while (top > 0 && h[stack[top - 1]] >= curHeight)
                {
                    int height = (int)h[stack[--top]];
                    int width = (top == 0) ? i : i - stack[top - 1] - 1;
                    long area = (long)height * width;
                    if (area > maxArea) maxArea = area;
                }
                stack[top++] = i;
            }
            *res = maxArea;
            return maxArea;
        }
    }

    public static unsafe class LargestRectangleGrid
    {
        public static long Run(int height, int width, long* grid, long* res)
        {
            long maxArea = 0;
            long* heights = stackalloc long[width];
            for (int i = 0; i < width; i++) heights[i] = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (grid[i * width + j] == 1)
                        heights[j] = 0;
                    else
                        heights[j]++;
                }
                int* stack = stackalloc int[width];
                int top = 0;
                for (int j = 0; j <= width; j++)
                {
                    long curHeight = (j < width) ? heights[j] : 0;
                    while (top > 0 && heights[stack[top - 1]] >= curHeight)
                    {
                        int h = (int)heights[stack[--top]];
                        int w = (top == 0) ? j : j - stack[top - 1] - 1;
                        long area = (long)h * w;
                        if (area > maxArea) maxArea = area;
                    }
                    stack[top++] = j;
                }
            }
            *res = maxArea;
            return maxArea;
        }
    }

    public static unsafe class MaximalSquare
    {
        public static int Run(int height, int width, long* grid, int* res)
        {
            int maxSide = 0;
            int* dp = stackalloc int[width];
            for (int j = 0; j < width; j++) dp[j] = 0;
            for (int i = 0; i < height; i++)
            {
                int prev = 0;
                for (int j = 0; j < width; j++)
                {
                    int temp = dp[j];
                    if (grid[i * width + j] == 1)
                    {
                        dp[j] = Math.Min(Math.Min(dp[j], dp[j - 1 < 0 ? 0 : j - 1]), prev) + 1;
                        if (dp[j] > maxSide) maxSide = dp[j];
                    }
                    else
                    {
                        dp[j] = 0;
                    }
                    prev = temp;
                }
            }
            *res = maxSide;
            return maxSide;
        }
    }

    public static unsafe class ScanlineEvents
    {
        public static int Run(int n, long* xs, long* ys1, long* ys2, long* ys, long* res, long mod)
        {
            long* events = stackalloc long[n * 2];
            for (int i = 0; i < n; i++)
            {
                events[i * 2] = xs[i];
                events[i * 2 + 1] = ys1[i];
                events[i * 2 + n * 2] = xs[i] + 1;
                events[i * 2 + n * 2 + 1] = ys2[i];
            }
            int eventCount = n * 2;
            for (int i = 0; i < eventCount; i++) ys[i] = events[i * 2 + 1];
            for (int i = 0; i < eventCount; i++) res[i] = 0;
            return eventCount;
        }
    }

    public static unsafe class SweepLine
    {
        public static long Run(int n, long* xs1, long* ys1, long* xs2, long* ys2, long* res)
        {
            long* events = stackalloc long[n * 4];
            int eventCount = 0;
            for (int i = 0; i < n; i++)
            {
                events[eventCount++] = xs1[i];
                events[eventCount++] = ys1[i];
                events[eventCount++] = xs2[i];
                events[eventCount++] = ys2[i];
            }
            for (int i = 0; i < eventCount; i++) res[i] = 0;
            return 0;
        }
    }

    public static unsafe class IntervalUnion
    {
        public static int Run(int n, long* starts, long* ends, long* res)
        {
            for (int i = 0; i < n; i++)
            {
                res[i * 2] = starts[i];
                res[i * 2 + 1] = ends[i];
            }
            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    if (res[j * 2] < res[i * 2])
                    {
                        long tmp = res[i * 2]; res[i * 2] = res[j * 2]; res[j * 2] = tmp;
                        tmp = res[i * 2 + 1]; res[i * 2 + 1] = res[j * 2 + 1]; res[j * 2 + 1] = tmp;
                    }
                }
            }
            int count = 0;
            long curStart = res[0];
            long curEnd = res[1];
            for (int i = 1; i < n; i++)
            {
                long s = res[i * 2];
                long e = res[i * 2 + 1];
                if (s <= curEnd)
                {
                    if (e > curEnd) curEnd = e;
                }
                else
                {
                    res[count * 2] = curStart;
                    res[count * 2 + 1] = curEnd;
                    count++;
                    curStart = s;
                    curEnd = e;
                }
            }
            res[count * 2] = curStart;
            res[count * 2 + 1] = curEnd;
            count++;
            return count;
        }
    }
}