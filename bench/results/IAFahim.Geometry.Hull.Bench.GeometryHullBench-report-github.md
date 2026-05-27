```

BenchmarkDotNet v0.14.0, Ubuntu 25.10 (Questing Quokka)
Intel Core i5-4200U CPU 1.60GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.108
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method             | N   | Mean       | Error | Ratio | Allocated | Alloc Ratio |
|------------------- |---- |-----------:|------:|------:|----------:|------------:|
| **ConvexHullTrickAdd** | **64**  | **1,474.2 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| ConvexHull3D_Basic | 64  | 3,216.0 μs |    NA |  2.18 |         - |          NA |
|                    |     |            |       |       |           |             |
| **ConvexHullTrickAdd** | **256** |   **971.6 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| ConvexHull3D_Basic | 256 | 4,799.5 μs |    NA |  4.94 |         - |          NA |
