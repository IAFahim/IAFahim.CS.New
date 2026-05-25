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
            if (maxDim > Epsilon)
            {
                QuickSort(items, start, end, axis);
            }

            int mid = start + count / 2;

            node->TriangleIndex = -1;
            node->Left = BuildNode(vertices, indices, items, start, mid - 1, outNodes, outNodeCount);
            node->Right = BuildNode(vertices, indices, items, mid, end, outNodes, outNodeCount);

            return nodeIdx;
        }

        private static void QuickSort(CentroidSortItem* items, int left, int right, int axis)
        {
            if (left >= right)
            {
                return;
            }
            int pivot = Partition(items, left, right, axis);
            QuickSort(items, left, pivot - 1, axis);
            QuickSort(items, pivot + 1, right, axis);
        }

        private static int Partition(CentroidSortItem* items, int left, int right, int axis)
        {
            CentroidSortItem pivotValue = items[right];
            int i = left - 1;
            for (int j = left; j < right; j++)
            {
                bool less = false;
                if (axis == 0) less = items[j].Centroid.x < pivotValue.Centroid.x;
                else if (axis == 1) less = items[j].Centroid.y < pivotValue.Centroid.y;
                else less = items[j].Centroid.z < pivotValue.Centroid.z;

                if (less)
                {
                    i++;
                    CentroidSortItem temp = items[i];
                    items[i] = items[j];
                    items[j] = temp;
                }
            }
            CentroidSortItem t1 = items[i + 1];
            items[i + 1] = items[right];
            items[right] = t1;
            return i + 1;
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
            float3 invDir;
            invDir.x = math.abs(direction.x) > Epsilon ? One / direction.x : (direction.x >= Zero ? 1e6f : -1e6f);
            invDir.y = math.abs(direction.y) > Epsilon ? One / direction.y : (direction.y >= Zero ? 1e6f : -1e6f);
            invDir.z = math.abs(direction.z) > Epsilon ? One / direction.z : (direction.z >= Zero ? 1e6f : -1e6f);

            float3 t1 = (boxMin - origin) * invDir;
            float3 t2 = (boxMax - origin) * invDir;

            float3 tMin = math.min(t1, t2);
            float3 tMax = math.max(t1, t2);

            float tEnter = math.max(tMin.x, math.max(tMin.y, tMin.z));
            float tExit = math.min(tMax.x, math.min(tMax.y, tMax.z));

            if (math.isnan(tEnter) || math.isnan(tExit))
            {
                return false;
            }

            return tEnter <= tExit && tExit >= Zero;
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
