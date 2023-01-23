

using BenchmarkDotNet.Attributes;
using Nethermind.Evm.EOF;
using static EofTestsBase;

[MemoryDiagnoser, Config(typeof(AntiVirusFriendlyConfig))]
public class MyBenchmark
{
    public byte[] InvalidBytecode = ScenarioCase.GenerateFormatScenarios(Scenario.Invalid);
    public byte[] ValidBytecode = ScenarioCase.GenerateFormatScenarios(Scenario.Valid);
    EofHeader dummy = new EofHeader();

    [Benchmark]
    public bool Sequential_Pooled_Valid_Bytecode_No_Bit_Manip()
    {
        return EvmObjectFormatPSDouble.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Invalid_Bytecode_No_Bit_Manip()
    {
        return EvmObjectFormatPSDouble.IsValidEof(InvalidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Valid_Bytecode_No_Bit_Manip_With_Opaque_ReachableCode_Check()
    {
        return EvmObjectFormatPSDoubleReachableCodeOpt.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Invalid_Bytecode_No_Bit_Manip_With_Opaque_ReachableCode_Check()
    {
        return EvmObjectFormatPSDoubleReachableCodeOpt.IsValidEof(InvalidBytecode, out _);
    }

    
    [Benchmark]
    public bool Sequential_Pooled_Valid_Bytecode_Struct()
    {
        return EvmObjectFormatPSStruct.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Invalid_Bytecode_Struct()
    {
        return EvmObjectFormatPSStruct.IsValidEof(InvalidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Valid_Bytecode_Struct_With_Opaque_ReachableCode_Check()
    {
        return EvmObjectFormatPSStructReachableCodeOpt.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Invalid_Bytecode_Struct_With_Opaque_ReachableCode_Check()
    {
        return EvmObjectFormatPSStructReachableCodeOpt.IsValidEof(InvalidBytecode, out _);
    }
}