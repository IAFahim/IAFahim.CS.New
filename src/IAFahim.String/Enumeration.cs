namespace IAFahim.String
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public static unsafe class Enumeration
    {
        public static int ShortestCommonSupersequence(byte* a, int aLen, byte* b, int bLen, byte* c, int* dp)
        {
            int cols = bLen + 1;
            for (int i = 0; i <= aLen; i++)
            {
                dp[i * cols] = i;
            }
            for (int j = 0; j <= bLen; j++)
            {
                dp[j] = j;
            }
            for (int i = 1; i <= aLen; i++)
            {
                for (int j = 1; j <= bLen; j++)
                {
                    if (a[i - 1] == b[j - 1])
                    {
                        dp[i * cols + j] = dp[(i - 1) * cols + (j - 1)] + 1;
                    }
                    else
                    {
                        int val1 = dp[(i - 1) * cols + j] + 1;
                        int val2 = dp[i * cols + (j - 1)] + 1;
                        dp[i * cols + j] = val1 < val2 ? val1 : val2;
                    }
                }
            }
            int idxA = aLen;
            int idxB = bLen;
            int writeIdx = dp[aLen * cols + bLen];
            int totalLen = writeIdx;
            while (idxA > 0 && idxB > 0)
            {
                if (a[idxA - 1] == b[idxB - 1])
                {
                    c[--writeIdx] = a[idxA - 1];
                    idxA--;
                    idxB--;
                }
                else if (dp[(idxA - 1) * cols + idxB] < dp[idxA * cols + (idxB - 1)])
                {
                    c[--writeIdx] = a[idxA - 1];
                    idxA--;
                }
                else
                {
                    c[--writeIdx] = b[idxB - 1];
                    idxB--;
                }
            }
            while (idxA > 0)
            {
                c[--writeIdx] = a[idxA - 1];
                idxA--;
            }
            while (idxB > 0)
            {
                c[--writeIdx] = b[idxB - 1];
                idxB--;
            }
            return totalLen;
        }

        public static int ShortestAbsentSubsequence(byte* s, int len, int alphabetSize, byte* result, int* nextOcc, int* dp, int* path)
        {
            for (int c = 0; c < alphabetSize; c++)
            {
                nextOcc[(long)(len + 1) * alphabetSize + c] = len;
                nextOcc[(long)len * alphabetSize + c] = len;
            }
            for (int i = len - 1; i >= 0; i--)
            {
                for (int c = 0; c < alphabetSize; c++)
                {
                    nextOcc[(long)i * alphabetSize + c] = nextOcc[(long)(i + 1) * alphabetSize + c];
                }
                int charVal = s[i];
                if (charVal >= 0 && charVal < alphabetSize)
                {
                    nextOcc[(long)i * alphabetSize + charVal] = i;
                }
            }
            dp[len] = 1;
            path[len] = -1;
            dp[len + 1] = 0;
            path[len + 1] = -1;
            for (int i = len - 1; i >= 0; i--)
            {
                int bestVal = int.MaxValue;
                int bestChar = -1;
                for (int c = 0; c < alphabetSize; c++)
                {
                    int nxt = nextOcc[(long)i * alphabetSize + c];
                    int val = 1 + dp[nxt + 1];
                    if (val < bestVal)
                    {
                        bestVal = val;
                        bestChar = c;
                    }
                }
                dp[i] = bestVal;
                path[i] = bestChar;
            }
            int curr = 0;
            int writeIdx = 0;
            while (curr < len)
            {
                int c = path[curr];
                if (c == -1)
                {
                    break;
                }
                result[writeIdx++] = (byte)c;
                curr = nextOcc[(long)curr * alphabetSize + c] + 1;
            }
            if (curr == len)
            {
                result[writeIdx++] = 0;
            }
            return writeIdx;
        }

        public static int ShortestMissingSubstring(byte* s, int len, int alphabetSize, byte* result, bool* seen)
        {
            for (int subLen = 1; ; subLen++)
            {
                long limit = 1;
                bool overflow = false;
                for (int i = 0; i < subLen; i++)
                {
                    limit *= alphabetSize;
                    if (limit > len + 1)
                    {
                        overflow = true;
                    }
                }
                if (overflow || limit > len)
                {
                    int foundLen = -1;
                    FindMissing(s, len, subLen, alphabetSize, result, &foundLen, seen);
                    if (foundLen != -1)
                    {
                        return foundLen;
                    }
                }
                else
                {
                    int foundLen = -1;
                    FindMissing(s, len, subLen, alphabetSize, result, &foundLen, seen);
                    if (foundLen != -1)
                    {
                        return foundLen;
                    }
                }
            }
        }

        private static void FindMissing(byte* s, int len, int subLen, int alphabetSize, byte* result, int* foundLen, bool* seen)
        {
            long limit = 1;
            for (int i = 0; i < subLen; i++)
            {
                limit *= alphabetSize;
            }
            for (int i = 0; i < limit; i++)
            {
                seen[i] = false;
            }
            for (int i = 0; i <= len - subLen; i++)
            {
                long hash = 0;
                bool valid = true;
                for (int j = 0; j < subLen; j++)
                {
                    int val = s[i + j];
                    if (val < 0 || val >= alphabetSize)
                    {
                        valid = false;
                        break;
                    }
                    hash = hash * alphabetSize + val;
                }
                if (valid)
                {
                    seen[hash] = true;
                }
            }
            for (long i = 0; i < limit; i++)
            {
                if (!seen[i])
                {
                    long temp = i;
                    for (int j = subLen - 1; j >= 0; j--)
                    {
                        result[j] = (byte)(temp % alphabetSize);
                        temp /= alphabetSize;
                    }
                    *foundLen = subLen;
                    return;
                }
            }
            *foundLen = -1;
        }
    }
}
