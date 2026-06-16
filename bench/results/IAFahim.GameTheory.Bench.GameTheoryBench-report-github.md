```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method    | N    | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|---------- |----- |---------:|------:|------:|----------:|------------:|
| **GrundyDAG** | **256**  | **958.6 μs** |    **NA** |  **1.75** |         **-** |          **NA** |
| NimSum    | 256  | 547.7 μs |    NA |  1.00 |         - |          NA |
|           |      |          |       |       |           |             |
| **GrundyDAG** | **1024** | **981.2 μs** |    **NA** |  **1.77** |         **-** |          **NA** |
| NimSum    | 1024 | 555.3 μs |    NA |  1.00 |         - |          NA |
