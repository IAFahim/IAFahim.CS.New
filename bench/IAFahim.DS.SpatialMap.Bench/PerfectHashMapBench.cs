namespace IAFahim.DS.SpatialMap.Bench
{
    using System;
    using System.Collections.Generic;
    using IAFahim.DS.PerfectHashMap;
    using BenchmarkDotNet.Attributes;
    using Unity.Collections;

    [MemoryDiagnoser]
    public class PerfectHashMapBench
    {
        [Params(64, 256, 1024)]
        public int N;

        private NativeArray<int> keys;
        private NativeArray<int> values;
        private NativePerfectHashMap<int, int> perfectMap;
        private Dictionary<int, int> bclMap;

        [GlobalSetup]
        public void Setup()
        {
            this.keys = new NativeArray<int>(this.N, Allocator.Persistent);
            this.values = new NativeArray<int>(this.N, Allocator.Persistent);
            this.bclMap = new Dictionary<int, int>(this.N);

            Random rng = new Random(42);
            for (int i = 0; i < this.N; i++)
            {
                int k = rng.Next(1, 1000000);
                while (this.bclMap.ContainsKey(k))
                {
                    k = rng.Next(1, 1000000);
                }

                this.keys[i] = k;
                this.values[i] = i;
                this.bclMap[k] = i;
            }

            this.perfectMap = new NativePerfectHashMap<int, int>(this.keys, this.values, -1, Allocator.Persistent);
        }

        [Benchmark(Baseline = true)]
        public int BclDictionaryLookup()
        {
            int sum = 0;
            for (int i = 0; i < this.N; i++)
            {
                int key = this.keys[i];
                sum += this.bclMap[key];
            }
            return sum;
        }

        [Benchmark]
        public int PerfectHashMapLookup()
        {
            int sum = 0;
            for (int i = 0; i < this.N; i++)
            {
                int key = this.keys[i];
                sum += this.perfectMap[key];
            }
            return sum;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            this.keys.Dispose();
            this.values.Dispose();
            this.perfectMap.Dispose();
        }
    }
}
