# AGENTS.md — IAFahim.CS

Workflow: **Create → Test → Benchmark**. Every algorithm passes all three.

---

## What This Is

Unmanaged algorithm and data structure library. Runs on .NET. Runs on Unity. Zero code changes between them.

**.NET path**: `IAFahim.Collections.NoDeps` provides stubs (Allocator, AllocatorManager, UnsafeUtility, NativeContainer, NativeDisableUnsafePtrRestriction). `UnityMathematics.NoDeps` provides math types (float3, int3, math.*). Code compiles against these.

**Unity path**: Unity links its own `com.unity.collections` and `com.unity.mathematics`. NoDeps assemblies excluded via `PrivateAssets="All"`. Stubs vanish. Real implementations bind. Same source, different linker target.

---

## Two Package Kinds

**Algorithm** — pure `T* ptr, int len`. Zero dependencies. No allocator, no container, no Unity namespace. Callable from anything that exposes a pointer.

**Data Structure** — depends on `IAFahim.Collections.NoDeps`. Uses Allocator, AllocatorManager, UnsafeUtility. Owns memory. Implements IDisposable.

One algorithm or data structure = one NuGet package. Independently consumable. Deletion shrinks the repo, never breaks it.

---

## Dependency Graph

```
IAFahim.Collections.NoDeps         ← zero deps (stubs only)
UnityMathematics.NoDeps            ← zero deps (math types only)

IAFahim.Math.*                     ← UnityMathematics.NoDeps
IAFahim.DS.*                       ← IAFahim.Collections.NoDeps
IAFahim.Sort.*                     ← zero deps (algorithms)
IAFahim.Search.*                   ← zero deps (algorithms)
IAFahim.Graph.*                    ← zero deps (algorithms)
IAFahim.String.*                   ← zero deps (algorithms)
IAFahim.IO.*                       ← zero deps (algorithms)
```

If a package needs a dependency not listed here, that dependency must be declared before the package is created. No implicit edges.

---

## Type Constraints

| Allowed                                  | Forbidden                                |
|------------------------------------------|------------------------------------------|
| `static class`                           | `class`, `sealed class`, `abstract class`|
| `struct` (unmanaged fields only)         | `interface`                              |
| `where T : unmanaged`                    | `string`, `object`, `dynamic`            |
| `void*`, `T*`, `nint`, `nuint`           | managed arrays `T[]`                     |
| `bool`, value types                      | `List<T>`, `Dictionary<K,V>`             |
| `stackalloc`                             | `new` on any class                       |
| `ReadOnlySpan<byte>` (parameter only)    | `string` literals without `u8`           |
| `Span<T>` (test and bench only)          | `Span<T>` in `src/`                      |
| `sizeof(T)`                              | `Marshal.SizeOf<T>()`                    |
| `"text"u8` (UTF-8 literals)             | managed exception handling               |

**`sizeof(T)` vs `UnsafeUtility.SizeOf<T>()`**: algorithms use `sizeof(T)` (zero deps). Data structures use `UnsafeUtility.SizeOf<T>()` and `UnsafeUtility.AlignOf<T>()` (already depend on Collections.NoDeps).

If it touches the GC heap, it does not belong in `src/`.

---

## Code Style

- No comments. Names carry all meaning.
- No `var`. Explicit types everywhere.
- No structs wrapping single primitives.
- No implicit conversions. Cast explicitly.
- No magic numbers. Named constants only.
- No auto-properties on structs with `[StructLayout]`. Explicit readonly fields only.
- Data separated from behavior. Structs = fields + Dispose. Static classes = methods.
- `[MethodImpl(MethodImplOptions.AggressiveInlining)]` on hot-path leaf functions.

---

## Totality

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

**Bounds check pattern:**
```csharp
(uint)index < (uint)length
```

---

## Allocation Size Safety

Never multiply `int * int` for byte sizes. Widen first:

```csharp
long byteCount = (long)length * sizeof(T);
```

Applies everywhere: constructors, MemClear calls, MemCopy calls.

---

## Phase 1: Create

1. Pick package kind: algorithm (zero deps) or data structure (Collections.NoDeps).
2. Create `src/IAFahim.{Family}.{Name}/` with minimal csproj.
3. One `static class` per algorithm. File = class name.
4. Algorithms take `T* ptr, int len`. Data structures expose `T* Ptr, int Length`.
5. Unchecked variant first. `Try*` if caller cannot guarantee preconditions.
6. No allocation unless algorithm fundamentally requires scratch space (`stackalloc`).

### Algorithm Template

```csharp
namespace IAFahim.Sort
{
    using System;
    using System.Runtime.CompilerServices;

    public static unsafe class Insertion
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Run<T>(T* ptr, int len)
            where T : unmanaged, IComparable<T>
        {
            for (int i = 1; i < len; i++)
            {
                T key = ptr[i];
                int j = i - 1;
                while (j >= 0 && ptr[j].CompareTo(key) > 0)
                {
                    ptr[j + 1] = ptr[j];
                    j--;
                }
                ptr[j + 1] = key;
            }
        }
    }
}
```

### Data Structure Template

```csharp
namespace IAFahim.DS
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;

    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    public unsafe struct UnsafeArray<T> : IDisposable where T : unmanaged
    {
        [NativeDisableUnsafePtrRestriction]
        public T* Ptr;

        public readonly int Length;

        public readonly Allocator Allocator;

        public UnsafeArray(int length, Allocator allocator)
        {
            long byteCount = (long)length * UnsafeUtility.SizeOf<T>();
            Ptr = (T*)AllocatorManager.Allocate(
                allocator,
                byteCount,
                UnsafeUtility.AlignOf<T>());
            UnsafeUtility.MemClear(Ptr, byteCount);
            Length = length;
            Allocator = allocator;
        }

        public void Dispose()
        {
            if (Ptr != null)
            {
                AllocatorManager.Free(Allocator, Ptr);
            }
            this = default;
        }
    }
}
```

---

## Phase 2: Test

Framework: xUnit. Projects in `test/`. Target `net8.0`.

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

    public sealed unsafe class InsertionTests
    {
        [Fact]
        public void EmptyInput_NoOp()
        {
            Insertion.Run<int>(null, 0);
        }

        [Fact]
        public void Reversed_Sorts()
        {
            const int N = 64;
            int* ptr = (int*)Marshal.AllocHGlobal(N * sizeof(int));
            try
            {
                for (int i = 0; i < N; i++)
                    ptr[i] = N - i;

                Insertion.Run(ptr, N);

                for (int i = 0; i < N; i++)
                    Assert.Equal(i + 1, ptr[i]);
            }
            finally
            {
                Marshal.FreeHGlobal((nint)ptr);
            }
        }
    }
}
```

### Rules
- No mocking. No DI. Raw pointers, allocate, run, assert, free.
- Every test wraps allocation in `try/finally`. No leaks on assertion failure.
- Test names: `Condition_ExpectedResult`.

---

## Phase 3: Benchmark

Framework: BenchmarkDotNet. Projects in `bench/`. Target `net8.0`.

### Checklist
1. Baseline comparison — `Span<T>.Sort()` or BCL equivalent.
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
            BenchmarkRunner.Run<InsertionBench>(args: args);
        }
    }

    [MemoryDiagnoser]
    public unsafe class InsertionBench
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
        public void Insertion()
        {
            Sort.Insertion.Run(_work, N);
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            Marshal.FreeHGlobal((nint)_source);
            Marshal.FreeHGlobal((nint)_work);
        }
    }
}
```

---

## Project Layout

```
IAFahim.CS/
├── src/
│   ├── Directory.Build.props              ← netstandard2.1, packable
│   ├── IAFahim.Collections.NoDeps/        ← stubs: Allocator, AllocatorManager,
│   │                                         UnsafeUtility, NativeContainer,
│   │                                         NativeDisableUnsafePtrRestriction
│   ├── IAFahim.DS.UnsafeArray/            ← data structure
│   ├── IAFahim.Sort.Insertion/            ← algorithm
│   └── ...
├── test/
│   ├── Directory.Build.props              ← net8.0, xUnit
│   └── ...
├── bench/
│   ├── Directory.Build.props              ← net8.0, BenchmarkDotNet
│   └── ...
├── .github/
│   └── workflows/
│       └── publish-nuget.yml
├── Directory.Build.props                  ← shared: unsafe, LangVersion 12
├── IAFahim.CS.sln
├── TODO.md
└── AGENTS.md
```

---

## Unity Integration

1. `dotnet pack` each `src/` project → NuGet packages.
2. Unity project installs via NuGetForUnity.
3. `IAFahim.Collections.NoDeps` excluded (`PrivateAssets="All"`) — Unity provides the real assemblies.
4. `UnityMathematics.NoDeps` excluded — same reason.
5. Algorithm DLLs contain only algorithm code. Zero Unity deps in the binary.

Burst-compatible by construction: all types unmanaged, all methods static, no virtual dispatch, no managed types in call chain, no GC allocation.

---

## Conversation Style

Terse. All technical substance stays. Only fluff dies. Fragments OK.

Pattern: `[thing] [action] [reason]. [next step].`

Exceptions: security warnings, irreversible ops, confusion → full clarity.
