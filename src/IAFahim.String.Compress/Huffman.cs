namespace IAFahim.String.Compress
{
using System.Runtime.InteropServices;
    using System;
    
    using System.Runtime.CompilerServices;

    internal unsafe struct NodeHeap
    {
        public int* Freq;
        public int* Id;
        public int Size;

        public NodeHeap(int capacity)
        {
            Freq = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(capacity * sizeof(int));
            Id = (int*)System.Runtime.InteropServices.Marshal.AllocHGlobal(capacity * sizeof(int));
            Size = 0;
        }

        public void Dispose()
        {
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)Freq);
            System.Runtime.InteropServices.Marshal.FreeHGlobal((nint)Id);
        }

        public void Push(int id, int freq)
        {
            int idx = Size++;
            Freq[idx] = freq;
            Id[idx] = id;
            while (idx > 0)
            {
                int p = (idx - 1) / 2;
                if (Freq[p] <= Freq[idx]) break;
                int tmpF = Freq[p]; Freq[p] = Freq[idx]; Freq[idx] = tmpF;
                int tmpI = Id[p]; Id[p] = Id[idx]; Id[idx] = tmpI;
                idx = p;
            }
        }

        public int Pop(out int freq)
        {
            int id = Id[0];
            freq = Freq[0];
            Size--;
            if (Size > 0)
            {
                Freq[0] = Freq[Size];
                Id[0] = Id[Size];
                int idx = 0;
                while (idx * 2 + 1 < Size)
                {
                    int left = idx * 2 + 1;
                    int right = idx * 2 + 2;
                    int smallest = left;
                    if (right < Size && Freq[right] < Freq[left]) smallest = right;
                    if (Freq[idx] <= Freq[smallest]) break;
                    int tmpF = Freq[idx]; Freq[idx] = Freq[smallest]; Freq[smallest] = tmpF;
                    int tmpI = Id[idx]; Id[idx] = Id[smallest]; Id[smallest] = tmpI;
                    idx = smallest;
                }
            }
            return id;
        }
    }

    public static unsafe class Huffman
    {
        public struct Code
        {
            public int Length;
            public long Bits;
        }

        public struct Node
        {
            public int Freq;
            public int Symbol;
            public int Left;
            public int Right;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Build(int* freq, int sigma, Code* codes)
        {
            Node* nodes = stackalloc Node[2 * sigma];
            int nodeCount = 0;
            var pq = new NodeHeap(2 * sigma);
            try
            {
                for (int c = 0; c < sigma; c++)
                {
                    if (freq[c] > 0)
                    {
                        nodes[nodeCount] = new Node { Freq = freq[c], Symbol = c, Left = -1, Right = -1 };
                        pq.Push(nodeCount, freq[c]);
                        nodeCount++;
                    }
                }
                if (pq.Size == 0) return;
                while (pq.Size > 1)
                {
                    int leftId = pq.Pop(out int leftFreq);
                    int rightId = pq.Pop(out int rightFreq);
                    int parentId = nodeCount++;
                    nodes[parentId] = new Node { Freq = leftFreq + rightFreq, Symbol = -1, Left = leftId, Right = rightId };
                    pq.Push(parentId, nodes[parentId].Freq);
                }
                int rootId = pq.Pop(out _);
                Traverse(rootId, nodes, 0, 0, codes);
            }
            finally { pq.Dispose(); }
        }

        private static void Traverse(int id, Node* nodes, int length, long bits, Code* codes)
        {
            if (nodes[id].Left == -1 && nodes[id].Right == -1)
            {
                codes[nodes[id].Symbol] = new Code { Length = length, Bits = bits };
                return;
            }
            if (nodes[id].Left != -1) Traverse(nodes[id].Left, nodes, length + 1, bits << 1, codes);
            if (nodes[id].Right != -1) Traverse(nodes[id].Right, nodes, length + 1, (bits << 1) | 1, codes);
        }

        public static void Encode(byte* input, int len, int* output, int* outLen, Code* codes)
        {
            long buffer = 0;
            int bits = 0;
            int pos = 0;
            for (int i = 0; i < len; i++)
            {
                var code = codes[input[i]];
                buffer = (buffer << code.Length) | code.Bits;
                bits += code.Length;
                while (bits >= 32)
                {
                    output[pos++] = (int)(buffer >> (bits - 32));
                    bits -= 32;
                }
            }
            if (bits > 0)
                output[pos++] = (int)(buffer << (32 - bits));
            *outLen = pos;
        }

        public static void Decode(int* input, int inLen, Code* codes, byte* output, int* outLen)
        {
            long buffer = 0;
            int bits = 0;
            int ipos = 0;
            int opos = 0;
            while (ipos < inLen && opos < *outLen)
            {
                while (bits < 32 && ipos < inLen)
                {
                    buffer = (buffer << 32) | ((uint)input[ipos++]);
                    bits += 32;
                }
            }
        }
    }
}
