import os

algo_list = [
    "BfsLayerGraph",
    "HopcroftKarpBfs",
    "HopcroftKarpDfs",
    "DinicCurrentArc",
    "IsapGapOptimization",
    "PushRelabelGap",
    "PushRelabelGlobalRelabel",
    "ExcessScalingMaxFlow",
    "DynamicTreeMaxFlow",
    "FlowWithVertexCapacities",
    "FlowWithEdgeDemands",
    "FlowRecoverLowerBound",
    "MinimumCutRecover",
    "MinimumSTCutAll",
    "MaximumClosureProjectSelection",
    "MaximumClosureFlow",
    "MinimumWeightClosure",
    "PicardQueyranneClosure",
    "MinCostFlowSsp",
    "MinCostFlowSpfa",
    "MinCostFlowDijkstra",
    "MinCostFlowPrimalDual",
    "MinCostFlowCancelCycle",
    "MinCostFlowCapacityScaling",
    "MinCostFlowCostScaling",
    "MinCostFlowNetworkSimplex"
]

template = """namespace IAFahim.Graph.Flow
{{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class {name}
    {{
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(int* head, int* to, int* next, int* cap, int n, int s, int t)
        {{
            // Stub implementation
        }}
    }}
}}
"""

os.makedirs("src/IAFahim.Graph.Flow", exist_ok=True)
for name in algo_list:
    with open(f"src/IAFahim.Graph.Flow/{name}.cs", "w") as f:
        f.write(template.format(name=name))

