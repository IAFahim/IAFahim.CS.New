namespace IAFahim.Optimization.DivideConquer
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class SlopeTrick
    {
        public struct State
        {
            public long L, R;
            public long Lc, Rc;
            public long Offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Init(State* s)
        {
            s->L = s->R = 0;
            s->Lc = s->Rc = long.MaxValue;
            s->Offset = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAbs(State* s, long a)
        {
            long l = a - s->L, r = s->R - a;
            if (l > r) UpdateLeftSlope(s, a, l);
            else UpdateRightSlope(s, a, r);
        }

        private static void UpdateLeftSlope(State* s, long a, long l)
        {
            s->Offset += l;
            if (a < s->Lc) s->Lc = a;
            s->R = s->Lc;
        }

        private static void UpdateRightSlope(State* s, long a, long r)
        {
            s->Offset += r;
            if (a > s->Rc) s->Rc = a;
            s->L = s->Rc;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddAbsWithHeaps(State* s, long a, long* leftHeap, ref int leftSize, long* rightHeap, ref int rightSize)
        {
            leftHeap[leftSize++] = s->L - a;
            rightHeap[rightSize++] = a - s->R;
            
            if (leftSize > 0 && rightSize > 0)
            {
                AdjustHeaps(s, leftHeap, ref leftSize, rightHeap, ref rightSize);
            }
            
            UpdateStateFromHeaps(s, a, leftHeap, leftSize, rightHeap, rightSize);
        }

        private static void AdjustHeaps(State* s, long* leftHeap, ref int leftSize, long* rightHeap, ref int rightSize)
        {
            long topL = leftHeap[leftSize - 1], topR = rightHeap[rightSize - 1];
            if (topL > topR)
            {
                leftHeap[leftSize - 1] = topR; rightHeap[rightSize - 1] = topL;
                long diff = topL - topR;
                s->Offset += diff; s->L -= diff; s->R += diff;
            }
        }

        private static void UpdateStateFromHeaps(State* s, long a, long* leftHeap, int leftSize, long* rightHeap, int rightSize)
        {
            s->L = a - (leftSize > 0 ? leftHeap[leftSize - 1] : 0);
            s->R = a + (rightSize > 0 ? rightHeap[rightSize - 1] : 0);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddMaxZero(State* s)
        {
            s->L--; s->R++;
            if (s->L > 0) { s->Offset += s->L; s->L = 0; }
            if (s->R < 0) { s->Offset -= s->R; s->R = 0; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shift(State* s, long add) { s->L += add; s->R += add; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Query(State* s)
        {
            if (s->Lc <= s->Rc) return s->Offset + s->L * s->L;
            return s->Offset + s->R * s->R;
        }
    }
}