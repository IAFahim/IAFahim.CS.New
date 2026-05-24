namespace IAFahim.Search.Automaton
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class ModMatrixPow
    {
        public static void Run(int n, long* a, long* result, long exp, long mod)
        {
            InitializeIdentity(n, result);
            long* temp = stackalloc long[n * n];
            Buffer.MemoryCopy(a, temp, n * n * sizeof(long), n * n * sizeof(long));
            
            while (exp > 0)
            {
                if ((exp & 1) == 1) MultiplyMatrices(n, result, temp, mod);
                if (exp > 1) MultiplyMatrices(n, temp, temp, mod);
                exp >>= 1;
            }
        }

        private static void InitializeIdentity(int n, long* res)
        {
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++) res[i * n + j] = (i == j) ? 1 : 0;
        }

        private static void MultiplyMatrices(int n, long* a, long* b, long mod)
        {
            long* tmp = stackalloc long[n * n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {
                    long sum = 0;
                    for (int k = 0; k < n; k++) sum = (sum + a[i * n + k] * b[k * n + j]) % mod;
                    tmp[i * n + j] = sum;
                }
            Buffer.MemoryCopy(tmp, a, n * n * sizeof(long), n * n * sizeof(long));
        }
    }

    public static unsafe class BuildAutomaton
    {
        public static int Run(int n, int* transitions, int* failure, int* output, int alphabetSize)
        {
            int front = 0, rear = 0;
            int* queue = stackalloc int[n];
            InitializeRootTransitions(alphabetSize, transitions, failure, queue, ref rear);
            
            while (front < rear)
            {
                int state = queue[front++];
                for (int i = 0; i < alphabetSize; i++)
                    ProcessTransition(state, i, alphabetSize, transitions, failure, output, queue, ref rear);
            }
            return rear;
        }

        private static void InitializeRootTransitions(int alphabetSize, int* transitions, int* failure, int* queue, ref int rear)
        {
            for (int i = 0; i < alphabetSize; i++)
            {
                int next = transitions[i];
                if (next != 0) { failure[next] = 0; queue[rear++] = next; }
            }
        }

        private static void ProcessTransition(int state, int i, int alphabetSize, int* trans, int* fail, int* output, int* queue, ref int rear)
        {
            int next = trans[state * alphabetSize + i];
            if (next != 0)
            {
                int f = fail[state];
                while (f != 0 && trans[f * alphabetSize + i] == 0) f = fail[f];
                fail[next] = trans[f * alphabetSize + i];
                output[next] |= output[fail[next]];
                queue[rear++] = next;
            }
            else
            {
                trans[state * alphabetSize + i] = trans[fail[state] * alphabetSize + i];
            }
        }
    }
}
