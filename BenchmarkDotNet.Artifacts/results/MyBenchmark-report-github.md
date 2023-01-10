``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22621.963)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  DefaultJob : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2


```
|                                         Method |           Mean |        Error |       StdDev |       Gen0 |     Gen1 |     Gen2 |   Allocated |
|----------------------------------------------- |---------------:|-------------:|-------------:|-----------:|---------:|---------:|------------:|
|                        Bitarray_Valid_Bytecode |    37,251.9 μs |    676.60 μs |    599.79 μs | 38714.2857 |        - |        - | 121593847 B |
|                    BinarySearch_Valid_Bytecode |     3,542.7 μs |     36.85 μs |     32.67 μs |   171.8750 | 121.0938 | 121.0938 |    956609 B |
|                     Bitmap_Valid_Bytecode_Heap |     2,155.5 μs |     22.27 μs |     20.83 μs |    89.8438 |  42.9688 |        - |    398623 B |
|                    Bitmap_Valid_Bytecode_Stack |     2,144.0 μs |     20.31 μs |     18.01 μs |    89.8438 |  27.3438 |        - |    392463 B |
|                 Bitmap_Valid_Bytecode_2_Stacks |       727.6 μs |      5.07 μs |      4.74 μs |          - |        - |        - |         2 B |
|      Bitmap_Valid_Bytecode_2_Stacks_With_PpCnt |       744.4 μs |      4.90 μs |      4.35 μs |          - |        - |        - |         2 B |
|                Bitmap_Valid_Bytecode_ArrayPool |     2,123.7 μs |     18.74 μs |     16.61 μs |    85.9375 |  46.8750 |        - |    400673 B |
|              Bitmap_Valid_Bytecode_2_ArrayPool |       766.5 μs |     15.24 μs |     13.51 μs |     7.8125 |        - |        - |     24519 B |
|   Bitmap_Valid_Bytecode_2_ArrayPool_With_PpCnt |       778.3 μs |      5.73 μs |      4.79 μs |          - |        - |        - |         2 B |
|                        NaiveWay_Valid_Bytecode | 1,416,975.0 μs | 14,218.83 μs | 13,300.30 μs |          - |        - |        - |    919120 B |
|                      Bitarray_Invalid_Bytecode |    35,973.9 μs |    564.49 μs |    528.02 μs | 38733.3333 |        - |        - | 121593899 B |
|                  BinarySearch_Invalid_Bytecode |     2,089.4 μs |     38.36 μs |     34.01 μs |   140.6250 | 121.0938 | 121.0938 |    916984 B |
|                   Bitmap_Invalid_Bytecode_Heap |     1,924.4 μs |     15.49 μs |     12.93 μs |    85.9375 |  44.9219 |        - |    398388 B |
|                  Bitmap_Invalid_Bytecode_Stack |     1,845.6 μs |     16.96 μs |     15.03 μs |    89.8438 |  31.2500 |        - |    392235 B |
|               Bitmap_Invalid_Bytecode_2_Stacks |       706.8 μs |      4.31 μs |      4.03 μs |          - |        - |        - |         2 B |
|    Bitmap_Invalid_Bytecode_2_Stacks_With_PpCnt |       736.6 μs |      6.57 μs |      6.15 μs |          - |        - |        - |         2 B |
|              Bitmap_Invalid_Bytecode_ArrayPool |     1,843.1 μs |     12.66 μs |     11.22 μs |    83.9844 |  41.0156 |        - |    400441 B |
|            Bitmap_Invalid_Bytecode_2_ArrayPool |       712.8 μs |      4.32 μs |      3.83 μs |     7.8125 |        - |        - |     24519 B |
| Bitmap_Invalid_Bytecode_2_ArrayPool_With_PpCnt |       741.5 μs |      5.86 μs |      5.48 μs |          - |        - |        - |         2 B |
|                      NaiveWay_Invalid_Bytecode |     2,077.4 μs |     10.10 μs |      8.44 μs |   148.4375 | 121.0938 | 121.0938 |    916996 B |