todo_4_N.md

106. IAFahim.Geometry.Hull.ConvexHull3D

Problem: Uses Marshal.AllocHGlobal and try/finally. Reason: Violates the "no
managed exception handling (try/finally)" and "algorithms take pointers (zero
allocation)" constraints. Algorithms cannot allocate memory on the heap. Fix:
Require caller to pass scratchFaces and scratchHead buffers.

public static int Build(double* xs, double* ys, double* zs, int n, Face* outFaces, Face* scratchFaces, int* scratchHead)
{
    // Remove Marshal.AllocHGlobal and try/finally.
    // Use scratchFaces and scratchHead directly.

107. IAFahim.Geometry.Hull.HalfSpaceIntersection

Problem: Uses Marshal.AllocHGlobal and try/finally for planes and q. Reason:
Algorithms must be zero-allocation and exception-free. Fix: Add HalfPlane*
scratchPlanes and int* scratchQ parameters.

public static int Run(double* nx, double* ny, double* d, int m, double* outX, double* outY, int* outSize, HalfPlane* scratchPlanes, int* scratchQ)

108. IAFahim.Geometry.Hull.StraightSkeleton

Problem: Uses Marshal.AllocHGlobal and try/finally for nodes. Reason: Zero
allocation constraint. Fix: Caller provides Node* scratchNodes.

public static int Build(double* xs, double* ys, int n, double* outX, double* outY, Node* scratchNodes)

109. IAFahim.Geometry.Voronoi.ShortestPath

Problem: Uses Marshal.AllocHGlobal and try/finally for 6 arrays. Reason: Zero
allocation constraint. Fix: Caller provides scratch memory or pre-allocated
pointers.

public static double Run(double* ox, double* oy, int n, int src, int dst, int* from, int* to, double* w, int e, double* dist, PQNode* pq, int* head, int* next, int* toEdge, double* weight)

110. IAFahim.Geometry.Voronoi.NearestNeighbor

Problem: FromVoronoi and Range use Marshal.AllocHGlobal and try/finally. Reason:
Zero allocation constraint. Fix: Pass PointIdx* pts and KDNode* nodes from
caller.

public static int FromVoronoi(double qx, double qy, double* xs, double* ys, int n, PointIdx* pts, KDNode* nodes)

111. IAFahim.Graph.Connectivity.FullyDynamicConnectivity

Problem: Heavy use of Marshal.AllocHGlobal and try/finally for events, sorted,
match, edges, history. Reason: Memory leak risk without proper unmanaged
containers; violates algorithm constraints. Fix: Flatten parameters and require
the caller to provide scratch space.

112. IAFahim.Graph.ShortestPath.ApspRepeatedSquaring

Problem: Uses Marshal.AllocHGlobal and try/finally for temp and current. Reason:
Zero allocation constraint. Fix: Ask caller for long* temp and long* current.

113. IAFahim.Graph.ShortestPath.MinimumCycleMean

Problem: Uses Marshal.AllocHGlobal and try/finally for dp. Reason: Zero
allocation constraint. Fix: Ask caller for long* dp.

public static double Run(int n, int m, int* eu, int* ev, long* ew, long* dp)

114. IAFahim.Graph.SpanningTrees.DilworthDecomposition

Problem: Uses Marshal.AllocHGlobal and try/finally for bool* tc. Reason: Zero
allocation constraint. Fix: Ask caller for bool* tc.

115. IAFahim.Graph.SpanningTrees.MaximumAntichain

Problem: Uses Marshal.AllocHGlobal and try/finally for bool* tc. Reason: Zero
allocation constraint. Fix: Ask caller for bool* tc.

116. IAFahim.Graph.SpanningTrees.TransitiveReductionDag

Problem: Uses Marshal.AllocHGlobal and try/finally for bool* tc. Reason: Zero
allocation constraint. Fix: Ask caller for bool* tc.

117. IAFahim.Graph.Flow.Flow (MinHeap)

Problem: MinHeap allocates Dist, V, Pos internally and requires explicit
.Dispose(). Used inside try/finally across all flow algorithms. Reason: Violates
structural separation. Algorithms shouldn't own memory structures. Fix: Convert
MinHeap to a purely unmanaged operational struct that accepts pointers
initialized by the caller.

public struct MinHeap
{
    public long* Dist;
    public int* V;
    public int* Pos;
    public int Size;
    // No constructor allocating memory. Caller sets pointers.
}

118. IAFahim.String.Automata.FiniteAutomaton

Problem: BuildDfa uses Marshal.AllocHGlobal. Reason: Algorithms cannot allocate.
Fix: Pass powerSet, visited, and queue.

119. IAFahim.String.Compress.GrammarCompress

Problem: Marshal.AllocHGlobal(len) for byte* work without try/finally (memory
leak on throw, though exceptions are disabled, it still allocates). Reason: Zero
allocation constraint. Fix: Pass byte* work.

120. IAFahim.String.Compress.Slp

Problem: Static _rules allocated via Marshal.AllocHGlobal in Build. Reason:
Global state + unmanaged allocation causes leaks and threading issues. Fix: Pass
Rule* rules to Build.

public static void Build(byte* s, int len, int maxRules, Rule* rules, ref int ruleCount)

121. IAFahim.String.Match.AhoCorasick

Problem: Static _st allocated via Marshal.AllocHGlobal. Reason: Static state
prevents concurrent use. Allocator violates constraints. Fix: Pass State* st and
maintain tree via caller.

122. IAFahim.String.Match.ApproximateMatch

Problem: LandauVishkin uses Marshal.AllocHGlobal for D, curr, prev. Reason: Zero
allocation constraint. Fix: Provide scratch buffers int* curr, int* prev.

123. IAFahim.String.Match.PatternMatch

Problem: Abelian and Parameterized use Marshal.AllocHGlobal. Reason: Zero
allocation constraint. Fix: Caller provides int* cntA, int* cntB, int* mapA,
int* mapB.

124. IAFahim.String.FMIndex.Bwt

Problem: Inverse uses Marshal.AllocHGlobal. Reason: Zero allocation constraint.
Fix: Caller provides int* temp, int* count, int* LF.

125. IAFahim.String.FMIndex.FMIndex

Problem: Uses static _occ allocated via Marshal.AllocHGlobal and a Dispose
method. Reason: Statefulness in static class. Fix: Pass int* occ arrays directly
into Count and Locate.

126. IAFahim.String.SuffixArray.SuffixArray

Problem: Build uses Marshal.AllocHGlobal and try/finally. Reason: Zero
allocation constraint. Fix: Pass int* rank, int* tmpSa, int* count, int*
tmpRank.

127. IAFahim.String.SuffixAutomaton.GeneralizedSam

Problem: intText allocated via Marshal.AllocHGlobal. Reason: Zero allocation
constraint. Fix: Pass int* intText.

128. IAFahim.String.SuffixAutomaton.KthSubstring

Problem: long* dp = (long*)Marshal.AllocHGlobal(...). Reason: Zero allocation
constraint. Fix: Pass long* dp.

129. IAFahim.String.SuffixAutomaton.LinkTree

Problem: int* stack = (int*)Marshal.AllocHGlobal(...). Reason: Zero allocation
constraint. Fix: Pass int* stack.

130. IAFahim.String.SuffixAutomaton.PersistentSam

Problem: Static _roots allocated. Reason: Global mutable state and allocation.
Fix: Pass int* roots from caller.

131. IAFahim.String.SuffixAutomaton.SuffixAutomaton

Problem: Static _st allocated. Reason: Global mutable state. Fix: Pass State*
st.

132. IAFahim.String.Enumeration

Problem: ShortestCommonSupersequence and ShortestAbsentSubsequence use
Marshal.AllocHGlobal with try/finally. Reason: Zero allocation constraint. Fix:
Pass dp, nextOcc, path from caller.

133. IAFahim.String.Probabilistic

Problem: FreivaldsMatrixVerify, SchwartzZippelTest, RandomizedMstVerify use
Marshal.AllocHGlobal with try/finally. Reason: Zero allocation constraint. Fix:
Caller provides scratch memory arrays.

134. IAFahim.Graph.GraphAdvanced

Problem: KargerSteinMinCut uses Random rng = new Random(42);. Reason: Violates
"no new on any class" constraint. Heap allocation. Fix: Use unmanaged PRNG
(e.g., uint seed).

public static void KargerSteinMinCut(int n, int m, int* u, int* v, int* bestCutU, int* bestCutV, int* bestCutCount, ref uint seed)
{
    // ...
    int e = (int)(seed % (uint)m); // after advancing seed
}

135. IAFahim.Math.Transform.PosetTransforms

Problem: MobiusTransform uses Marshal.AllocHGlobal and try/finally. Reason: Zero
allocation constraint. Fix: Caller provides long* mu.

136. IAFahim.Math.Transform.SubsetConvolutionRanked

Problem: Run uses Marshal.AllocHGlobal and try/finally. Reason: Zero allocation
constraint. Fix: Caller provides long* f, long* g, long* h.

137. IAFahim.Math.NT.MoebiusPrefix

Problem: Uses Marshal.AllocHGlobal and try/finally for mu, primes, isPrime.
Reason: Zero allocation constraint. Fix: Pass int* mu, int* primes, bool*
isPrime.

138. IAFahim.Math.NT.TotientPrefix

Problem: Uses Marshal.AllocHGlobal and try/finally. Reason: Zero allocation
constraint. Fix: Pass int* phi, int* primes, bool* isPrime.

139. IAFahim.Math.NT.LinearSieveMultiplicative

Problem: Uses Marshal.AllocHGlobal and try/finally. Reason: Zero allocation
constraint. Fix: Pass int* e, long* pk, bool* isPrime.

140. IAFahim.Search.ExactCover.ExactCover

Problem: SolveDlx uses Marshal.AllocHGlobal for dancing links array state and
wraps in try/finally. Reason: Zero allocation constraint. Fix: Pass L, R, U, D,
C, RowIdx, colSize buffers from the caller.
