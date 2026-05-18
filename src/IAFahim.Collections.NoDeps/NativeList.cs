namespace Unity.Collections
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Unity.Collections.LowLevel.Unsafe;

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct NativeList<T> : IDisposable where T : unmanaged
    {
        private IntPtr _buffer;
        private int _length;
        private int _capacity;
        private Allocator _allocator;

        private static readonly int _elementSize = UnsafeUtility.SizeOf<T>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeList(int initialCapacity, Allocator allocator)
        {
            this = default;
            _capacity = initialCapacity;
            _allocator = allocator;
            long byteCount = (long)initialCapacity * _elementSize;
            _buffer = Marshal.AllocHGlobal((nint)byteCount);
            UnsafeUtility.MemClear((void*)_buffer, byteCount);
        }

        public int Length
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _length;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value < 0)
                    value = 0;
                if (value > _capacity)
                    ResizeCapacity(value);
                _length = value;
            }
        }

        public int Capacity
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _capacity;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value < 0)
                    value = 0;
                if (value == _capacity)
                    return;

                var newBuffer = value == 0
                    ? IntPtr.Zero
                    : Marshal.AllocHGlobal((nint)((long)value * _elementSize));

                if (_buffer != IntPtr.Zero)
                {
                    if (value > 0)
                    {
                        var copySize = _length < value ? _length : value;
                        UnsafeUtility.MemCpy((void*)newBuffer, (void*)_buffer, (long)copySize * _elementSize);
                    }

                    Marshal.FreeHGlobal(_buffer);
                }

                _buffer = newBuffer;
                _capacity = value;
                if (_length > _capacity)
                    _length = _capacity;
            }
        }

        public bool IsCreated
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _buffer != IntPtr.Zero;
        }

        public T* GetUnsafePtr()
        {
            return (T*)_buffer;
        }

        public T this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                var byteOffset = _buffer + index * _elementSize;
                return System.Runtime.CompilerServices.Unsafe.Read<T>((void*)byteOffset);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                var byteOffset = _buffer + index * _elementSize;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            _length = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResizeCapacity(int newCapacity)
        {
            var cap = 4;
            while (cap < newCapacity)
                cap <<= 1;
            Capacity = cap;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(in T item)
        {
            if (_length >= _capacity)
                ResizeCapacity(_capacity < 4 ? 8 : _capacity << 1);

            var byteOffset = _buffer + _length * _elementSize;
            System.Runtime.CompilerServices.Unsafe.Write<T>((void*)byteOffset, item);
            _length++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _length)
                return;

            _length--;
            if (index == _length)
                return;

            var byteIndex = _buffer + index * _elementSize;
            var byteCount = (_length - index) * _elementSize;
            var src = (byte*)_buffer + (index + 1) * _elementSize;
            var dst = (byte*)_buffer + index * _elementSize;
            UnsafeUtility.MemCpy(dst, src, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveRange(int index, int count)
        {
            if (index < 0 || count < 0 || index >= _length)
                return;

            if (count > _length - index)
                count = _length - index;

            _length -= count;
            if (index == _length || count == 0)
                return;

            var byteIndex = _buffer + index * _elementSize;
            var byteCount = (_length - index) * _elementSize;
            var src = (byte*)_buffer + (index + count) * _elementSize;
            var dst = (byte*)_buffer + index * _elementSize;
            UnsafeUtility.MemCpy(dst, src, byteCount);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Resize(int length, NativeArrayOptions options)
        {
            if (length < 0)
                length = 0;

            if (length > _capacity)
                Capacity = length;

            var oldLength = _length;
            _length = length;

            if (options == NativeArrayOptions.ClearMemory && length > oldLength)
            {
                var startByte = _buffer + oldLength * _elementSize;
                var clearByteCount = (long)(length - oldLength) * _elementSize;
                UnsafeUtility.MemClear((byte*)startByte, clearByteCount);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ResizeUninitialized(int length)
        {
            if (length < 0)
                length = 0;

            if (length > _capacity)
                Capacity = length;

            _length = length;
        }
    }
}