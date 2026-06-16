namespace IAFahim.String.SuffixTree
{
    using System.Runtime.CompilerServices;

    public static unsafe class SuffixTreeUkkonen
    {
        public struct Node { public int Link; public int Start; public int Len; public int FirstEdge; }
        public struct Edge { public int To; public int Char; public int Next; public int Min; public int Max; }

        private const int Nil = -1;

        /// <summary>
        /// Builds a suffix tree for s[0..len) using Ukkonen's algorithm.
        ///
        /// Layout (unchecked by design — the caller guarantees s/nodes/edges are non-null and large
        /// enough: nodes needs up to 2*len, edges up to 2*len):
        ///   nodes[v].FirstEdge heads a singly-linked list of outgoing edges chained via Edge.Next.
        ///   nodes[v].Link is the suffix link. nodes[v].Start/Len describe the incoming edge label span.
        ///   edges[e] labels span s[Min..Max); Char is the first label character (for O(degree) lookup);
        ///   To is the child node. Leaf edges keep Max == len after construction.
        /// nodeCount/edgeCount/last are outputs; node 0 is the root. last receives the last leaf created.
        /// </summary>
        public static void Build(int* s, int len, Node* nodes, Edge* edges, ref int nodeCount, ref int edgeCount, ref int last)
        {
            nodeCount = 1; edgeCount = 0; last = 0;
            nodes[0].Start = -1; nodes[0].Len = 0; nodes[0].Link = Nil; nodes[0].FirstEdge = Nil;

            if (len <= 0) { return; }

            // Sentinel used as the open (growing) end of every leaf edge during construction.
            // Resolved to len once a leaf stops growing or at finalization.
            int openEnd = len + 1;

            int activeNode = 0;     // current active node
            int activeEdge = 0;     // index into s of the first character of the active edge (when activeLen > 0)
            int activeLen = 0;      // number of matched characters along the active edge from its start
            int remainder = 0;      // suffixes still to be inserted

            for (int pos = 0; pos < len; pos++)
            {
                int curChar = s[pos];
                remainder++;
                int lastInternal = Nil; // pending suffix-link target within this phase

                while (remainder > 0)
                {
                    if (activeLen == 0)
                    {
                        // The active point sits exactly on activeNode; the edge to take is the one
                        // beginning with the current character.
                        activeEdge = pos;
                    }

                    int curEdge = FindEdge(nodes, edges, activeNode, s[activeEdge]);

                    if (curEdge == Nil)
                    {
                        // No edge starts with s[activeEdge]: create a new leaf edge from activeNode.
                        NewLeaf(nodes, edges, ref nodeCount, ref edgeCount, activeNode, pos, openEnd, curChar);
                        last = edges[nodes[activeNode].FirstEdge].To;

                        if (lastInternal != Nil)
                        {
                            nodes[lastInternal].Link = activeNode;
                            lastInternal = Nil;
                        }
                    }
                    else
                    {
                        int edgeStart = edges[curEdge].Min;
                        int edgeLen = EdgeLength(edges, curEdge, len);

                        // Walk down: if activeLen reaches the current edge length, descend to its child
                        // and keep matching from there (skip/count trick).
                        if (activeLen >= edgeLen)
                        {
                            activeNode = edges[curEdge].To;
                            activeEdge += edgeLen;
                            activeLen -= edgeLen;
                            continue;
                        }

                        // Character on the edge at the active point.
                        if (s[edgeStart + activeLen] == curChar)
                        {
                            // Already present: extend the active point and stop this phase (rule 3).
                            activeLen++;
                            if (lastInternal != Nil)
                            {
                                nodes[lastInternal].Link = activeNode;
                                lastInternal = Nil;
                            }
                            break;
                        }

                        // Mismatch in the middle of the edge: split it with a new internal node.
                        int split = nodeCount++;
                        nodes[split].Start = edgeStart;
                        nodes[split].Len = activeLen;
                        nodes[split].Link = Nil;
                        nodes[split].FirstEdge = Nil;

                        // Lower half of the original edge becomes a child of the split node.
                        int lowerEdge = edgeCount++;
                        edges[lowerEdge].To = edges[curEdge].To;
                        edges[lowerEdge].Char = s[edgeStart + activeLen];
                        edges[lowerEdge].Min = edgeStart + activeLen;
                        edges[lowerEdge].Max = edges[curEdge].Max;
                        edges[lowerEdge].Next = Nil;
                        nodes[split].FirstEdge = lowerEdge;

                        // Shorten the original edge to the upper half and point it at the split node.
                        edges[curEdge].To = split;
                        edges[curEdge].Max = edgeStart + activeLen;

                        // New leaf for the mismatching current character, hung off the split node.
                        NewLeaf(nodes, edges, ref nodeCount, ref edgeCount, split, pos, openEnd, curChar);
                        last = edges[nodes[split].FirstEdge].To; // most recently prepended leaf

                        // Suffix link from the previously created internal node to this one.
                        if (lastInternal != Nil)
                        {
                            nodes[lastInternal].Link = split;
                        }
                        lastInternal = split;
                    }

                    remainder--;

                    if (activeNode == 0 && activeLen > 0)
                    {
                        activeLen--;
                        activeEdge = pos - remainder + 1;
                    }
                    else if (activeNode != 0)
                    {
                        activeNode = nodes[activeNode].Link != Nil ? nodes[activeNode].Link : 0;
                    }
                }
            }

            // Resolve every leaf's open end to len and finalize node span bookkeeping.
            for (int e = 0; e < edgeCount; e++)
            {
                if (edges[e].Max == openEnd)
                {
                    edges[e].Max = len;
                }
                int child = edges[e].To;
                nodes[child].Start = edges[e].Min;
                nodes[child].Len = edges[e].Max - edges[e].Min;
            }
        }

        /// <summary>Finds the outgoing edge of node whose first character equals c, or Nil.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int FindEdge(Node* nodes, Edge* edges, int node, int c)
        {
            int e = nodes[node].FirstEdge;
            while (e != Nil)
            {
                if (edges[e].Char == c) { return e; }
                e = edges[e].Next;
            }
            return Nil;
        }

        /// <summary>Length of an edge's label, treating an open (still-growing) leaf end as len.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int EdgeLength(Edge* edges, int e, int len)
        {
            int max = edges[e].Max;
            if (max > len) { max = len; }
            return max - edges[e].Min;
        }

        /// <summary>
        /// Creates a new leaf node and an edge from parent labelled s[start..openEnd) (open end),
        /// prepended to the parent's edge list. Returns the new edge index.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int NewLeaf(Node* nodes, Edge* edges, ref int nodeCount, ref int edgeCount, int parent, int start, int openEnd, int firstChar)
        {
            int leafNode = nodeCount++;
            nodes[leafNode].Start = start;
            nodes[leafNode].Len = 0;
            nodes[leafNode].Link = Nil;
            nodes[leafNode].FirstEdge = Nil;

            int leafEdge = edgeCount++;
            edges[leafEdge].To = leafNode;
            edges[leafEdge].Char = firstChar;
            edges[leafEdge].Min = start;
            edges[leafEdge].Max = openEnd;
            edges[leafEdge].Next = nodes[parent].FirstEdge;
            nodes[parent].FirstEdge = leafEdge;
            return leafEdge;
        }
    }
}
