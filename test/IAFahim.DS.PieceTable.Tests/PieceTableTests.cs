namespace IAFahim.DS.PieceTable.Tests
{
    using System.Runtime.InteropServices;
    using NUnit.Framework;

    public sealed unsafe class PieceTableTests
    {
        [Test]
        public void PieceTableInsert_Basic()
        {
            const int OrigCap = 32;
            const int AddCap = 64;
            byte* orig = (byte*)Marshal.AllocHGlobal(OrigCap);
            byte* added = (byte*)Marshal.AllocHGlobal(AddCap);
            Piece* pieces = (Piece*)Marshal.AllocHGlobal(32 * sizeof(Piece));

            try
            {
                for (int i = 0; i < OrigCap; i++) orig[i] = 0;
                for (int i = 0; i < AddCap; i++) added[i] = 0;

                PieceTableState s;
                s.Original = orig;
                s.OriginalLen = 0;
                s.Added = added;
                s.AddedLen = 0;
                s.AddedCap = AddCap;
                s.Head = null;

                int pieceCount = 0;
                byte* data = stackalloc byte[5];
                for (int i = 0; i < 5; i++) data[i] = (byte)('A' + i);
                PieceTableInsert.Run(ref s, 0, data, 5, pieces, ref pieceCount);

                Assert.AreEqual(5, s.AddedLen);
                Assert.IsTrue(pieceCount >= 1);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)orig);
                Marshal.FreeHGlobal((nint)added);
                Marshal.FreeHGlobal((nint)pieces);
            }
        }

        [Test]
        public void PieceTableInsert_CapShort_UsesCopiedLength()
        {
            const int AddCap = 3;
            byte* orig = (byte*)Marshal.AllocHGlobal(8);
            byte* added = (byte*)Marshal.AllocHGlobal(AddCap);
            Piece* pieces = (Piece*)Marshal.AllocHGlobal(8 * sizeof(Piece));
            try
            {
                PieceTableState s;
                s.Original = orig;
                s.OriginalLen = 0;
                s.Added = added;
                s.AddedLen = 0;
                s.AddedCap = AddCap;
                s.Head = null;
                int pc = 0;
                byte* data = stackalloc byte[5];
                for (int i = 0; i < 5; i++) data[i] = (byte)('A' + i);
                PieceTableInsert.Run(ref s, 0, data, 5, pieces, ref pc);
                Assert.AreEqual(3, s.AddedLen);
                Assert.AreEqual(3, pieces[0].Length);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)orig);
                Marshal.FreeHGlobal((nint)added);
                Marshal.FreeHGlobal((nint)pieces);
            }
        }

        [Test]
        public void PieceTableDelete_Basic()
        {
            const int AddCap = 64;
            byte* orig = (byte*)Marshal.AllocHGlobal(16);
            byte* added = (byte*)Marshal.AllocHGlobal(AddCap);
            Piece* pieces = (Piece*)Marshal.AllocHGlobal(32 * sizeof(Piece));

            try
            {
                PieceTableState s;
                s.Original = orig;
                s.OriginalLen = 0;
                s.Added = added;
                s.AddedLen = 0;
                s.AddedCap = AddCap;
                s.Head = null;

                int pc = 0;
                byte* data = stackalloc byte[5];
                for (int i = 0; i < 5; i++) data[i] = (byte)('A' + i);
                PieceTableInsert.Run(ref s, 0, data, 5, pieces, ref pc);

                PieceTableDelete.Run(ref s, 1, 2, pieces, ref pc);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)orig);
                Marshal.FreeHGlobal((nint)added);
                Marshal.FreeHGlobal((nint)pieces);
            }
        }
    }
}
