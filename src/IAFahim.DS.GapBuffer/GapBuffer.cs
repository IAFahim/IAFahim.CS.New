namespace IAFahim.DS.GapBuffer
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct GapBufferState
    {
        public byte* Buffer;
        public int Capacity;
        public int GapStart;
        public int GapEnd;
    }

    public static unsafe class GapBufferInsert
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(ref GapBufferState s, int pos, byte* data, int len)
        {
            MoveGap(ref s, pos);
            int gapSpace = s.GapEnd - s.GapStart;
            int n = len < gapSpace ? len : gapSpace;
            Buffer.MemoryCopy(data, s.Buffer + s.GapStart, n, n);
            s.GapStart += n;
        }

        private static void MoveGap(ref GapBufferState s, int pos)
        {
            if (pos == s.GapStart) return;
            int gapLen = s.GapEnd - s.GapStart;
            if (pos < s.GapStart)
            {
                int shift = s.GapStart - pos;
                Buffer.MemoryCopy(s.Buffer + pos, s.Buffer + s.GapEnd - shift, shift, shift);
                s.GapStart = pos;
                s.GapEnd = pos + gapLen;
            }
            else
            {
                int shift = pos - s.GapStart;
                Buffer.MemoryCopy(s.Buffer + s.GapEnd, s.Buffer + s.GapStart, shift, shift);
                s.GapStart = pos;
                s.GapEnd = pos + gapLen;
            }
        }
    }

    public static unsafe class GapBufferDelete
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run(ref GapBufferState s, int pos, int len)
        {
            MoveGapTo(ref s, pos);
            int del = len < (s.Capacity - s.GapEnd) ? len : (s.Capacity - s.GapEnd);
            s.GapEnd += del;
        }

        private static void MoveGapTo(ref GapBufferState s, int pos)
        {
            if (pos == s.GapStart) return;
            int gapLen = s.GapEnd - s.GapStart;
            if (pos < s.GapStart)
            {
                int shift = s.GapStart - pos;
                Buffer.MemoryCopy(s.Buffer + pos, s.Buffer + s.GapEnd - shift, shift, shift);
                s.GapStart = pos;
                s.GapEnd = pos + gapLen;
            }
            else
            {
                int shift = pos - s.GapStart;
                Buffer.MemoryCopy(s.Buffer + s.GapEnd, s.Buffer + s.GapStart, shift, shift);
                s.GapStart = pos;
                s.GapEnd = pos + gapLen;
            }
        }
    }
}
