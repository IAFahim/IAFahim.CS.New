namespace IAFahim.Search.RangeQueries.Tests
{
    using IAFahim.Search.RangeQueries;
    using System;
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class FractionalCascadingTests
    {
        [Test]
        public void Query_MatchesPerListBinarySearch_Random()
        {
            Random rng = new Random(2024);
            for (int t = 0; t < 100; t++)
            {
                int k = rng.Next(1, 6);
                int[][] lists = new int[k][];
                int total = 0;
                for (int i = 0; i < k; i++)
                {
                    int len = rng.Next(0, 20);
                    lists[i] = new int[len];
                    for (int j = 0; j < len; j++) lists[i][j] = rng.Next(0, 50);
                    Array.Sort(lists[i]);
                    total += len;
                }
                int* data = (int*)Marshal.AllocHGlobal(sizeof(int) * (total + 1));
                int* sizes = (int*)Marshal.AllocHGlobal(sizeof(int) * k);
                int cap = 4 * total + k * 4 + 16;
                int* merged = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
                int* aux = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
                int* orig = (int*)Marshal.AllocHGlobal(sizeof(int) * cap);
                int* offsets = (int*)Marshal.AllocHGlobal(sizeof(int) * (k + 1));
                int* outPos = (int*)Marshal.AllocHGlobal(sizeof(int) * (k + 1));
                try
                {
                    int idx = 0;
                    for (int i = 0; i < k; i++)
                    {
                        sizes[i] = lists[i].Length;
                        for (int j = 0; j < lists[i].Length; j++) data[idx++] = lists[i][j];
                    }
                    FractionalCascadingBuild.Run(data, sizes, k, merged, aux, orig, offsets);
                    for (int q = 0; q < 40; q++)
                    {
                        int key = rng.Next(-5, 55);
                        FractionalCascadingQuery.Run(merged, aux, orig, offsets, k, key, outPos);
                        for (int i = 0; i < k; i++)
                        {
                            int brute = 0;
                            for (int j = 0; j < lists[i].Length; j++) if (lists[i][j] < key) brute++;
                            Assert.AreEqual(brute, outPos[i], $"t={t} key={key} list={i}");
                        }
                    }
                }
                finally
                {
                    Marshal.FreeHGlobal((nint)data); Marshal.FreeHGlobal((nint)sizes);
                    Marshal.FreeHGlobal((nint)merged); Marshal.FreeHGlobal((nint)aux);
                    Marshal.FreeHGlobal((nint)orig); Marshal.FreeHGlobal((nint)offsets);
                    Marshal.FreeHGlobal((nint)outPos);
                }
            }
        }
    }
}
