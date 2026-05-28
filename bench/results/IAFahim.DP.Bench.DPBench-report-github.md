```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX2

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method           | N  | Mean     | Error | Ratio | RatioSD | Allocated | Alloc Ratio |
|----------------- |--- |---------:|------:|------:|--------:|----------:|------------:|
| **Baseline**         | **16** |       **NA** |    **NA** |     **?** |       **?** |        **NA** |           **?** |
| SosDp_Bench      | 16 |       NA |    NA |     ? |       ? |        NA |           ? |
| IntervalDp_Bench | 16 | 830.7 μs |    NA |     ? |       ? |         - |           ? |
|                  |    |          |       |       |         |           |             |
| **Baseline**         | **20** | **913.5 μs** |    **NA** |  **1.00** |    **0.00** |         **-** |          **NA** |
| SosDp_Bench      | 20 |       NA |    NA |     ? |       ? |        NA |           ? |
| IntervalDp_Bench | 20 | 702.9 μs |    NA |  0.77 |    0.00 |         - |          NA |

Benchmarks with issues:
  DPBench.Baseline: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=16]
  DPBench.SosDp_Bench: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=16]
  DPBench.SosDp_Bench: Dry(IterationCount=1, LaunchCount=1, RunStrategy=ColdStart, UnrollFactor=1, WarmupCount=1) [N=20]
