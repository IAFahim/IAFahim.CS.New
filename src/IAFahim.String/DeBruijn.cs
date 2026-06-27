namespace IAFahim.String
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class DeBruijn
    {
        public static void SequenceBuild(int n, int k, int* sequence, int* seqLen)
        {
            int* a = stackalloc int[n + 1];
            for (int i = 0; i <= n; i++)
            {
                a[i] = 0;
            }
            *seqLen = 0;
            DbDfs(1, 1, n, k, a, sequence, seqLen);
        }

        private static void DbDfs(int t, int p, int n, int k, int* a, int* sequence, int* seqLen)
        {
            if (t > n)
            {
                if (n % p == 0)
                {
                    for (int i = 1; i <= p; i++)
                    {
                        sequence[(*seqLen)++] = a[i];
                    }
                }
            }
            else
            {
                a[t] = a[t - p];
                DbDfs(t + 1, p, n, k, a, sequence, seqLen);
                for (int j = a[t - p] + 1; j < k; j++)
                {
                    a[t] = j;
                    DbDfs(t + 1, t, n, k, a, sequence, seqLen);
                }
            }
        }

        public static void GraphBuild(int n, int k, int* adj)
        {
            int numVertices = 1;
            for (int i = 0; i < n - 1; i++)
            {
                numVertices *= k;
            }
            for (int u = 0; u < numVertices; u++)
            {
                for (int i = 0; i < k; i++)
                {
                    adj[u * k + i] = (u * k + i) % numVertices;
                }
            }
        }

        public static void EulerianPath(int n, int k, int* path, int* pathLen)
        {
            int numVertices = 1;
            for (int i = 0; i < n - 1; i++)
            {
                numVertices *= k;
            }
            int numEdges = numVertices * k;
            int* edgeIdx = stackalloc int[numVertices];
            for (int i = 0; i < numVertices; i++)
            {
                edgeIdx[i] = 0;
            }
            int* stack = null;
            bool allocated = false;
            if (numEdges + 1 > 1024)
            {
                stack = (int*)Marshal.AllocHGlobal((nint)((numEdges + 1) * sizeof(int)));
                allocated = true;
            }
            else
            {
                int* tempStack = stackalloc int[numEdges + 1];
                stack = tempStack;
            }
            int stackPtr = 0;
            stack[stackPtr++] = 0;
            int writePtr = 0;
            while (stackPtr > 0)
            {
                int u = stack[stackPtr - 1];
                if (edgeIdx[u] < k)
                {
                    int nextEdge = edgeIdx[u]++;
                    int v = (u * k + nextEdge) % numVertices;
                    stack[stackPtr++] = v;
                }
                else
                {
                    path[writePtr++] = u;
                    stackPtr--;
                }
            }
            *pathLen = writePtr;
            if (allocated)
            {
                Marshal.FreeHGlobal((nint)stack);
            }
        }
    }
}
