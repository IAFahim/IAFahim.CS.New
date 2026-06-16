```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method           | N    | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|----------------- |----- |---------:|------:|------:|----------:|------------:|
| **PieceTableInsert** | **1024** | **592.8 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| PieceTableDelete | 1024 | 628.8 μs |    NA |  1.06 |         - |          NA |
|                  |      |          |       |       |           |             |
| **PieceTableInsert** | **4096** | **610.9 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| PieceTableDelete | 4096 | 607.0 μs |    NA |  0.99 |         - |          NA |
