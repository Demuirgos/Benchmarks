

using BenchmarkDotNet.Attributes;
using Nethermind.Evm.EOF;
using static EofTestsBase;

[MemoryDiagnoser]
public class MyBenchmark
{
    public byte[] InvalidBytecode = ScenarioCase.GenerateFormatScenarios(Scenario.Invalid);
    public byte[] ValidBytecode = ScenarioCase.GenerateFormatScenarios(Scenario.Valid);
    EofHeader dummy = new EofHeader();


    [Benchmark]
    public bool Sequential_Pooled_Valid_Bytecode_Old()
    {
        return EvmObjectFormatPSOld.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Invalid_Bytecode_Old()
    {
        return EvmObjectFormatPSOld.IsValidEof(InvalidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Valid_Bytecode_New()
    {
        return EvmObjectFormatPSOpt.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool Sequential_Pooled_Invalid_Bytecode_New()
    {
        return EvmObjectFormatPSOpt.IsValidEof(InvalidBytecode, out _);
    }


}