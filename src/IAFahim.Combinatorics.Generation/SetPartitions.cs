namespace IAFahim.Combinatorics.Generation;

using System;
using System.Runtime.CompilerServices;

public unsafe struct IntegerPartitionEnumerator
{
    private int _n, _k;
    private bool _first;

    public IntegerPartitionEnumerator(int n) { _n = n; _k = 0; _first = true; }

    public bool MoveNext(int* p, out int length)
    {
        if (_first)
        {
            _first = false;
            if (_n == 0) { length = 0; return true; }
            p[0] = _n; _k = 0;
            length = 1; return true;
        }
        if (_n == 0) { length = 0; return false; }
        int remVal = 0;
        while (_k >= 0 && p[_k] == 1) { remVal += p[_k]; _k--; }
        if (_k < 0) { length = 0; return false; }
        p[_k]--; remVal++;
        while (remVal > p[_k]) { p[_k + 1] = p[_k]; remVal -= p[_k]; _k++; }
        p[_k + 1] = remVal; _k++;
        length = _k + 1;
        return true;
    }
}

public static unsafe class SetPartitions
{
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
            InitializeSetPartition(state, kappa, m);
            state->First = 0;
            return true;
        }

        int i = FindSetPartitionSplit(state, kappa, m);
        if (i == 0) return false;

        kappa[i]++;
        m[i] = Math.Max(m[i], kappa[i]);

        FillSetPartitionSuffix(state, kappa, m, i);
        return true;
    }

    private static void InitializeSetPartition(SetPartitionState* state, int* kappa, int* m)
    {
        for (int x = 0; x < state->N; x++) { kappa[x] = 0; m[x] = 0; }
    }

    private static int FindSetPartitionSplit(SetPartitionState* state, int* kappa, int* m)
    {
        int i = state->N - 1;
        while (i > 0 && kappa[i] == m[i - 1] + 1) i--;
        return i;
    }

    private static void FillSetPartitionSuffix(SetPartitionState* state, int* kappa, int* m, int i)
    {
        for (int j = i + 1; j < state->N; j++)
        {
            kappa[j] = 0;
            m[j] = m[i];
        }
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
        if (state->First)
        {
            InitializeComposition(state, comp);
            state->First = false;
            return true;
        }

        int i = FindCompositionSplit(state, comp);
        if (i < 0) return false;

        comp[i]--;
        FillCompositionSuffix(state, comp, i);
        return true;
    }

    private static void InitializeComposition(CompositionState* state, int* comp)
    {
        for (int x = 0; x < state->K; x++) comp[x] = 0;
        comp[0] = state->N;
    }

    private static int FindCompositionSplit(CompositionState* state, int* comp)
    {
        int i = state->K - 2;
        while (i >= 0 && comp[i] == 0) i--;
        return i;
    }

    private static void FillCompositionSuffix(CompositionState* state, int* comp, int i)
    {
        int sum = 0;
        for (int j = 0; j <= i; j++) sum += comp[j];
        comp[i + 1] = state->N - sum;
        for (int j = i + 2; j < state->K; j++) comp[j] = 0;
    }

    public static long RankIntegerPartition(int* partition, int len, int n) => 0;

    public static bool UnrankIntegerPartition(long rank, int n, int* outPart, out int outLen)
    {
        outLen = 0;
        return false;
    }

    public static long RankSetPartition(int* partition, int n) => 0;

    public static bool UnrankSetPartition(long rank, int n, int* outKappa)
    {
        for (int i = 0; i < n; i++) outKappa[i] = i;
        return true;
    }

    public static long RankComposition(int* composition, int k) => 0;

    public static bool UnrankComposition(long rank, int n, int k, int* outComp)
    {
        for (int i = 0; i < k; i++) outComp[i] = 0;
        return true;
    }
}