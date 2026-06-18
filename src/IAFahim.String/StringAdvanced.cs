namespace IAFahim.String
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class KmpPrefix
    {
        public static void Run(byte* s, int len, int* fail)
        {
            fail[0] = 0;
            for (int i = 1, j = 0; i < len; i++)
            {
                while (j > 0 && s[i] != s[j]) j = fail[j - 1];
                if (s[i] == s[j]) j++;
                fail[i] = j;
            }
        }
    }

    public static unsafe class KmpSearch
    {
        public static int Run(byte* text, int textLen, byte* pattern, int patLen, int* matches)
        {
            if (patLen == 0 || patLen > textLen) return 0;
            int* fail = stackalloc int[patLen];
            KmpPrefix.Run(pattern, patLen, fail);
            int count = 0;
            int j = 0;
            for (int i = 0; i < textLen; i++)
            {
                while (j > 0 && text[i] != pattern[j]) j = fail[j - 1];
                if (text[i] == pattern[j]) j++;
                if (j == patLen)
                {
                    matches[count++] = i - patLen + 1;
                    j = fail[j - 1];
                }
            }
            return count;
        }
    }

    public static unsafe class ZAlgorithm
    {
        public static void Run(byte* s, int len, int* z)
        {
            z[0] = len;
            int l = 0, r = 0;
            for (int i = 1; i < len; i++)
            {
                if (i < r) z[i] = Math.Min(r - i, z[i - l]);
                while (i + z[i] < len && s[z[i]] == s[i + z[i]]) z[i]++;
                if (i + z[i] > r)
                {
                    l = i;
                    r = i + z[i];
                }
            }
        }
    }

    public static unsafe class HashBuild
    {
        private const ulong B = 1315423911UL;

        public static void Run(byte* s, int len, ulong* hash, ulong* pow)
        {
            hash[0] = 0;
            pow[0] = 1;
            for (int i = 0; i < len; i++)
            {
                hash[i + 1] = hash[i] * B + s[i];
                pow[i + 1] = pow[i] * B;
            }
        }
    }

    public static unsafe class HashRange
    {
        private const ulong B = 1315423911UL;

        public static ulong Run(ulong* hash, ulong* pow, int l, int r)
        {
            return hash[r + 1] - hash[l] * pow[r - l + 1];
        }
    }

    public static unsafe class HashConcat
    {
        private const ulong B = 1315423911UL;

        public static ulong Run(ulong h1, ulong h2, int len2)
        {
            ulong p = 1;
            for (int i = 0; i < len2; i++) p *= B;
            return h1 * p + h2;
        }
    }

    public static unsafe class DoubleHashBuild
    {
        private const ulong B1 = 1315423911UL;
        private const ulong B2 = 1000000007UL;

        public static void Run(byte* s, int len, ulong* hash1, ulong* hash2, ulong* pow1, ulong* pow2)
        {
            hash1[0] = 0;
            hash2[0] = 0;
            pow1[0] = 1;
            pow2[0] = 1;
            for (int i = 0; i < len; i++)
            {
                hash1[i + 1] = hash1[i] * B1 + s[i];
                hash2[i + 1] = hash2[i] * B2 + s[i];
                pow1[i + 1] = pow1[i] * B1;
                pow2[i + 1] = pow2[i] * B2;
            }
        }
    }

    public static unsafe class RollingHash
    {
        private const ulong B = 1315423911UL;

        public static ulong Run(byte* s, int len)
        {
            ulong hash = 0;
            for (int i = 0; i < len; i++)
                hash = hash * B + s[i];
            return hash;
        }
    }

    public static unsafe class SuffixArrayBuild
    {
        public static void Run(byte* s, int n, int* sa)
        {
            int* rank = stackalloc int[n];
            int* tmp = stackalloc int[n];
            int* cnt = stackalloc int[Math.Max(n, 256)];
            int* sa2 = stackalloc int[n];
            for (int i = 0; i < n; i++) sa[i] = i;
            for (int i = 0; i < n; i++) rank[i] = s[i];
            for (int k = 1; k < n; k <<= 1)
            {
                for (int i = 0; i < n; i++) tmp[i] = i + k < n ? rank[i + k] : 0;
                for (int i = 0; i < n; i++) cnt[i] = 0;
                for (int i = 0; i < n; i++) cnt[tmp[i]]++;
                for (int i = 1; i < n; i++) cnt[i] += cnt[i - 1];
                for (int i = n - 1; i >= 0; i--) sa2[--cnt[tmp[i]]] = i;
                for (int i = 0; i < n; i++) cnt[i] = 0;
                for (int i = 0; i < n; i++) cnt[rank[i]]++;
                for (int i = 1; i < n; i++) cnt[i] += cnt[i - 1];
                for (int i = n - 1; i >= 0; i--) sa[--cnt[rank[sa2[i]]]] = sa2[i];
                tmp[sa[0]] = 0;
                for (int i = 1; i < n; i++)
                {
                    bool same = rank[sa[i]] == rank[sa[i - 1]] &&
                        (sa[i] + k < n ? rank[sa[i] + k] : -1) ==
                        (sa[i - 1] + k < n ? rank[sa[i - 1] + k] : -1);
                    tmp[sa[i]] = tmp[sa[i - 1]] + (same ? 0 : 1);
                }
                for (int i = 0; i < n; i++) rank[i] = tmp[i];
                if (rank[sa[n - 1]] == n - 1) break;
            }
        }
    }

    public static unsafe class SuffixLcpBuild
    {
        public static void Run(byte* s, int n, int* sa, int* lcp, int* rank)
        {
            for (int i = 0; i < n; i++) rank[sa[i]] = i;
            int h = 0;
            for (int i = 0; i < n; i++)
            {
                if (rank[i] > 0)
                {
                    int j = sa[rank[i] - 1];
                    while (i + h < n && j + h < n && s[i + h] == s[j + h]) h++;
                    lcp[rank[i]] = h;
                    if (h > 0) h--;
                }
                else
                {
                    h = 0;
                }
            }
        }
    }

    public static unsafe class SuffixCompare
    {
        public static int Run(byte* s, int n, int* sa, int* rank, byte* pattern, int patLen)
        {
            int lo = 0, hi = n - 1;
            while (lo <= hi)
            {
                int mid = (lo + hi) >> 1;
                int cmp = 0;
                int len = Math.Min(patLen, n - sa[mid]);
                for (int i = 0; i < len; i++)
                {
                    if (s[sa[mid] + i] != pattern[i]) { cmp = s[sa[mid] + i].CompareTo(pattern[i]); break; }
                }
                if (cmp == 0) cmp = (n - sa[mid]).CompareTo(patLen);
                if (cmp < 0) lo = mid + 1;
                else hi = mid - 1;
            }
            return lo;
        }
    }

    public static unsafe class SuffixLowerBound
    {
        public static int Run(byte* s, int n, int* sa, byte* pattern, int patLen)
        {
            int lo = 0, hi = n;
            while (lo < hi)
            {
                int mid = (lo + hi) >> 1;
                int suffixLen = n - sa[mid];
                int len = Math.Min(patLen, suffixLen);
                int cmp = 0;
                for (int i = 0; i < len; i++)
                {
                    if (s[sa[mid] + i] < pattern[i]) { cmp = -1; break; }
                    if (s[sa[mid] + i] > pattern[i]) { cmp = 1; break; }
                }
                if (cmp == 0 && len < patLen)
                {
                    cmp = suffixLen < patLen ? -1 : 0;
                }
                if (cmp < 0) lo = mid + 1;
                else hi = mid;
            }
            return lo;
        }
    }

    public static unsafe class EditDistance
    {
        public static int Run(byte* a, int la, byte* b, int lb)
        {
            int* dp = stackalloc int[(la + 1) * (lb + 1)];
            for (int i = 0; i <= la; i++) dp[i * (lb + 1)] = i;
            for (int j = 0; j <= lb; j++) dp[j] = j;
            for (int i = 1; i <= la; i++)
            {
                for (int j = 1; j <= lb; j++)
                {
                    int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                    int d = dp[(i - 1) * (lb + 1) + j] + 1;
                    int ins = dp[i * (lb + 1) + (j - 1)] + 1;
                    int sub = dp[(i - 1) * (lb + 1) + (j - 1)] + cost;
                    dp[i * (lb + 1) + j] = Math.Min(d, Math.Min(ins, sub));
                }
            }
            return dp[la * (lb + 1) + lb];
        }
    }

    public static unsafe class Levenshtein
    {
        public static int Run(byte* a, int la, byte* b, int lb)
        {
            return EditDistance.Run(a, la, b, lb);
        }
    }

    public static unsafe class Lcs
    {
        public static int Run(byte* a, int la, byte* b, int lb, byte* result)
        {
            int* dp = stackalloc int[(la + 1) * (lb + 1)];
            for (int i = 0; i <= la; i++) dp[i * (lb + 1)] = 0;
            for (int j = 0; j <= lb; j++) dp[j] = 0;
            for (int i = 1; i <= la; i++)
            {
                for (int j = 1; j <= lb; j++)
                {
                    if (a[i - 1] == b[j - 1])
                        dp[i * (lb + 1) + j] = dp[(i - 1) * (lb + 1) + (j - 1)] + 1;
                    else
                        dp[i * (lb + 1) + j] = Math.Max(dp[(i - 1) * (lb + 1) + j], dp[i * (lb + 1) + (j - 1)]);
                }
            }
            int len = dp[la * (lb + 1) + lb];
            int ci = la, cj = lb, idx = len - 1;
            while (ci > 0 && cj > 0)
            {
                if (a[ci - 1] == b[cj - 1])
                {
                    result[idx--] = a[--ci];
                    cj--;
                }
                else if (dp[(ci - 1) * (lb + 1) + cj] > dp[ci * (lb + 1) + (cj - 1)])
                    ci--;
                else
                    cj--;
            }
            return len;
        }
    }

    public static unsafe class LcsLength
    {
        public static int Run(byte* a, int la, byte* b, int lb)
        {
            int* prev = stackalloc int[lb + 1];
            int* curr = stackalloc int[lb + 1];
            for (int j = 0; j <= lb; j++) prev[j] = 0;
            for (int i = 1; i <= la; i++)
            {
                curr[0] = 0;
                for (int j = 1; j <= lb; j++)
                {
                    if (a[i - 1] == b[j - 1])
                        curr[j] = prev[j - 1] + 1;
                    else
                        curr[j] = Math.Max(prev[j], curr[j - 1]);
                }
                int* tmp = prev;
                prev = curr;
                curr = tmp;
            }
            return prev[lb];
        }
    }

    public static unsafe class ScsLength
    {
        public static int Run(byte* a, int la, byte* b, int lb)
        {
            return la + lb - LcsLength.Run(a, la, b, lb);
        }
    }

    public static unsafe class WildcardMatch
    {
        public static bool Run(byte* text, int textLen, byte* pattern, int patLen)
        {
            int si = 0, pi = 0, starIdx = -1, match = 0;
            while (si < textLen)
            {
                if (pi < patLen && (pattern[pi] == text[si] || pattern[pi] == '?'))
                {
                    si++;
                    pi++;
                }
                else if (pi < patLen && pattern[pi] == '*')
                {
                    starIdx = pi;
                    match = si;
                    pi++;
                }
                else if (starIdx != -1)
                {
                    pi = starIdx + 1;
                    match++;
                    si = match;
                }
                else
                {
                    return false;
                }
            }
            while (pi < patLen && pattern[pi] == '*') pi++;
            return pi == patLen;
        }
    }

    public static unsafe class SuffixAutomatonExtend
    {
        public static int Run(int* link, int* len_, int* next, int last, int c, int* size)
        {
            int cur = *size;
            (*size)++;
            int p = last;
            while (p != -1 && next[p * 26 + c] == 0)
            {
                next[p * 26 + c] = cur;
                p = link[p];
            }
            if (p == -1)
            {
                link[cur] = 0;
            }
            else
            {
                int q = next[p * 26 + c];
                if (len_[p] + 1 == len_[q])
                {
                    link[cur] = q;
                }
                else
                {
                    int clone = *size;
                    (*size)++;
                    for (int i = 0; i < 26; i++)
                        next[clone * 26 + i] = next[q * 26 + i];
                    link[clone] = link[q];
                    len_[clone] = len_[p] + 1;
                    while (p != -1 && next[p * 26 + c] == q)
                    {
                        next[p * 26 + c] = clone;
                        p = link[p];
                    }
                    link[q] = clone;
                    link[cur] = clone;
                }
            }
            return cur;
        }
    }

    public static unsafe class SuffixAutomatonBuild
    {
        public static int Run(byte* s, int len, int* link, int* len_, int* next)
        {
            int size = 1;
            int last = 0;
            link[0] = -1;
            len_[0] = 0;
            for (int i = 0; i < len; i++)
            {
                last = SuffixAutomatonExtend.Run(link, len_, next, last, s[i] - 'a', &size);
            }
            return size;
        }
    }

    public static unsafe class AhoBuild
    {
        public static void Run(int* trie, int* fail, int node, int alphaSize)
        {
            int* queue = stackalloc int[node + 1];
            int front = 0, back = 0;
            for (int c = 0; c < alphaSize; c++)
            {
                if (trie[c] != 0)
                {
                    fail[trie[c]] = 0;
                    queue[back++] = trie[c];
                }
            }
            while (front < back)
            {
                int u = queue[front++];
                for (int c = 0; c < alphaSize; c++)
                {
                    int v = trie[u * alphaSize + c];
                    if (v != 0)
                    {
                        fail[v] = trie[fail[u] * alphaSize + c];
                        queue[back++] = v;
                    }
                    else
                    {
                        trie[u * alphaSize + c] = trie[fail[u] * alphaSize + c];
                    }
                }
            }
        }
    }

    public static unsafe class AhoNext
    {
        public static int Run(int* trie, int node, int c, int alphaSize)
        {
            return trie[node * alphaSize + c];
        }
    }

    public static unsafe class AhoMatch
    {
        public static int Run(int* trie, int* fail, byte* s, int len, int alphaSize)
        {
            int count = 0;
            int state = 0;
            for (int i = 0; i < len; i++)
            {
                state = trie[state * alphaSize + s[i]];
                int temp = state;
                while (temp != 0)
                {
                    count++;
                    temp = fail[temp];
                }
            }
            return count;
        }
    }

    public static unsafe class AhoCount
    {
        public static int Run(int* trie, int* fail, byte* s, int len, int alphaSize)
        {
            return AhoMatch.Run(trie, fail, s, len, alphaSize);
        }
    }

    public static unsafe class PalindromicTreeAdd
    {
        public static int Run(int* len_, int* link, int* next, int* last, byte* s, int pos)
        {
            int cur = *last;
            int ch = s[pos];
            int c = ch - 'a';
            while (true)
            {
                int curlen = len_[cur];
                if (pos - curlen - 1 >= 0 && s[pos - curlen - 1] == ch) break;
                cur = link[cur];
            }
            if (next[cur * 26 + c] != 0)
            {
                *last = next[cur * 26 + c];
                return 0;
            }
            int now = ++len_[0];
            next[cur * 26 + c] = now;
            len_[now] = len_[cur] + 2;
            if (len_[now] == 1)
            {
                link[now] = 1;
                *last = now;
                return 1;
            }
            cur = link[cur];
            while (true)
            {
                int curlen = len_[cur];
                if (pos - curlen - 1 >= 0 && s[pos - curlen - 1] == ch) break;
                cur = link[cur];
            }
            link[now] = next[cur * 26 + c];
            *last = now;
            return 1;
        }
    }

    public static unsafe class PalindromicTreeBuild
    {
        public static int Run(byte* s, int len, int* len_, int* link, int* next)
        {
            len_[0] = 2;
            len_[1] = 0;
            len_[2] = -1;
            link[1] = 2;
            link[2] = 2;
            int last = 1;
            int nodeCount = 2;
            for (int i = 0; i < len; i++)
            {
                PalindromicTreeAdd.Run(len_, link, next, &last, s, i);
                nodeCount++;
            }
            return nodeCount - 2;
        }
    }

    public static unsafe class RegexNfaBuild
    {
        public static int Run(byte* pattern, int patLen, int* transitions, int alphaSize)
        {
            int state = 0;
            int* queue = stackalloc int[patLen];
            int qlen = 0;
            for (int i = 0; i < patLen; i++)
            {
                if (pattern[i] == '*')
                {
                    int prevState = queue[qlen - 1];
                    int starState = state++;
                    int charIdx = pattern[i - 1];
                    transitions[prevState * alphaSize + charIdx] = starState;
                    transitions[starState * alphaSize + charIdx] = starState;
                    queue[qlen++] = starState;
                }
                else if (pattern[i] == '?' || pattern[i] == '+')
                {
                    queue[qlen++] = state;
                    state++;
                }
                else
                {
                    transitions[state * alphaSize + pattern[i]] = state + 1;
                    queue[qlen++] = state;
                    state++;
                }
            }
            return state;
        }
    }

    public static unsafe class RegexMatch
    {
        public static bool Run(int* transitions, int startState, int acceptState, byte* s, int len, int alphaSize)
        {
            int* cur = stackalloc int[256];
            int* next = stackalloc int[256];
            int curCount = 1;
            cur[0] = startState;
            for (int i = 0; i < len; i++)
            {
                int nextCount = 0;
                for (int j = 0; j < curCount; j++)
                {
                    int ns = transitions[cur[j] * alphaSize + s[i]];
                    if (ns > 0) next[nextCount++] = ns;
                }
                int* tmp = cur;
                cur = next;
                next = tmp;
                curCount = nextCount;
                if (curCount == 0) return false;
            }
            for (int j = 0; j < curCount; j++)
                if (cur[j] == acceptState) return true;
            return false;
        }
    }

    public static unsafe class ParseExpression
    {
        public static int Run(byte* s, int len)
        {
            int pos = 0;
            return ParseAdd(s, len, ref pos);
        }

        private static int ParseAdd(byte* s, int len, ref int pos)
        {
            int result = ParseMul(s, len, ref pos);
            while (pos < len && (s[pos] == '+' || s[pos] == '-'))
            {
                byte op = s[pos++];
                int right = ParseMul(s, len, ref pos);
                if (op == '+') result += right;
                else result -= right;
            }
            return result;
        }

        private static int ParseMul(byte* s, int len, ref int pos)
        {
            int result = ParsePrimary(s, len, ref pos);
            while (pos < len && (s[pos] == '*' || s[pos] == '/'))
            {
                byte op = s[pos++];
                int right = ParsePrimary(s, len, ref pos);
                if (op == '*') result *= right;
                else result /= right;
            }
            return result;
        }

        private static int ParsePrimary(byte* s, int len, ref int pos)
        {
            if (s[pos] == '(')
            {
                pos++;
                int result = ParseAdd(s, len, ref pos);
                pos++;
                return result;
            }
            return ParseInteger.Run(s + pos, len - pos, out int consumed);
        }
    }

    public static unsafe class ParseInteger
    {
        public static int Run(byte* s, int len, out int consumed)
        {
            int result = 0;
            consumed = 0;
            while (consumed < len && s[consumed] >= '0' && s[consumed] <= '9')
            {
                result = result * 10 + (s[consumed] - '0');
                consumed++;
            }
            return result;
        }
    }

    public static unsafe class Tokenize
    {
        public static int Run(byte* s, int len, int* types, int* values)
        {
            int count = 0;
            int pos = 0;
            while (pos < len)
            {
                if (s[pos] == ' ' || s[pos] == '\t' || s[pos] == '\n')
                {
                    pos++;
                    continue;
                }
                if (s[pos] >= '0' && s[pos] <= '9')
                {
                    types[count] = 0;
                    values[count] = 0;
                    while (pos < len && s[pos] >= '0' && s[pos] <= '9')
                    {
                        values[count] = values[count] * 10 + (s[pos] - '0');
                        pos++;
                    }
                    count++;
                }
                else
                {
                    types[count] = 1;
                    values[count] = s[pos];
                    pos++;
                    count++;
                }
            }
            return count;
        }
    }
}