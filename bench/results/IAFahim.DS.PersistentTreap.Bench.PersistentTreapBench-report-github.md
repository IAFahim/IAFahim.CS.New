```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method                | N    | Mean     | Error | Ratio | RatioSD | Allocated | Alloc Ratio |
|---------------------- |----- |---------:|------:|------:|--------:|----------:|------------:|
| **PersistentTreapInsert** | **128**  |       **NA** |    **NA** |     **?** |       **?** |        **NA** |           **?** |
| PersistentTreapFind   | 128  | 586.7 μs |    NA |     ? |       ? |         - |           ? |
|                       |      |          |       |       |         |           |             |
| **PersistentTreapInsert** | **512**  |       **NA** |    **NA** |     **?** |       **?** |        **NA** |           **?** |
| PersistentTreapFind   | 512  | 620.6 μs |    NA |     ? |       ? |         - |           ? |
|                       |      |          |       |       |         |           |             |
| **PersistentTreapInsert** | **2048** |       **NA** |    **NA** |     **?** |       **?** |        **NA** |           **?** |
| PersistentTreapFind   | 2048 | 778.9 μs |    NA |     ? |       ? |         - |           ? |

Benchmarks with issues:
  PersistentTreapBench.PersistentTreapInsert: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=128]
  PersistentTreapBench.PersistentTreapInsert: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=512]
  PersistentTreapBench.PersistentTreapInsert: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=2048]
