using System;
namespace IAFahim.DS.GapBuffer.Bench
{
    using IAFahim.DS.GapBuffer;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<GapBufferBench>(args: args);
    }

    [MemoryDiagnoser]
    public unsafe class GapBufferBench
    {
        [Params(1024, 4096, 16384)]
        public int N;

        private byte* _buffer;
        private GapBufferState* _state;
        private byte* _insertBuf;

        [GlobalSetup]
        public void Setup()
        {
            _buffer = (byte*)Marshal.AllocHGlobal(N * 2 * sizeof(byte));
            _state = (GapBufferState*)Marshal.AllocHGlobal(sizeof(GapBufferState));
            _insertBuf = (byte*)Marshal.AllocHGlobal(256 * sizeof(byte));
            _state->Buffer = _buffer;
            _state->Capacity = N * 2;
            _state->GapStart = N;
            _state->GapEnd = N * 2;
            for (int i = 0; i < N; i++)
                _state->Buffer[i] = (byte)(i & 0xFF);
        }

        [IterationSetup]
        public void ResetState()
        {
            _state->GapStart = N;
            _state->GapEnd = N * 2;
            for (int i = 0; i < N; i++)
                _state->Buffer[i] = (byte)(i & 0xFF);
            for (int i = 0; i < 64; i++)
                _insertBuf[i] = (byte)(i * 3);
        }

        [Benchmark(Baseline = true)]
        public void GapBufferInsert()
        {
            ResetState();
            for (int i = 0; i < 32; i++)
                global::IAFahim.DS.GapBuffer.GapBufferInsert.Run(ref *_state, i * 16, _insertBuf, 16);
        }

        [Benchmark]
        public void GapBufferDelete()
        {
            for (int i = 0; i < 16; i++)
                global::IAFahim.DS.GapBuffer.GapBufferDelete.Run(ref *_state, i * 32, 16);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_buffer);
            Marshal.FreeHGlobal((nint)_state);
            Marshal.FreeHGlobal((nint)_insertBuf);
        }
    }
}