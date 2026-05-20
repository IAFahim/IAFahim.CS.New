namespace IAFahim.Search.Automaton
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ModMatrixPow
    {
        public static void Run(int n, long* a, long* result, long exp, long mod)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    result[i * n + j] = (i == j) ? 1 : 0;
            long* temp = stackalloc long[n * n];
            for (int i = 0; i < n * n; i++) temp[i] = a[i];
            while (exp > 0)
            {
                if ((exp & 1) == 1)
                {
                    long* res2 = stackalloc long[n * n];
                    for (int i = 0; i < n; i++)
                        for (int j = 0; j < n; j++)
                        {
                            long sum = 0;
                            for (int k = 0; k < n; k++)
                                sum = (sum + result[i * n + k] * temp[k * n + j]) % mod;
                            res2[i * n + j] = sum;
                        }
                    for (int i = 0; i < n * n; i++) result[i] = res2[i];
                }
                long* temp2 = stackalloc long[n * n];
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                    {
                        long sum = 0;
                        for (int k = 0; k < n; k++)
                            sum = (sum + temp[i * n + k] * temp[k * n + j]) % mod;
                        temp2[i * n + j] = sum;
                    }
                for (int i = 0; i < n * n; i++) temp[i] = temp2[i];
                exp >>= 1;
            }
        }
    }

    public static unsafe class BuildAutomaton
    {
        public static int Run(int n, int* transitions, int* failure, int* output, int alphabetSize)
        {
            int front = 0, rear = 0;
            int* queue = stackalloc int[n];
            for (int i = 0; i < alphabetSize; i++)
            {
                int next = transitions[i];
                if (next != 0)
                {
                    failure[next] = 0;
                    queue[rear++] = next;
                }
            }
            while (front < rear)
            {
                int state = queue[front++];
                for (int i = 0; i < alphabetSize; i++)
                {
                    int next = transitions[state * alphabetSize + i];
                    if (next != 0)
                    {
                        int f = failure[state];
                        while (f != 0 && transitions[f * alphabetSize + i] == 0)
                            f = failure[f];
                        failure[next] = transitions[f * alphabetSize + i];
                        output[next] |= output[failure[next]];
                        queue[rear++] = next;
                    }
                    else
                    {
                        transitions[state * alphabetSize + i] = transitions[failure[state] * alphabetSize + i];
                    }
                }
            }
            return rear;
        }
    }

    public static unsafe class DfaTransition
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Run(int state, int symbol, int* transitions, int alphabetSize)
        {
            return transitions[state * alphabetSize + symbol];
        }
    }

    public static unsafe class NfaClosure
    {
        public static int Run(int n, int* transitions, int startState, long* visited, int* result)
        {
            int* stack = stackalloc int[n];
            int top = 0;
            stack[top++] = startState;
            int count = 0;
            while (top > 0)
            {
                int state = stack[--top];
                if ((visited[state >> 6] & (1L << (state & 63))) != 0) continue;
                visited[state >> 6] |= 1L << (state & 63);
                result[count++] = state;
                for (int i = 0; i < n; i++)
                {
                    if (transitions[state * n + i] != 0 && (visited[i >> 6] & (1L << (i & 63))) == 0)
                        stack[top++] = i;
                }
            }
            return count;
        }
    }
}