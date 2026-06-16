namespace IAFahim.DS.Trie
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class TrieInsert
    {
        public static void Run(int* trie, int node, byte* s, int len)
        {
            int cur = node;
            for (int i = 0; i < len; i++)
            {
                int c = s[i] - 'a';
                int baseIndex = cur * 27;
                int next = trie[baseIndex + c];
                if (next == 0)
                {
                    next = ++trie[0];
                    trie[baseIndex + c] = next;
                }
                cur = next;
            }
            trie[cur * 27 + 26]++;
        }
    }

    public static unsafe class TrieDelete
    {
        public static bool Run(int* trie, int node, byte* s, int len)
        {
            int cur = node;
            int* path = stackalloc int[256];
            int pathLen = 0;
            for (int i = 0; i < len; i++)
            {
                int c = s[i] - 'a';
                int next = trie[cur * 27 + c];
                if (next == 0) return false;
                path[pathLen++] = cur;
                cur = next;
            }
            int leafCount = cur * 27 + 26;
            if (trie[leafCount] <= 0) return false;
            trie[leafCount]--;
            for (int i = pathLen - 1; i >= 0; i--)
            {
                int p = path[i];
                int pbase = p * 27;
                bool hasChild = false;
                for (int c = 0; c < 26; c++)
                {
                    if (trie[pbase + c] != 0) { hasChild = true; break; }
                }
                if (hasChild) break;
                int pc = s[i] - 'a';
                trie[pbase + pc] = 0;
            }
            return true;
        }
    }

    public static unsafe class TrieFind
    {
        public static bool Run(int* trie, int node, byte* s, int len)
        {
            int cur = node;
            for (int i = 0; i < len; i++)
            {
                int c = s[i] - 'a';
                int next = trie[cur * 27 + c];
                if (next == 0) return false;
                cur = next;
            }
            return trie[cur * 27 + 26] > 0;
        }
    }

    public static unsafe class TriePrefixCount
    {
        public static int Run(int* trie, int node, byte* s, int len)
        {
            int cur = node;
            for (int i = 0; i < len; i++)
            {
                int c = s[i] - 'a';
                int next = trie[cur * 27 + c];
                if (next == 0) return 0;
                cur = next;
            }
            return SumSubtree(trie, cur);
        }

        private static int SumSubtree(int* trie, int cur)
        {
            int baseIndex = cur * 27;
            int sum = trie[baseIndex + 26];
            for (int c = 0; c < 26; c++)
            {
                int next = trie[baseIndex + c];
                if (next != 0)
                {
                    sum += SumSubtree(trie, next);
                }
            }
            return sum;
        }
    }

    public static unsafe class BinaryTrieInsert
    {
        public static void Run(int* trie, int node, int val, int bits)
        {
            int cur = node;
            for (int i = bits - 1; i >= 0; i--)
            {
                int b = (val >> i) & 1;
                int childBase = cur * 3 + 1;
                int next = trie[childBase + b];
                if (next == 0)
                {
                    next = ++trie[0];
                    trie[childBase + b] = next;
                }
                cur = next;
            }
            trie[cur * 3 + 3]++;
        }
    }

    public static unsafe class BinaryTrieErase
    {
        public static void Run(int* trie, int node, int val, int bits)
        {
            int cur = node;
            int* path = stackalloc int[64];
            int pathLen = 0;
            for (int i = bits - 1; i >= 0; i--)
            {
                int b = (val >> i) & 1;
                path[pathLen++] = cur;
                cur = trie[cur * 3 + 1 + b];
            }
            trie[cur * 3 + 3]--;
            for (int i = pathLen - 1; i >= 0; i--)
            {
                int b = (val >> (bits - 1 - i)) & 1;
                int pbase = path[i] * 3 + 1;
                int child = trie[pbase + b];
                int childBase = child * 3 + 1;
                if (trie[childBase + 2] == 0 && trie[childBase] == 0 && trie[childBase + 1] == 0)
                    trie[pbase + b] = 0;
            }
        }
    }

    public static unsafe class BinaryTrieMaxXor
    {
        public static int Run(int* trie, int node, int val, int bits)
        {
            int cur = node;
            int result = 0;
            for (int i = bits - 1; i >= 0; i--)
            {
                int b = (val >> i) & 1;
                int want = 1 - b;
                int childBase = cur * 3 + 1;
                int chosen = trie[childBase + want];
                if (chosen != 0)
                {
                    result |= (1 << i);
                    cur = chosen;
                }
                else
                {
                    cur = trie[childBase + b];
                }
            }
            return result;
        }
    }

    public static unsafe class BinaryTrieMinXor
    {
        public static int Run(int* trie, int node, int val, int bits)
        {
            int cur = node;
            int result = 0;
            for (int i = bits - 1; i >= 0; i--)
            {
                int b = (val >> i) & 1;
                int want = b;
                int childBase = cur * 3 + 1;
                int chosen = trie[childBase + want];
                if (chosen != 0)
                {
                    cur = chosen;
                }
                else
                {
                    result |= (1 << i);
                    cur = trie[childBase + (1 - want)];
                }
            }
            return result;
        }
    }

    public static unsafe class PersistentTrieInsert
    {
        // Persistent (path-copying) binary trie insert.
        // Old and new versions share a single backing array `trie` (node 0 slot holds
        // the node-allocation counter, matching the rest of this file and the house
        // PersistentSegment convention). `oldRoot` is the root node id of the version
        // being extended (0 if starting from an empty version). A fresh spine is
        // allocated along the inserted path; at every copied node the un-followed
        // sibling pointer and the count are carried over from the old node so the new
        // version shares all previously inserted values. Returns the new root node id.
        public static int Run(int* trie, int oldRoot, int val, int bits)
        {
            int newRoot = ++trie[0];
            CopyNode(trie, newRoot, oldRoot);
            int newCur = newRoot;
            int cur = oldRoot;
            for (int i = bits - 1; i >= 0; i--)
            {
                int b = (val >> i) & 1;
                int oldNext = cur == 0 ? 0 : trie[cur * 3 + 1 + b];
                int newNext = ++trie[0];
                CopyNode(trie, newNext, oldNext);
                trie[newCur * 3 + 1 + b] = newNext;
                newCur = newNext;
                cur = oldNext;
            }
            trie[newCur * 3 + 3]++;
            return newRoot;
        }

        // Copy an existing node `src` (both child pointers and the count) into the
        // freshly allocated node `dst`. A `src` of 0 denotes "no previous node", so the
        // destination is cleared to a fresh empty node.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void CopyNode(int* trie, int dst, int src)
        {
            int dstBase = dst * 3 + 1;
            if (src == 0)
            {
                trie[dstBase] = 0;
                trie[dstBase + 1] = 0;
                trie[dstBase + 2] = 0;
            }
            else
            {
                int srcBase = src * 3 + 1;
                trie[dstBase] = trie[srcBase];
                trie[dstBase + 1] = trie[srcBase + 1];
                trie[dstBase + 2] = trie[srcBase + 2];
            }
        }
    }

    public static unsafe class PersistentTrieQuery
    {
        public static int Run(int* trie, int node, int val, int bits)
        {
            int cur = node;
            int result = 0;
            for (int i = bits - 1; i >= 0; i--)
            {
                int b = (val >> i) & 1;
                int childBase = cur * 3 + 1;
                int next = trie[childBase + b];
                if (next == 0)
                {
                    next = trie[childBase + (1 - b)];
                }
                else
                {
                    result |= (1 << i);
                }
                cur = next;
            }
            return result;
        }
    }
}
