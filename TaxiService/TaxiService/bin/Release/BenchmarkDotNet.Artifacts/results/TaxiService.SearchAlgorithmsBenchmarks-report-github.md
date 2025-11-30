```

BenchmarkDotNet v0.15.6, Windows 10 (10.0.19045.5011/22H2/2022Update)
AMD Ryzen 5 3500U with Radeon Vega Mobile Gfx 2.10GHz, 1 CPU, 8 logical and 4 physical cores
  [Host]     : .NET Framework 4.8.1 (4.8.9277.0), X86 LegacyJIT
  DefaultJob : .NET Framework 4.8.1 (4.8.9277.0), X86 LegacyJIT


```
| Method                           | Mean           | Error        | StdDev       | Rank | Gen0    | Allocated |
|--------------------------------- |---------------:|-------------:|-------------:|-----:|--------:|----------:|
| SmallMap_PriorityQueueSearch     |       873.9 ns |     17.39 ns |     37.06 ns |    1 |  0.3662 |     192 B |
| SmallMap_EnumerationSearch       |     1,209.9 ns |     23.63 ns |     40.76 ns |    2 |  0.4730 |     248 B |
| SmallMap_RadialSearch            |     1,487.3 ns |     29.43 ns |     71.08 ns |    3 |  0.4730 |     248 B |
| MediumMap_EnumerationSearch      |    15,275.3 ns |    261.67 ns |    399.60 ns |    4 |  0.6409 |     345 B |
| MediumMap_PriorityQueueSearch    |    29,328.2 ns |    489.21 ns |    457.60 ns |    5 |  0.3662 |     192 B |
| MediumMap_RadialSearch           |    31,084.6 ns |    196.31 ns |    183.63 ns |    6 |  3.5400 |    1867 B |
| LargeMap_RadialSearch            |   246,933.3 ns |  1,993.88 ns |  1,865.08 ns |    7 |  6.8359 |    3720 B |
| LargeMap_EnumerationSearch       |   453,008.7 ns |  2,531.79 ns |  2,244.36 ns |    8 |  1.4648 |     912 B |
| VeryLargeMap_RadialSearch        |   674,552.7 ns |  7,941.63 ns |  7,040.05 ns |    9 | 20.5078 |   10873 B |
| LargeMap_PriorityQueueSearch     |   788,671.5 ns |  9,105.03 ns |  7,108.61 ns |   10 |       - |     192 B |
| VeryLargeMap_EnumerationSearch   | 1,778,531.3 ns | 12,121.50 ns | 10,122.00 ns |   11 |  1.9531 |    1633 B |
| VeryLargeMap_PriorityQueueSearch | 3,082,060.8 ns | 28,512.18 ns | 23,808.96 ns |   12 |       - |     174 B |
