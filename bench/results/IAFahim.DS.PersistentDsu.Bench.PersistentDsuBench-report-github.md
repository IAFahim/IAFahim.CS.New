```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method             | N    | Mean     | Error | Ratio | RatioSD | Allocated | Alloc Ratio |
|------------------- |----- |---------:|------:|------:|--------:|----------:|------------:|
| **PersistentDsuUnion** | **64**   |       **NA** |    **NA** |     **?** |       **?** |        **NA** |           **?** |
| PersistentDsuFind  | 64   | 517.0 μs |    NA |     ? |       ? |         - |           ? |
|                    |      |          |       |       |         |           |             |
| **PersistentDsuUnion** | **256**  |       **NA** |    **NA** |     **?** |       **?** |        **NA** |           **?** |
| PersistentDsuFind  | 256  | 554.2 μs |    NA |     ? |       ? |         - |           ? |
|                    |      |          |       |       |         |           |             |
| **PersistentDsuUnion** | **1024** |       **NA** |    **NA** |     **?** |       **?** |        **NA** |           **?** |
| PersistentDsuFind  | 1024 | 801.9 μs |    NA |     ? |       ? |         - |           ? |

Benchmarks with issues:
  PersistentDsuBench.PersistentDsuUnion: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=64]
  PersistentDsuBench.PersistentDsuUnion: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=256]
  PersistentDsuBench.PersistentDsuUnion: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=1024]
