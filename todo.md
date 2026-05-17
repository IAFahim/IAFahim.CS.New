You have a great start! By utilizing UnityMathematics.NoDeps, you successfully
eliminate the heavy Unity.Mathematics package dependency. However,
BovineLabs.Recast still heavily relies on Unity's proprietary memory and
collections ecosystem (Unity.Collections).

While you've successfully created the foundation with Allocator,
AllocatorManager, and UnsafeUtility, there are several missing structures,
methods, and API signatures that the Recast port uses.

Here is exactly what you are missing:

1.  AllocatorManager.AllocatorHandle: The codebase passes
    AllocatorManager.AllocatorHandle instead of the raw Allocator enum in
    several places.
2.  Missing UnsafeUtility methods: The codebase uses UnsafeUtility.AddressOf()
    and UnsafeUtility.As<T, U>().
3.  NativeArrayOptions Enum: Used for specifying memory clearing options during
    resizing.
4.  NativeArray<T>: You made UnsafeArray<T>, but BovineLabs.Recast explicitly
    uses NativeArray<T>.
5.  NativeList<T>: Heavily used for contours and pathfinding stacks. It requires
    dynamically resizable logic.
6.  UnsafeList<T>: Used in DtNavMeshQuery.cs for internal path buffers.
7.  Leftover Unity References: UnityEngine.Debug.LogError is used in Recast.cs,
    and Unity.Entities is used in BovineLabs.Recast.Tests/AssemblyInfo.cs.

Here is a comprehensive, highly-detailed todo.md you can feed to a smaller AI to
perfectly generate the missing C# code.

TODO.md

# NoDeps Unity.Collections Implementation Tasks

The goal is to implement missing structs and methods to mimic `Unity.Collections` so that a Unity-independent port of a library compiles successfully in pure .NET. 

Please implement the following tasks strictly using `System.Runtime.InteropServices.Marshal` and `System.Runtime.CompilerServices.Unsafe` where appropriate.

## Task 1: Update `AllocatorManager` with `AllocatorHandle`
Create a struct `AllocatorHandle` inside `Unity.Collections.AllocatorManager` to allow implicit conversions from the `Allocator` enum, and update the existing methods to use it.

**Requirements:**
- Implement `public struct AllocatorHandle` inside `AllocatorManager`.
- Include `public int Value;` field.
- Add implicit operator from `Allocator` to `AllocatorHandle`.
- Update `AllocatorManager.Allocate` and `AllocatorManager.Free` to accept `AllocatorHandle allocator` instead of `Allocator allocator`.

## Task 2: Implement Missing `UnsafeUtility` Methods
Extend the `Unity.Collections.LowLevel.Unsafe.UnsafeUtility` class with memory manipulation and casting tools used by the library.

**Requirements:**
- Add `public static unsafe void* AddressOf<T>(ref T output) where T : unmanaged`. (Hint: Use `fixed` or `System.Runtime.CompilerServices.Unsafe.AsPointer(ref output)`).
- Add `public static ref U As<T, U>(ref T source)`. (Hint: Use `return ref System.Runtime.CompilerServices.Unsafe.As<T, U>(ref source);`).

## Task 3: Create `NativeArrayOptions` Enum
Create the enum used for specifying initialization behavior on allocations.

**Requirements:**
- Namespace: `Unity.Collections`
- Name: `NativeArrayOptions`
- Values: `UninitializedMemory = 0`, `ClearMemory = 1`

## Task 4: Implement `NativeArray<T>`
Implement a basic unmanaged struct that holds a pointer and length, matching `Unity.Collections.NativeArray<T>`.

**Requirements:**
- Namespace: `Unity.Collections`
- Signature: `public unsafe struct NativeArray<T> : IDisposable where T : unmanaged`
- Include a constructor: `public NativeArray(int length, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)`
- Include properties: `public int Length { get; }`, `public bool IsCreated { get; }`
- Include `public void Dispose()` (Frees memory via `AllocatorManager.Free`).
- Include a 1D indexer: `public T this[int index] { get; set; }`

## Task 5: Implement `NativeList<T>`
Implement a resizable unmanaged list struct matching `Unity.Collections.NativeList<T>`. 

**Requirements:**
- Namespace: `Unity.Collections`
- Signature: `public unsafe struct NativeList<T> : IDisposable where T : unmanaged`
- Fields: A pointer to the buffer, an `int Length`, an `int Capacity`, and an `Allocator Allocator`.
- Constructor: `public NativeList(int initialCapacity, Allocator allocator)`
- Properties: `public int Length { get; set; }`, `public int Capacity { get; set; }`, `public bool IsCreated { get; }`
- Indexer: `public T this[int index] { get; set; }`
- Methods to implement:
  - `public void Dispose()`
  - `public void Clear()` (sets Length to 0)
  - `public void Add(in T item)` (resizes capacity if necessary using `Marshal.ReAllocHGlobal` or allocating new/copying/freeing, then appends item)
  - `public void RemoveAt(int index)` (shifts elements left to fill the gap)
  - `public void RemoveRange(int index, int count)` (shifts elements left)
  - `public void Resize(int length, NativeArrayOptions options)` (resizes capacity if needed, sets `Length = length`, and if `options == NativeArrayOptions.ClearMemory`, zeroes out the new memory area).
  - `public void ResizeUninitialized(int length)` (same as Resize but never clears memory).
  - `public T* GetUnsafePtr()` (returns the raw pointer buffer).

## Task 6: Implement `UnsafeList<T>`
Implement `Unity.Collections.LowLevel.Unsafe.UnsafeList<T>`. It functions identically to `NativeList<T>` but resides in the Unsafe namespace and handles pointers slightly differently in Unity (though for our NoDeps version, it can practically mirror `NativeList<T>`).

**Requirements:**
- Namespace: `Unity.Collections.LowLevel.Unsafe`
- Signature: `public unsafe struct UnsafeList<T> : IDisposable where T : unmanaged`
- Functionality requirements: Exactly the same as `NativeList<T>`, but ensure you add:
  - `public void AddRange(T* ptr, int count)` (Copies `count` items from `ptr` into the list, resizing if necessary).

## Task 7: Patch Out Unity Engine and Entities References
Find and replace residual Unity macros and imports in the project files.

**Requirements:**
- In `BovineLabs.Recast.Tests/AssemblyInfo.cs`: Remove `using Unity.Entities;` and the `[assembly: DisableAutoCreation]` attribute.
- In `BovineLabs.Recast/Recast/Recast.cs` (approx Line 181): Find the `UnityEngine.Debug.LogError` call and replace it with `System.Console.WriteLine` or `System.Diagnostics.Debug.WriteLine`. Remove any `using UnityEngine;` directive at the top of that file.

Explanation of the Strategy:

By handing the smaller AI this markdown file, it knows exactly what signatures
are requested, what namespaces they belong to, what internal libraries to use
(System.Runtime.CompilerServices.Unsafe), and how the memory lifecycle should
look.

Once these 7 tasks are generated and added to your IAFahim.Collections.NoDeps
project, BovineLabs.Recast will compile natively under .NET Standard 2.1 or
.NET 6/7/8 with absolutely zero ties to the Unity Editor or Unity Engine
assemblies!
