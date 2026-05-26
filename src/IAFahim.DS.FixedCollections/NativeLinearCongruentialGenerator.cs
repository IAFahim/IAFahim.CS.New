namespace IAFahim.DS.FixedCollections
{
    using System;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    using BovineLabs.Core.Memory;

    [NativeContainer]
    public unsafe struct NativeLinearCongruentialGenerator : IDisposable
    {
        private const int Multiplier = 134775813;
        private const int Increment = 1;
        private const int Modulus = int.MaxValue;

        [NativeDisableUnsafePtrRestriction]
        private int* current;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_Safety;
        private static readonly SharedStatic<int> s_staticSafetyId = SharedStatic<int>.GetOrCreate<NativeLinearCongruentialGenerator>();
#endif

        private AllocatorManager.AllocatorHandle allocatorLabel;

        public NativeLinearCongruentialGenerator(int seed, Allocator allocator)
        {
            Allocate(allocator, out this);
            *this.current = seed;
        }

        private static void Allocate(AllocatorManager.AllocatorHandle allocator, out NativeLinearCongruentialGenerator reference)
        {
            CollectionHelper.CheckAllocator(allocator);

            reference = default;
            reference.current = (int*)Unmanaged.Allocate((long)UnsafeUtility.SizeOf<int>(), UnsafeUtility.AlignOf<int>(), allocator);
            reference.allocatorLabel = allocator;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            reference.m_Safety = CollectionHelper.CreateSafetyHandle(allocator);

            CollectionHelper.SetStaticSafetyId<NativeLinearCongruentialGenerator>(ref reference.m_Safety, ref s_staticSafetyId.Data);
            AtomicSafetyHandle.SetBumpSecondaryVersionOnScheduleWrite(reference.m_Safety, true);
#endif
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            CollectionHelper.DisposeSafetyHandle(ref this.m_Safety);
#endif
            Unmanaged.Free(this.current, this.allocatorLabel);

            this.current = null;
        }

        public int Next()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(this.m_Safety);
#endif

            int x = *this.current;
            int x1 = ((Multiplier * x) + Increment) & Modulus;
            *this.current = x1;

            return *this.current;
        }
    }
}
