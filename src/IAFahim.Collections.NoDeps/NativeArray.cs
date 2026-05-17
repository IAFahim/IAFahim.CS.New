namespace Unity.Collections
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.Collections.LowLevel.Unsafe;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeArray<T> : IDisposable where T : unmanaged
    {
        private IntPtr _buffer;
        private int _length;
        private Allocator _allocator;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
        {
            this = default;
            _length = length;
            _allocator = allocator;
            if (length > 0)
            {
                _buffer = Marshal.AllocHGlobal(length * UnsafeUtility.SizeOf<T>());
                if (options == NativeArrayOptions.ClearMemory)
                    UnsafeUtility.MemClear((void*)_buffer, length * UnsafeUtility.SizeOf<T>());
            }
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _length;
        }

        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer != IntPtr.Zero;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var byteOffset = _buffer + index * UnsafeUtility.SizeOf<T>();
                return System.Runtime.CompilerServices.Unsafe.Read<T>((void*)byteOffset);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                var byteOffset = _buffer + index * UnsafeUtility.SizeOf<T>();
                System.Runtime.CompilerServices.Unsafe.Write<T>((void*)byteOffset, value);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose()
        {
            if (_buffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(_buffer);
                _buffer = IntPtr.Zero;
            }

            this = default;
        }
    }
}