

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
    public bool  Parallel_Stack_Valid_Bytecode()
    {
        return EvmObjectFormatSP.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool  Parallel_Stack_Invalid_Bytecode()
    {
        return EvmObjectFormatSP.IsValidEof(InvalidBytecode, out _);
    }

    [Benchmark]
    public bool  Parallel_Pooled_Valid_Bytecode()
    {
        return EvmObjectFormatPP.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool  Parallel_Pooled_Invalid_Bytecode()
    {
        return EvmObjectFormatPP.IsValidEof(InvalidBytecode, out _);
    }

    [Benchmark]
    public bool  Sequential_Stack_Valid_Bytecode()
    {
        return EvmObjectFormatSS.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool  Sequential_Stack_Invalid_Bytecode()
    {
        return EvmObjectFormatSS.IsValidEof(InvalidBytecode, out _);
    }

    [Benchmark]
    public bool  Sequential_Pooled_Valid_Bytecode()
    {
        return EvmObjectFormatPS.IsValidEof(ValidBytecode, out _);
    }

    [Benchmark]
    public bool  Sequential_Pooled_Invalid_Bytecode()
    {
        return EvmObjectFormatPS.IsValidEof(InvalidBytecode, out _);
    }

    
}