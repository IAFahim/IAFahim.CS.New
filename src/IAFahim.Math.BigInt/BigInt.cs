namespace IAFahim.Math.BigInt
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class BigIntAdd
    {
        public static int Run(int n, int* a, int m, int* b, int* res)
        {
            int len = n > m ? n : m;
            int* temp = stackalloc int[len + 1];
            int carry = 0;
            int i = 0;
            for (; i < n && i < m; i++)
            {
                int sum = a[n - 1 - i] + b[m - 1 - i] + carry;
                temp[len - i] = sum % 10;
                carry = sum / 10;
            }
            for (; i < n; i++)
            {
                int sum = a[n - 1 - i] + carry;
                temp[len - i] = sum % 10;
                carry = sum / 10;
            }
            for (; i < m; i++)
            {
                int sum = b[m - 1 - i] + carry;
                temp[len - i] = sum % 10;
                carry = sum / 10;
            }
            if (carry > 0)
            {
                temp[0] = carry;
                int finalLen = len + 1;
                for (int j = 0; j < finalLen; j++) res[j] = temp[j];
                return finalLen;
            }
            else
            {
                for (int j = 0; j < len; j++) res[j] = temp[j + 1];
                return len;
            }
        }
    }

    public static unsafe class BigIntSub
    {
        public static int Run(int n, int* a, int m, int* b, int* res)
        {
            int len = n > m ? n : m;
            int* temp = stackalloc int[len];
            int borrow = 0;
            int i = 0;
            for (; i < n && i < m; i++)
            {
                int diff = a[n - 1 - i] - b[m - 1 - i] - borrow;
                if (diff < 0) { diff += 10; borrow = 1; }
                else borrow = 0;
                temp[len - 1 - i] = diff;
            }
            for (; i < n; i++)
            {
                int diff = a[n - 1 - i] - borrow;
                if (diff < 0) { diff += 10; borrow = 1; }
                else borrow = 0;
                temp[len - 1 - i] = diff;
            }
            int start = 0;
            while (start < len && temp[start] == 0) start++;
            if (start == len)
            {
                res[0] = 0;
                return 1;
            }
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
            int* temp = stackalloc int[len];
            for (int i = 0; i < len; i++) temp[i] = 0;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    temp[i + j] += a[n - 1 - i] * b[m - 1 - j];
                }
            }
            int carry = 0;
            for (int i = 0; i < len; i++)
            {
                int sum = temp[i] + carry;
                temp[i] = sum % 10;
                carry = sum / 10;
            }
            int actualLen = len - 1;
            while (actualLen > 0 && temp[actualLen] == 0) actualLen--;
            int finalLen = actualLen + 1;
            for (int i = 0; i < finalLen; i++) res[i] = temp[finalLen - 1 - i];
            return finalLen;
        }
    }


    public static unsafe class BigIntDiv
    {
        public static int Run(int n, int* a, int divisor, int* res)
        {
            long remainder = 0;
            int len = 0;
            for (int i = 0; i < n; i++)
            {
                remainder = remainder * 10 + a[i];
                res[len++] = (int)(remainder / divisor);
                remainder %= divisor;
            }
            while (len > 0 && res[0] == 0) { for (int i = 0; i < len - 1; i++) res[i] = res[i + 1]; len--; }
            if (len == 0) len = 1;
            return len;
        }
    }

    public static unsafe class BigIntMod
    {
        public static int Run(int n, int* a, int mod)
        {
            long remainder = 0;
            for (int i = 0; i < n; i++)
            {
                remainder = (remainder * 10 + a[i]) % mod;
            }
            return (int)remainder;
        }
    }

    public static unsafe class BigIntPow
    {
        public static int Run(int baseNum, int exp, int* res)
        {
            res[0] = 1;
            int len = 1;
            int* temp = stackalloc int[1000];
            for (int i = 0; i < 1000; i++) temp[i] = 0;
            temp[0] = baseNum;
            int tempLen = 1;
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    len = BigIntMul.Run(len, res, tempLen, temp, res);
                }
                tempLen = BigIntMul.Run(tempLen, temp, tempLen, temp, temp);
                exp >>= 1;
            }
            return len;
        }
    }

    public static unsafe class DecimalNormalize
    {
        public static int Run(int n, int* a, int exp, int* res, int* newExp)
        {
            int pos = 0;
            while (pos < n && a[pos] == 0) pos++;
            if (pos == n) { res[0] = 0; *newExp = 0; return 1; }
            newExp[0] = exp + (n - pos - 1);
            int len = 0;
            int decimalShown = 0;
            for (int i = pos; i < n; i++)
            {
                if (len == 1 && decimalShown == 0) { res[len++] = -1; decimalShown = 1; }
                res[len++] = a[i];
            }
            return len;
        }
    }

    public static unsafe class FractionReduce
    {
        public static void Run(int* num, int* den)
        {
            int a = num[0], b = den[0];
            while (b != 0) { int t = b; b = a % b; a = t; }
            num[0] /= a;
            den[0] /= a;
        }
    }

    public static unsafe class FractionAdd
    {
        public static void Run(int* aNum, int* aDen, int* bNum, int* bDen, int* resNum, int* resDen)
        {
            int lcm = aDen[0] * bDen[0];
            resNum[0] = aNum[0] * bDen[0] + bNum[0] * aDen[0];
            resDen[0] = lcm;
            FractionReduce.Run(resNum, resDen);
        }
    }

    public static unsafe class FractionMul
    {
        public static void Run(int* aNum, int* aDen, int* bNum, int* bDen, int* resNum, int* resDen)
        {
            resNum[0] = aNum[0] * bNum[0];
            resDen[0] = aDen[0] * bDen[0];
            FractionReduce.Run(resNum, resDen);
        }
    }
}