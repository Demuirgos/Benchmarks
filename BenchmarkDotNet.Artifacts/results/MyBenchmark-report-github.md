``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22621.1105)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host] : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2

Job=MediumRun  Toolchain=InProcessNoEmitToolchain  IterationCount=15  
LaunchCount=2  WarmupCount=10  

```
|                                          Method |     Mean |     Error |    StdDev |     Gen0 | Allocated |
|------------------------------------------------ |---------:|----------:|----------:|---------:|----------:|
|            Sequential_Pooled_Valid_Bytecode_Old | 4.988 ms | 1.0679 ms | 1.5653 ms | 773.4375 | 2448844 B |
|          Sequential_Pooled_Invalid_Bytecode_Old | 1.727 ms | 0.0991 ms | 0.1324 ms | 388.6719 | 1224567 B |
|   Sequential_Pooled_Valid_Bytecode_No_Bit_Manip | 2.415 ms | 0.2789 ms | 0.4175 ms |        - |     351 B |
| Sequential_Pooled_Invalid_Bytecode_No_Bit_Manip | 1.097 ms | 0.0346 ms | 0.0461 ms |        - |     324 B |
|         Sequential_Pooled_Valid_Bytecode_Struct | 2.525 ms | 0.1997 ms | 0.2864 ms |        - |     352 B |
|       Sequential_Pooled_Invalid_Bytecode_Struct | 1.231 ms | 0.0096 ms | 0.0138 ms |        - |     303 B |
|            Sequential_Pooled_Valid_Bytecode_New | 2.164 ms | 0.0206 ms | 0.0296 ms |        - |     312 B |
|          Sequential_Pooled_Invalid_Bytecode_New | 1.115 ms | 0.0082 ms | 0.0117 ms |        - |     304 B |
