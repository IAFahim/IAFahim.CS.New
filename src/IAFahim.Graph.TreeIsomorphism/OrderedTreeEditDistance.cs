namespace IAFahim.Graph.TreeIsomorphism
{
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;

    public static unsafe class OrderedTreeEditDistance
    {
        // Zhang-Shasha ordered tree edit distance between two ordered rooted trees.
        // p1/p2 = parent arrays; the (single) root is the node with p[i] < 0, every other
        // node's p[i] is its parent (module convention, see RootedTreeCanonicalForm). Children
        // of a parent are ordered left-to-right by ascending node index, which fixes the sibling
        // order required for an *ordered* edit distance.
        //
        // Nodes carry no separate label channel in this data model (the structure-only convention
        // shared with TreeIsomorphismAhU / RootedTreeCanonicalForm), so every node compares equal:
        // relabel cost = 0, insert = 1, delete = 1. The result is the minimum number of node
        // insertions + deletions that turns tree 1 into tree 2 (0 iff they are isomorphic as
        // ordered trees).
        //
        // Returns the edit distance, or -1 when an input is not a single-rooted tree (no/multiple
        // roots, or a forest), since the algorithm is undefined on such inputs.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int* p1, int* p2, int n1, int n2)
        {
            // Empty-tree fast paths: distance is the size of the other tree (pure inserts/deletes).
            if (n1 <= 0 && n2 <= 0) return 0;
            if (n1 <= 0) return n2;
            if (n2 <= 0) return n1;

            // Per-tree scratch laid out as one contiguous block to keep allocation/free simple and
            // cache-friendly. For tree i (size n) we need:
            //   post      [n]     postorder index -> node id
            //   postOf    [n]     node id -> postorder index
            //   lmld      [n]     postorder index -> leftmost-leaf-descendant postorder index
            //   keyroots  [n]     list of LR keyroots (postorder indices), count tracked separately
            //   childStart[n+1]   CSR child offsets (scratch for postorder generation)
            //   children  [n]     CSR child ids (scratch)
            //   stackNode [n]     iterative-traversal stack (scratch)
            //   stackState[n]     iterative-traversal child cursor (scratch)
            // Plus a shared forest-distance DP table of size (n1+1)*(n2+1) ints.

            long fdSize = (long)(n1 + 1) * (n2 + 1);
            long t1Words = (long)n1 * 7 + (n1 + 1); // 7 n-sized + 1 (n+1)-sized
            long t2Words = (long)n2 * 7 + (n2 + 1);
            long totalWords = t1Words + t2Words + fdSize;

            int* block = (int*)Marshal.AllocHGlobal((nint)(totalWords * sizeof(int)));
            {
                int* cur = block;

                int* post1 = cur; cur += n1;
                int* postOf1 = cur; cur += n1;
                int* lmld1 = cur; cur += n1;
                int* keyroots1 = cur; cur += n1;
                int* childStart1 = cur; cur += (n1 + 1);
                int* children1 = cur; cur += n1;
                int* stackNode1 = cur; cur += n1;
                int* stackState1 = cur; cur += n1;

                int* post2 = cur; cur += n2;
                int* postOf2 = cur; cur += n2;
                int* lmld2 = cur; cur += n2;
                int* keyroots2 = cur; cur += n2;
                int* childStart2 = cur; cur += (n2 + 1);
                int* children2 = cur; cur += n2;
                int* stackNode2 = cur; cur += n2;
                int* stackState2 = cur; cur += n2;

                int* fd = cur; // (n1+1)*(n2+1)

                int kr1 = Preprocess(p1, n1, post1, postOf1, lmld1, keyroots1,
                    childStart1, children1, stackNode1, stackState1);
                if (kr1 < 0) { Marshal.FreeHGlobal((nint)block); return -1; }

                int kr2 = Preprocess(p2, n2, post2, postOf2, lmld2, keyroots2,
                    childStart2, children2, stackNode2, stackState2);
                if (kr2 < 0) { Marshal.FreeHGlobal((nint)block); return -1; }

                // The DP needs a persistent tree-distance table (allocated inside ZhangShasha) plus
                // the per-keyroot-pair forest-distance scratch 'fd' carved from this block.
                int result = ZhangShasha(
                    n1, post1, lmld1, keyroots1, kr1,
                    n2, post2, lmld2, keyroots2, kr2,
                    fd);
                Marshal.FreeHGlobal((nint)block);
                return result;
            }
        }

        // Builds postorder, postOf, leftmost-leaf-descendant table and the LR keyroot list for a
        // tree given as a parent array. Returns the number of keyroots, or -1 if the input is not
        // a single-rooted tree. childStart/children/stackNode/stackState are caller-provided
        // scratch (childStart sized n+1, the rest sized n).
        private static int Preprocess(
            int* p, int n,
            int* post, int* postOf, int* lmld, int* keyroots,
            int* childStart, int* children, int* stackNode, int* stackState)
        {
            // Locate the single root and validate every non-root parent index.
            int root = -1;
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par < 0)
                {
                    if (root >= 0) return -1; // multiple roots -> forest, not a tree.
                    root = i;
                }
                else if (par >= n)
                {
                    return -1; // out-of-range parent.
                }
            }
            if (root < 0) return -1; // no root.

            // CSR child layout. Children are kept in ascending node-index order (the ordered-tree
            // sibling order) because we scatter in increasing i.
            for (int i = 0; i <= n; i++) childStart[i] = 0;
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par >= 0) childStart[par + 1]++;
            }
            for (int i = 0; i < n; i++) childStart[i + 1] += childStart[i];
            // Scatter using a temporary cursor reusing stackNode as cursor scratch.
            int* cursor = stackNode;
            for (int i = 0; i < n; i++) cursor[i] = childStart[i];
            for (int i = 0; i < n; i++)
            {
                int par = p[i];
                if (par >= 0) children[cursor[par]++] = i;
            }

            // Iterative left-to-right postorder traversal. stackState[d] holds the index of the
            // next child of stackNode[d] to descend into (relative to that node's child block).
            int sp = 0;
            stackNode[sp] = root;
            stackState[sp] = childStart[root];
            int postIdx = 0;
            int visited = 0;
            while (sp >= 0)
            {
                int node = stackNode[sp];
                int end = childStart[node + 1];
                int next = stackState[sp];
                if (next < end)
                {
                    int child = children[next];
                    stackState[sp] = next + 1; // advance cursor for this node
                    sp++;
                    stackNode[sp] = child;
                    stackState[sp] = childStart[child];
                }
                else
                {
                    // All children done -> emit node in postorder.
                    post[postIdx] = node;
                    postOf[node] = postIdx;
                    postIdx++;
                    visited++;
                    sp--;
                }
            }
            if (visited != n) return -1; // disconnected (cycle or unreachable node) -> not a tree.

            // Leftmost-leaf descendant in postorder coordinates. For a leaf, lmld = its own
            // postorder index. For an internal node, lmld = lmld of its first (leftmost) child,
            // which in postorder is the leftmost-leaf of the whole subtree.
            for (int pi = 0; pi < n; pi++)
            {
                int node = post[pi];
                int s = childStart[node];
                int e = childStart[node + 1];
                if (s == e)
                {
                    lmld[pi] = pi; // leaf
                }
                else
                {
                    int firstChild = children[s];
                    lmld[pi] = lmld[postOf[firstChild]];
                }
            }

            // LR keyroots: a node is a keyroot iff it has no left sibling, i.e. it is the root or
            // the leftmost child of its parent. Equivalently, keyroots are nodes i for which there
            // is no j > i with lmld[j] == lmld[i]; we collect them by keeping, for each distinct
            // lmld value, the node with the largest postorder index.
            // Standard collection: iterate postorder, mark the last-seen index per lmld.
            // We reuse children[] is busy; use stackState as a "seen lmld -> postidx" map of size n
            // (postorder indices range 0..n-1, lmld values are valid postorder indices).
            int* lastForLmld = stackState; // size n scratch, free to reuse now
            for (int i = 0; i < n; i++) lastForLmld[i] = -1;
            for (int pi = 0; pi < n; pi++)
            {
                lastForLmld[lmld[pi]] = pi; // overwrite -> ends up as the max postidx per lmld
            }
            // Collect in ASCENDING POSTORDER ORDER: scan postorder indices and keep pi iff it is the
            // last (highest) postorder index sharing its lmld value. This guarantees children
            // keyroots precede their ancestor keyroots, the processing order the Zhang-Shasha outer
            // loops require so every td[pi][pj] is final before the else-branch reads it.
            int krCount = 0;
            for (int pi = 0; pi < n; pi++)
            {
                if (lastForLmld[lmld[pi]] == pi) keyroots[krCount++] = pi;
            }
            return krCount;
        }

        // Core Zhang-Shasha DP. treedist values are accumulated into a dedicated table; the forest
        // distance scratch 'fd' (sized (n1+1)*(n2+1)) is reused for every keyroot pair.
        private static int ZhangShasha(
            int n1, int* post1, int* lmld1, int* keyroots1, int kr1,
            int n2, int* post2, int* lmld2, int* keyroots2, int kr2,
            int* fd)
        {
            // Persistent tree-distance table between every (postorder i in T1, postorder j in T2).
            // Only filled at keyroot crossings but read back by later (outer) keyroot pairs.
            long tdSize = (long)n1 * n2;
            int* td = (int*)Marshal.AllocHGlobal((nint)(tdSize * sizeof(int)));
            for (long t = 0; t < tdSize; t++) td[t] = 0;

            int fdW = n2 + 1; // row stride of fd

                for (int a = 0; a < kr1; a++)
                {
                    int i = keyroots1[a];     // postorder index of keyroot in T1
                    int li = lmld1[i];        // leftmost leaf of subtree i
                    for (int b = 0; b < kr2; b++)
                    {
                        int j = keyroots2[b];
                        int lj = lmld2[j];

                        int m = i - li + 1;   // # nodes in T1 forest [li..i]
                        int nn = j - lj + 1;  // # nodes in T2 forest [lj..j]

                        // Local forest-distance indices: 0..m, 0..nn (0 = empty forest).
                        // fd[x*fdW + y].
                        fd[0] = 0;
                        for (int x = 1; x <= m; x++) fd[x * fdW + 0] = fd[(x - 1) * fdW + 0] + 1; // delete
                        for (int y = 1; y <= nn; y++) fd[0 * fdW + y] = fd[0 * fdW + (y - 1)] + 1; // insert

                        for (int x = 1; x <= m; x++)
                        {
                            int pi = li + x - 1;       // postorder index in T1
                            for (int y = 1; y <= nn; y++)
                            {
                                int pj = lj + y - 1;   // postorder index in T2

                                int del = fd[(x - 1) * fdW + y] + 1;
                                int ins = fd[x * fdW + (y - 1)] + 1;

                                if (lmld1[pi] == li && lmld2[pj] == lj)
                                {
                                    // Both pi and pj are roots of their subtrees within this pair:
                                    // matching cost is the relabel cost (0 here, structure-only).
                                    int match = fd[(x - 1) * fdW + (y - 1)] + 0;
                                    int best = del < ins ? del : ins;
                                    if (match < best) best = match;
                                    fd[x * fdW + y] = best;
                                    td[pi * n2 + pj] = best; // record final subtree distance
                                }
                                else
                                {
                                    // pi or pj is an internal forest node: combine the forest
                                    // distance up to their leftmost leaves with the already-computed
                                    // subtree distance td[pi][pj].
                                    int fi = lmld1[pi] - li;   // local index of (lmld(pi)-1)+1 ...
                                    int fj = lmld2[pj] - lj;
                                    int match = fd[fi * fdW + fj] + td[pi * n2 + pj];
                                    int best = del < ins ? del : ins;
                                    if (match < best) best = match;
                                    fd[x * fdW + y] = best;
                                }
                            }
                        }
                    }
                }

                // Roots are the last node in each postorder (postorder index n-1).
                int result = td[(n1 - 1) * n2 + (n2 - 1)];
                Marshal.FreeHGlobal((nint)td);
                return result;
        }
    }
}
