```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.301
  [Host] : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.9 (10.0.926.27113), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method   | N    | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|--------- |----- |---------:|------:|------:|----------:|------------:|
| **FindPath** | **64**   | **3.397 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
| Raycast  | 64   | 3.879 ms |    NA |  1.14 |         - |          NA |
|          |      |          |       |       |           |             |
| **FindPath** | **256**  | **3.515 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
| Raycast  | 256  | 4.223 ms |    NA |  1.20 |         - |          NA |
|          |      |          |       |       |           |             |
| **FindPath** | **1024** | **3.909 ms** |    **NA** |  **1.00** |         **-** |          **NA** |
| Raycast  | 1024 | 4.176 ms |    NA |  1.07 |         - |          NA |
