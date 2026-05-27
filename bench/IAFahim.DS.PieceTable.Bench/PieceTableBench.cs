using System;
namespace IAFahim.DS.PieceTable.Bench
{
    using IAFahim.DS.PieceTable;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<PieceTableBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class PieceTableBench
    {
        [Params(1024, 4096)]
        public int N;

        private byte* _original;
        private byte* _added;
        private PieceTableState* _state;
        private Piece* _pieces;
        private byte* _insertBuf;

        [GlobalSetup]
        public void Setup()
        {
            _original = (byte*)Marshal.AllocHGlobal(N * sizeof(byte));
            _added = (byte*)Marshal.AllocHGlobal(N * sizeof(byte));
            _state = (PieceTableState*)Marshal.AllocHGlobal(sizeof(PieceTableState));
            _pieces = (Piece*)Marshal.AllocHGlobal(N * sizeof(Piece));
            _insertBuf = (byte*)Marshal.AllocHGlobal(256 * sizeof(byte));

            _state->Original = _original;
            _state->OriginalLen = N;
            _state->Added = _added;
            _state->AddedLen = 0;
            _state->AddedCap = N;
            _state->Head = _pieces;

            _state->Head->BufferIndex = 0;
            _state->Head->Start = 0;
            _state->Head->Length = N;
            _state->Head->Next = null;

            for (int i = 0; i < N; i++)
                _original[i] = (byte)((i * 7) & 0xFF);
        }

        private int _pieceCount;

        [IterationSetup]
        public void ResetState()
        {
            _state->AddedLen = 0;
            _state->Head = _pieces;
            _state->Head->BufferIndex = 0;
            _state->Head->Start = 0;
            _state->Head->Length = N;
            _state->Head->Next = null;
            _pieceCount = 1;

            for (int i = 0; i < 64; i++)
                _insertBuf[i] = (byte)((i * 13) & 0xFF);
        }

        [Benchmark(Baseline = true)]
        public void PieceTableInsert()
        {
            for (int i = 0; i < 16; i++)
            {
                global::IAFahim.DS.PieceTable.PieceTableInsert.Run(ref *_state, i * 16, _insertBuf, 16, _pieces, ref _pieceCount);
            }
        }

        [Benchmark]
        public void PieceTableDelete()
        {
            for (int i = 0; i < 8; i++)
                global::IAFahim.DS.PieceTable.PieceTableDelete.Run(ref *_state, i * 32, 16, _pieces, ref _pieceCount);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_original);
            Marshal.FreeHGlobal((nint)_added);
            Marshal.FreeHGlobal((nint)_state);
            Marshal.FreeHGlobal((nint)_pieces);
            Marshal.FreeHGlobal((nint)_insertBuf);
        }
    }
}