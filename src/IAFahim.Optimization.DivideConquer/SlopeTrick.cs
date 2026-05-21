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
            long l = a - s->L;
            long r = s->R - a;
            if (l > r)
            {
                s->Offset += l;
                s->Lc = Math.Min(s->Lc, a);
            }
            else
            {
                s->Offset += r;
                s->Rc = Math.Max(s->Rc, a);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AddMaxZero(State* s)
        {
            s->L--;
            s->R++;
            if (s->L > 0) { s->Offset += s->L; s->L = 0; }
            if (s->R < 0) { s->Offset -= s->R; s->R = 0; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Shift(State* s, long add)
        {
            s->L += add;
            s->R += add;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Query(State* s)
        {
            if (s->Lc <= s->Rc) return s->Offset + s->L * s->L;
            return s->Offset + s->R * s->R;
        }
    }
}
