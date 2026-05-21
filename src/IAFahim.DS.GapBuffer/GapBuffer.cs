namespace IAFahim.DS.GapBuffer
{
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
            for (int i = 0; i < len && s.GapStart < s.GapEnd; i++)
            {
                s.Buffer[s.GapStart++] = data[i];
            }
        }

        private static void MoveGap(ref GapBufferState s, int pos)
        {
            if (pos == s.GapStart) return;
            int gapLen = s.GapEnd - s.GapStart;
            if (pos < s.GapStart)
            {
                int shift = s.GapStart - pos;
                for (int i = gapLen - 1; i >= 0; i--)
                    s.Buffer[pos + gapLen + i] = s.Buffer[pos + i];
                s.GapStart = pos;
                s.GapEnd = pos + gapLen;
            }
            else
            {
                int newPos = pos - s.GapStart;
                for (int i = 0; i < newPos; i++)
                    s.Buffer[s.GapStart + i] = s.Buffer[s.GapEnd + i];
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
                for (int i = 0; i < gapLen; i++)
                    s.Buffer[pos + gapLen + (gapLen - 1 - i)] = s.Buffer[pos + (gapLen - 1 - i)];
                s.GapStart = pos;
                s.GapEnd = pos + gapLen;
            }
            else
            {
                int shift = pos - s.GapStart;
                for (int i = 0; i < shift; i++)
                    s.Buffer[s.GapStart + i] = s.Buffer[s.GapEnd + i];
                s.GapStart = pos;
                s.GapEnd = pos + gapLen;
            }
        }
    }
}
