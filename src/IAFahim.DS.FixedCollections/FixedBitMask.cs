namespace IAFahim.DS.FixedCollections
{
    using System;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Unity.Collections.LowLevel.Unsafe;

    public unsafe struct FixedBitMask<T>
        where T : unmanaged
    {
        private const int Idx = 3;
        private const int Shift = (1 << Idx) - 1;

        private T data;

        public int Length => UnsafeUtility.SizeOf<T>() << 3;

        public void Set(int pos, bool value)
        {
            this.CheckArgs(pos, 1);

            fixed (T* t = &this.data)
            {
                byte* ptr = (byte*)t;

                int idx = pos >> Idx;
                int shift = pos & Shift;
                byte mask = (byte)(1 << shift);

                byte bits = (byte)((ptr[idx] & ~mask) | (-AsByte(value) & mask));
                ptr[idx] = bits;
            }
        }

        public bool IsSet(int pos)
        {
            this.CheckArgs(pos, 1);

            fixed (T* t = &this.data)
            {
                byte* ptr = (byte*)t;

                int idx = pos >> Idx;
                int shift = pos & Shift;
                byte mask = (byte)(1 << shift);
                return (ptr[idx] & mask) != 0;
            }
        }

        public void Reset()
        {
            fixed (T* t = &this.data)
            {
                UnsafeUtility.MemClear(t, UnsafeUtility.SizeOf<T>());
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static int AsByte(bool value)
        {
            return value ? 1 : 0;
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private void CheckArgs(int pos, int numBits)
        {
            if (pos < 0 || pos >= this.Length || numBits < 1)
            {
                throw new ArgumentException();
            }
        }
    }
}
