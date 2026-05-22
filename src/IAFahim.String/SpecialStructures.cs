namespace IAFahim.String
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct XmlNode
    {
        public int TagHash;
        public int ValueHash;
        public int ChildStart;
        public int ChildCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct JsonElement
    {
        public int KeyHash;
        public int ValueHash;
        public int Type;
        public int ChildStart;
        public int ChildCount;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ChildInfo
    {
        public int KeyHash;
        public uint HashVal;
    }

    public static unsafe class SpecialStructures
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HashStart()
        {
            return 2166136261u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static uint HashUpdate(uint hash, uint value)
        {
            hash ^= value;
            hash *= 16777619u;
            return hash;
        }

        public static uint XmlTreeHash(XmlNode* nodes, int nodeIndex, int* childIndices, uint* calculatedHashes)
        {
            XmlNode node = nodes[nodeIndex];
            if (node.ChildCount == 0)
            {
                uint hash = HashStart();
                hash = HashUpdate(hash, (uint)node.TagHash);
                hash = HashUpdate(hash, (uint)node.ValueHash);
                calculatedHashes[nodeIndex] = hash;
                return hash;
            }
            uint* tempHashes = stackalloc uint[node.ChildCount];
            for (int i = 0; i < node.ChildCount; i++)
            {
                int childIdx = childIndices[node.ChildStart + i];
                tempHashes[i] = XmlTreeHash(nodes, childIdx, childIndices, calculatedHashes);
            }
            for (int i = 1; i < node.ChildCount; i++)
            {
                uint key = tempHashes[i];
                int j = i - 1;
                while (j >= 0 && tempHashes[j] > key)
                {
                    tempHashes[j + 1] = tempHashes[j];
                    j--;
                }
                tempHashes[j + 1] = key;
            }
            uint nodeHash = HashStart();
            nodeHash = HashUpdate(nodeHash, (uint)node.TagHash);
            nodeHash = HashUpdate(nodeHash, (uint)node.ValueHash);
            for (int i = 0; i < node.ChildCount; i++)
            {
                nodeHash = HashUpdate(nodeHash, tempHashes[i]);
            }
            calculatedHashes[nodeIndex] = nodeHash;
            return nodeHash;
        }

        public static uint JsonCanonicalHash(JsonElement* elements, int elementIndex, int* childIndices, uint* calculatedHashes)
        {
            JsonElement elem = elements[elementIndex];
            if (elem.Type == 0)
            {
                uint hash = HashStart();
                hash = HashUpdate(hash, (uint)elem.KeyHash);
                hash = HashUpdate(hash, (uint)elem.ValueHash);
                calculatedHashes[elementIndex] = hash;
                return hash;
            }
            ChildInfo* info = stackalloc ChildInfo[elem.ChildCount];
            for (int i = 0; i < elem.ChildCount; i++)
            {
                int childIdx = childIndices[elem.ChildStart + i];
                info[i].KeyHash = elements[childIdx].KeyHash;
                info[i].HashVal = JsonCanonicalHash(elements, childIdx, childIndices, calculatedHashes);
            }
            if (elem.Type == 1)
            {
                for (int i = 1; i < elem.ChildCount; i++)
                {
                    ChildInfo key = info[i];
                    int j = i - 1;
                    while (j >= 0 && info[j].KeyHash > key.KeyHash)
                    {
                        info[j + 1] = info[j];
                        j--;
                    }
                    info[j + 1] = key;
                }
            }
            uint nodeHash = HashStart();
            nodeHash = HashUpdate(nodeHash, (uint)elem.KeyHash);
            nodeHash = HashUpdate(nodeHash, (uint)elem.ValueHash);
            for (int i = 0; i < elem.ChildCount; i++)
            {
                nodeHash = HashUpdate(nodeHash, info[i].HashVal);
            }
            calculatedHashes[elementIndex] = nodeHash;
            return nodeHash;
        }
    }
}
