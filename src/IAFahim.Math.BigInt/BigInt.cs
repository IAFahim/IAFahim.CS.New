namespace IAFahim.Math.BigInt
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BigIntAdd
    {
        public static int Run(int n, int* a, int m, int* b, int* res)
        {
            int maxLen = Math.Max(n, m);
            int* temp = stackalloc int[maxLen + 1];
            int carry = 0, ptr = 0;
            for (int i = 0; i < maxLen || carry > 0; i++)
            {
                int valA = i < n ? a[n - 1 - i] : 0;
                int valB = i < m ? b[m - 1 - i] : 0;
                int sum = valA + valB + carry;
                temp[ptr++] = sum % 10;
                carry = sum / 10;
            }
            for (int i = 0; i < ptr; i++) res[i] = temp[ptr - 1 - i];
            return ptr;
        }
    }

    public static unsafe class BigIntSub
    {
        public static int Run(int n, int* a, int m, int* b, int* res)
        {
            int len = n; int* temp = stackalloc int[len]; int borrow = 0;
            for (int i = 0; i < n; i++)
            {
                int valA = a[n - 1 - i], valB = i < m ? b[m - 1 - i] : 0;
                int diff = valA - valB - borrow;
                if (diff < 0) { diff += 10; borrow = 1; } else borrow = 0;
                temp[len - 1 - i] = diff;
            }
            int start = 0; while (start < len - 1 && temp[start] == 0) start++;
            int finalLen = len - start;
            for (int j = 0; j < finalLen; j++) res[j] = temp[start + j];
            return finalLen;
        }
    }

    public static unsafe class BigIntMul
    {
        public static int Run(int n, int* a, int m, int* b, int* res)
        {
            int len = n + m;
            int* temp = stackalloc int[len]; for (int i = 0; i < len; i++) temp[i] = 0;
            for (int i = 0; i < n; i++) for (int j = 0; j < m; j++) temp[i + j] += a[n - 1 - i] * b[m - 1 - j];
            int carry = 0; for (int i = 0; i < len; i++) { int sum = temp[i] + carry; temp[i] = sum % 10; carry = sum / 10; }
            int actualLen = len - 1; while (actualLen > 0 && temp[actualLen] == 0) actualLen--;
            int finalLen = actualLen + 1;
            for (int i = 0; i < finalLen; i++) res[i] = temp[finalLen - 1 - i];
            return finalLen;
        }
    }

    public static unsafe class BigIntPow
    {
        public static int Run(int n, int* a, int e, int* res)
        {
            if (e == 0) { res[0] = 1; return 1; }
            int curLen = n; int* cur = stackalloc int[1000]; for (int i = 0; i < n; i++) cur[i] = a[i];
            int* temp = stackalloc int[1000], ans = stackalloc int[1000]; ans[0] = 1; int ansLen = 1;
            while (e > 0)
            {
                if ((e & 1) == 1) { ansLen = BigIntMul.Run(ansLen, ans, curLen, cur, temp); for (int i = 0; i < ansLen; i++) ans[i] = temp[i]; }
                if (e > 1) { int nextLen = BigIntMul.Run(curLen, cur, curLen, cur, temp); for (int i = 0; i < nextLen; i++) cur[i] = temp[i]; curLen = nextLen; }
                e >>= 1;
            }
            for (int i = 0; i < ansLen; i++) res[i] = ans[i]; return ansLen;
        }
    }

    public static unsafe class BigIntDiv
    {
        public static int Run(int n, int* a, int divisor, int* res)
        {
            long rem = 0; int len = 0;
            for (int i = 0; i < n; i++) { rem = rem * 10 + a[i]; res[len++] = (int)(rem / divisor); rem %= divisor; }
            int start = 0; while (start < len - 1 && res[start] == 0) start++;
            if (start > 0) { for (int i = 0; i < len - start; i++) res[i] = res[start + i]; len -= start; }
            return len;
        }
    }

    public static unsafe class BigIntMod
    {
        public static int Run(int n, int* a, int mod) { long rem = 0; for (int i = 0; i < n; i++) rem = (rem * 10 + a[i]) % mod; return (int)rem; }
    }
}
