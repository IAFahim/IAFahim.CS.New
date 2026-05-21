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
                int next = trie[cur * 27 + c];
                if (next == 0)
                {
                    next = ++trie[0];
                    trie[cur * 27 + c] = next;
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
            if (trie[cur * 27 + 26] <= 0) return false;
            trie[cur * 27 + 26]--;
            for (int i = pathLen - 1; i >= 0; i--)
            {
                int p = path[i];
                bool hasChild = false;
                for (int c = 0; c < 26; c++)
                {
                    if (trie[p * 27 + c] != 0) { hasChild = true; break; }
                }
                if (hasChild) break;
                int pc = s[i] - 'a';
                trie[p * 27 + pc] = 0;
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
            int sum = trie[cur * 27 + 26];
            for (int c = 0; c < 26; c++)
            {
                int next = trie[cur * 27 + c];
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
                int next = trie[cur * 2 + b];
                if (next == 0)
                {
                    next = ++trie[0];
                    trie[cur * 2 + b] = next;
                }
                cur = next;
            }
            trie[cur * 2]++;
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
                cur = trie[cur * 2 + b];
            }
            trie[cur * 2]--;
            for (int i = pathLen - 1; i >= 0; i--)
            {
                int b = (val >> (bits - 1 - i)) & 1;
                int child = trie[path[i] * 2 + b];
                if (trie[child * 2] == 0 && trie[child * 2 + 0] == 0 && trie[child * 2 + 1] == 0)
                    trie[path[i] * 2 + b] = 0;
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
                if (trie[cur * 2 + want] != 0)
                {
                    result |= (1 << i);
                    cur = trie[cur * 2 + want];
                }
                else
                {
                    cur = trie[cur * 2 + b];
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
                if (trie[cur * 2 + want] != 0)
                {
                    cur = trie[cur * 2 + want];
                }
                else
                {
                    result |= (1 << i);
                    cur = trie[cur * 2 + (1 - want)];
                }
            }
            return result;
        }
    }

    public static unsafe class PersistentTrieInsert
    {
        public static int Run(int* oldTrie, int oldNode, int val, int bits, int* newTrie)
        {
            int cur = oldNode;
            int newCur = 0;
            for (int i = bits - 1; i >= 0; i--)
            {
                int b = (val >> i) & 1;
                int oldNext = oldTrie[cur * 2 + b];
                int newNext = ++newTrie[0];
                newTrie[newCur * 2 + b] = newNext;
                newCur = newNext;
                cur = oldNext;
            }
            newTrie[newCur * 2]++;
            return newCur;
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
                int next = trie[cur * 2 + b];
                if (next == 0)
                {
                    next = trie[cur * 2 + (1 - b)];
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
