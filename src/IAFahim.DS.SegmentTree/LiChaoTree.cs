namespace IAFahim.DS.SegmentTree;

using System;
using System.Runtime.CompilerServices;

public static unsafe class LiChaoTree
{
    public struct Line
    {
        public long M, C;
        public long Eval(long x) => M * x + C;
    }

    public static void PersistentLiChaoAdd() { }
    public static void PersistentLiChaoQuery() { }
    public static void DynamicLiChaoRollback() { }
    public static void LiChaoRollback() { }
    public static void DivideConquerHullOptimization() { }
    public static void OnlineChtAdd() { }
    public static void OnlineChtQuery() { }
}
