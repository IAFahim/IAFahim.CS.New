```

BenchmarkDotNet v0.14.0, Ubuntu 24.04.4 LTS (Noble Numbat)
Intel Xeon Platinum 8370C CPU 2.80GHz, 1 CPU, 4 logical and 2 physical cores
.NET SDK 10.0.300
  [Host] : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
  Dry    : .NET 10.0.8 (10.0.826.23019), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI

Job=Dry  IterationCount=1  LaunchCount=1  
RunStrategy=ColdStart  UnrollFactor=1  WarmupCount=1  

```
| Method             | LogN | Rot | Mean     | Error | Ratio | Allocated | Alloc Ratio |
|------------------- |----- |---- |---------:|------:|------:|----------:|------------:|
| **HilbertOrderRun**    | **10**   | **0**   | **671.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 10   | 0   | 646.4 μs |    NA |  0.96 |         - |          NA |
| BlockOrderEncode   | 10   | 0   | 496.9 μs |    NA |  0.74 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **10**   | **1**   | **665.4 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 10   | 1   | 686.2 μs |    NA |  1.03 |         - |          NA |
| BlockOrderEncode   | 10   | 1   | 514.3 μs |    NA |  0.77 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **10**   | **2**   | **612.2 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 10   | 2   | 666.3 μs |    NA |  1.09 |         - |          NA |
| BlockOrderEncode   | 10   | 2   | 523.8 μs |    NA |  0.86 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **10**   | **3**   | **645.5 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 10   | 3   | 668.0 μs |    NA |  1.03 |         - |          NA |
| BlockOrderEncode   | 10   | 3   | 692.4 μs |    NA |  1.07 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **16**   | **0**   | **584.0 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 16   | 0   | 641.3 μs |    NA |  1.10 |         - |          NA |
| BlockOrderEncode   | 16   | 0   | 454.8 μs |    NA |  0.78 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **16**   | **1**   | **645.9 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 16   | 1   | 660.0 μs |    NA |  1.02 |         - |          NA |
| BlockOrderEncode   | 16   | 1   | 491.0 μs |    NA |  0.76 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **16**   | **2**   | **724.3 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 16   | 2   | 610.9 μs |    NA |  0.84 |         - |          NA |
| BlockOrderEncode   | 16   | 2   | 465.8 μs |    NA |  0.64 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **16**   | **3**   | **688.7 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 16   | 3   | 624.3 μs |    NA |  0.91 |         - |          NA |
| BlockOrderEncode   | 16   | 3   | 523.1 μs |    NA |  0.76 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **20**   | **0**   | **624.1 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 20   | 0   | 614.6 μs |    NA |  0.98 |         - |          NA |
| BlockOrderEncode   | 20   | 0   | 476.2 μs |    NA |  0.76 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **20**   | **1**   | **620.7 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 20   | 1   | 631.0 μs |    NA |  1.02 |         - |          NA |
| BlockOrderEncode   | 20   | 1   | 554.5 μs |    NA |  0.89 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **20**   | **2**   | **683.0 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 20   | 2   | 652.2 μs |    NA |  0.95 |         - |          NA |
| BlockOrderEncode   | 20   | 2   | 525.4 μs |    NA |  0.77 |         - |          NA |
|                    |      |     |          |       |       |           |             |
| **HilbertOrderRun**    | **20**   | **3**   | **722.6 μs** |    **NA** |  **1.00** |         **-** |          **NA** |
| HilbertOrderEncode | 20   | 3   | 607.1 μs |    NA |  0.84 |         - |          NA |
| BlockOrderEncode   | 20   | 3   | 523.6 μs |    NA |  0.72 |         - |          NA |
