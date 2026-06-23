namespace IAFahim.DS.PieceTable
{
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct Piece
    {
        public int BufferIndex;
        public int Start;
        public int Length;
        public Piece* Next;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct PieceTableState
    {
        public byte* Original;
        public int OriginalLen;
        public byte* Added;
        public int AddedLen;
        public int AddedCap;
        public Piece* Head;
    }

    public static unsafe class PieceTableShared
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Piece* MakeTailPiece(Piece* newPieces, ref int pieceCount, Piece* src, int splitAt)
        {
            Piece* tail = newPieces + pieceCount++;
            tail->BufferIndex = src->BufferIndex;
            tail->Start = src->Start + splitAt;
            tail->Length = src->Length - splitAt;
            tail->Next = src->Next;
            return tail;
        }
    }

    public static unsafe class PieceTableInsert
    {
        public static void Run(ref PieceTableState s, int pos, byte* data, int len,
            Piece* newPieces, ref int pieceCount)
        {
            if (len <= 0) return;
            int addStart = s.AddedLen;
            for (int i = 0; i < len && s.AddedLen < s.AddedCap; i++)
                s.Added[s.AddedLen++] = data[i];
            int copied = s.AddedLen - addStart;

            Piece* after = s.Head;
            Piece* prev = null;
            int offset = 0;

            while (after != null && offset + after->Length <= pos)
            {
                offset += after->Length;
                prev = after;
                after = after->Next;
            }

            Piece* newPiece = newPieces + pieceCount++;
            newPiece->BufferIndex = 1;
            newPiece->Start = addStart;
            newPiece->Length = copied;
            newPiece->Next = null;

            if (after != null && offset < pos)
            {
                int splitAt = pos - offset;
                Piece* tail = PieceTableShared.MakeTailPiece(newPieces, ref pieceCount, after, splitAt);
                after->Length = splitAt;
                after->Next = newPiece;
                newPiece->Next = tail;
            }
            else
            {
                newPiece->Next = after;
                if (prev != null) prev->Next = newPiece;
                else s.Head = newPiece;
            }
        }
    }

    public static unsafe class PieceTableDelete
    {
        public static void Run(ref PieceTableState s, int pos, int len,
            Piece* newPieces, ref int pieceCount)
        {
            if (len <= 0) return;
            int end = pos + len;

            Piece* cur = s.Head;
            Piece* prev = null;
            int offset = 0;

            while (cur != null && offset + cur->Length <= pos)
            {
                offset += cur->Length;
                prev = cur;
                cur = cur->Next;
            }

            if (cur == null) return;

            int startSplit = pos - offset;
            if (startSplit > 0)
            {
                Piece* tail = PieceTableShared.MakeTailPiece(newPieces, ref pieceCount, cur, startSplit);
                cur->Length = startSplit;
                cur->Next = tail;
                prev = cur;
                cur = tail;
                offset = pos;
            }

            while (cur != null && offset + cur->Length <= end)
            {
                int removed = cur->Length;
                Piece* next = cur->Next;
                if (prev != null) prev->Next = next;
                else s.Head = next;
                offset += removed;
                cur = next;
            }

            if (cur != null && offset < end)
            {
                int trim = end - offset;
                cur->Start += trim;
                cur->Length -= trim;
            }
        }
    }
}
