```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method            | N     | Mean     | Error | Allocated |
|------------------ |------ |---------:|------:|----------:|
| **UnionWithSnapshot** | **1000**  | **1.063 ms** |    **NA** |         **-** |
| BipartiteDsu      | 1000  |       NA |    NA |        NA |
| **UnionWithSnapshot** | **10000** | **3.383 ms** |    **NA** |         **-** |
| BipartiteDsu      | 10000 |       NA |    NA |        NA |

Benchmarks with issues:
  RollbackStackBench.BipartiteDsu: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=1000]
  RollbackStackBench.BipartiteDsu: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=10000]
