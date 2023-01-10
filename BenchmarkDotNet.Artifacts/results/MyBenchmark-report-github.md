``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22621.963)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2


```
|                                         Method |           Mean |        Error |       StdDev |       Gen0 |     Gen1 |     Gen2 |   Allocated |
|----------------------------------------------- |---------------:|-------------:|-------------:|-----------:|---------:|---------:|------------:|
|                        NaiveWay_Valid_Bytecode | 1,421,979.1 μs | 15,465.68 μs | 13,709.92 μs |          - |        - |        - |    919120 B |
|                        Bitarray_Valid_Bytecode |    37,280.7 μs |    690.02 μs |    611.68 μs | 38714.2857 |        - |        - | 121593847 B |
|                    BinarySearch_Valid_Bytecode |     3,575.2 μs |     20.11 μs |     16.79 μs |   171.8750 | 121.0938 | 121.0938 |    956609 B |
|                    Bitmap_Valid_Bytecode_Stack |     2,163.0 μs |     15.11 μs |     13.40 μs |    89.8438 |  31.2500 |        - |    392463 B |
|                 Bitmap_Valid_Bytecode_2_Stacks |       718.1 μs |      8.66 μs |      7.68 μs |          - |        - |        - |         2 B |
|      Bitmap_Valid_Bytecode_2_Stacks_With_PpCnt |       776.9 μs |      4.86 μs |      4.31 μs |          - |        - |        - |         2 B |
|                Bitmap_Valid_Bytecode_ArrayPool |     2,182.3 μs |     20.04 μs |     18.75 μs |    85.9375 |  42.9688 |        - |    408745 B |
|              Bitmap_Valid_Bytecode_2_ArrayPool |       754.6 μs |      7.33 μs |      6.49 μs |     7.8125 |        - |        - |     24519 B |
|   Bitmap_Valid_Bytecode_2_ArrayPool_With_PpCnt |       750.7 μs |      4.31 μs |      3.60 μs |     7.8125 |        - |        - |     24519 B |
|                      NaiveWay_Invalid_Bytecode |     2,069.1 μs |     14.15 μs |     11.82 μs |   140.6250 | 121.0938 | 121.0938 |    916984 B |
|                      Bitarray_Invalid_Bytecode |    35,838.5 μs |    352.99 μs |    294.76 μs | 38714.2857 |        - |        - | 121593879 B |
|                  BinarySearch_Invalid_Bytecode |     2,067.8 μs |     16.74 μs |     13.98 μs |   140.6250 | 121.0938 | 121.0938 |    916984 B |
|                  Bitmap_Invalid_Bytecode_Stack |     1,848.0 μs |     12.94 μs |     10.80 μs |    76.1719 |  48.8281 |        - |    392213 B |
|               Bitmap_Invalid_Bytecode_2_Stacks |       707.8 μs |      5.00 μs |      4.44 μs |          - |        - |        - |         2 B |
|    Bitmap_Invalid_Bytecode_2_Stacks_With_PpCnt |       740.4 μs |      2.98 μs |      2.64 μs |          - |        - |        - |         2 B |
|              Bitmap_Invalid_Bytecode_ArrayPool |     1,852.2 μs |     11.05 μs |     10.34 μs |    87.8906 |  42.9688 |        - |    408519 B |
|            Bitmap_Invalid_Bytecode_2_ArrayPool |       716.8 μs |     10.59 μs |      8.84 μs |     7.8125 |        - |        - |     24519 B |
| Bitmap_Invalid_Bytecode_2_ArrayPool_With_PpCnt |       740.4 μs |      3.80 μs |      3.55 μs |          - |        - |        - |         2 B |
