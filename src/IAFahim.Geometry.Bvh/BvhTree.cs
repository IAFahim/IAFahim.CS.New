namespace IAFahim.Geometry.Bvh
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.CompilerServices;
    using Unity.Mathematics;

    [StructLayout(LayoutKind.Sequential)]
    public struct BvhNode
    {
        public float3 Min;
        public float3 Max;
        public int Left;
        public int Right;
        public int TriangleIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CentroidSortItem
    {
        public float3 Centroid;
        public int TriangleIdx;
    }

    public static unsafe class BvhTree
    {
        private const float Epsilon = 1e-6f;
        private const float LargeValue = 1e30f;
        private const float Zero = 0.0f;
        private const float One = 1.0f;
        private const float Half = 0.5f;

        public static int Build(
            float3* vertices, 
            int* indices, 
            int indexCount, 
            BvhNode* outNodes, 
            int* outNodeCount)
        {
            *outNodeCount = 0;
            int triangleCount = indexCount / 3;
            if (triangleCount <= 0)
            {
                return -1;
            }

            CentroidSortItem* items = stackalloc CentroidSortItem[triangleCount];
            for (int i = 0; i < triangleCount; i++)
            {
                float3 v0 = vertices[indices[i * 3]];
                float3 v1 = vertices[indices[i * 3 + 1]];
                float3 v2 = vertices[indices[i * 3 + 2]];
                items[i].Centroid = (v0 + v1 + v2) / 3.0f;
                items[i].TriangleIdx = i;
            }

            return BuildNode(vertices, indices, items, 0, triangleCount - 1, outNodes, outNodeCount);
        }

        private static int BuildNode(
            float3* vertices, 
            int* indices, 
            CentroidSortItem* items, 
            int start, 
            int end, 
            BvhNode* outNodes, 
            int* outNodeCount)
        {
            int nodeIdx = (*outNodeCount)++;
            BvhNode* node = &outNodes[nodeIdx];

            float3 boxMin = new float3(LargeValue, LargeValue, LargeValue);
            float3 boxMax = new float3(-LargeValue, -LargeValue, -LargeValue);

            for (int i = start; i <= end; i++)
            {
                int triIdx = items[i].TriangleIdx;
                for (int j = 0; j < 3; j++)
                {
                    float3 v = vertices[indices[triIdx * 3 + j]];
                    boxMin = math.min(boxMin, v);
                    boxMax = math.max(boxMax, v);
                }
            }

            node->Min = boxMin;
            node->Max = boxMax;

            int count = end - start + 1;
            if (count == 1)
            {
                node->Left = -1;
                node->Right = -1;
                node->TriangleIndex = items[start].TriangleIdx;
                return nodeIdx;
            }

            float3 size = boxMax - boxMin;
            int axis = 0;
            if (size.y > size.x) axis = 1;
            if (size.z > math.max(size.x, size.y)) axis = 2;

            float maxDim = math.max(size.x, math.max(size.y, size.z));
            int mid = start + count / 2;

            if (maxDim > Epsilon)
            {
                NthElement(items, start, end, mid, axis);
            }

            node->TriangleIndex = -1;
            node->Left = BuildNode(vertices, indices, items, start, mid - 1, outNodes, outNodeCount);
            node->Right = BuildNode(vertices, indices, items, mid, end, outNodes, outNodeCount);

            return nodeIdx;
        }

        // Iterative three-way quickselect (Dutch national flag) placing the
        // nth-smallest element of items[left..right] at position nth and
        // partitioning around it. No recursion -> no stack overflow; the
        // equal-range handling makes all-equal input O(n).
        private static void NthElement(CentroidSortItem* items, int left, int right, int nth, int axis)
        {
            while (left < right)
            {
                int mid = left + ((right - left) >> 1);
                CentroidSortItem pivotValue = MedianOfThree(items[left], items[mid], items[right], axis);

                int lt = left, gt = right, i = left;
                while (i <= gt)
                {
                    int c = CompareAxis(items[i], pivotValue, axis);
                    if (c < 0) { Swap(ref items[lt], ref items[i]); lt++; i++; }
                    else if (c > 0) { Swap(ref items[i], ref items[gt]); gt--; }
                    else i++;
                }

                if (nth < lt) right = lt - 1;
                else if (nth > gt) left = gt + 1;
                else return;
            }
        }

        private static int CompareAxis(CentroidSortItem a, CentroidSortItem b, int axis)
        {
            float av = axis == 0 ? a.Centroid.x : (axis == 1 ? a.Centroid.y : a.Centroid.z);
            float bv = axis == 0 ? b.Centroid.x : (axis == 1 ? b.Centroid.y : b.Centroid.z);
            if (av < bv) return -1;
            if (av > bv) return 1;
            return 0;
        }

        private static CentroidSortItem MedianOfThree(CentroidSortItem a, CentroidSortItem b, CentroidSortItem c, int axis)
        {
            int ab = CompareAxis(a, b, axis);
            int bc = CompareAxis(b, c, axis);
            int ac = CompareAxis(a, c, axis);
            if (ab <= 0 && bc <= 0) return b;
            if (ab >= 0 && bc >= 0) return b;
            if (ac <= 0 && ab >= 0) return a;
            if (ac >= 0 && ab <= 0) return a;
            return c;
        }

        private static void Swap(ref CentroidSortItem a, ref CentroidSortItem b)
        {
            CentroidSortItem t = a; a = b; b = t;
        }

        public static bool Raycast(
            BvhNode* nodes, 
            float3* vertices, 
            int* indices, 
            int rootIdx, 
            float3 rayOrigin, 
            float3 rayDirection, 
            float* outDist, 
            int* outTriangleIdx)
        {
            float bestDist = LargeValue;
            int bestTriIdx = -1;

            RaycastNode(nodes, vertices, indices, rootIdx, rayOrigin, rayDirection, &bestDist, &bestTriIdx);

            if (bestTriIdx != -1)
            {
                *outDist = bestDist;
                *outTriangleIdx = bestTriIdx;
                return true;
            }

            *outDist = Zero;
            *outTriangleIdx = -1;
            return false;
        }

        private static void RaycastNode(
            BvhNode* nodes, 
            float3* vertices, 
            int* indices, 
            int nodeIdx, 
            float3 origin, 
            float3 direction, 
            float* bestDist, 
            int* bestTriIdx)
        {
            if (nodeIdx == -1)
            {
                return;
            }

            BvhNode* node = &nodes[nodeIdx];
            if (!RayAABBIntersection(origin, direction, node->Min, node->Max))
            {
                return;
            }

            if (node->TriangleIndex != -1)
            {
                float t;
                if (RayTriangleIntersection(origin, direction, vertices, indices, node->TriangleIndex, &t))
                {
                    if (t < *bestDist)
                    {
                        *bestDist = t;
                        *bestTriIdx = node->TriangleIndex;
                    }
                }
                return;
            }

            RaycastNode(nodes, vertices, indices, node->Left, origin, direction, bestDist, bestTriIdx);
            RaycastNode(nodes, vertices, indices, node->Right, origin, direction, bestDist, bestTriIdx);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool RayAABBIntersection(float3 origin, float3 direction, float3 boxMin, float3 boxMax)
        {
            float tEnter = float.NegativeInfinity;
            float tExit = float.PositiveInfinity;

            ProcessSlab(origin.x, direction.x, boxMin.x, boxMax.x, ref tEnter, ref tExit);
            ProcessSlab(origin.y, direction.y, boxMin.y, boxMax.y, ref tEnter, ref tExit);
            ProcessSlab(origin.z, direction.z, boxMin.z, boxMax.z, ref tEnter, ref tExit);

            if (math.isnan(tEnter) || math.isnan(tExit)) return false;
            return tEnter <= tExit && tExit >= Zero;
        }

        // Slab test: for a non-parallel ray intersect the two slab planes; for a ray
        // parallel to the slab (direction ~ 0) the ray hits only if the origin lies
        // within [min, max] on this axis, otherwise it misses entirely.
        private static void ProcessSlab(float o, float d, float min, float max, ref float tEnter, ref float tExit)
        {
            if (math.abs(d) > Epsilon)
            {
                float inv = One / d;
                float ta = (min - o) * inv;
                float tb = (max - o) * inv;
                float tlo = ta < tb ? ta : tb;
                float thi = ta < tb ? tb : ta;
                if (tlo > tEnter) tEnter = tlo;
                if (thi < tExit) tExit = thi;
            }
            else
            {
                if (o < min || o > max)
                {
                    tEnter = float.PositiveInfinity;
                    tExit = float.NegativeInfinity;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool RayTriangleIntersection(
            float3 origin, 
            float3 direction, 
            float3* vertices, 
            int* indices, 
            int triIdx, 
            float* outT)
        {
            float3 v0 = vertices[indices[triIdx * 3]];
            float3 v1 = vertices[indices[triIdx * 3 + 1]];
            float3 v2 = vertices[indices[triIdx * 3 + 2]];

            float3 edge1 = v1 - v0;
            float3 edge2 = v2 - v0;
            float3 h = math.cross(direction, edge2);
            float a = math.dot(edge1, h);

            if (a > -Epsilon && a < Epsilon)
            {
                return false;
            }

            float f = One / a;
            float3 s = origin - v0;
            float u = f * math.dot(s, h);

            if (u < Zero || u > One)
            {
                return false;
            }

            float3 q = math.cross(s, edge1);
            float v = f * math.dot(direction, q);

            if (v < Zero || u + v > One)
            {
                return false;
            }

            float t = f * math.dot(edge2, q);
            if (t > Epsilon)
            {
                *outT = t;
                return true;
            }

            return false;
        }
    }
}
