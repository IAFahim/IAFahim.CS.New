# IAFahim.Collections.NoDeps

## Description
This package provides minimal stub definitions and compile-time mocks for Unity's collections, job system, and math types. Its primary purpose is to allow unmanaged data structures and algorithms in `IAFahim.CS` to compile under pure .NET environments without requiring Unity dependencies or dragging Unity assemblies into non-Unity builds. In Unity target builds, these stubs are excluded, enabling direct binding to the actual implementation within `com.unity.collections` and `com.unity.mathematics`.

## Complexity
- Stub Allocation/Deallocation: O(1) constant time complexity (delegates directly to `System.Runtime.InteropServices.Marshal`).
- Memory Copy/Clear: O(N) linear time complexity, where N is the number of bytes.
- Attributes and Interfaces: N/A (compile-time metadata only).

## API Signature

### Namespaces and Types

#### `Unity.Collections`
- `public enum Allocator`
  - `Invalid = 0`
  - `None = 1`
  - `Temp = 2`
  - `TempJob = 3`
  - `Persistent = 4`
  - `FirstUserIndex = 64`
- `public static class AllocatorManager`
  - `public struct AllocatorHandle`
  - `public static void* Allocate(AllocatorHandle allocator, long sizeInBytes, int alignInBytes)`
  - `public static void* Allocate(AllocatorHandle allocator, int itemSizeInBytes, int alignmentInBytes, int items)`
  - `public static T* Allocate<T>(AllocatorHandle allocator, int items = 1) where T : unmanaged`
  - `public static void Free(AllocatorHandle allocator, void* pointer)`
- `public sealed class ReadOnlyAttribute : Attribute`
- `public sealed class NativeDisableParallelForRestrictionAttribute : Attribute`

#### `Unity.Collections.LowLevel.Unsafe`
- `public static class UnsafeUtility`
  - `public static int SizeOf<T>() where T : unmanaged`
  - `public static int AlignOf<T>() where T : unmanaged`
  - `public static void MemCpy(void* destination, void* source, long size)`
  - `public static void MemClear(void* destination, long size)`
  - `public static void MemSet(void* destination, byte value, long size)`
  - `public static void* AddressOf<T>(ref T output) where T : unmanaged`
  - `public static ref U As<T, U>(ref T source) where T : unmanaged where U : unmanaged`
- `public sealed class NativeDisableUnsafePtrRestrictionAttribute : Attribute`
- `public sealed class NativeContainerAttribute : Attribute`
- `public sealed class NativeSetThreadIndexAttribute : Attribute`
- `public sealed class NativeContainerIsAtomicWriteOnlyAttribute : Attribute`

#### `Unity.Burst`
- `public sealed class BurstCompileAttribute : Attribute`
- `public sealed class NoAliasAttribute : Attribute`

## Usage Example
Below is an example demonstrating the manual allocation, initialization, and cleanup of an unmanaged array of elements using explicit types and memory management utilities.

```csharp
namespace IAFahim.Collections.Example
{
    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    public static unsafe class UnsafeMemoryHelper
    {
        public static void ProcessData()
        {
            int length = 100;
            long byteCount = (long)length * UnsafeUtility.SizeOf<int>();
            int alignment = UnsafeUtility.AlignOf<int>();
            
            int* ptr = null;
            try
            {
                ptr = (int*)AllocatorManager.Allocate(Allocator.Persistent, byteCount, alignment);
                UnsafeUtility.MemClear(ptr, byteCount);

                for (int i = 0; i < length; i++)
                {
                    UnsafeUtility.WriteArrayElement<int>(ptr, i, i * 10);
                }

                for (int i = 0; i < length; i++)
                {
                    int value = UnsafeUtility.ReadArrayElement<int>(ptr, i);
                    int squared = value * value;
                }
            }
            finally
            {
                if (ptr != null)
                {
                    AllocatorManager.Free(Allocator.Persistent, ptr);
                }
            }
        }
    }
}
```
