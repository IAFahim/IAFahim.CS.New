namespace IAFahim.DS.SpatialMap
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Jobs;
    using Unity.Mathematics;

    public struct SpatialMap3<T> : IDisposable
        where T : unmanaged, ISpatialPosition3
    {
        private readonly float quantizeStep;
        private readonly int quantizeSize;
        private readonly int3 halfSize;

        private NativeParallelMultiHashMap<long, int> map;

        public SpatialMap3(float quantizeStep, int size, Allocator allocator = Allocator.Persistent)
        {
            this.quantizeStep = quantizeStep;
            this.quantizeSize = (int)math.ceil((float)size / quantizeStep);
            this.halfSize = new int3(size) / 2;

            this.map = new NativeParallelMultiHashMap<long, int>(0, allocator);
        }

        public readonly bool IsCreated => this.map.IsCreated;

        public readonly void Dispose()
        {
            this.map.Dispose();
        }

        public readonly JobHandle Dispose(JobHandle dependency)
        {
            return this.map.Dispose(dependency);
        }

        public readonly SpatialMapReadOnly3 AsReadOnly()
        {
            return new SpatialMapReadOnly3(this.quantizeStep, this.quantizeSize, this.halfSize, this.map);
        }

        public JobHandle Build(
            NativeArray<T> positions, 
            JobHandle dependency, 
            int workerCount)
        {
            dependency = new ResizeNativeParallelHashMapJob
            {
                Length = positions,
                Map = this.map,
            }.Schedule(dependency);

            dependency = new QuantizeJob
            {
                Positions = positions,
                Map = this.map,
                QuantizeStep = this.quantizeStep,
                QuantizeWidth = this.quantizeSize,
                QuantizeDepth = this.quantizeSize,
                HalfSize = this.halfSize,
                Workers = workerCount,
            }.ScheduleParallel(workerCount, 1, dependency);

            dependency = new CalculateMapJob
            {
                SpatialHashMap = this.map,
            }.Schedule(dependency);

            return dependency;
        }

        [BurstCompile]
        public struct ResizeNativeParallelHashMapJob : IJob
        {
            public NativeParallelMultiHashMap<long, int> Map;

            [ReadOnly]
            public NativeArray<T> Length;

            public void Execute()
            {
                if (this.Map.Capacity < this.Length.Length)
                {
                    this.Map.Capacity = this.Length.Length;
                }

                this.Map.Clear();
                this.Map.SetAllocatedIndexLength(this.Length.Length);
            }
        }

        [BurstCompile]
        public unsafe struct QuantizeJob : IJobFor
        {
            [ReadOnly]
            public NativeArray<T> Positions;

            [NativeDisableParallelForRestriction]
            public NativeParallelMultiHashMap<long, int> Map;

            public float QuantizeStep;
            public int QuantizeWidth;
            public int QuantizeDepth;
            public int3 HalfSize;
            public int Workers;

            public void Execute(int index)
            {
                int length = this.Positions.Length / this.Workers;
                int start = index * length;
                int end = start + length;
                if (index == this.Workers - 1)
                {
                    end += this.Positions.Length % this.Workers;
                }

                long* keys = (long*)this.Map.GetUnsafeBucketData().keys;
                int* values = (int*)this.Map.GetUnsafeBucketData().values;

                for (int entityInQueryIndex = start; entityInQueryIndex < end; entityInQueryIndex++)
                {
                    float3 position = this.Positions[entityInQueryIndex].Position;
                    int3 quantized = SpatialMapUtility3.Quantized(position, this.QuantizeStep, this.HalfSize);

                    if (math.any(quantized >= this.QuantizeWidth) || math.any(quantized < 0))
                    {
                        continue;
                    }

                    long hashed = SpatialMapUtility3.Hash(quantized, this.QuantizeWidth, this.QuantizeDepth);
                    keys[entityInQueryIndex] = hashed;
                    values[entityInQueryIndex] = entityInQueryIndex;
                }
            }
        }

        [BurstCompile]
        public struct CalculateMapJob : IJob
        {
            public NativeParallelMultiHashMap<long, int> SpatialHashMap;

            public void Execute()
            {
                this.SpatialHashMap.RecalculateBuckets();
            }
        }
    }

    public static class SpatialMapUtility3
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 Quantized(float3 position, float step, int3 halfSize)
        {
            return new int3(math.floor((position + halfSize) / step));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long Hash(int3 quantized, int width, int depth)
        {
            return (long)quantized.x + ((long)quantized.y * width) + ((long)quantized.z * width * depth);
        }
    }

    public readonly struct SpatialMapReadOnly3
    {
        private readonly float quantizeStep;
        private readonly int quantizeWidth;
        private readonly int3 halfSize;

        public SpatialMapReadOnly3(float quantizeStep, int quantizeWidth, int3 halfSize, NativeParallelMultiHashMap<long, int> map)
        {
            this.quantizeStep = quantizeStep;
            this.quantizeWidth = quantizeWidth;
            this.halfSize = halfSize;
            this.Map = map;
        }

        public NativeParallelMultiHashMap<long, int> Map { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int3 Quantized(float3 position)
        {
            return SpatialMapUtility3.Quantized(position, this.quantizeStep, this.halfSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long Hash(int3 quantized)
        {
            return SpatialMapUtility3.Hash(quantized, this.quantizeWidth, this.quantizeWidth);
        }
    }
}
