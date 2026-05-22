namespace IAFahim.Combinatorics.Generation;

using System;
using System.Collections.Generic;
using System.Linq;

public static class RandomStructures
{
    private static Random rnd = new Random();

    public static int[] RandomTreePrufer(int n)
    {
        if (n <= 2) return new int[0];
        int[] prufer = new int[n - 2];
        for (int i = 0; i < n - 2; i++) prufer[i] = rnd.Next(n);
        return prufer;
    }

    public static int[][] RandomGraphErdosRenyi(int n, double p)
    {
        var edges = new List<int[]>();
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (rnd.NextDouble() < p)
                {
                    edges.Add(new[] { i, j });
                }
            }
        }
        return edges.ToArray();
    }

    public static int[][] RandomDAG(int n, double p)
    {
        var edges = new List<int[]>();
        // Topologically sorted by index implicitly
        for (int i = 0; i < n; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                if (rnd.NextDouble() < p)
                {
                    edges.Add(new[] { i, j });
                }
            }
        }
        return edges.ToArray();
    }

    public static int[][] RandomConnectedGraph(int n, int m)
    {
        if (m < n - 1) throw new ArgumentException("m must be >= n - 1");
        if (m > (long)n * (n - 1) / 2) m = n * (n - 1) / 2;

        var edges = new HashSet<(int, int)>();
        int[] perm = Permutations.RandomPermutation(n);
        for (int i = 1; i < n; i++)
        {
            int u = perm[i];
            int v = perm[rnd.Next(i)];
            edges.Add((Math.Min(u, v), Math.Max(u, v)));
        }

        while (edges.Count < m)
        {
            int u = rnd.Next(n);
            int v = rnd.Next(n);
            if (u != v)
            {
                var edge = (Math.Min(u, v), Math.Max(u, v));
                edges.Add(edge);
            }
        }

        return edges.Select(e => new[] { e.Item1, e.Item2 }).ToArray();
    }

    public static int[][] RandomPlanarGraph(int n)
    {
        // Simple heuristic: just generate a tree for now. Generating uniformly random planar graph is very hard.
        return RandomConnectedGraph(n, n - 1);
    }

    public static int[][] RandomRegularGraph(int n, int d)
    {
        if (n * d % 2 != 0) throw new ArgumentException("n * d must be even");
        // Configuration model
        while (true)
        {
            int[] points = new int[n * d];
            for (int i = 0; i < n * d; i++) points[i] = i / d;
            for (int i = points.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (points[i], points[j]) = (points[j], points[i]);
            }

            var edges = new HashSet<(int, int)>();
            bool ok = true;
            for (int i = 0; i < n * d; i += 2)
            {
                int u = points[i];
                int v = points[i + 1];
                if (u == v) { ok = false; break; }
                var edge = (Math.Min(u, v), Math.Max(u, v));
                if (!edges.Add(edge)) { ok = false; break; }
            }
            if (ok) return edges.Select(e => new[] { e.Item1, e.Item2 }).ToArray();
        }
    }

    public static int[][] RandomBipartiteGraph(int n1, int n2, int m)
    {
        if (m > (long)n1 * n2) m = n1 * n2;
        var edges = new HashSet<(int, int)>();
        while (edges.Count < m)
        {
            int u = rnd.Next(n1);
            int v = rnd.Next(n2);
            edges.Add((u, v));
        }
        return edges.Select(e => new[] { e.Item1, e.Item2 }).ToArray();
    }

    public static string RandomTestcaseGenerate(string format)
    {
        return "Generated Test Case based on " + format;
    }
}
