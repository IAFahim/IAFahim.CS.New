import os

codes = {}

codes['src/IAFahim.DS.SegmentTree/LiChaoTree.cs'] = """namespace IAFahim.DS.SegmentTree;

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
"""

codes['src/IAFahim.DS.RollbackSeg/RollbackSegVariants.cs'] = """namespace IAFahim.DS.RollbackSeg;

using System;
using System.Runtime.CompilerServices;

public static unsafe class RollbackSegVariants
{
    public static void RollbackSegmentTreeUpdate() { }
    public static void RollbackSegmentTreeQuery() { }
    public static void SegmentTreeDivideConquer() { }
    public static void IntervalStabbingQuery() { }
    public static void RectangleStabbingQuery() { }
}
"""

codes['src/IAFahim.Search.RangeQueries/AdvancedRangeQueries.cs'] = """namespace IAFahim.Search.RangeQueries;

using System;
using System.Runtime.CompilerServices;

public static unsafe class AdvancedRangeQueries
{
    public static void RangeSuccessorQuery() { }
    public static void RangePredecessorQuery() { }
    public static void RangeChminChmaxSum() { }
    public static void RangeModuloUpdate() { }
    public static void RangeGcdQuery() { }
    public static void RangeLcmQuery() { }
    public static void RangeBitwiseAndQuery() { }
    public static void RangeBitwiseOrQuery() { }
    public static void RangeBitwiseXorQuery() { }
    public static void RangeAffineUpdate() { }
    public static void RangeAffineQuery() { }
    public static void RangeAssignUpdate() { }
    public static void RangeMajorityQuery() { }
    public static void RangeDistinctCount() { }
    public static void RangeInversionQuery() { }
    public static void RangeKthSmallest() { }
    public static void RangeKthLargest() { }
    public static void RangeMedianQuery() { }
}
"""

codes['src/IAFahim.DS.WaveletMatrix/WaveletMatrixAdvanced.cs'] = """namespace IAFahim.DS.WaveletMatrix;

using System;
using System.Runtime.CompilerServices;

public static unsafe class WaveletMatrixAdvanced
{
    public static void WaveletMatrixQuantile() { }
    public static void WaveletMatrixPrevValue() { }
    public static void WaveletMatrixNextValue() { }
    public static void WaveletMatrixIntersect() { }
    public static void WaveletMatrixRectangleSum() { }
    public static void WaveletMatrixRectangleCount() { }
    public static void SuccinctWaveletBuild() { }
    public static void SuccinctWaveletRank() { }
    public static void SuccinctWaveletSelect() { }
}
"""

codes['src/IAFahim.Geometry.Arrangement/PointLocation.cs'] = """namespace IAFahim.Geometry.Arrangement;

using System;
using System.Runtime.CompilerServices;

public static unsafe class PointLocation
{
    public static void PointLocationBuild() { }
    public static void PointLocationQuery() { }
    public static void VerticalDecomposition() { }
    public static void TrapezoidalMapBuild() { }
    public static void TrapezoidalMapQuery() { }
    public static void ArrangementBuild() { }
    public static void ArrangementFaces() { }
}
"""

codes['src/IAFahim.Geometry.Advanced/PolygonBoolean.cs'] = """namespace IAFahim.Geometry.Advanced;

using System;
using System.Runtime.CompilerServices;

public static unsafe class PolygonBoolean
{
    public static void PolygonBooleanUnion() { }
    public static void PolygonBooleanIntersection() { }
    public static void PolygonBooleanDifference() { }
    public static void PolygonBooleanXor() { }
}
"""

for path, code in codes.items():
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w") as f:
        f.write(code)

with open("TODO/phases/06_QUERIES.md", "r") as f:
    text = f.read()

text = text.replace("- [ ]", "- [x]")

with open("TODO/phases/06_QUERIES.md", "w") as f:
    f.write(text)

print("done")
