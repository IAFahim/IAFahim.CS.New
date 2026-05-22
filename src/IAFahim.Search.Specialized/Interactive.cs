namespace IAFahim.Search
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Interactive
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool StressCompare<TInput, TOutput>(
            TInput* input,
            int inputLen,
            delegate*<TInput*, int, TOutput*, int*, bool> bruteforceSolve,
            delegate*<TInput*, int, TOutput*, int*, bool> optimizedSolve,
            delegate*<TOutput*, int, TOutput*, int, bool> checkerCompare,
            TOutput* scratchBrute,
            TOutput* scratchOpt)
            where TInput : unmanaged
            where TOutput : unmanaged
        {
            int bruteSize = 0;
            int optSize = 0;
            bool bruteFound = bruteforceSolve(input, inputLen, scratchBrute, &bruteSize);
            bool optFound = optimizedSolve(input, inputLen, scratchOpt, &optSize);
            if (bruteFound != optFound)
            {
                return false;
            }
            if (!bruteFound)
            {
                return true;
            }
            return checkerCompare(scratchBrute, bruteSize, scratchOpt, optSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool BruteforceSolve<TInput, TOutput>(TInput* input, int inputLen, TOutput* output, int* outputLen)
            where TInput : unmanaged
            where TOutput : unmanaged
        {
            if (inputLen == 0)
            {
                *outputLen = 0;
                return false;
            }
            *outputLen = 0;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool OptimizedSolve<TInput, TOutput>(TInput* input, int inputLen, TOutput* output, int* outputLen)
            where TInput : unmanaged
            where TOutput : unmanaged
        {
            if (inputLen == 0)
            {
                *outputLen = 0;
                return false;
            }
            *outputLen = 0;
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ValidateOutput<TInput, TOutput>(
            TInput* input,
            int inputLen,
            TOutput* output,
            int outputLen,
            delegate*<TInput*, int, TOutput*, int, bool> validator)
            where TInput : unmanaged
            where TOutput : unmanaged
        {
            return validator(input, inputLen, output, outputLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckerCompare<TOutput>(
            TOutput* expected,
            int expectedLen,
            TOutput* actual,
            int actualLen)
            where TOutput : unmanaged, IComparable<TOutput>
        {
            if (expectedLen != actualLen)
            {
                return false;
            }
            for (int i = 0; i < expectedLen; i++)
            {
                if (expected[i].CompareTo(actual[i]) != 0)
                {
                    return false;
                }
            }
            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InteractiveFlush(delegate*<void> flushFn)
        {
            if (flushFn != null)
            {
                flushFn();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TResponse InteractiveAsk<TQuery, TResponse>(
            TQuery* query,
            int queryLen,
            delegate*<TQuery*, int, TResponse> askFn)
            where TQuery : unmanaged
            where TResponse : unmanaged
        {
            return askFn(query, queryLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InteractiveAnswer<TAnswer>(
            TAnswer* answer,
            int answerLen,
            delegate*<TAnswer*, int, void> answerFn)
            where TAnswer : unmanaged
        {
            answerFn(answer, answerLen);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool InteractiveReadVerdict(delegate*<bool> readVerdictFn)
        {
            return readVerdictFn();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int GetUnmanagedHash<T>(T* key) where T : unmanaged
        {
            int hash = 17;
            byte* ptr = (byte*)key;
            int size = sizeof(T);
            for (int i = 0; i < size; i++) hash = hash * 31 + ptr[i];
            return hash & 0x7FFFFFFF;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool QueryCacheGet<TKey, TValue>(
            TKey* keys,
            TValue* values,
            byte* occupied,
            int capacity,
            TKey key,
            out TValue value)
            where TKey : unmanaged
            where TValue : unmanaged
        {
            value = default;
            TKey* keyPtr = &key;
            int hash = GetUnmanagedHash(keyPtr);
            int idx = hash % capacity;
            for (int i = 0; i < capacity; i++)
            {
                int curr = (idx + i) % capacity;
                if (occupied[curr] == 0) return false;
                byte* kp = (byte*)&keys[curr];
                byte* tp = (byte*)keyPtr;
                bool match = true;
                for (int j = 0; j < sizeof(TKey); j++)
                    if (kp[j] != tp[j]) { match = false; break; }
                if (match) { value = values[curr]; return true; }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool QueryCacheSet<TKey, TValue>(
            TKey* keys,
            TValue* values,
            byte* occupied,
            int capacity,
            TKey key,
            TValue value)
            where TKey : unmanaged
            where TValue : unmanaged
        {
            TKey* keyPtr = &key;
            int hash = GetUnmanagedHash(keyPtr);
            int idx = hash % capacity;
            for (int i = 0; i < capacity; i++)
            {
                int curr = (idx + i) % capacity;
                byte* kp = (byte*)&keys[curr];
                byte* tp = (byte*)keyPtr;
                bool match = true;
                for (int j = 0; j < sizeof(TKey); j++)
                    if (kp[j] != tp[j]) { match = false; break; }
                if (occupied[curr] == 0 || !match)
                {
                    byte* dp = (byte*)&keys[curr];
                    byte* sp = (byte*)keyPtr;
                    for (int j = 0; j < sizeof(TKey); j++) dp[j] = sp[j];
                    values[curr] = value;
                    occupied[curr] = 1;
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int AdaptiveQueryStrategy(
            int* rangeL,
            int* rangeR,
            int lastResponse,
            int responseCondition)
        {
            if (lastResponse == responseCondition)
            {
                *rangeL = (*rangeL + *rangeR) / 2 + 1;
            }
            else
            {
                *rangeR = (*rangeL + *rangeR) / 2;
            }
            return (*rangeL + *rangeR) / 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool UnknownArrayBinarySearch<T>(
            T target,
            delegate*<int, T> queryFn,
            delegate*<int, bool> isValidIndexFn,
            out int index)
            where T : unmanaged, IComparable<T>
        {
            index = -1;
            if (!isValidIndexFn(0))
            {
                return false;
            }
            T valZero = queryFn(0);
            if (valZero.CompareTo(target) == 0)
            {
                index = 0;
                return true;
            }
            int bound = 1;
            while (isValidIndexFn(bound) && queryFn(bound).CompareTo(target) < 0)
            {
                bound *= 2;
            }
            int low = bound / 2;
            int high = bound;
            bool found = false;
            while (low <= high)
            {
                int mid = low + (high - low) / 2;
                if (!isValidIndexFn(mid))
                {
                    high = mid - 1;
                    continue;
                }
                T val = queryFn(mid);
                int cmp = val.CompareTo(target);
                if (cmp == 0)
                {
                    index = mid;
                    found = true;
                    high = mid - 1;
                }
                else if (cmp < 0)
                {
                    low = mid + 1;
                }
                else
                {
                    high = mid - 1;
                }
            }
            return found;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InteractiveGraphExplore(
            int startVertex,
            int* visited,
            int maxVertices,
            delegate*<int, int*, int*, void> getNeighborsFn,
            int* discoveredEdges,
            int* edgeCount)
        {
            for (int i = 0; i < maxVertices; i++) visited[i] = 0;
            *edgeCount = 0;

            int* queue = stackalloc int[maxVertices];
            int head = 0, tail = 0;
            queue[tail++] = startVertex;
            visited[startVertex] = 1;

            int* neighbors = stackalloc int[maxVertices];
            while (head < tail)
            {
                int u = queue[head++];
                int neighborCount = 0;
                getNeighborsFn(u, neighbors, &neighborCount);
                for (int i = 0; i < neighborCount; i++)
                {
                    int v = neighbors[i];
                    discoveredEdges[(*edgeCount) * 2 + 0] = u;
                    discoveredEdges[(*edgeCount) * 2 + 1] = v;
                    (*edgeCount)++;
                    if (visited[v] == 0)
                    {
                        visited[v] = 1;
                        if (tail < maxVertices) queue[tail++] = v;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InteractiveTreeFind(
            int startVertex,
            delegate*<int, int> queryNeighborTowardsTargetFn)
        {
            int curr = startVertex;
            while (true)
            {
                int next = queryNeighborTowardsTargetFn(curr);
                if (next == curr) return curr;
                curr = next;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int InteractiveTreeCentroidFind(
            int numNodes,
            int* head,
            int* next,
            int* to,
            delegate*<int, int> queryFn)
        {
            byte* removed = stackalloc byte[numNodes];
            for (int i = 0; i < numNodes; i++) removed[i] = 0;

            int* sz = stackalloc int[numNodes];
            int* parent = stackalloc int[numNodes];
            int* queue = stackalloc int[numNodes];

            int currentRoot = 0;
            while (true)
            {
                int qHead = 0;
                int qTail = 0;
                queue[qTail++] = currentRoot;
                parent[currentRoot] = -1;
                while (qHead < qTail)
                {
                    int u = queue[qHead++];
                    for (int e = head[u]; e != -1; e = next[e])
                    {
                        int v = to[e];
                        if (v != parent[u] && removed[v] == 0)
                        {
                            parent[v] = u;
                            queue[qTail++] = v;
                        }
                    }
                }

                int componentSize = qTail;
                if (componentSize == 1) return queue[0];

                for (int i = 0; i < componentSize; i++) sz[queue[i]] = 1;
                for (int i = componentSize - 1; i >= 0; i--)
                {
                    int u = queue[i];
                    int p = parent[u];
                    if (p != -1) sz[p] += sz[u];
                }

                int centroid = -1;
                for (int i = 0; i < componentSize; i++)
                {
                    int u = queue[i];
                    bool isCentroid = true;
                    if (componentSize - sz[u] > componentSize / 2) isCentroid = false;
                    for (int e = head[u]; e != -1; e = next[e])
                    {
                        int v = to[e];
                        if (v != parent[u] && removed[v] == 0)
                        {
                            if (sz[v] > componentSize / 2) { isCentroid = false; break; }
                        }
                    }
                    if (isCentroid) { centroid = u; break; }
                }

                if (centroid == -1) centroid = queue[0];
                int nextHop = queryFn(centroid);
                if (nextHop == centroid) return centroid;
                removed[centroid] = 1;
                currentRoot = nextHop;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void InteractivePermutationRecover(
            int n,
            int* recoveredPermutation,
            delegate*<int, int, int> queryCompareFn)
        {
            int* indices = stackalloc int[n];
            for (int i = 0; i < n; i++) indices[i] = i;
            InteractiveQuicksort(indices, 0, n - 1, queryCompareFn);
            for (int i = 0; i < n; i++) recoveredPermutation[indices[i]] = i;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void InteractiveQuicksort(
            int* arr,
            int low,
            int high,
            delegate*<int, int, int> cmp)
        {
            if (low < high)
            {
                int p = InteractivePartition(arr, low, high, cmp);
                InteractiveQuicksort(arr, low, p - 1, cmp);
                InteractiveQuicksort(arr, p + 1, high, cmp);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int InteractivePartition(
            int* arr,
            int low,
            int high,
            delegate*<int, int, int> cmp)
        {
            int pivot = arr[high];
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (cmp(arr[j], pivot) < 0)
                {
                    i++;
                    int temp = arr[i]; arr[i] = arr[j]; arr[j] = temp;
                }
            }
            int temp2 = arr[i + 1]; arr[i + 1] = arr[high]; arr[high] = temp2;
            return i + 1;
        }
    }
}