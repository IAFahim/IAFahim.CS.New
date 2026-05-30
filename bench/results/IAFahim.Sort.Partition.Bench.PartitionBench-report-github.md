```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
AMD EPYC 9V74, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method          | N    | Mean     | Error | Allocated |
|---------------- |----- |---------:|------:|----------:|
| **Partition_Bench** | **256**  | **519.2 μs** |    **NA** |         **-** |
| NthElement      | 256  | 616.1 μs |    NA |         - |
| **Partition_Bench** | **1024** | **511.1 μs** |    **NA** |         **-** |
| NthElement      | 1024 | 609.5 μs |    NA |         - |
