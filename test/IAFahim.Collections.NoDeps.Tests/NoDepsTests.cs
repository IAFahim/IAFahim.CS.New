namespace IAFahim.Collections.NoDeps.Tests
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using Xunit;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public sealed unsafe class NativeArrayTests
    {
        [Fact]
        public void Ctor_ZeroLength_IsCreatedFalse()
        {
            var arr = new NativeArray<int>(0, Allocator.Persistent);
            Assert.False(arr.IsCreated);
            arr.Dispose();
        }

        [Fact]
        public void Ctor_NormalLength_IsCreatedTrue()
        {
            var arr = new NativeArray<int>(4, Allocator.Persistent);
            Assert.True(arr.IsCreated);
            Assert.Equal(4, arr.Length);
            arr.Dispose();
        }

        [Fact]
        public void Ctor_ClearMemory_FillsWithZero()
        {
            var arr = new NativeArray<int>(4, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            for (int i = 0; i < arr.Length; i++)
                Assert.Equal(0, arr[i]);
            arr.Dispose();
        }

        [Fact]
        public void Ctor_UninitializedMemory_ContainsGarbage()
        {
            var arr = new NativeArray<int>(4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            int sum = 0;
            for (int i = 0; i < arr.Length; i++)
                sum += arr[i];
            arr.Dispose();
            GC.KeepAlive(sum);
        }

        [Fact]
        public void Indexer_ReadWrite_Works()
        {
            var arr = new NativeArray<int>(4, Allocator.Persistent);
            arr[0] = 10;
            arr[1] = 20;
            arr[2] = 30;
            arr[3] = 40;
            Assert.Equal(10, arr[0]);
            Assert.Equal(20, arr[1]);
            Assert.Equal(30, arr[2]);
            Assert.Equal(40, arr[3]);
            arr.Dispose();
        }

        [Fact]
        public void Dispose_Idempotent()
        {
            var arr = new NativeArray<int>(8, Allocator.Persistent);
            arr.Dispose();
            arr.Dispose();
        }

        [Fact]
        public void Dispose_CanAllocateAfter()
        {
            {
                var arr = new NativeArray<int>(8, Allocator.Persistent);
                arr.Dispose();
            }
            {
                var arr = new NativeArray<double>(4, Allocator.Persistent);
                Assert.True(arr.IsCreated);
                arr[0] = 1.5;
                Assert.Equal(1.5, arr[0]);
                arr.Dispose();
            }
        }
    }

    public sealed unsafe class NativeListTests
    {
        [Fact]
        public void Default_Ctor_IsNotCreated()
        {
            var list = default(NativeList<int>);
            Assert.False(list.IsCreated);
        }

        [Fact]
        public void Ctor_InitialCapacity_Works()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            Assert.True(list.IsCreated);
            Assert.Equal(4, list.Capacity);
            Assert.Equal(0, list.Length);
            list.Dispose();
        }

        [Fact]
        public void Add_AppendsItem()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            Assert.Equal(3, list.Length);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            Assert.Equal(3, list[2]);
            list.Dispose();
        }

        [Fact]
        public void Add_ResizesWhenNeeded()
        {
            var list = new NativeList<int>(1, Allocator.Persistent);
            for (int i = 0; i < 16; i++)
                list.Add(i);
            Assert.Equal(16, list.Length);
            Assert.True(list.Capacity >= 16);
            for (int i = 0; i < 16; i++)
                Assert.Equal(i, list[i]);
            list.Dispose();
        }

        [Fact]
        public void Clear_ResetsLength()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Clear();
            Assert.Equal(0, list.Length);
            list.Dispose();
        }

        [Fact]
        public void RemoveAt_Last_RemovesWithoutShift()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.RemoveAt(2);
            Assert.Equal(2, list.Length);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            list.Dispose();
        }

        [Fact]
        public void RemoveAt_Middle_ShiftsElements()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.RemoveAt(1);
            Assert.Equal(2, list.Length);
            Assert.Equal(1, list[0]);
            Assert.Equal(3, list[1]);
            list.Dispose();
        }

        [Fact]
        public void RemoveRange_RemovesMultiple()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            for (int i = 0; i < 8; i++)
                list.Add(i);
            list.RemoveRange(2, 3);
            Assert.Equal(5, list.Length);
            Assert.Equal(0, list[0]);
            Assert.Equal(1, list[1]);
            Assert.Equal(5, list[2]);
            Assert.Equal(6, list[3]);
            Assert.Equal(7, list[4]);
            list.Dispose();
        }

        [Fact]
        public void Resize_ClearMemory_ZeroesNew()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Resize(4, NativeArrayOptions.ClearMemory);
            Assert.Equal(4, list.Length);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            Assert.Equal(0, list[2]);
            Assert.Equal(0, list[3]);
            list.Dispose();
        }

        [Fact]
        public void ResizeUninitialized_SkipsZeroing()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.ResizeUninitialized(4);
            Assert.Equal(4, list.Length);
            list.Dispose();
            GC.KeepAlive(list);
        }

        [Fact]
        public void GetUnsafePtr_ReturnsPointer()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(42);
            var ptr = list.GetUnsafePtr();
            Assert.Equal(42, *ptr);
            list.Dispose();
        }

        [Fact]
        public void Dispose_Idempotent()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Dispose();
            list.Dispose();
        }

        [Fact]
        public void Capacity_Set_Reallocates()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Capacity = 32;
            Assert.Equal(32, list.Capacity);
            Assert.Equal(3, list.Length);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            Assert.Equal(3, list[2]);
            list.Dispose();
        }

        [Fact]
        public void Length_Set_Expands()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Length = 8;
            Assert.Equal(8, list.Length);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            list.Dispose();
        }

        [Fact]
        public void Length_Set_Shrinks()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Length = 1;
            Assert.Equal(1, list.Length);
            Assert.Equal(1, list[0]);
            list.Dispose();
        }
    }

    public sealed unsafe class UnsafeListTests
    {
        [Fact]
        public void Default_IsNotCreated()
        {
            var list = default(UnsafeList<int>);
            Assert.False(list.IsCreated);
        }

        [Fact]
        public void Ctor_Works()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            Assert.True(list.IsCreated);
            Assert.Equal(4, list.Capacity);
            Assert.Equal(0, list.Length);
            list.Dispose();
        }

        [Fact]
        public void Add_AppendsItem()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            Assert.Equal(2, list.Length);
            Assert.Equal(1, list[0]);
            Assert.Equal(2, list[1]);
            list.Dispose();
        }

        [Fact]
        public void AddRange_CopiesItems()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            int* src = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            for (int i = 0; i < 4; i++)
                src[i] = i * 10;

            list.AddRange(src, 4);

            Assert.Equal(4, list.Length);
            Assert.Equal(0, list[0]);
            Assert.Equal(10, list[1]);
            Assert.Equal(20, list[2]);
            Assert.Equal(30, list[3]);
            Marshal.FreeHGlobal((IntPtr)src);
            list.Dispose();
        }

        [Fact]
        public void Clear_ResetsLength()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Clear();
            Assert.Equal(0, list.Length);
            list.Dispose();
        }

        [Fact]
        public void Dispose_Idempotent()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.Dispose();
            list.Dispose();
        }

        [Fact]
        public void Ptr_ReturnsValidPointer()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.Add(99);
            Assert.True(list.Ptr != null);
            Assert.Equal(99, *list.Ptr);
            list.Dispose();
        }
    }

    public sealed unsafe class AllocatorManagerTests
    {
        [Fact]
        public void AllocatorHandle_ImplicitConversion()
        {
            var handle = (AllocatorHandle)Allocator.Persistent;
            Assert.Equal((int)Allocator.Persistent, handle.Value);
        }

        [Fact]
        public void AllocateWithHandle_Allocates()
        {
            var handle = (AllocatorHandle)Allocator.Persistent;
            var ptr = AllocatorManager.Allocate(handle, 64, 8);
            Assert.True(ptr != null);
            AllocatorManager.Free(handle, ptr);
        }

        [Fact]
        public void AllocateResizeHint_NoOp()
        {
            var handle = (AllocatorHandle)Allocator.Persistent;
            IntPtr mem = Marshal.AllocHGlobal(128);
            AllocatorManager.Allocate(handle, (void*)mem, 128, 8);
            Marshal.FreeHGlobal(mem);
        }
    }

    public sealed unsafe class UnsafeUtilityTests
    {
        [Fact]
        public void AddressOf_ReturnsPointer()
        {
            int value = 42;
            var ptr = UnsafeUtility.AddressOf(ref value);
            Assert.Equal((IntPtr)Unsafe.AsPointer(ref value), (IntPtr)ptr);
        }

        [Fact]
        public void As_ReinterpretsRef()
        {
            int intVal = 0x12345678;
            ref byte bRef = ref UnsafeUtility.As<int, byte>(ref intVal);
            byte* bPtr = (byte*)UnsafeUtility.AddressOf(ref bRef);
            Assert.Equal(0x78, *bPtr);
        }

        [Fact]
        public void MemClear_ZerosMemory()
        {
            int* ptr = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4;
            UnsafeUtility.MemClear(ptr, 4 * sizeof(int));
            for (int i = 0; i < 4; i++)
                Assert.Equal(0, ptr[i]);
            Marshal.FreeHGlobal((IntPtr)ptr);
        }

        [Fact]
        public void MemSet_FillsMemory()
        {
            byte* ptr = (byte*)Marshal.AllocHGlobal(16);
            UnsafeUtility.MemSet(ptr, 0xAB, 16);
            for (int i = 0; i < 16; i++)
                Assert.Equal(0xAB, ptr[i]);
            Marshal.FreeHGlobal((IntPtr)ptr);
        }

        [Fact]
        public void MemCpy_CopiesMemory()
        {
            int* src = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* dst = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            for (int i = 0; i < 4; i++)
                src[i] = i * 7;
            UnsafeUtility.MemCpy(dst, src, 4 * sizeof(int));
            for (int i = 0; i < 4; i++)
                Assert.Equal(src[i], dst[i]);
            Marshal.FreeHGlobal((IntPtr)src);
            Marshal.FreeHGlobal((IntPtr)dst);
        }
    }
}