```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method         | N    | Mean     | Error | Allocated |
|--------------- |----- |---------:|------:|----------:|
| **BuildPrefix**    | **1024** | **543.0 μs** |    **NA** |         **-** |
| RangeSum_Bench | 1024 | 666.4 μs |    NA |         - |
| RangeAdd_Bench | 1024 | 575.7 μs |    NA |         - |
| **BuildPrefix**    | **4096** | **531.0 μs** |    **NA** |         **-** |
| RangeSum_Bench | 4096 | 665.4 μs |    NA |         - |
| RangeAdd_Bench | 4096 | 573.1 μs |    NA |         - |
