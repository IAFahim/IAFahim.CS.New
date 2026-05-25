namespace IAFahim.DS.SpatialMap
{
    using System;
    using System.Runtime.CompilerServices;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Jobs;
    using Unity.Mathematics;

    public struct SpatialMap<T> : IDisposable
        where T : unmanaged, ISpatialPosition
    {
        private readonly float quantizeStep;
        private readonly int quantizeSize;
        private readonly int2 halfSize;

        private NativeParallelMultiHashMap<int, int> map;

        public SpatialMap(float quantizeStep, int size, Allocator allocator = Allocator.Persistent)
        {
            this.quantizeStep = quantizeStep;
            this.quantizeSize = (int)math.ceil((float)size / quantizeStep);
            this.halfSize = new int2(size) / 2;

            this.map = new NativeParallelMultiHashMap<int, int>(0, allocator);
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

        public readonly SpatialMapReadOnly AsReadOnly()
        {
            return new SpatialMapReadOnly(this.quantizeStep, this.quantizeSize, this.halfSize, this.map);
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
            public NativeParallelMultiHashMap<int, int> Map;

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
            public NativeParallelMultiHashMap<int, int> Map;

            public float QuantizeStep;
            public int QuantizeWidth;
            public int2 HalfSize;
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

                int* keys = (int*)this.Map.GetUnsafeBucketData().keys;
                int* values = (int*)this.Map.GetUnsafeBucketData().values;

                for (int entityInQueryIndex = start; entityInQueryIndex < end; entityInQueryIndex++)
                {
                    float2 position = this.Positions[entityInQueryIndex].Position;
                    int2 quantized = SpatialMapUtility.Quantized(position, this.QuantizeStep, this.HalfSize);

                    if (math.any(quantized >= this.QuantizeWidth))
                    {
                        continue;
                    }

                    int hashed = SpatialMapUtility.Hash(quantized, this.QuantizeWidth);
                    keys[entityInQueryIndex] = hashed;
                    values[entityInQueryIndex] = entityInQueryIndex;
                }
            }
        }

        [BurstCompile]
        public struct CalculateMapJob : IJob
        {
            public NativeParallelMultiHashMap<int, int> SpatialHashMap;

            public void Execute()
            {
                this.SpatialHashMap.RecalculateBuckets();
            }
        }
    }

    public static class SpatialMapUtility
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 Quantized(float2 position, float step, int2 halfSize)
        {
            return new int2(math.floor((position + halfSize) / step));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Hash(int2 quantized, int width)
        {
            return quantized.x + (quantized.y * width);
        }
    }

    public readonly struct SpatialMapReadOnly
    {
        private readonly float quantizeStep;
        private readonly int quantizeWidth;
        private readonly int2 halfSize;

        public SpatialMapReadOnly(float quantizeStep, int quantizeWidth, int2 halfSize, NativeParallelMultiHashMap<int, int> map)
        {
            this.quantizeStep = quantizeStep;
            this.quantizeWidth = quantizeWidth;
            this.halfSize = halfSize;
            this.Map = map;
        }

        public NativeParallelMultiHashMap<int, int> Map { get; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int2 Quantized(float2 position)
        {
            return SpatialMapUtility.Quantized(position, this.quantizeStep, this.halfSize);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Hash(int2 quantized)
        {
            return SpatialMapUtility.Hash(quantized, this.quantizeWidth);
        }
    }
}
