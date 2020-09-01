# benchmark-dotnet-boxing
Benchmark of the real overhead (including GC) of the value types boxing compared to direct serialization into the byte array.

After the performance-critical synchronous procedure, there's basically two non-blocking ways of sharing the result of processing with other threads:
 - populate some kind of generic buffer, e.g. `object[]`
   - easy to implement (every value is an `object` already)
   - extra boxing cost in the main procedure for every value in the message
   - extra collection cost in the application for every value in the message
 - populate value-typed buffer, e.g. `byte[]`
   - need to implement serialization for every possible type (can be tricky for custom reference types)
   - extra serialization cost in the main procedure for every value in the message
   - no boxing, no collection cost
  
  The goal is to find out the optimum conditions for selecting one of the above strategies.

## Results
``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.18363.1016 (1909/November2018Update/19H2)
Intel Core i7-7820HQ CPU 2.90GHz (Kaby Lake), 1 CPU, 8 logical and 4 physical cores
.NET Core SDK=3.1.301
  [Host]     : .NET Core 3.1.5 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.27001), X64 RyuJIT
  Job-IKJAKY : .NET Core 3.1.5 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.27001), X64 RyuJIT

IterationCount=5  WarmupCount=1  

```
|         Method | IterCount | MessageNum |         Mean |        Error |      StdDev |         Gen 0 |         Gen 1 |         Gen 2 |    Allocated |
|--------------- |---------- |----------- |-------------:|-------------:|------------:|--------------:|--------------:|--------------:|-------------:|
| **RunWithObjects** |     **16000** |       **5000** |   **9,718.0 ms** |     **93.25 ms** |    **24.22 ms** |  **1186000.0000** |   **593000.0000** |   **593000.0000** |   **7324.59 MB** |
|   RunWithBytes |     16000 |       5000 |   4,421.4 ms |     46.79 ms |     7.24 ms |   593000.0000 |   593000.0000 |   593000.0000 |   1831.42 MB |
| **RunWithObjects** |     **16000** |      **10000** |  **19,459.8 ms** |    **218.79 ms** |    **56.82 ms** |  **2286000.0000** |  **1143000.0000** |  **1143000.0000** |   **14648.8 MB** |
|   RunWithBytes |     16000 |      10000 |   8,881.2 ms |    294.69 ms |    76.53 ms |  1143000.0000 |  1143000.0000 |  1143000.0000 |   3662.48 MB |
| **RunWithObjects** |     **16000** |      **20000** |  **39,103.3 ms** |    **673.01 ms** |   **174.78 ms** |  **4572000.0000** |  **2286000.0000** |  **2286000.0000** |  **29297.24 MB** |
|   RunWithBytes |     16000 |      20000 |  17,892.4 ms |    471.22 ms |    72.92 ms |  2286000.0000 |  2286000.0000 |  2286000.0000 |   7324.59 MB |
| **RunWithObjects** |     **16000** |      **40000** |  **93,927.5 ms** |  **1,691.43 ms** |   **439.26 ms** |  **9777000.0000** |  **6221000.0000** |  **3556000.0000** |  **58594.93 MB** |
|   RunWithBytes |     16000 |      40000 |  35,392.0 ms |    457.73 ms |    70.83 ms |  4000000.0000 |  4000000.0000 |  4000000.0000 |   14648.8 MB |
| **RunWithObjects** |     **16000** |      **80000** | **215,643.5 ms** |  **1,526.78 ms** |   **396.50 ms** | **18644000.0000** | **11494000.0000** |  **7019000.0000** | **117188.79 MB** |
|   RunWithBytes |     16000 |      80000 |  71,040.3 ms |    631.03 ms |   163.88 ms |  8000000.0000 |  8000000.0000 |  8000000.0000 |  29297.24 MB |
| **RunWithObjects** |     **16000** |     **160000** | **447,694.4 ms** |  **2,373.70 ms** |   **616.44 ms** | **38338000.0000** | **24914000.0000** | **15998000.0000** | **234377.38 MB** |
|   RunWithBytes |     16000 |     160000 | 140,678.0 ms |  1,639.81 ms |   253.76 ms | 16000000.0000 | 16000000.0000 | 16000000.0000 |  58594.12 MB |


## Conclusions
For the provided message structure and size (three `long` fields), direct serialization is at least two times faster than the cost of boxing.

The bigger the number of messages, the bigger the difference factor (non-linear dependency): x2.2 for 5k, and x3.2 for 160k.

Allocated memory difference is proportional to the number of messages.

Different number of iterations, didn't reveal any effects (was excluded from results for brevity).


