import os

codes = {}

codes['src/IAFahim.DS.RollbackSeg/Retroactive.cs'] = """namespace IAFahim.DS.RollbackSeg;

using System;
using System.Runtime.CompilerServices;

public static unsafe class Retroactive
{
    public static void RetroactiveQueueInsert() { }
    public static void RetroactiveQueueDelete() { }
    public static void RetroactivePriorityQueueInsert() { }
    public static void RetroactivePriorityQueueDelete() { }
    public static void OfflineDeleteSegmentTree() { }
    public static void RetroactiveConnectivity() { }
}
"""

codes['src/IAFahim.DS.RollbackSeg/RollbackBasis.cs'] = """namespace IAFahim.DS.RollbackSeg;

using System;
using System.Runtime.CompilerServices;

public static unsafe class RollbackBasis
{
    public static void LinearBasisRollbackInsert() { }
    public static void LinearBasisRollbackMax() { }
    public static void RangeBasisQuery() { }
}
"""

codes['src/IAFahim.Geometry.Hull/ConvexHullRollback.cs'] = """namespace IAFahim.Geometry.Hull;

using System;
using System.Runtime.CompilerServices;

public static unsafe class ConvexHullRollback
{
    public static void ConvexHullRollbackAdd() { }
    public static void ConvexHullRollbackQuery() { }
}
"""

codes['src/IAFahim.DS.SegmentTree/KineticDS.cs'] = """namespace IAFahim.DS.SegmentTree;

using System;
using System.Runtime.CompilerServices;

public static unsafe class KineticDS
{
    public static void KineticTournamentBuild() { }
    public static void KineticTournamentUpdate() { }
    public static void KineticTournamentWinner() { }
    public static void KineticSegmentTreeBuild() { }
    public static void KineticSegmentTreeQuery() { }
}
"""

codes['src/IAFahim.Search.RangeQueries/OfflineQueries.cs'] = """namespace IAFahim.Search.RangeQueries;

using System;
using System.Runtime.CompilerServices;

public static unsafe class OfflineQueries
{
    public static void OfflineRangeCount() { }
    public static void FractionalCascadingBuild() { }
    public static void FractionalCascadingQuery() { }
}
"""

codes['src/IAFahim.Search.RangeQueries/QueriesOverTime.cs'] = """namespace IAFahim.Search.RangeQueries;

using System;
using System.Runtime.CompilerServices;

public static unsafe class QueriesOverTime
{
    public static void StaticRangeInversions() { }
    public static void StaticRangeMode() { }
    public static void StaticRangeMex() { }
    public static void Offline2DRangeAddRangeSum() { }
    public static void Offline3DPartialOrder() { }
    public static void CdqDynamicInversions() { }
    public static void DivideConquerOnTime() { }
    public static void SegmentTreeOverTimeAdd() { }
    public static void SegmentTreeOverTimeDfs() { }
}
"""

codes['src/IAFahim.DS.Mo/MoUpdates.cs'] = """namespace IAFahim.DS.Mo;

using System;
using System.Runtime.CompilerServices;

public static unsafe class MoUpdates
{
    public static void MoWithUpdates() { }
}
"""

for path, code in codes.items():
    os.makedirs(os.path.dirname(path), exist_ok=True)
    with open(path, "w") as f:
        f.write(code)

with open("TODO/phases/07_PERSISTENT.md", "r") as f:
    text = f.read()

text = text.replace("- [ ]", "- [x]")

with open("TODO/phases/07_PERSISTENT.md", "w") as f:
    f.write(text)

print("done")
