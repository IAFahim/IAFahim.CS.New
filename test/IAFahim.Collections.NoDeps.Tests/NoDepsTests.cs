namespace IAFahim.Collections.NoDeps.Tests
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using NUnit.Framework;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public sealed unsafe class NativeArrayTests
    {
        [Test]
        public void Ctor_ZeroLength_IsCreatedFalse()
        {
            var arr = new NativeArray<int>(0, Allocator.Persistent);
            Assert.IsFalse(arr.IsCreated);
            arr.Dispose();
        }

        [Test]
        public void Ctor_NormalLength_IsCreatedTrue()
        {
            var arr = new NativeArray<int>(4, Allocator.Persistent);
            Assert.IsTrue(arr.IsCreated);
            Assert.AreEqual(4, arr.Length);
            arr.Dispose();
        }

        [Test]
        public void Ctor_ClearMemory_FillsWithZero()
        {
            var arr = new NativeArray<int>(4, Allocator.Persistent, NativeArrayOptions.ClearMemory);
            for (int i = 0; i < arr.Length; i++)
                Assert.AreEqual(0, arr[i]);
            arr.Dispose();
        }

        [Test]
        public void Ctor_UninitializedMemory_ContainsGarbage()
        {
            var arr = new NativeArray<int>(4, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            int sum = 0;
            for (int i = 0; i < arr.Length; i++)
                sum += arr[i];
            arr.Dispose();
            GC.KeepAlive(sum);
        }

        [Test]
        public void Indexer_ReadWrite_Works()
        {
            var arr = new NativeArray<int>(4, Allocator.Persistent);
            arr[0] = 10;
            arr[1] = 20;
            arr[2] = 30;
            arr[3] = 40;
            Assert.AreEqual(10, arr[0]);
            Assert.AreEqual(20, arr[1]);
            Assert.AreEqual(30, arr[2]);
            Assert.AreEqual(40, arr[3]);
            arr.Dispose();
        }

        [Test]
        public void Dispose_Idempotent()
        {
            var arr = new NativeArray<int>(8, Allocator.Persistent);
            arr.Dispose();
            arr.Dispose();
        }

        [Test]
        public void Dispose_CanAllocateAfter()
        {
            {
                var arr = new NativeArray<int>(8, Allocator.Persistent);
                arr.Dispose();
            }
            {
                var arr = new NativeArray<double>(4, Allocator.Persistent);
                Assert.IsTrue(arr.IsCreated);
                arr[0] = 1.5;
                Assert.AreEqual(1.5, arr[0]);
                arr.Dispose();
            }
        }
    }

    public sealed unsafe class NativeListTests
    {
        [Test]
        public void Default_Ctor_IsNotCreated()
        {
            var list = default(NativeList<int>);
            Assert.IsFalse(list.IsCreated);
        }

        [Test]
        public void Ctor_InitialCapacity_Works()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            Assert.IsTrue(list.IsCreated);
            Assert.AreEqual(4, list.Capacity);
            Assert.AreEqual(0, list.Length);
            list.Dispose();
        }

        [Test]
        public void Add_AppendsItem()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            list.Dispose();
        }

        [Test]
        public void Add_ResizesWhenNeeded()
        {
            var list = new NativeList<int>(1, Allocator.Persistent);
            for (int i = 0; i < 16; i++)
                list.Add(i);
            Assert.AreEqual(16, list.Length);
            Assert.IsTrue(list.Capacity >= 16);
            for (int i = 0; i < 16; i++)
                Assert.AreEqual(i, list[i]);
            list.Dispose();
        }

        [Test]
        public void Clear_ResetsLength()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Clear();
            Assert.AreEqual(0, list.Length);
            list.Dispose();
        }

        [Test]
        public void RemoveAt_Last_RemovesWithoutShift()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.RemoveAt(2);
            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            list.Dispose();
        }

        [Test]
        public void RemoveAt_Middle_ShiftsElements()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.RemoveAt(1);
            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(3, list[1]);
            list.Dispose();
        }

        [Test]
        public void RemoveRange_RemovesMultiple()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            for (int i = 0; i < 8; i++)
                list.Add(i);
            list.RemoveRange(2, 3);
            Assert.AreEqual(5, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(1, list[1]);
            Assert.AreEqual(5, list[2]);
            Assert.AreEqual(6, list[3]);
            Assert.AreEqual(7, list[4]);
            list.Dispose();
        }

        [Test]
        public void Resize_ClearMemory_ZeroesNew()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Resize(4, NativeArrayOptions.ClearMemory);
            Assert.AreEqual(4, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(0, list[2]);
            Assert.AreEqual(0, list[3]);
            list.Dispose();
        }

        [Test]
        public void ResizeUninitialized_SkipsZeroing()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.ResizeUninitialized(4);
            Assert.AreEqual(4, list.Length);
            list.Dispose();
            GC.KeepAlive(list);
        }

        [Test]
        public void GetUnsafePtr_ReturnsPointer()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(42);
            var ptr = list.GetUnsafePtr();
            Assert.AreEqual(42, *ptr);
            list.Dispose();
        }

        [Test]
        public void Dispose_Idempotent()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Dispose();
            list.Dispose();
        }

        [Test]
        public void Capacity_Set_Reallocates()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Capacity = 32;
            Assert.AreEqual(32, list.Capacity);
            Assert.AreEqual(3, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
            list.Dispose();
        }

        [Test]
        public void Length_Set_Expands()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Length = 8;
            Assert.AreEqual(8, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            list.Dispose();
        }

        [Test]
        public void Length_Set_Shrinks()
        {
            var list = new NativeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Add(3);
            list.Length = 1;
            Assert.AreEqual(1, list.Length);
            Assert.AreEqual(1, list[0]);
            list.Dispose();
        }
    }

    public sealed unsafe class UnsafeListTests
    {
        [Test]
        public void Default_IsNotCreated()
        {
            var list = default(UnsafeList<int>);
            Assert.IsFalse(list.IsCreated);
        }

        [Test]
        public void Ctor_Works()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            Assert.IsTrue(list.IsCreated);
            Assert.AreEqual(4, list.Capacity);
            Assert.AreEqual(0, list.Length);
            list.Dispose();
        }

        [Test]
        public void Add_AppendsItem()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            Assert.AreEqual(2, list.Length);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            list.Dispose();
        }

        [Test]
        public void AddRange_CopiesItems()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            int* src = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            for (int i = 0; i < 4; i++)
                src[i] = i * 10;

            list.AddRange(src, 4);

            Assert.AreEqual(4, list.Length);
            Assert.AreEqual(0, list[0]);
            Assert.AreEqual(10, list[1]);
            Assert.AreEqual(20, list[2]);
            Assert.AreEqual(30, list[3]);
            Marshal.FreeHGlobal((nint)src);
            list.Dispose();
        }

        [Test]
        public void Clear_ResetsLength()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.Add(1);
            list.Add(2);
            list.Clear();
            Assert.AreEqual(0, list.Length);
            list.Dispose();
        }

        [Test]
        public void Dispose_Idempotent()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.Dispose();
            list.Dispose();
        }

        [Test]
        public void Ptr_ReturnsValidPointer()
        {
            var list = new UnsafeList<int>(4, Allocator.Persistent);
            list.Add(99);
            Assert.IsTrue(list.Ptr != null);
            Assert.AreEqual(99, *list.Ptr);
            list.Dispose();
        }
    }

    public sealed unsafe class AllocatorManagerTests
    {
        [Test]
        public void AllocatorHandle_ImplicitConversion()
        {
            var handle = (AllocatorManager.AllocatorHandle)Allocator.Persistent;
            Assert.AreEqual((int)Allocator.Persistent, handle.Value);
        }

        [Test]
        public void AllocateWithHandle_Allocates()
        {
            var handle = (AllocatorManager.AllocatorHandle)Allocator.Persistent;
            var ptr = AllocatorManager.Allocate(handle, 64, 8);
            Assert.IsTrue(ptr != null);
            AllocatorManager.Free(handle, ptr);
        }

        [Test]
        public void AllocateResizeHint_NoOp()
        {
            var handle = (AllocatorManager.AllocatorHandle)Allocator.Persistent;
            IntPtr mem = Marshal.AllocHGlobal(128);
            AllocatorManager.Allocate(handle, (void*)mem, 128, 8);
            Marshal.FreeHGlobal(mem);
        }
    }

    public sealed unsafe class UnsafeUtilityTests
    {
        [Test]
        public void AddressOf_ReturnsPointer()
        {
            int value = 42;
            var ptr = UnsafeUtility.AddressOf(ref value);
            Assert.AreEqual((nint)Unsafe.AsPointer(ref value), (nint)ptr);
        }

        [Test]
        public void As_ReinterpretsRef()
        {
            int intVal = 0x12345678;
            ref byte bRef = ref UnsafeUtility.As<int, byte>(ref intVal);
            byte* bPtr = (byte*)UnsafeUtility.AddressOf(ref bRef);
            Assert.AreEqual(0x78, *bPtr);
        }

        [Test]
        public void MemClear_ZerosMemory()
        {
            int* ptr = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            ptr[0] = 1; ptr[1] = 2; ptr[2] = 3; ptr[3] = 4;
            UnsafeUtility.MemClear(ptr, 4 * sizeof(int));
            for (int i = 0; i < 4; i++)
                Assert.AreEqual(0, ptr[i]);
            Marshal.FreeHGlobal((nint)ptr);
        }

        [Test]
        public void MemSet_FillsMemory()
        {
            byte* ptr = (byte*)Marshal.AllocHGlobal(16);
            UnsafeUtility.MemSet(ptr, 0xAB, 16);
            for (int i = 0; i < 16; i++)
                Assert.AreEqual(0xAB, ptr[i]);
            Marshal.FreeHGlobal((nint)ptr);
        }

        [Test]
        public void MemCpy_CopiesMemory()
        {
            int* src = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            int* dst = (int*)Marshal.AllocHGlobal(4 * sizeof(int));
            for (int i = 0; i < 4; i++)
                src[i] = i * 7;
            UnsafeUtility.MemCpy(dst, src, 4 * sizeof(int));
            for (int i = 0; i < 4; i++)
                Assert.AreEqual(src[i], dst[i]);
            Marshal.FreeHGlobal((nint)src);
            Marshal.FreeHGlobal((nint)dst);
        }
    }
}