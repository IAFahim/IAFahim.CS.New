#!/bin/bash
cd /home/l/Github/IAFahim.CS.New
L=scratch/pinpoint.log; : > $L
dotnet build test/IAFahim.Graph.Tests/IAFahim.Graph.Tests.csproj --nologo -v q >/dev/null 2>&1
for m in Bfs_TwoNodes_Connected Toposort_Basic DsuComponents_Connected TarjanScc_Basic Bridges_Simple ArticulationPoints_Linear Dijkstra_TwoNodes Mst_Kruskal_Basic BellmanFord_NegativeCycle \
         Solve_XOrNotX_Unsatisfiable Solve_ThreeVariablesContradiction_Unsatisfiable Solve_FiveClausesAllSatisfiable_Satisfiable Solve_LargeFormula_ConsistentAssignment Solve_EdgeCaseSingleVariableSelfImplication_Satisfiable; do
  timeout 20 dotnet test test/IAFahim.Graph.Tests/IAFahim.Graph.Tests.csproj --no-build --nologo -v q --filter "FullyQualifiedName~$m" >/dev/null 2>&1
  rc=$?
  if [ $rc -eq 124 ]; then echo "HANG  $m" >>$L; elif [ $rc -eq 0 ]; then echo "pass  $m" >>$L; else echo "FAIL  $m (rc=$rc)" >>$L; fi
done
echo "=== PINPOINT DONE ===" >>$L
