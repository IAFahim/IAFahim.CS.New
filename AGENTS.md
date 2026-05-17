# AGENTS.md — IAFahim.CS Unmanaged Algorithm Library

Workflow: **Create → Test → Benchmark**. Every algorithm passes all three.

---

## Architecture

### The Unity Linking Trick

Code uses `Unity.Collections` and `Unity.Mathematics` namespaces directly.

In **.NET**: `IAFahim.Collections.NoDeps` provides stubs (Allocator, AllocatorManager, UnsafeUtility). `UnityMathematics.NoDeps` provides math types (float3, int3, math.*).

In **Unity**: Unity links its own `com.unity.collections` and `com.unity.mathematics`. NoDeps assemblies excluded via `PrivateAssets="All"`. Zero code changes between environments.

### Two Package Kinds

**Data structures** — use `Unity.Collections` (Allocator, AllocatorManager, UnsafeUtility). Follow the BovineLabs pattern: `StructLayout(Sequential)`, `IDisposable`, `Allocator` property, pointer fields, `MemClear` on init.

**Algorithms** — pure `T* ptr, int len`. Zero dependencies on Unity or data structures. Callable from any container that exposes a pointer.

### Dependency Rules

```
IAFahim.Collections.NoDeps      ← zero deps. Stubs only.
IAFahim.DS.*                    ← Collections.NoDeps, UnityMathematics.NoDeps
IAFahim.Sort.*                  ← zero deps (pure T* + len)
IAFahim.Search.*                ← zero deps (pure T* + len)
IAFahim.Graph.*                 ← Collections.NoDeps (needs allocation for scratch)
IAFahim.Math.*                  ← UnityMathematics.NoDeps
IAFahim.String.*                ← zero deps (pure byte* + len)
IAFahim.IO.*                    ← zero deps or String
```

Algorithms never depend on data structures. Connection happens at call site:
```csharp
// .NET
UnsafeArray<int> arr = new UnsafeArray<int>(1024, Allocator.Persistent);
Insertion.Run(arr.Ptr, arr.Length);
arr.Dispose();

// Unity
NativeArray<int> arr = new NativeArray<int>(1024, Allocator.Persistent);
Insertion.Run((int*)arr.GetUnsafePtr(), arr.Length);
arr.Dispose();
```

### Package Granularity

One algorithm = one NuGet package. 200+ packages. Each independently consumable.

```
src/
├── IAFahim.Collections.NoDeps/
├── IAFahim.DS.UnsafeArray/
├── IAFahim.DS.UnsafeList/
├── IAFahim.DS.UnsafeHashMap/
├── IAFahim.Sort.Insertion/
├── IAFahim.Sort.Quick/
├── IAFahim.Sort.Merge/
├── IAFahim.Sort.Radix/
├── IAFahim.Search.Binary/
├── IAFahim.Search.Linear/
├── IAFahim.Graph.BFS/
├── IAFahim.Graph.Dijkstra/
└── ...
```

Each package csproj is minimal — `Directory.Build.props` handles shared config. Most algorithm csprojs are empty `<Project Sdk="Microsoft.NET.Sdk"></Project>`.

---

## Type Constraints

| Allowed                                  | Forbidden                                |
|------------------------------------------|------------------------------------------|
| `static class`                           | `class`, `sealed class`, `abstract class`|
| `struct` (unmanaged fields only)         | `interface`                              |
| `where T : unmanaged`                    | `string`, `object`, `dynamic`            |
| `void*`, `T*`, `nuint`, `nint`           | managed arrays `T[]`                     |
| `bool`, value types                      | `List<T>`, `Dictionary<K,V>`             |
| `stackalloc`                             | `new` on any class                       |
| `ReadOnlySpan<byte>` (parameter only)    | Any GC-allocated type                    |
| `"text"u8` (UTF-8 literals)             | `string` literals without `u8`           |
| `sizeof(T)`                              | `Marshal.SizeOf<T>()`                    |

If it touches the GC heap, it does not belong here.

---

## Code Style

- No comments. Names carry all meaning.
- No `var`. Explicit types everywhere.
- No structs wrapping single primitives.
- No implicit conversions. Cast explicitly.
- No magic numbers. Named constants only.
- Data separated from behavior. Structs = fields + Dispose. Static classes = methods.
- Deletion shrinks, never breaks. Every package independently removable.
- `[MethodImpl(MethodImplOptions.AggressiveInlining)]` on hot-path leaf functions.

### Totality

**Unchecked (caller guarantees validity):**
```csharp
public static void Run<T>(T* ptr, int len)
    where T : unmanaged, IComparable<T>
```

**Checked (`Try*` prefix, returns `bool`, `out` result):**
```csharp
public static bool TryFind<T>(T* ptr, int len, T key, out int index)
    where T : unmanaged, IComparable<T>
```

### Bounds Check Pattern
```csharp
(uint)index < (uint)length
```

### Data Structure Template (BovineLabs Pattern)

```csharp
namespace IAFahim.DS
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    public unsafe struct UnsafeThing<T> : IDisposable where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction]
        public T* Ptr;

        public int Length;

        public UnsafeThing(int length, Allocator allocator)
        {
            this = default;
            this.Length = length;
            this.Allocator = allocator;
            this.Ptr = (T*)AllocatorManager.Allocate(
                allocator,
                length * UnsafeUtility.SizeOf<T>(),
                UnsafeUtility.AlignOf<T>());
            UnsafeUtility.MemClear(this.Ptr, length * UnsafeUtility.SizeOf<T>());
        }

        public Allocator Allocator { get; }

        public void Dispose()
        {
            if (this.Ptr != null)
            {
                AllocatorManager.Free(this.Allocator, this.Ptr);
                this.Ptr = null;
            }
            this = default;
        }
    }
}
```

### Algorithm Template

```csharp
namespace IAFahim.Sort
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class QuickSort
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int len)
            where T : unmanaged, IComparable<T>
        {
            // algorithm on raw pointers, zero allocation
        }
    }
}
```

---

## Phase 1: Create

### Checklist
1. Pick package kind: algorithm (zero deps) or data structure (Collections.NoDeps).
2. Create `src/IAFahim.{Family}.{Name}/` with minimal csproj.
3. One `static class` per algorithm. File = class name.
4. Algorithms take `T* ptr, int len`. Data structures expose `T* Ptr, int Length`.
5. Unchecked variant first. `Try*` if caller can't guarantee preconditions.
6. No allocation unless algorithm fundamentally requires scratch space.

---

## Phase 2: Test

Framework: xUnit. Test projects in `test/`. Target `net8.0`.

### Checklist Per Public Method
1. Empty input — `len == 0` does not crash.
2. Single element — trivial case correct.
3. Already sorted / already reversed.
4. Duplicates — all identical elements.
5. Large N — 1K+ elements, verify correctness.
6. `Try*` false path — invalid input returns `false`, out param is default.
7. `Try*` true path — valid input returns `true`, out param correct.

### Test Template
```csharp
namespace IAFahim.Sort.Tests
{
    using System.Runtime.InteropServices;
    using Xunit;

    public sealed unsafe class QuickSortTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            QuickSort.Run<int>(null, 0);
        }

        [Fact]
        public void Reversed_Sorts()
        {
            const int N = 64;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));

            for (int i = 0; i < N; i++)
                ptr[i] = N - i;

            QuickSort.Run(ptr, N);

            for (int i = 0; i < N; i++)
                Assert.Equal(i + 1, ptr[i]);

            Marshal.FreeHGlobal((System.IntPtr)ptr);
        }
    }
}
```

### Rules
- No mocking. No DI. Raw pointers, allocate, run, assert, free.
- Every test allocates and frees its own memory.
- Test names: `Condition_ExpectedResult`.

---

## Phase 3: Benchmark

Framework: BenchmarkDotNet. Bench projects in `bench/`. Target `net8.0`.

### Checklist
1. Baseline comparison — compare against BCL or known fast impl.
2. Multiple sizes — `[Params(64, 256, 1024, 4096)]`.
3. `[MemoryDiagnoser]` confirms zero managed allocation.
4. `[IterationSetup]` resets mutated data.
5. Same seed (42) for reproducible distributions.

### Bench Template
```csharp
namespace IAFahim.Sort.Bench
{
    using System;
    using System.Runtime.InteropServices;
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Running;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<QuickSortBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class QuickSortBench
    {
        [Params(64, 256, 1024, 4096)]
        public int N;

        private int* _source;
        private int* _work;

        [GlobalSetup]
        public void Setup()
        {
            _source = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            _work = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            Random rng = new Random(42);
            for (int i = 0; i < N; i++)
                _source[i] = rng.Next();
        }

        [IterationSetup]
        public void CopySource()
        {
            Buffer.MemoryCopy(_source, _work, N * sizeof(int), N * sizeof(int));
        }

        [Benchmark(Baseline = true)]
        public void SpanSort()
        {
            new Span<int>(_work, N).Sort();
        }

        [Benchmark]
        public void QuickSort()
        {
            Sort.QuickSort.Run(_work, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((IntPtr)_source);
            Marshal.FreeHGlobal((IntPtr)_work);
        }
    }
}
```

---

## Project Layout

```
IAFahim.CS/
├── src/
│   ├── Directory.Build.props          ← netstandard2.1, packable
│   ├── IAFahim.Collections.NoDeps/    ← Unity.Collections stubs
│   ├── IAFahim.DS.UnsafeArray/        ← data structure
│   ├── IAFahim.Sort.Insertion/        ← algorithm
│   └── ... (200+ packages)
├── test/
│   ├── Directory.Build.props          ← net8.0, xUnit
│   ├── IAFahim.DS.UnsafeArray.Tests/
│   ├── IAFahim.Sort.Insertion.Tests/
│   └── ...
├── bench/
│   ├── Directory.Build.props          ← net8.0, BenchmarkDotNet
│   ├── IAFahim.Sort.Insertion.Bench/
│   └── ...
├── TODO/
│   └── Roadmap.md
├── Directory.Build.props              ← shared: unsafe, LangVersion 12
├── IAFahim.CS.sln
└── AGENTS.md
```

---

## Unity Integration

1. `dotnet pack` each `src/` project → NuGet packages.
2. Unity project installs via NuGetForUnity.
3. `IAFahim.Collections.NoDeps` excluded (`PrivateAssets="All"`) — Unity provides `Unity.Collections`.
4. `UnityMathematics.NoDeps` excluded — Unity provides `Unity.Mathematics`.
5. Algorithm DLLs contain only the algorithm code. Zero Unity deps in the binary.

Burst compilation works because:
- All types are `unmanaged` by construction.
- All methods are `static` on `static` classes.
- No virtual dispatch. No managed types in call chain.
- No GC allocation.

```csharp
[BurstCompile]
struct SortJob : IJob
{
    [NativeDisableUnsafePtrRestriction]
    public int* Ptr;
    public int Len;

    public void Execute()
    {
        Insertion.Run(Ptr, Len);
    }
}
```

---

## Things That Are Always Wrong

```csharp
new List<int>()              // managed heap
string s = "YES"             // managed string — use "YES"u8
class Foo { }                // regular class
interface IFoo { }           // interface
catch (Exception e)          // managed exception handling
Marshal.SizeOf<T>()          // use sizeof(T)
Console.WriteLine(...)       // managed I/O
int[] arr = new int[n]       // managed array
var x = ...                  // implicit typing
```

---

## Conversation Style

Terse. All technical substance stays. Only fluff dies.

Drop: articles, filler, pleasantries, hedging. Fragments OK.
Pattern: `[thing] [action] [reason]. [next step].`

Exceptions: security warnings, irreversible ops, confusion → full clarity.
