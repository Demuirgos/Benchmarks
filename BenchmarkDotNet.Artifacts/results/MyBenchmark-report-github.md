``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.22621.1105)
Intel Core i5-8265U CPU 1.60GHz (Whiskey Lake), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.100
  [Host] : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2

Job=MediumRun  Toolchain=InProcessNoEmitToolchain  IterationCount=15  
LaunchCount=2  WarmupCount=10  

```
|                                                                          Method |     Mean |     Error |    StdDev |   Median | Allocated |
|-------------------------------------------------------------------------------- |---------:|----------:|----------:|---------:|----------:|
|                                   Sequential_Pooled_Valid_Bytecode_No_Bit_Manip | 2.326 ms | 0.1365 ms | 0.2000 ms | 2.287 ms |     351 B |
|                                 Sequential_Pooled_Invalid_Bytecode_No_Bit_Manip | 1.164 ms | 0.0777 ms | 0.1139 ms | 1.095 ms |     324 B |
|   Sequential_Pooled_Valid_Bytecode_No_Bit_Manip_With_Opaque_ReachableCode_Check | 2.663 ms | 0.3935 ms | 0.5890 ms | 2.450 ms |     351 B |
| Sequential_Pooled_Invalid_Bytecode_No_Bit_Manip_With_Opaque_ReachableCode_Check | 1.298 ms | 0.1471 ms | 0.2201 ms | 1.270 ms |     351 B |
|                                         Sequential_Pooled_Valid_Bytecode_Struct | 2.752 ms | 0.1962 ms | 0.2876 ms | 2.694 ms |     360 B |
|                                       Sequential_Pooled_Invalid_Bytecode_Struct | 1.488 ms | 0.2798 ms | 0.4188 ms | 1.322 ms |     324 B |
|         Sequential_Pooled_Valid_Bytecode_Struct_With_Opaque_ReachableCode_Check | 2.715 ms | 0.2398 ms | 0.3590 ms | 2.565 ms |     354 B |
|       Sequential_Pooled_Invalid_Bytecode_Struct_With_Opaque_ReachableCode_Check | 1.467 ms | 0.1098 ms | 0.1610 ms | 1.488 ms |     312 B |
