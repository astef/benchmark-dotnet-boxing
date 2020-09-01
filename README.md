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
| **RunWithObjects** |       **500** |       **5000** |     **346.6 ms** |     **91.46 ms** |    **23.75 ms** |    **38000.0000** |    **19000.0000** |    **19000.0000** |    **228.89 MB** |
|   RunWithBytes |       500 |       5000 |     147.1 ms |      4.97 ms |     0.77 ms |    19000.0000 |    19000.0000 |    19000.0000 |     57.23 MB |
| **RunWithObjects** |       **500** |      **10000** |     **642.0 ms** |     **22.65 ms** |     **3.51 ms** |    **72000.0000** |    **36000.0000** |    **36000.0000** |    **457.78 MB** |
|   RunWithBytes |       500 |      10000 |     293.5 ms |     21.52 ms |     5.59 ms |    36000.0000 |    36000.0000 |    36000.0000 |    114.45 MB |
| **RunWithObjects** |       **500** |      **20000** |   **1,268.7 ms** |     **29.58 ms** |     **7.68 ms** |   **143000.0000** |    **72000.0000** |    **72000.0000** |    **915.54 MB** |
|   RunWithBytes |       500 |      20000 |     558.8 ms |     10.31 ms |     2.68 ms |    72000.0000 |    72000.0000 |    72000.0000 |    228.89 MB |
| **RunWithObjects** |       **500** |      **40000** |   **2,972.9 ms** |    **145.24 ms** |    **37.72 ms** |   **305000.0000** |   **194000.0000** |   **112000.0000** |   **1831.08 MB** |
|   RunWithBytes |       500 |      40000 |   1,132.6 ms |     43.86 ms |    11.39 ms |   125000.0000 |   125000.0000 |   125000.0000 |    457.78 MB |
| **RunWithObjects** |       **500** |      **80000** |   **6,946.1 ms** |    **242.30 ms** |    **37.50 ms** |   **584000.0000** |   **358000.0000** |   **223000.0000** |   **3662.17 MB** |
|   RunWithBytes |       500 |      80000 |   2,289.5 ms |    151.17 ms |    39.26 ms |   250000.0000 |   250000.0000 |   250000.0000 |    915.54 MB |
| **RunWithObjects** |       **500** |     **160000** |  **14,357.0 ms** |    **181.39 ms** |    **28.07 ms** |  **1204000.0000** |   **776000.0000** |   **498000.0000** |    **7324.3 MB** |
|   RunWithBytes |       500 |     160000 |   4,593.6 ms |    322.79 ms |    83.83 ms |   500000.0000 |   500000.0000 |   500000.0000 |   1831.07 MB |
| **RunWithObjects** |      **1000** |       **5000** |     **634.7 ms** |     **24.33 ms** |     **6.32 ms** |    **75000.0000** |    **38000.0000** |    **38000.0000** |    **457.79 MB** |
|   RunWithBytes |      1000 |       5000 |     285.3 ms |      5.82 ms |     0.90 ms |    38000.0000 |    38000.0000 |    38000.0000 |    114.46 MB |
| **RunWithObjects** |      **1000** |      **10000** |   **1,247.8 ms** |     **38.69 ms** |    **10.05 ms** |   **143000.0000** |    **72000.0000** |    **72000.0000** |    **915.55 MB** |
|   RunWithBytes |      1000 |      10000 |     571.5 ms |     22.73 ms |     5.90 ms |    72000.0000 |    72000.0000 |    72000.0000 |     228.9 MB |
| **RunWithObjects** |      **1000** |      **20000** |   **2,510.7 ms** |     **39.27 ms** |    **10.20 ms** |   **286000.0000** |   **143000.0000** |   **143000.0000** |   **1831.08 MB** |
|   RunWithBytes |      1000 |      20000 |   1,134.9 ms |      8.80 ms |     1.36 ms |   143000.0000 |   143000.0000 |   143000.0000 |    457.79 MB |
| **RunWithObjects** |      **1000** |      **40000** |   **5,955.1 ms** |    **250.27 ms** |    **64.99 ms** |   **611000.0000** |   **389000.0000** |   **223000.0000** |   **3662.21 MB** |
|   RunWithBytes |      1000 |      40000 |   2,250.4 ms |     72.98 ms |    11.29 ms |   250000.0000 |   250000.0000 |   250000.0000 |    915.55 MB |
| **RunWithObjects** |      **1000** |      **80000** |  **13,933.1 ms** |    **178.53 ms** |    **27.63 ms** |  **1167000.0000** |   **722000.0000** |   **440000.0000** |   **7324.29 MB** |
|   RunWithBytes |      1000 |      80000 |   4,487.9 ms |     46.61 ms |    12.10 ms |   500000.0000 |   500000.0000 |   500000.0000 |   1831.08 MB |
| **RunWithObjects** |      **1000** |     **160000** |  **28,656.6 ms** |    **317.74 ms** |    **82.52 ms** |  **2398000.0000** |  **1554000.0000** |   **998000.0000** |  **14648.55 MB** |
|   RunWithBytes |      1000 |     160000 |   8,952.9 ms |    107.50 ms |    16.64 ms |  1000000.0000 |  1000000.0000 |  1000000.0000 |   3662.13 MB |
| **RunWithObjects** |      **2000** |       **5000** |   **1,252.9 ms** |     **26.09 ms** |     **6.77 ms** |   **149000.0000** |    **75000.0000** |    **75000.0000** |    **915.57 MB** |
|   RunWithBytes |      2000 |       5000 |     572.9 ms |     17.25 ms |     2.67 ms |    75000.0000 |    75000.0000 |    75000.0000 |    228.93 MB |
| **RunWithObjects** |      **2000** |      **10000** |   **2,485.9 ms** |     **31.25 ms** |     **8.12 ms** |   **286000.0000** |   **143000.0000** |   **143000.0000** |    **1831.1 MB** |
|   RunWithBytes |      2000 |      10000 |   1,119.3 ms |     24.03 ms |     3.72 ms |   143000.0000 |   143000.0000 |   143000.0000 |    457.81 MB |
| **RunWithObjects** |      **2000** |      **20000** |   **5,009.6 ms** |     **63.11 ms** |    **16.39 ms** |   **572000.0000** |   **286000.0000** |   **286000.0000** |   **3662.16 MB** |
|   RunWithBytes |      2000 |      20000 |   2,268.0 ms |     40.89 ms |     6.33 ms |   286000.0000 |   286000.0000 |   286000.0000 |    915.57 MB |
| **RunWithObjects** |      **2000** |      **40000** |  **11,910.2 ms** |    **253.20 ms** |    **65.75 ms** |  **1221000.0000** |   **776000.0000** |   **445000.0000** |   **7324.38 MB** |
|   RunWithBytes |      2000 |      40000 |   4,496.9 ms |     54.48 ms |    14.15 ms |   500000.0000 |   500000.0000 |   500000.0000 |    1831.1 MB |
| **RunWithObjects** |      **2000** |      **80000** |  **27,682.7 ms** |    **442.31 ms** |   **114.87 ms** |  **2330000.0000** |  **1433000.0000** |   **875000.0000** |  **14648.58 MB** |
|   RunWithBytes |      2000 |      80000 |   8,983.2 ms |    167.40 ms |    25.91 ms |  1000000.0000 |  1000000.0000 |  1000000.0000 |   3662.16 MB |
| **RunWithObjects** |      **2000** |     **160000** |  **57,413.5 ms** |    **324.54 ms** |    **84.28 ms** |  **4783000.0000** |  **3118000.0000** |  **1998000.0000** |  **29297.17 MB** |
|   RunWithBytes |      2000 |     160000 |  17,946.7 ms |    452.92 ms |    70.09 ms |  2000000.0000 |  2000000.0000 |  2000000.0000 |   7324.26 MB |
| **RunWithObjects** |      **4000** |       **5000** |   **2,513.5 ms** |     **61.23 ms** |     **9.47 ms** |   **297000.0000** |   **149000.0000** |   **149000.0000** |   **1831.15 MB** |
|   RunWithBytes |      4000 |       5000 |   1,123.0 ms |      6.20 ms |     1.61 ms |   149000.0000 |   149000.0000 |   149000.0000 |    457.86 MB |
| **RunWithObjects** |      **4000** |      **10000** |   **5,017.6 ms** |     **67.47 ms** |    **17.52 ms** |   **572000.0000** |   **286000.0000** |   **286000.0000** |    **3662.2 MB** |
|   RunWithBytes |      4000 |      10000 |   2,242.8 ms |     40.11 ms |     6.21 ms |   286000.0000 |   286000.0000 |   286000.0000 |    915.62 MB |
| **RunWithObjects** |      **4000** |      **20000** |   **9,987.1 ms** |    **254.20 ms** |    **66.02 ms** |  **1143000.0000** |   **572000.0000** |   **572000.0000** |   **7324.31 MB** |
|   RunWithBytes |      4000 |      20000 |   4,545.1 ms |    196.02 ms |    50.91 ms |   572000.0000 |   572000.0000 |   572000.0000 |   1831.15 MB |
| **RunWithObjects** |      **4000** |      **40000** |  **23,691.0 ms** |    **308.77 ms** |    **80.19 ms** |  **2444000.0000** |  **1555000.0000** |   **890000.0000** |  **14648.77 MB** |
|   RunWithBytes |      4000 |      40000 |   9,106.0 ms |    163.52 ms |    42.47 ms |  1000000.0000 |  1000000.0000 |  1000000.0000 |    3662.2 MB |
| **RunWithObjects** |      **4000** |      **80000** |  **55,359.1 ms** |  **1,262.89 ms** |   **195.43 ms** |  **4666000.0000** |  **2881000.0000** |  **1763000.0000** |  **29297.18 MB** |
|   RunWithBytes |      4000 |      80000 |  17,967.5 ms |    359.72 ms |    93.42 ms |  2000000.0000 |  2000000.0000 |  2000000.0000 |   7324.31 MB |
| **RunWithObjects** |      **4000** |     **160000** | **114,073.8 ms** |  **4,924.79 ms** | **1,278.95 ms** |  **9569000.0000** |  **6216000.0000** |  **3998000.0000** |   **58594.3 MB** |
|   RunWithBytes |      4000 |     160000 |  35,633.2 ms |    775.77 ms |   120.05 ms |  4000000.0000 |  4000000.0000 |  4000000.0000 |  14648.53 MB |
| **RunWithObjects** |      **8000** |       **5000** |   **4,930.8 ms** |     **66.33 ms** |    **17.23 ms** |   **593000.0000** |   **297000.0000** |   **297000.0000** |   **3662.29 MB** |
|   RunWithBytes |      8000 |       5000 |   2,218.2 ms |     55.83 ms |     8.64 ms |   297000.0000 |   297000.0000 |   297000.0000 |    915.71 MB |
| **RunWithObjects** |      **8000** |      **10000** |   **9,869.9 ms** |    **222.22 ms** |    **57.71 ms** |  **1143000.0000** |   **572000.0000** |   **572000.0000** |    **7324.4 MB** |
|   RunWithBytes |      8000 |      10000 |   4,463.3 ms |    159.57 ms |    41.44 ms |   572000.0000 |   572000.0000 |   572000.0000 |   1831.24 MB |
| **RunWithObjects** |      **8000** |      **20000** |  **19,684.4 ms** |    **108.02 ms** |    **16.72 ms** |  **2286000.0000** |  **1143000.0000** |  **1143000.0000** |  **14648.62 MB** |
|   RunWithBytes |      8000 |      20000 |   8,954.2 ms |    212.45 ms |    55.17 ms |  1143000.0000 |  1143000.0000 |  1143000.0000 |   3662.29 MB |
| **RunWithObjects** |      **8000** |      **40000** |  **48,160.6 ms** |  **1,606.41 ms** |   **417.18 ms** |  **4888000.0000** |  **3110000.0000** |  **1778000.0000** |  **29297.61 MB** |
|   RunWithBytes |      8000 |      40000 |  18,104.6 ms |    446.09 ms |   115.85 ms |  2000000.0000 |  2000000.0000 |  2000000.0000 |    7324.4 MB |
| **RunWithObjects** |      **8000** |      **80000** | **111,919.1 ms** |  **7,564.50 ms** | **1,964.48 ms** |  **9308000.0000** |  **5719000.0000** |  **3469000.0000** |  **58594.46 MB** |
|   RunWithBytes |      8000 |      80000 |  36,898.4 ms |    852.26 ms |   131.89 ms |  4000000.0000 |  4000000.0000 |  4000000.0000 |  14648.62 MB |
| **RunWithObjects** |      **8000** |     **160000** | **227,864.9 ms** | **16,049.05 ms** | **4,167.89 ms** | **19166000.0000** | **12451000.0000** |  **7998000.0000** | **117188.62 MB** |
|   RunWithBytes |      8000 |     160000 |  70,800.1 ms |    473.14 ms |    73.22 ms |  8000000.0000 |  8000000.0000 |  8000000.0000 |  29297.06 MB |
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




