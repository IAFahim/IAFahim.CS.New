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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Push(int id, int freq)
        {
            int idx = Size++;
            Freq[idx] = freq;
            Id[idx] = id;
            while (idx > 0)
            {
                int p = (idx - 1) >> 1;
                if (Freq[p] <= Freq[idx]) break;
                int tmpF = Freq[p]; Freq[p] = Freq[idx]; Freq[idx] = tmpF;
                int tmpI = Id[p]; Id[p] = Id[idx]; Id[idx] = tmpI;
                idx = p;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
                int left = (idx << 1) + 1;
                while (left < Size)
                {
                    int right = left + 1;
                    int smallest = left;
                    if (right < Size && Freq[right] < Freq[left]) smallest = right;
                    if (Freq[idx] <= Freq[smallest]) break;
                    int tmpF = Freq[idx]; Freq[idx] = Freq[smallest]; Freq[smallest] = tmpF;
                    int tmpI = Id[idx]; Id[idx] = Id[smallest]; Id[smallest] = tmpI;
                    idx = smallest;
                    left = (idx << 1) + 1;
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

        public static void Build(int* freq, int sigma, Code* codes)
        {
            Node* nodes = stackalloc Node[2 * sigma];
            int nodeCount = 0;
            var pq = new NodeHeap(2 * sigma);
            for (int c = 0; c < sigma; c++)
            {
                if (freq[c] > 0)
                {
                    nodes[nodeCount] = new Node { Freq = freq[c], Symbol = c, Left = -1, Right = -1 };
                    pq.Push(nodeCount, freq[c]);
                    nodeCount++;
                }
            }
            if (pq.Size == 0) { pq.Dispose(); return; }
            while (pq.Size > 1)
            {
                int leftId = pq.Pop(out int leftFreq);
                int rightId = pq.Pop(out int rightFreq);
                int parentId = nodeCount++;
                nodes[parentId] = new Node { Freq = leftFreq + rightFreq, Symbol = -1, Left = leftId, Right = rightId };
                pq.Push(parentId, nodes[parentId].Freq);
            }
            int rootId = pq.Pop(out _);
            Traverse(rootId, nodes, nodeCount, codes);
            pq.Dispose();
        }

        private static void Traverse(int rootId, Node* nodes, int capacity, Code* codes)
        {
            // Iterative depth-first walk over the Huffman tree.
            // The tree has at most `capacity` nodes, so an explicit stack of that
            // size can never overflow (each node is pushed at most once).
            int* stackId = stackalloc int[capacity];
            int* stackLength = stackalloc int[capacity];
            long* stackBits = stackalloc long[capacity];
            int sp = 0;
            stackId[sp] = rootId;
            stackLength[sp] = 0;
            stackBits[sp] = 0;
            sp++;
            while (sp > 0)
            {
                sp--;
                int id = stackId[sp];
                int length = stackLength[sp];
                long bits = stackBits[sp];
                int leftChild = nodes[id].Left;
                int rightChild = nodes[id].Right;
                if (leftChild == -1 && rightChild == -1)
                {
                    codes[nodes[id].Symbol] = new Code { Length = length, Bits = bits };
                    continue;
                }
                // Push right first so left is processed first (LIFO), preserving
                // left = bits<<1, right = (bits<<1)|1 code assignment order.
                if (rightChild != -1)
                {
                    stackId[sp] = rightChild;
                    stackLength[sp] = length + 1;
                    stackBits[sp] = (bits << 1) | 1;
                    sp++;
                }
                if (leftChild != -1)
                {
                    stackId[sp] = leftChild;
                    stackLength[sp] = length + 1;
                    stackBits[sp] = bits << 1;
                    sp++;
                }
            }
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

        private const int SymbolCount = 256;

        public static void Decode(int* input, int inLen, Code* codes, byte* output, int* outLen)
        {
            int target = *outLen;
            int opos = 0;
            long curBits = 0;
            int curLen = 0;
            // Read bits MSB-first from each input int (bit 31 down to bit 0),
            // mirroring Encode's MSB-first packing. Accumulate a prefix and
            // emit a symbol as soon as the prefix-free code table matches.
            for (int ipos = 0; ipos < inLen && opos < target; ipos++)
            {
                uint word = (uint)input[ipos];
                for (int b = 31; b >= 0 && opos < target; b--)
                {
                    int bit = (int)((word >> b) & 1u);
                    curBits = (curBits << 1) | (uint)bit;
                    curLen++;
                    for (int s = 0; s < SymbolCount; s++)
                    {
                        if (codes[s].Length == curLen && codes[s].Bits == curBits)
                        {
                            output[opos++] = (byte)s;
                            curBits = 0;
                            curLen = 0;
                            break;
                        }
                    }
                }
            }
            *outLen = opos;
        }
    }
}
