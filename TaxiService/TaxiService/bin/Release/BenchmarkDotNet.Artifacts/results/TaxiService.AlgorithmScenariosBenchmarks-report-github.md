```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.5011/22H2/2022Update)
AMD Ryzen 5 3500U with Radeon Vega Mobile Gfx 2.10GHz, 1 CPU, 8 logical and 4 physical cores
  [Host]     : .NET Framework 4.8.1 (4.8.9277.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8.1 (4.8.9277.0), X86 LegacyJIT


```
| Method                                        | Mean          | Error       | StdDev      | Rank | Gen0     | Gen1    | Gen2    | Allocated |
|---------------------------------------------- |--------------:|------------:|------------:|-----:|---------:|--------:|--------:|----------:|
| fewDriversOnSmallMap_EnumerationSearch        |      2.605 μs |   0.0222 μs |   0.0185 μs |    1 |   0.4730 |       - |       - |     248 B |
| fewDriversOnSmallMap_PriorityQueueSearch      |      3.497 μs |   0.0263 μs |   0.0219 μs |    2 |   0.3662 |       - |       - |     192 B |
| manyDriversOnSmallMap_PriorityQueueSearch     |      4.818 μs |   0.0775 μs |   0.0687 μs |    3 |   0.3662 |       - |       - |     192 B |
| manyDriversOnVeryLargeMap_RadialSearch        |      4.940 μs |   0.0340 μs |   0.0284 μs |    3 |   1.3885 |       - |       - |     729 B |
| manyDriversOnSmallMap_RadialSearch            |      6.020 μs |   0.0299 μs |   0.0265 μs |    4 |   1.5488 |       - |       - |     813 B |
| manyDriversOnSmallMap_EnumerationSearch       |      6.873 μs |   0.0423 μs |   0.0375 μs |    5 |   1.7395 |       - |       - |     913 B |
| fewDriversOnSmallMap_RadialSearch             |     29.694 μs |   0.3498 μs |   0.2921 μs |    6 |   4.2114 |       - |       - |    2227 B |
| fewDriversOnVeryLargeMap_EnumerationSearch    |  1,816.779 μs |  14.2905 μs |  12.6681 μs |    7 |        - |       - |       - |     336 B |
| fewDriversOnVeryLargeMap_PriorityQueueSearch  |  3,076.966 μs |  20.4581 μs |  19.1365 μs |    8 |        - |       - |       - |     192 B |
| manyDriversOnVeryLargeMap_PriorityQueueSearch | 15,288.522 μs | 102.2217 μs |  85.3598 μs |    9 |        - |       - |       - |         - |
| manyDriversOnVeryLargeMap_EnumerationSearch   | 47,164.766 μs | 904.7887 μs | 929.1513 μs |   10 | 272.7273 | 90.9091 | 90.9091 | 6195241 B |
| fewDriversOnVeryLargeMap_RadialSearch         | 74,876.476 μs | 690.6180 μs | 612.2148 μs |   11 |        - |       - |       - |   15214 B |
