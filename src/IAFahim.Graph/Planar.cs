namespace IAFahim.Graph
{
    using System;
    using System.Runtime.CompilerServices;
    using IAFahim.Graph.Flow;

    public static unsafe class Planar
    {
        public static void GomoryHuBuild(int n, int m, int* head, int* to, int* next, int* cap, int* parent, int* weight)
        {
            for (int i = 0; i < n; i++)
            {
                parent[i] = 0;
                weight[i] = 0;
            }

            int maxEdgeIdx = 0;
            for (int i = 0; i < n; i++)
            {
                for (int e = head[i]; e != 0; e = next[e])
                {
                    if (e > maxEdgeIdx)
                    {
                        maxEdgeIdx = e;
                    }
                }
            }
            int flowSize = maxEdgeIdx + 2;

            int* flow = stackalloc int[flowSize];

            for (int i = 1; i < n; i++)
            {
                int s = i;
                int t = parent[i];

                for (int j = 0; j < flowSize; j++)
                {
                    flow[j] = 0;
                }

                long cutVal = DinicMaxFlow.Run(n, s, t, head, to, next, cap, flow);

                byte* visited = stackalloc byte[n];
                for (int j = 0; j < n; j++)
                {
                    visited[j] = 0;
                }

                int* queue = stackalloc int[n];
                int qHead = 0, qTail = 0;
                visited[s] = 1;
                queue[qTail++] = s;

                while (qHead < qTail)
                {
                    int u = queue[qHead++];
                    for (int e = head[u]; e != 0; e = next[e])
                    {
                        int v = to[e];
                        if (visited[v] == 0 && cap[e] - flow[e] > 0)
                        {
                            visited[v] = 1;
                            queue[qTail++] = v;
                        }
                    }
                }

                weight[i] = (int)cutVal;

                for (int j = i + 1; j < n; j++)
                {
                    if (parent[j] == t && visited[j] == 1)
                    {
                        parent[j] = s;
                    }
                }
            }
        }

        public static int GomoryHuQuery(int n, int* parent, int* weight, int u, int v)
        {
            int* queue = stackalloc int[n];
            int* edgeWeight = stackalloc int[n];
            byte* visited = stackalloc byte[n];
            for (int i = 0; i < n; i++)
            {
                visited[i] = 0;
                edgeWeight[i] = int.MaxValue;
            }

            int qHead = 0, qTail = 0;
            visited[u] = 1;
            queue[qTail++] = u;

            while (qHead < qTail)
            {
                int curr = queue[qHead++];
                if (curr == v)
                {
                    return edgeWeight[v];
                }

                if (curr > 0)
                {
                    int p = parent[curr];
                    if (visited[p] == 0)
                    {
                        visited[p] = 1;
                        edgeWeight[p] = Math.Min(edgeWeight[curr], weight[curr]);
                        queue[qTail++] = p;
                    }
                }
                for (int j = 1; j < n; j++)
                {
                    if (parent[j] == curr)
                    {
                        if (visited[j] == 0)
                        {
                            visited[j] = 1;
                            edgeWeight[j] = Math.Min(edgeWeight[curr], weight[j]);
                            queue[qTail++] = j;
                        }
                    }
                }
            }
            return 0;
        }

        public static bool SplittingOff(int n, int* m, int s, int* u, int* v, int* resultU, int* resultV, int* resultCount)
        {
            *resultCount = 0;
            int initialMin = GetMinConnectivity(n, *m, u, v, s);

            while (true)
            {
                int incidentCount = 0;
                int* incidentEdges = stackalloc int[*m];
                for (int i = 0; i < *m; i++)
                {
                    if (u[i] == s || v[i] == s)
                    {
                        incidentEdges[incidentCount++] = i;
                    }
                }

                if (incidentCount == 0)
                {
                    break;
                }
                if (incidentCount == 1)
                {
                    return false;
                }

                bool found = false;
                for (int i = 0; i < incidentCount; i++)
                {
                    for (int j = i + 1; j < incidentCount; j++)
                    {
                        int e1 = incidentEdges[i];
                        int e2 = incidentEdges[j];

                        int a = (u[e1] == s) ? v[e1] : u[e1];
                        int b = (u[e2] == s) ? v[e2] : u[e2];

                        int tempM = *m - 1;
                        int* tempU = stackalloc int[tempM];
                        int* tempV = stackalloc int[tempM];
                        int tIdx = 0;
                        for (int k = 0; k < *m; k++)
                        {
                            if (k != e1 && k != e2)
                            {
                                tempU[tIdx] = u[k];
                                tempV[tIdx] = v[k];
                                tIdx++;
                            }
                        }
                        tempU[tIdx] = a;
                        tempV[tIdx] = b;

                        int newMin = GetMinConnectivity(n, tempM, tempU, tempV, s);
                        if (newMin >= initialMin)
                        {
                            u[e1] = a;
                            v[e1] = b;
                            
                            u[e2] = u[*m - 1];
                            v[e2] = v[*m - 1];
                            (*m)--;

                            resultU[*resultCount] = a;
                            resultV[*resultCount] = b;
                            (*resultCount)++;

                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }

                if (!found)
                {
                    return false;
                }
            }
            return true;
        }

        private static int GetMinConnectivity(int n, int m, int* u, int* v, int s)
        {
            int minConn = 999999;
            int maxV = n;
            int* head = stackalloc int[maxV];
            int* to = stackalloc int[m * 2 + 2];
            int* next = stackalloc int[m * 2 + 2];
            int* cap = stackalloc int[m * 2 + 2];
            int* flow = stackalloc int[m * 2 + 2];

            for (int i = 0; i < maxV; i++) head[i] = -1;
            int edgeIdx = 0;
            for (int i = 0; i < m; i++)
            {
                int ui = u[i];
                int vi = v[i];
                to[edgeIdx] = vi; cap[edgeIdx] = 1; next[edgeIdx] = head[ui]; head[ui] = edgeIdx++;
                to[edgeIdx] = ui; cap[edgeIdx] = 1; next[edgeIdx] = head[vi]; head[vi] = edgeIdx++;
            }

            int activeCount = 0;
            int* activeNodes = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                if (i != s)
                {
                    activeNodes[activeCount++] = i;
                }
            }

            if (activeCount <= 1) return 0;

            for (int i = 0; i < activeCount; i++)
            {
                for (int j = i + 1; j < activeCount; j++)
                {
                    int src = activeNodes[i];
                    int dest = activeNodes[j];
                    for (int k = 0; k < edgeIdx; k++) flow[k] = 0;
                    long flowVal = DinicMaxFlow.Run(maxV, src, dest, head, to, next, cap, flow);
                    if (flowVal < minConn)
                    {
                        minConn = (int)flowVal;
                    }
                }
            }
            return minConn;
        }

        public static bool EarDecomposition(int n, int m, int* u, int* v, int* earEdges, int* earLengths, int* earCount)
        {
            *earCount = 0;
            byte* edgeUsed = stackalloc byte[m];
            byte* vertexVisited = stackalloc byte[n];
            for (int i = 0; i < m; i++) edgeUsed[i] = 0;
            for (int i = 0; i < n; i++) vertexVisited[i] = 0;

            int* parent = stackalloc int[n];
            int* parentEdge = stackalloc int[n];
            for (int i = 0; i < n; i++) { parent[i] = -1; parentEdge[i] = -1; }

            int cycleStart = -1, cycleEnd = -1, cycleEdge = -1;
            
            bool FindCycle(int curr, int p, int pe)
            {
                vertexVisited[curr] = 1;
                for (int e = 0; e < m; e++)
                {
                    if (u[e] != curr && v[e] != curr) continue;
                    if (e == pe) continue;
                    int neighbor = (u[e] == curr) ? v[e] : u[e];
                    if (vertexVisited[neighbor] == 1)
                    {
                        cycleStart = neighbor;
                        cycleEnd = curr;
                        cycleEdge = e;
                        return true;
                    }
                    else if (vertexVisited[neighbor] == 0)
                    {
                        parent[neighbor] = curr;
                        parentEdge[neighbor] = e;
                        if (FindCycle(neighbor, curr, e)) return true;
                    }
                }
                vertexVisited[curr] = 2;
                return false;
            }

            bool foundCycle = false;
            for (int i = 0; i < n; i++)
            {
                if (vertexVisited[i] == 0)
                {
                    if (FindCycle(i, -1, -1))
                    {
                        foundCycle = true;
                        break;
                    }
                }
            }

            if (!foundCycle)
            {
                return false;
            }

            for (int i = 0; i < n; i++) vertexVisited[i] = 0;

            int currCycleNode = cycleEnd;
            int earEdgeIdx = 0;
            
            earEdges[earEdgeIdx++] = cycleEdge;
            edgeUsed[cycleEdge] = 1;
            vertexVisited[cycleStart] = 1;
            vertexVisited[cycleEnd] = 1;

            int pathLen = 1;
            while (currCycleNode != cycleStart)
            {
                int pe = parentEdge[currCycleNode];
                earEdges[earEdgeIdx++] = pe;
                edgeUsed[pe] = 1;
                vertexVisited[currCycleNode] = 1;
                currCycleNode = parent[currCycleNode];
                pathLen++;
            }
            earLengths[*earCount] = pathLen;
            (*earCount)++;

            while (true)
            {
                int startEdge = -1;
                for (int e = 0; e < m; e++)
                {
                    if (edgeUsed[e] == 0 && (vertexVisited[u[e]] == 1 || vertexVisited[v[e]] == 1))
                    {
                        startEdge = e;
                        break;
                    }
                }

                if (startEdge == -1)
                {
                    break;
                }

                int startNode = u[startEdge];
                int nextNode = v[startEdge];
                if (vertexVisited[nextNode] == 1)
                {
                    startNode = v[startEdge];
                    nextNode = u[startEdge];
                }

                if (vertexVisited[startNode] == 1 && vertexVisited[nextNode] == 1)
                {
                    earEdges[earEdgeIdx++] = startEdge;
                    edgeUsed[startEdge] = 1;
                    earLengths[*earCount] = 1;
                    (*earCount)++;
                    continue;
                }

                int* pathNode = stackalloc int[n];
                int pHead = 0, pTail = 0;
                
                byte* tempVis = stackalloc byte[n];
                for (int i = 0; i < n; i++) tempVis[i] = 0;

                int* bfsParent = stackalloc int[n];
                int* bfsParentEdge = stackalloc int[n];
                for (int i = 0; i < n; i++) { bfsParent[i] = -1; bfsParentEdge[i] = -1; }

                tempVis[nextNode] = 1;
                pathNode[pTail++] = nextNode;
                int targetNode = -1;

                while (pHead < pTail)
                {
                    int cNode = pathNode[pHead++];
                    if (vertexVisited[cNode] == 1)
                    {
                        targetNode = cNode;
                        break;
                    }

                    for (int e = 0; e < m; e++)
                    {
                        if (edgeUsed[e] == 0 && e != startEdge)
                        {
                            if (u[e] != cNode && v[e] != cNode) continue;
                            int neighbor = (u[e] == cNode) ? v[e] : u[e];
                            if (tempVis[neighbor] == 0)
                            {
                                tempVis[neighbor] = 1;
                                bfsParent[neighbor] = cNode;
                                bfsParentEdge[neighbor] = e;
                                pathNode[pTail++] = neighbor;
                            }
                        }
                    }
                }

                if (targetNode == -1)
                {
                    return false;
                }

                int* tempEdges = stackalloc int[n];
                int tempEdgeCount = 0;
                int c = targetNode;
                while (c != nextNode)
                {
                    tempEdges[tempEdgeCount++] = bfsParentEdge[c];
                    c = bfsParent[c];
                }

                earEdges[earEdgeIdx++] = startEdge;
                edgeUsed[startEdge] = 1;
                vertexVisited[nextNode] = 1;

                for (int i = tempEdgeCount - 1; i >= 0; i--)
                {
                    int e = tempEdges[i];
                    earEdges[earEdgeIdx++] = e;
                    edgeUsed[e] = 1;
                    int other = (u[e] == nextNode) ? v[e] : u[e];
                    vertexVisited[other] = 1;
                    nextNode = other;
                }

                earLengths[*earCount] = tempEdgeCount + 1;
                (*earCount)++;
            }

            for (int i = 0; i < m; i++)
            {
                if (edgeUsed[i] == 0) return false;
            }
            return true;
        }

        public static bool StNumbering(int n, int m, int* u, int* v, int s, int t, int* stOrder)
        {
            int* head = stackalloc int[n];
            int* toNode = stackalloc int[m * 2];
            int* nextEdge = stackalloc int[m * 2];
            for (int i = 0; i < n; i++) head[i] = -1;
            int edgeIdx = 0;
            for (int i = 0; i < m; i++)
            {
                int ui = u[i];
                int vi = v[i];
                toNode[edgeIdx] = vi; nextEdge[edgeIdx] = head[ui]; head[ui] = edgeIdx++;
                toNode[edgeIdx] = ui; nextEdge[edgeIdx] = head[vi]; head[vi] = edgeIdx++;
            }

            int* dfn = stackalloc int[n];
            int* low = stackalloc int[n];
            int* parent = stackalloc int[n];
            int* dfsOrder = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                dfn[i] = 0;
                low[i] = 0;
                parent[i] = -1;
                dfsOrder[i] = -1;
            }

            int timer = 0;
            StDfs(s, s, head, toNode, nextEdge, dfn, low, parent, dfsOrder, &timer, t);

            for (int i = 0; i < n; i++)
            {
                if (dfn[i] == 0) return false;
            }

            byte* sign = stackalloc byte[n];
            for (int i = 0; i < n; i++) sign[i] = 0;
            sign[s] = 1;

            int* nextLink = stackalloc int[n];
            int* prevLink = stackalloc int[n];
            for (int i = 0; i < n; i++) { nextLink[i] = -1; prevLink[i] = -1; }

            nextLink[s] = t;
            prevLink[s] = -1;
            nextLink[t] = -1;
            prevLink[t] = s;

            for (int i = 0; i < n; i++)
            {
                int curr = dfsOrder[i];
                if (curr == s || curr == t) continue;

                int p = parent[curr];
                int lowValNode = dfsOrder[low[curr] - 1];

                if (sign[lowValNode] == 0)
                {
                    int nextOfP = nextLink[p];
                    nextLink[p] = curr;
                    prevLink[curr] = p;
                    nextLink[curr] = nextOfP;
                    if (nextOfP != -1)
                    {
                        prevLink[nextOfP] = curr;
                    }
                    sign[p] = 1;
                }
                else
                {
                    int prevOfP = prevLink[p];
                    prevLink[p] = curr;
                    nextLink[curr] = p;
                    prevLink[curr] = prevOfP;
                    if (prevOfP != -1)
                    {
                        nextLink[prevOfP] = curr;
                    }
                    sign[p] = 0;
                }
            }

            int count = 0;
            int node = s;
            while (node != -1)
            {
                stOrder[count++] = node;
                node = nextLink[node];
            }

            return count == n;
        }

        private static void StDfs(int u, int p, int* head, int* to, int* next, int* dfn, int* low, int* parent, int* dfsOrder, int* timer, int t)
        {
            (*timer)++;
            dfn[u] = *timer;
            low[u] = dfn[u];
            parent[u] = p;
            dfsOrder[*timer - 1] = u;

            if (u == parent[u])
            {
                int tEdge = -1;
                for (int e = head[u]; e != -1; e = next[e])
                {
                    if (to[e] == t)
                    {
                        tEdge = e;
                        break;
                    }
                }
                if (tEdge != -1)
                {
                    int v = to[tEdge];
                    if (dfn[v] == 0)
                    {
                        StDfs(v, u, head, to, next, dfn, low, parent, dfsOrder, timer, t);
                        low[u] = Math.Min(low[u], low[v]);
                    }
                }
            }

            for (int e = head[u]; e != -1; e = next[e])
            {
                int v = to[e];
                if (u == parent[u] && v == t)
                {
                    continue;
                }
                if (v == p)
                {
                    continue;
                }
                if (dfn[v] == 0)
                {
                    StDfs(v, u, head, to, next, dfn, low, parent, dfsOrder, timer, t);
                    low[u] = Math.Min(low[u], low[v]);
                }
                else
                {
                    low[u] = Math.Min(low[u], dfn[v]);
                }
            }
        }

        public static bool PlanarEmbedding(int n, int m, int* u, int* v, int* embeddingHead, int* embeddingNext, int* embeddingTo)
        {
            int* x = stackalloc int[n];
            int* y = stackalloc int[n];
            if (!FindCoordinates(n, m, u, v, x, y))
            {
                return false;
            }

            for (int i = 0; i < n; i++)
            {
                embeddingHead[i] = -1;
            }

            int embedEdgeIdx = 0;

            for (int i = 0; i < n; i++)
            {
                int deg = 0;
                int* neighbors = stackalloc int[n];
                int* edgeIds = stackalloc int[n];
                for (int e = 0; e < m; e++)
                {
                    if (u[e] == i)
                    {
                        neighbors[deg] = v[e];
                        edgeIds[deg] = e;
                        deg++;
                    }
                    else if (v[e] == i)
                    {
                        neighbors[deg] = u[e];
                        edgeIds[deg] = e;
                        deg++;
                    }
                }

                double* angles = stackalloc double[deg];
                for (int k = 0; k < deg; k++)
                {
                    int nbr = neighbors[k];
                    angles[k] = Math.Atan2(y[nbr] - y[i], x[nbr] - x[i]);
                }

                for (int k = 1; k < deg; k++)
                {
                    double keyAngle = angles[k];
                    int keyNbr = neighbors[k];
                    int keyEdge = edgeIds[k];
                    int j = k - 1;
                    while (j >= 0 && angles[j] > keyAngle)
                    {
                        angles[j + 1] = angles[j];
                        neighbors[j + 1] = neighbors[j];
                        edgeIds[j + 1] = edgeIds[j];
                        j--;
                    }
                    angles[j + 1] = keyAngle;
                    neighbors[j + 1] = keyNbr;
                    edgeIds[j + 1] = keyEdge;
                }

                for (int k = 0; k < deg; k++)
                {
                    embeddingTo[embedEdgeIdx] = neighbors[k];
                    embeddingNext[embedEdgeIdx] = embeddingHead[i];
                    embeddingHead[i] = embedEdgeIdx++;
                }
            }

            return true;
        }

        private static bool FindCoordinates(int n, int m, int* u, int* v, int* x, int* y)
        {
            int gridW = n * 2 + 1;
            int gridH = n * 2 + 1;

            return PlaceVertex(0, n, m, u, v, x, y, gridW, gridH);
        }

        private static bool PlaceVertex(int idx, int n, int m, int* u, int* v, int* x, int* y, int w, int h)
        {
            if (idx == n)
            {
                return true;
            }

            for (int currX = 0; currX < w; currX++)
            {
                for (int currY = 0; currY < h; currY++)
                {
                    bool taken = false;
                    for (int j = 0; j < idx; j++)
                    {
                        if (x[j] == currX && y[j] == currY)
                        {
                            taken = true;
                            break;
                        }
                    }
                    if (taken) continue;

                    x[idx] = currX;
                    y[idx] = currY;

                    if (ValidatePlacement(idx, m, u, v, x, y))
                    {
                        if (PlaceVertex(idx + 1, n, m, u, v, x, y, w, h))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private static bool ValidatePlacement(int maxIdx, int m, int* u, int* v, int* x, int* y)
        {
            for (int e = 0; e < m; e++)
            {
                int uNode = u[e];
                int vNode = v[e];
                if (uNode <= maxIdx && vNode <= maxIdx)
                {
                    for (int i = 0; i <= maxIdx; i++)
                    {
                        if (i != uNode && i != vNode)
                        {
                            double cp = CrossProduct(x[uNode], y[uNode], x[vNode], y[vNode], x[i], y[i]);
                            if (Math.Abs(cp) < 1e-9)
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            for (int e1 = 0; e1 < m; e1++)
            {
                int u1 = u[e1];
                int v1 = v[e1];
                if (u1 <= maxIdx && v1 <= maxIdx)
                {
                    for (int e2 = e1 + 1; e2 < m; e2++)
                    {
                        int u2 = u[e2];
                        int v2 = v[e2];
                        if (u2 <= maxIdx && v2 <= maxIdx)
                        {
                            if (u1 == u2 || u1 == v2 || v1 == u2 || v1 == v2)
                            {
                                continue;
                            }

                            if (SegmentsIntersect(x[u1], y[u1], x[v1], y[v1], x[u2], y[u2], x[v2], y[v2]))
                            {
                                return false;
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static double CrossProduct(double ax, double ay, double bx, double by, double cx, double cy)
        {
            return (bx - ax) * (cy - ay) - (by - ay) * (cx - ax);
        }

        private static bool SegmentsIntersect(double ax, double ay, double bx, double by, double cx, double cy, double dx, double dy)
        {
            double cp1 = CrossProduct(ax, ay, bx, by, cx, cy);
            double cp2 = CrossProduct(ax, ay, bx, by, dx, dy);
            double cp3 = CrossProduct(cx, cy, dx, dy, ax, ay);
            double cp4 = CrossProduct(cx, cy, dx, dy, bx, by);

            if (((cp1 > 0 && cp2 < 0) || (cp1 < 0 && cp2 > 0)) &&
                ((cp3 > 0 && cp4 < 0) || (cp3 < 0 && cp4 > 0)))
            {
                return true;
            }
            return false;
        }

        public static bool PlanarDualBuild(int n, int m, int* u, int* v, int* embeddingHead, int* embeddingNext, int* embeddingTo, int* dualN, int* dualM, int* dualU, int* dualV, int* faceSizes)
        {
            byte* halfEdgeVisited = stackalloc byte[2 * m];
            for (int i = 0; i < 2 * m; i++) halfEdgeVisited[i] = 0;

            int* halfEdgeFace = stackalloc int[2 * m];
            for (int i = 0; i < 2 * m; i++) halfEdgeFace[i] = -1;

            int faceCount = 0;

            for (int i = 0; i < 2 * m; i++)
            {
                if (halfEdgeVisited[i] == 0)
                {
                    int currEdge = i;
                    int fSize = 0;
                    while (halfEdgeVisited[currEdge] == 0)
                    {
                        halfEdgeVisited[currEdge] = 1;
                        halfEdgeFace[currEdge] = faceCount;
                        fSize++;
                        
                        currEdge = GetNextHalfEdge(n, m, u, v, embeddingHead, embeddingNext, embeddingTo, currEdge);
                        if (currEdge == -1)
                        {
                            return false;
                        }
                    }
                    faceSizes[faceCount] = fSize;
                    faceCount++;
                }
            }

            *dualN = faceCount;

            int dM = 0;
            for (int e = 0; e < m; e++)
            {
                int f1 = halfEdgeFace[2 * e];
                int f2 = halfEdgeFace[2 * e + 1];
                if (f1 != -1 && f2 != -1)
                {
                    dualU[dM] = f1;
                    dualV[dM] = f2;
                    dM++;
                }
            }
            *dualM = dM;

            return true;
        }

        private static int GetNextHalfEdge(int n, int m, int* u, int* v, int* embeddingHead, int* embeddingNext, int* embeddingTo, int currentHalfEdge)
        {
            int source = (currentHalfEdge % 2 == 0) ? u[currentHalfEdge / 2] : v[currentHalfEdge / 2];
            int dest = (currentHalfEdge % 2 == 0) ? v[currentHalfEdge / 2] : u[currentHalfEdge / 2];

            int prevNodeIdx = -1;
            int curr = embeddingHead[dest];
            while (curr != -1)
            {
                if (embeddingTo[curr] == source)
                {
                    prevNodeIdx = curr;
                    break;
                }
                curr = embeddingNext[curr];
            }

            if (prevNodeIdx == -1) return -1;

            int nextNodeIdx = embeddingNext[prevNodeIdx];
            if (nextNodeIdx == -1)
            {
                nextNodeIdx = embeddingHead[dest];
            }

            int nextNeighbor = embeddingTo[nextNodeIdx];

            for (int e = 0; e < m; e++)
            {
                if (u[e] == dest && v[e] == nextNeighbor)
                {
                    return e * 2;
                }
                if (v[e] == dest && u[e] == nextNeighbor)
                {
                    return e * 2 + 1;
                }
            }
            return -1;
        }

        public static void PlanarShortestPath(int n, int m, int* u, int* v, long* w, int src, int dest, long* dist)
        {
            byte* visited = stackalloc byte[n];
            for (int i = 0; i < n; i++)
            {
                dist[i] = 999999999999999;
                visited[i] = 0;
            }
            dist[src] = 0;

            for (int iter = 0; iter < n; iter++)
            {
                int nextNode = -1;
                long minDist = 999999999999999;
                for (int i = 0; i < n; i++)
                {
                    if (visited[i] == 0 && dist[i] < minDist)
                    {
                        minDist = dist[i];
                        nextNode = i;
                    }
                }

                if (nextNode == -1 || nextNode == dest)
                {
                    break;
                }

                visited[nextNode] = 1;

                for (int e = 0; e < m; e++)
                {
                    if (u[e] == nextNode)
                    {
                        int neighbor = v[e];
                        if (visited[neighbor] == 0 && dist[nextNode] + w[e] < dist[neighbor])
                        {
                            dist[neighbor] = dist[nextNode] + w[e];
                        }
                    }
                    else if (v[e] == nextNode)
                    {
                        int neighbor = u[e];
                        if (visited[neighbor] == 0 && dist[nextNode] + w[e] < dist[neighbor])
                        {
                            dist[neighbor] = dist[nextNode] + w[e];
                        }
                    }
                }
            }
        }

        public static bool PlanarSeparator(int n, int m, int* u, int* v, int* separator, int* separatorCount, int* partA, int* partACount, int* partB, int* partBCount)
        {
            int* assignment = stackalloc int[n];
            int* bestAssignment = stackalloc int[n];
            for (int i = 0; i < n; i++)
            {
                assignment[i] = -1;
                bestAssignment[i] = -1;
            }

            int bestSeparatorSize = n + 1;

            SeparatorBacktrack(0, n, m, u, v, assignment, bestAssignment, &bestSeparatorSize, 0, 0, 0);

            if (bestSeparatorSize > n)
            {
                return false;
            }

            *separatorCount = 0;
            *partACount = 0;
            *partBCount = 0;

            for (int i = 0; i < n; i++)
            {
                if (bestAssignment[i] == 2)
                {
                    separator[(*separatorCount)++] = i;
                }
                else if (bestAssignment[i] == 0)
                {
                    partA[(*partACount)++] = i;
                }
                else if (bestAssignment[i] == 1)
                {
                    partB[(*partBCount)++] = i;
                }
            }

            return true;
        }

        private static void SeparatorBacktrack(int idx, int n, int m, int* u, int* v, int* assignment, int* bestAssignment, int* bestSeparatorSize, int countA, int countB, int countC)
        {
            if (countC >= *bestSeparatorSize)
            {
                return;
            }

            int maxPartSize = (2 * n) / 3;
            if (countA > maxPartSize || countB > maxPartSize)
            {
                return;
            }

            if (idx == n)
            {
                bool valid = true;
                for (int e = 0; e < m; e++)
                {
                    int a = assignment[u[e]];
                    int b = assignment[v[e]];
                    if ((a == 0 && b == 1) || (a == 1 && b == 0))
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    *bestSeparatorSize = countC;
                    for (int i = 0; i < n; i++)
                    {
                        bestAssignment[i] = assignment[i];
                    }
                }
                return;
            }

            assignment[idx] = 0;
            SeparatorBacktrack(idx + 1, n, m, u, v, assignment, bestAssignment, bestSeparatorSize, countA + 1, countB, countC);

            assignment[idx] = 1;
            SeparatorBacktrack(idx + 1, n, m, u, v, assignment, bestAssignment, bestSeparatorSize, countA, countB + 1, countC);

            assignment[idx] = 2;
            SeparatorBacktrack(idx + 1, n, m, u, v, assignment, bestAssignment, bestSeparatorSize, countA, countB, countC + 1);

            assignment[idx] = -1;
        }

        public static long PlanarMaxFlow(int n, int s, int t, int* head, int* to, int* next, int* cap, int m)
        {
            int* tempCap = stackalloc int[m * 2 + 2];
            for (int i = 0; i < m * 2 + 2; i++) tempCap[i] = cap[i];
            int* tempFlow = stackalloc int[m * 2 + 2];
            return DinicMaxFlow.Run(n, s, t, head, to, next, tempCap, tempFlow);
        }

        public static long PlanarMinCut(int n, int s, int t, int* head, int* to, int* next, int* cap, int m, int* cutEdges, int* cutCount)
        {
            int* tempCap = stackalloc int[m * 2 + 2];
            for (int i = 0; i < m * 2 + 2; i++) tempCap[i] = cap[i];

            int* flow = stackalloc int[m * 2 + 2];
            for (int i = 0; i < m * 2 + 2; i++) flow[i] = 0;

            long maxFlow = DinicMaxFlow.Run(n, s, t, head, to, next, tempCap, flow);

            byte* visited = stackalloc byte[n];
            for (int i = 0; i < n; i++) visited[i] = 0;

            int* queue = stackalloc int[n];
            int qHead = 0, qTail = 0;
            visited[s] = 1;
            queue[qTail++] = s;

            while (qHead < qTail)
            {
                int u = queue[qHead++];
                for (int e = head[u]; e != 0; e = next[e])
                {
                    int v = to[e];
                    if (visited[v] == 0 && tempCap[e] - flow[e] > 0)
                    {
                        visited[v] = 1;
                        queue[qTail++] = v;
                    }
                }
            }

            *cutCount = 0;
            byte* edgeCross = stackalloc byte[m + 1];
            for (int i = 0; i <= m; i++) edgeCross[i] = 0;

            for (int uNode = 0; uNode < n; uNode++)
            {
                if (visited[uNode] == 1)
                {
                    for (int e = head[uNode]; e != 0; e = next[e])
                    {
                        int vNode = to[e];
                        if (visited[vNode] == 0)
                        {
                            int origEdge = e / 2;
                            if (edgeCross[origEdge] == 0)
                            {
                                edgeCross[origEdge] = 1;
                                cutEdges[(*cutCount)++] = origEdge;
                            }
                        }
                    }
                }
            }

            return maxFlow;
        }

        public static void FacePotentialSolve(int numFaces, int numDualEdges, int* dualU, int* dualV, long* dualW, double* potentials)
        {
            byte* visited = stackalloc byte[numFaces];
            for (int i = 0; i < numFaces; i++)
            {
                potentials[i] = 99999999999999;
                visited[i] = 0;
            }
            potentials[0] = 0;

            for (int iter = 0; iter < numFaces; iter++)
            {
                int nextNode = -1;
                double minDist = 99999999999999;
                for (int i = 0; i < numFaces; i++)
                {
                    if (visited[i] == 0 && potentials[i] < minDist)
                    {
                        minDist = potentials[i];
                        nextNode = i;
                    }
                }

                if (nextNode == -1)
                {
                    break;
                }

                visited[nextNode] = 1;

                for (int e = 0; e < numDualEdges; e++)
                {
                    if (dualU[e] == nextNode)
                    {
                        int neighbor = dualV[e];
                        if (visited[neighbor] == 0 && potentials[nextNode] + dualW[e] < potentials[neighbor])
                        {
                            potentials[neighbor] = potentials[nextNode] + dualW[e];
                        }
                    }
                    else if (dualV[e] == nextNode)
                    {
                        int neighbor = dualU[e];
                        if (visited[neighbor] == 0 && potentials[nextNode] + dualW[e] < potentials[neighbor])
                        {
                            potentials[neighbor] = potentials[nextNode] + dualW[e];
                        }
                    }
                }
            }
        }

        public static bool KuratowskiSubgraph(int n, int m, int* u, int* v, int* kuratowskiU, int* kuratowskiV, int* kuratowskiCount)
        {
            *kuratowskiCount = 0;

            byte* edgeActive = stackalloc byte[m];
            for (int i = 0; i < m; i++) edgeActive[i] = 0;

            bool found = false;
            
            void Search(int edgeIdx, int activeCount)
            {
                if (found) return;

                if (activeCount >= 9)
                {
                    if (IsKuratowskiSubdivision(n, m, u, v, edgeActive))
                    {
                        found = true;
                        for (int i = 0; i < m; i++)
                        {
                            if (edgeActive[i] == 1)
                            {
                                kuratowskiU[*kuratowskiCount] = u[i];
                                kuratowskiV[*kuratowskiCount] = v[i];
                                (*kuratowskiCount)++;
                            }
                        }
                        return;
                    }
                }

                if (edgeIdx == m)
                {
                    return;
                }

                edgeActive[edgeIdx] = 1;
                Search(edgeIdx + 1, activeCount + 1);
                if (found) return;

                edgeActive[edgeIdx] = 0;
                Search(edgeIdx + 1, activeCount);
            }

            Search(0, 0);
            return found;
        }

        private static bool IsKuratowskiSubdivision(int n, int m, int* u, int* v, byte* edgeActive)
        {
            int* deg = stackalloc int[n];
            for (int i = 0; i < n; i++) deg[i] = 0;

            int activeCount = 0;
            int* activeU = stackalloc int[m];
            int* activeV = stackalloc int[m];
            for (int i = 0; i < m; i++)
            {
                if (edgeActive[i] == 1)
                {
                    activeU[activeCount] = u[i];
                    activeV[activeCount] = v[i];
                    activeCount++;
                }
            }

            int tempM = activeCount;
            int* tU = stackalloc int[m * 2];
            int* tV = stackalloc int[m * 2];
            for (int i = 0; i < tempM; i++)
            {
                tU[i] = activeU[i];
                tV[i] = activeV[i];
            }

            byte* nodeRemoved = stackalloc byte[n];
            for (int i = 0; i < n; i++) nodeRemoved[i] = 0;

            while (true)
            {
                for (int i = 0; i < n; i++) deg[i] = 0;
                for (int i = 0; i < tempM; i++)
                {
                    deg[tU[i]]++;
                    deg[tV[i]]++;
                }

                int degree2Node = -1;
                for (int i = 0; i < n; i++)
                {
                    if (nodeRemoved[i] == 0 && deg[i] == 2)
                    {
                        degree2Node = i;
                        break;
                    }
                }

                if (degree2Node == -1)
                {
                    break;
                }

                int n1 = -1, n2 = -1;
                int e1Idx = -1, e2Idx = -1;
                for (int i = 0; i < tempM; i++)
                {
                    if (tU[i] == degree2Node)
                    {
                        if (n1 == -1) { n1 = tV[i]; e1Idx = i; }
                        else { n2 = tV[i]; e2Idx = i; }
                    }
                    else if (tV[i] == degree2Node)
                    {
                        if (n1 == -1) { n1 = tU[i]; e1Idx = i; }
                        else { n2 = tU[i]; e2Idx = i; }
                    }
                }

                if (n1 == -1 || n2 == -1)
                {
                    break;
                }

                nodeRemoved[degree2Node] = 1;
                tU[e1Idx] = n1;
                tV[e1Idx] = n2;
                
                tU[e2Idx] = tU[tempM - 1];
                tV[e2Idx] = tV[tempM - 1];
                tempM--;
            }

            int remCount = 0;
            int* remNodes = stackalloc int[n];
            for (int i = 0; i < n; i++) deg[i] = 0;
            for (int i = 0; i < tempM; i++)
            {
                deg[tU[i]]++;
                deg[tV[i]]++;
            }
            for (int i = 0; i < n; i++)
            {
                if (deg[i] > 0)
                {
                    remNodes[remCount++] = i;
                }
            }

            if (remCount == 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (deg[remNodes[i]] != 4) return false;
                }
                return true;
            }
            else if (remCount == 6)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (deg[remNodes[i]] != 3) return false;
                }
                
                int* color = stackalloc int[n];
                for (int i = 0; i < n; i++) color[i] = -1;
                
                int* queue = stackalloc int[n];
                int qHead = 0, qTail = 0;
                
                color[remNodes[0]] = 0;
                queue[qTail++] = remNodes[0];
                
                while (qHead < qTail)
                {
                    int curr = queue[qHead++];
                    for (int i = 0; i < tempM; i++)
                    {
                        int neighbor = -1;
                        if (tU[i] == curr) neighbor = tV[i];
                        else if (tV[i] == curr) neighbor = tU[i];
                        
                        if (neighbor != -1)
                        {
                            if (color[neighbor] == -1)
                            {
                                color[neighbor] = 1 - color[curr];
                                queue[qTail++] = neighbor;
                            }
                            else if (color[neighbor] == color[curr])
                            {
                                return false;
                            }
                        }
                    }
                }

                int c0 = 0, c1 = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (color[remNodes[i]] == 0) c0++;
                    else if (color[remNodes[i]] == 1) c1++;
                }
                return c0 == 3 && c1 == 3;
            }

            return false;
        }

        public static bool OuterplanarCheck(int n, int m, int* u, int* v)
        {
            byte* edgeActive = stackalloc byte[m];
            for (int i = 0; i < m; i++) edgeActive[i] = 0;

            bool hasForbidden = false;

            void Search(int edgeIdx, int activeCount)
            {
                if (hasForbidden) return;

                if (activeCount >= 6)
                {
                    if (IsOuterplanarForbiddenSubdivision(n, m, u, v, edgeActive))
                    {
                        hasForbidden = true;
                        return;
                    }
                }

                if (edgeIdx == m)
                {
                    return;
                }

                edgeActive[edgeIdx] = 1;
                Search(edgeIdx + 1, activeCount + 1);
                if (hasForbidden) return;

                edgeActive[edgeIdx] = 0;
                Search(edgeIdx + 1, activeCount);
            }

            Search(0, 0);
            return !hasForbidden;
        }

        private static bool IsOuterplanarForbiddenSubdivision(int n, int m, int* u, int* v, byte* edgeActive)
        {
            int* deg = stackalloc int[n];
            for (int i = 0; i < n; i++) deg[i] = 0;

            int activeCount = 0;
            int* activeU = stackalloc int[m];
            int* activeV = stackalloc int[m];
            for (int i = 0; i < m; i++)
            {
                if (edgeActive[i] == 1)
                {
                    activeU[activeCount] = u[i];
                    activeV[activeCount] = v[i];
                    activeCount++;
                }
            }

            int tempM = activeCount;
            int* tU = stackalloc int[m * 2];
            int* tV = stackalloc int[m * 2];
            for (int i = 0; i < tempM; i++)
            {
                tU[i] = activeU[i];
                tV[i] = activeV[i];
            }

            byte* nodeRemoved = stackalloc byte[n];
            for (int i = 0; i < n; i++) nodeRemoved[i] = 0;

            while (true)
            {
                for (int i = 0; i < n; i++) deg[i] = 0;
                for (int i = 0; i < tempM; i++)
                {
                    deg[tU[i]]++;
                    deg[tV[i]]++;
                }

                int degree2Node = -1;
                for (int i = 0; i < n; i++)
                {
                    if (nodeRemoved[i] == 0 && deg[i] == 2)
                    {
                        degree2Node = i;
                        break;
                    }
                }

                if (degree2Node == -1)
                {
                    break;
                }

                int n1 = -1, n2 = -1;
                int e1Idx = -1, e2Idx = -1;
                for (int i = 0; i < tempM; i++)
                {
                    if (tU[i] == degree2Node)
                    {
                        if (n1 == -1) { n1 = tV[i]; e1Idx = i; }
                        else { n2 = tV[i]; e2Idx = i; }
                    }
                    else if (tV[i] == degree2Node)
                    {
                        if (n1 == -1) { n1 = tU[i]; e1Idx = i; }
                        else { n2 = tU[i]; e2Idx = i; }
                    }
                }

                if (n1 == -1 || n2 == -1)
                {
                    break;
                }

                nodeRemoved[degree2Node] = 1;
                tU[e1Idx] = n1;
                tV[e1Idx] = n2;

                tU[e2Idx] = tU[tempM - 1];
                tV[e2Idx] = tV[tempM - 1];
                tempM--;
            }

            int remCount = 0;
            int* remNodes = stackalloc int[n];
            for (int i = 0; i < n; i++) deg[i] = 0;
            for (int i = 0; i < tempM; i++)
            {
                deg[tU[i]]++;
                deg[tV[i]]++;
            }
            for (int i = 0; i < n; i++)
            {
                if (deg[i] > 0)
                {
                    remNodes[remCount++] = i;
                }
            }

            if (remCount == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (deg[remNodes[i]] != 3) return false;
                }
                return true;
            }
            else if (remCount == 5)
            {
                int countDeg3 = 0;
                int countDeg2 = 0;
                for (int i = 0; i < 5; i++)
                {
                    if (deg[remNodes[i]] == 3) countDeg3++;
                    else if (deg[remNodes[i]] == 2) countDeg2++;
                }

                if (countDeg3 != 2 || countDeg2 != 3) return false;

                int* color = stackalloc int[n];
                for (int i = 0; i < n; i++) color[i] = -1;

                int* queue = stackalloc int[n];
                int qHead = 0, qTail = 0;

                color[remNodes[0]] = 0;
                queue[qTail++] = remNodes[0];

                while (qHead < qTail)
                {
                    int curr = queue[qHead++];
                    for (int i = 0; i < tempM; i++)
                    {
                        int neighbor = -1;
                        if (tU[i] == curr) neighbor = tV[i];
                        else if (tV[i] == curr) neighbor = tU[i];

                        if (neighbor != -1)
                        {
                            if (color[neighbor] == -1)
                            {
                                color[neighbor] = 1 - color[curr];
                                queue[qTail++] = neighbor;
                            }
                            else if (color[neighbor] == color[curr])
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            return false;
        }

        public static bool SeriesParallelDecompose(int n, int m, int* u, int* v, int s, int t)
        {
            int tempM = m;
            int* tU = stackalloc int[m];
            int* tV = stackalloc int[m];
            for (int i = 0; i < m; i++)
            {
                tU[i] = u[i];
                tV[i] = v[i];
            }

            byte* nodeRemoved = stackalloc byte[n];
            for (int i = 0; i < n; i++) nodeRemoved[i] = 0;

            int* deg = stackalloc int[n];

            while (true)
            {
                bool reducedParallel = false;
                for (int i = 0; i < tempM; i++)
                {
                    for (int j = i + 1; j < tempM; j++)
                    {
                        if ((tU[i] == tU[j] && tV[i] == tV[j]) || (tU[i] == tV[j] && tV[i] == tU[j]))
                        {
                            tU[j] = tU[tempM - 1];
                            tV[j] = tV[tempM - 1];
                            tempM--;
                            reducedParallel = true;
                            break;
                        }
                    }
                    if (reducedParallel) break;
                }
                if (reducedParallel) continue;

                for (int i = 0; i < n; i++) deg[i] = 0;
                for (int i = 0; i < tempM; i++)
                {
                    deg[tU[i]]++;
                    deg[tV[i]]++;
                }

                int degree2Node = -1;
                for (int i = 0; i < n; i++)
                {
                    if (i != s && i != t && nodeRemoved[i] == 0 && deg[i] == 2)
                    {
                        degree2Node = i;
                        break;
                    }
                }

                if (degree2Node == -1)
                {
                    break;
                }

                int n1 = -1, n2 = -1;
                int e1Idx = -1, e2Idx = -1;
                for (int i = 0; i < tempM; i++)
                {
                    if (tU[i] == degree2Node)
                    {
                        if (n1 == -1) { n1 = tV[i]; e1Idx = i; }
                        else { n2 = tV[i]; e2Idx = i; }
                    }
                    else if (tV[i] == degree2Node)
                    {
                        if (n1 == -1) { n1 = tU[i]; e1Idx = i; }
                        else { n2 = tU[i]; e2Idx = i; }
                    }
                }

                if (n1 == -1 || n2 == -1)
                {
                    break;
                }

                nodeRemoved[degree2Node] = 1;
                tU[e1Idx] = n1;
                tV[e1Idx] = n2;

                tU[e2Idx] = tU[tempM - 1];
                tV[e2Idx] = tV[tempM - 1];
                tempM--;
            }

            if (tempM == 1)
            {
                if ((tU[0] == s && tV[0] == t) || (tU[0] == t && tV[0] == s))
                {
                    return true;
                }
            }
            return false;
        }

        public static int TriconnectedComponents(int n, int m, int* u, int* v, int* compType)
        {
            int compCount = 0;
            byte* edgeActive = stackalloc byte[m];
            for (int i = 0; i < m; i++) edgeActive[i] = 1;

            DecomposeTriconnected(n, m, u, v, edgeActive, &compCount, compType);
            return compCount;
        }

        public static int SpqrTreeBuild(int n, int m, int* u, int* v, int* compType)
        {
            return TriconnectedComponents(n, m, u, v, compType);
        }

        private static void DecomposeTriconnected(int n, int m, int* u, int* v, byte* edgeActive, int* compCount, int* compType)
        {
            int sepX = -1, sepY = -1;
            if (FindSeparationPair(n, m, u, v, edgeActive, &sepX, &sepY))
            {
                int* compId = stackalloc int[n];
                for (int i = 0; i < n; i++) compId[i] = -1;

                int cCount = 0;
                for (int i = 0; i < n; i++)
                {
                    if (i != sepX && i != sepY && compId[i] == -1)
                    {
                        int* queue = stackalloc int[n];
                        int qHead = 0, qTail = 0;
                        compId[i] = cCount;
                        queue[qTail++] = i;

                        while (qHead < qTail)
                        {
                            int curr = queue[qHead++];
                            for (int e = 0; e < m; e++)
                            {
                                if (edgeActive[e] == 1)
                                {
                                    int neighbor = -1;
                                    if (u[e] == curr) neighbor = v[e];
                                    else if (v[e] == curr) neighbor = u[e];

                                    if (neighbor != -1 && neighbor != sepX && neighbor != sepY && compId[neighbor] == -1)
                                    {
                                        compId[neighbor] = cCount;
                                        queue[qTail++] = neighbor;
                                    }
                                }
                            }
                        }
                        cCount++;
                    }
                }

                for (int c = 0; c < cCount; c++)
                {
                    byte* subEdgeActive = stackalloc byte[m];
                    for (int i = 0; i < m; i++) subEdgeActive[i] = 0;

                    for (int i = 0; i < m; i++)
                    {
                        if (edgeActive[i] == 1)
                        {
                            int ui = u[i];
                            int vi = v[i];
                            bool inComp = false;
                            if (ui != sepX && ui != sepY && compId[ui] == c) inComp = true;
                            if (vi != sepX && vi != sepY && compId[vi] == c) inComp = true;
                            if (inComp)
                            {
                                subEdgeActive[i] = 1;
                            }
                        }
                    }

                    DecomposeTriconnected(n, m, u, v, subEdgeActive, compCount, compType);
                }
            }
            else
            {
                int activeEdges = 0;
                for (int i = 0; i < m; i++)
                {
                    if (edgeActive[i] == 1) activeEdges++;
                }

                if (activeEdges > 0)
                {
                    int type = 2;
                    if (activeEdges == 3)
                    {
                        type = 0;
                    }
                    
                    compType[*compCount] = type;
                    (*compCount)++;
                }
            }
        }

        private static bool FindSeparationPair(int n, int m, int* u, int* v, byte* edgeActive, int* sepX, int* sepY)
        {
            for (int xNode = 0; xNode < n; xNode++)
            {
                for (int yNode = xNode + 1; yNode < n; yNode++)
                {
                    byte* visited = stackalloc byte[n];
                    for (int i = 0; i < n; i++) visited[i] = 0;

                    visited[xNode] = 1;
                    visited[yNode] = 1;

                    int startNode = -1;
                    for (int i = 0; i < n; i++)
                    {
                        if (i != xNode && i != yNode)
                        {
                            bool active = false;
                            for (int e = 0; e < m; e++)
                            {
                                if (edgeActive[e] == 1 && (u[e] == i || v[e] == i))
                                {
                                    active = true;
                                    break;
                                }
                            }
                            if (active)
                            {
                                startNode = i;
                                break;
                            }
                        }
                    }

                    if (startNode == -1) continue;

                    int* queue = stackalloc int[n];
                    int qHead = 0, qTail = 0;
                    visited[startNode] = 1;
                    queue[qTail++] = startNode;

                    while (qHead < qTail)
                    {
                        int curr = queue[qHead++];
                        for (int e = 0; e < m; e++)
                        {
                            if (edgeActive[e] == 1)
                            {
                                int neighbor = -1;
                                if (u[e] == curr) neighbor = v[e];
                                else if (v[e] == curr) neighbor = u[e];

                                if (neighbor != -1 && visited[neighbor] == 0)
                                {
                                    visited[neighbor] = 1;
                                    queue[qTail++] = neighbor;
                                }
                            }
                        }
                    }

                    bool disconnected = false;
                    for (int i = 0; i < n; i++)
                    {
                        if (i != xNode && i != yNode && visited[i] == 0)
                        {
                            bool active = false;
                            for (int e = 0; e < m; e++)
                            {
                                if (edgeActive[e] == 1 && (u[e] == i || v[e] == i))
                                {
                                    active = true;
                                    break;
                                }
                            }
                            if (active)
                            {
                                disconnected = true;
                                break;
                            }
                        }
                    }

                    if (disconnected)
                    {
                        *sepX = xNode;
                        *sepY = yNode;
                        return true;
                    }
                }
            }
            return false;
        }

        public static void MaximumPlanarMatching(int n, int m, int* u, int* v, int* matchU, int* matchV, int* matchCount)
        {
            int* match = stackalloc int[n];
            GeneralMatchingBlossom.Run(n, m, u, v, match);

            *matchCount = 0;
            for (int i = 0; i < n; i++)
            {
                if (match[i] != -1 && i < match[i])
                {
                    matchU[*matchCount] = i;
                    matchV[*matchCount] = match[i];
                    (*matchCount)++;
                }
            }
        }
    }
}
