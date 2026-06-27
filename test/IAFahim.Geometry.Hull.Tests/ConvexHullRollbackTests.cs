namespace IAFahim.Geometry.Hull.Tests
{
    using IAFahim.Geometry.Hull;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class ConvexHullRollbackTests
    {
        [Test]
        public void Add_Query_Rollback_MatchesBrute()
        {
            const int N = 60;
            Random rng = new Random(2024);
            int* px = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            int* py = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            int* count = (int*)Marshal.AllocHGlobal(sizeof(int));
            int* hullIdx = (int*)Marshal.AllocHGlobal(sizeof(int) * (N + 1));
            int* hullLen = (int*)Marshal.AllocHGlobal(sizeof(int));
            int* hist = (int*)Marshal.AllocHGlobal(sizeof(int) * (N + 1));
            int* top = (int*)Marshal.AllocHGlobal(sizeof(int));
            int* scratch = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            int* bpx = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            int* bpy = (int*)Marshal.AllocHGlobal(sizeof(int) * N);
            try
            {
                *count = 0; *hullLen = 0; *top = 0;
                int[] seqX = new int[N];
                int[] seqY = new int[N];
                for (int i = 0; i < N; i++) { seqX[i] = rng.Next(-1000, 1000); seqY[i] = rng.Next(-1000, 1000); }

                int[] checkpoints = new int[N];
                for (int i = 0; i < N; i++)
                {
                    checkpoints[i] = ConvexHullRollback.GetCheckpoint(top);
                    ConvexHullRollbackAdd.Run(px, py, count, hullIdx, hullLen, hist, top, scratch, seqX[i], seqY[i]);
                    for (int j = 0; j <= i; j++) { bpx[j] = seqX[j]; bpy[j] = seqY[j]; }
                    long[] dirs = { 1, 0, 0, 1, -1, 0, 0, -1, 7, 3, -5, 2 };
                    for (int d = 0; d < dirs.Length; d += 2)
                    {
                        long dx = dirs[d], dy = dirs[d + 1];
                        int fast = ConvexHullRollbackQuery.Run(px, py, hullIdx, *hullLen, dx, dy);
                        long best = long.MinValue; int brute = -1;
                        for (int j = 0; j <= i; j++)
                        {
                            long dot = dx * bpx[j] + dy * bpy[j];
                            if (dot > best) { best = dot; brute = j; }
                        }
                        Assert.AreEqual(best, dx * px[fast] + dy * py[fast], $"extreme dx={dx} dy={dy} after {i + 1} pts");
                        Assert.AreEqual(best, dx * bpx[brute] + dy * bpy[brute]);
                    }
                }

                for (int i = N - 1; i >= 1; i--)
                {
                    ConvexHullRollback.Run(px, py, count, hullIdx, hullLen, hist, top, scratch, checkpoints[i]);
                    Assert.AreEqual(i, *count, $"count after rollback to checkpoint {i}");
                    long dx = 3, dy = -7;
                    int fast = ConvexHullRollbackQuery.Run(px, py, hullIdx, *hullLen, dx, dy);
                    long best = long.MinValue;
                    for (int j = 0; j < i; j++) best = Math.Max(best, dx * seqX[j] + dy * seqY[j]);
                    Assert.AreEqual(best, dx * px[fast] + dy * py[fast], $"extreme after rollback to {i}");
                }
            }
            finally
            {
                Marshal.FreeHGlobal((nint)px);
                Marshal.FreeHGlobal((nint)py);
                Marshal.FreeHGlobal((nint)count);
                Marshal.FreeHGlobal((nint)hullIdx);
                Marshal.FreeHGlobal((nint)hullLen);
                Marshal.FreeHGlobal((nint)hist);
                Marshal.FreeHGlobal((nint)top);
                Marshal.FreeHGlobal((nint)scratch);
                Marshal.FreeHGlobal((nint)bpx);
                Marshal.FreeHGlobal((nint)bpy);
            }
        }
    }
}
