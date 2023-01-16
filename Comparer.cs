

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
    public bool  OldWay_Valid_Bytecode()
    {
        return EvmObjectFormatO.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool  OldWay_Invalid_Bytecode()
    {
        return EvmObjectFormatO.IsValidEof(InvalidBytecode, out _);
    }

    [Benchmark]
    public bool  NewWay_Valid_Bytecode()
    {
        return EvmObjectFormatN.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool  NewWay_Invalid_Bytecode()
    {
        return EvmObjectFormatN.IsValidEof(InvalidBytecode, out _);
    }
    
}