```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method          | N   | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|---------------- |---- |---------:|------:|------:|----------:|------------:|
| **RunDinicMaxFlow** | **100** | **1.203 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
|                 |     |          |       |       |           |             |
| **RunDinicMaxFlow** | **500** | **1.189 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
