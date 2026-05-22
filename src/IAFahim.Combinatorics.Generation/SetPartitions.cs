namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

public static unsafe class SetPartitions
{
    public struct IntegerPartitionState
    {
        public int N;
        public int K;
        public bool First;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InitIntegerPartition(int n, IntegerPartitionState* state)
    {
        state->N = n;
        state->K = 0;
        state->First = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NextIntegerPartition(IntegerPartitionState* state, int* p, out int length)
    {
        if (state->First)
        {
            p[0] = state->N;
            state->K = 0;
            state->First = false;
            length = 1;
            return true;
        }

        int remVal = 0;
        while (state->K >= 0 && p[state->K] == 1)
        {
            remVal += p[state->K];
            state->K--;
        }

        if (state->K < 0)
        {
            length = 0;
            return false;
        }

        p[state->K]--;
        remVal++;

        while (remVal > p[state->K])
        {
            p[state->K + 1] = p[state->K];
            remVal -= p[state->K];
            state->K++;
        }

        p[state->K + 1] = remVal;
        state->K++;
        length = state->K + 1;
        return true;
    }

    public struct SetPartitionState
    {
        public int N;
        public int First;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InitSetPartition(int n, SetPartitionState* state)
    {
        state->N = n;
        state->First = 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NextSetPartition(SetPartitionState* state, int* kappa, int* m)
    {
        if (state->First == 1)
        {
            for (int x = 0; x < state->N; x++) kappa[x] = 0;
            for (int x = 0; x < state->N; x++) m[x] = 0;
            state->First = 0;
            return true;
        }

        int i = state->N - 1;
        while (i > 0 && kappa[i] == m[i - 1] + 1) i--;

        if (i == 0) return false;

        kappa[i]++;
        m[i] = Math.Max(m[i], kappa[i]);

        for (int j = i + 1; j < state->N; j++)
        {
            kappa[j] = 0;
            m[j] = m[i];
        }

        return true;
    }

    public struct CompositionState
    {
        public int N;
        public int K;
        public bool First;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void InitComposition(int n, int k, CompositionState* state)
    {
        state->N = n;
        state->K = k;
        state->First = true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool NextComposition(CompositionState* state, int* comp)
    {
        int n = state->N;
        int k = state->K;

        if (state->First)
        {
            for (int x = 0; x < k; x++) comp[x] = 0;
            comp[0] = n;
            state->First = false;
            return true;
        }

        int i = k - 2;
        while (i >= 0 && comp[i] == 0) i--;

        if (i < 0) return false;

        comp[i]--;
        
        int sum = 0;
        for (int j = 0; j <= i; j++) sum += comp[j];

        comp[i + 1] = n - sum;

        for (int j = i + 2; j < k; j++) comp[j] = 0;

        return true;
    }

    public static long RankIntegerPartition(int* partition, int len, int n)
    {
        return 0;
    }

    public static bool UnrankIntegerPartition(long rank, int n, int* outPart, out int outLen)
    {
        outLen = 0;
        return false;
    }

    public static long RankSetPartition(int* partition, int n)
    {
        return 0;
    }

    public static bool UnrankSetPartition(long rank, int n, int* outKappa)
    {
        for (int i = 0; i < n; i++) outKappa[i] = i;
        return true;
    }

    public static long RankComposition(int* composition, int k)
    {
        return 0;
    }

    public static bool UnrankComposition(long rank, int n, int k, int* outComp)
    {
        for (int i = 0; i < k; i++) outComp[i] = 0;
        return true;
    }
}