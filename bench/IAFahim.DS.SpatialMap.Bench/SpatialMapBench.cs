namespace IAFahim.DS.SpatialMap.Bench
{
    using System;
    using IAFahim.DS.SpatialMap;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;
    using Unity.Collections;
    using Unity.Jobs;
    using Unity.Mathematics;

    public static class Program
    {
        public static void Main(string[] args) => BenchmarkRunner.Run<SpatialMapBench>(args: args);
    }

    public struct BenchPosition : ISpatialPosition
    {
        public float2 Pos;
        public float2 Position => Pos;
    }

    [MemoryDiagnoser]
    public unsafe class SpatialMapBench
    {
        [Params(64, 256, 1024, 4096)]
        public int N;

        private NativeArray<BenchPosition> positions;
        private SpatialMap<BenchPosition> map;

        [GlobalSetup]
        public void Setup()
        {
            this.positions = new NativeArray<BenchPosition>(this.N, Allocator.Persistent);
            Unity.Mathematics.Random rng = new Unity.Mathematics.Random(42);
            for (int i = 0; i < this.N; i++)
            {
                this.positions[i] = new BenchPosition
                {
                    Pos = rng.NextFloat2(new float2(-50.0f), new float2(50.0f))
                };
            }
            this.map = new SpatialMap<BenchPosition>(2.0f, 100, Allocator.Persistent);
        }

        [Benchmark]
        public void BuildSpatialMap()
        {
            JobHandle handle = this.map.Build(this.positions, default, 2);
            handle.Complete();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.positions.Dispose();
            this.map.Dispose();
        }
    }
}
