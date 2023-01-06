

using BenchmarkDotNet.Attributes;
using Nethermind.Evm.EOF;
using static EofTestsBase;

[MemoryDiagnoser]
public class MyBenchmark
{
    public byte[] BytecodeWithAllInvalidSections => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Repeat(ScenarioCase.CreateFromScenario(Scenario.Invalid), 32).ToArray(),
        new byte[0]
    ); 
    public byte[] BytecodeWithAllValidSections => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Repeat(ScenarioCase.CreateFromScenario(Scenario.Valid), 32).ToArray(),
        new byte[0]
    ); 
    public byte[] BytecodeWithAllMixedSections => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Range(0, 32).Select(i => ScenarioCase.CreateFromScenario((Scenario)(i % 2))).ToArray(),
        new byte[0]
    ); 


    [Benchmark]
    public void Bytecode_With_32_CodeSection_1536B_Each_All_Valid_Parallel()
    {
        PEvmObjectFormat.IsValidEof(BytecodeWithAllValidSections, out _);
    }

    [Benchmark]
    public void Bytecode_With_32_CodeSection_1536B_Each_All_Valid_Sequential()
    {
        SEvmObjectFormat.IsValidEof(BytecodeWithAllValidSections, out _);
    }

     [Benchmark]
    public void Bytecode_With_32_CodeSection_1536B_Each_All_Invalid_Parallel()
    {
        PEvmObjectFormat.IsValidEof(BytecodeWithAllInvalidSections, out _);
    }

    [Benchmark]
    public void Bytecode_With_32_CodeSection_1536B_Each_All_Invalid_Sequential()
    {
        SEvmObjectFormat.IsValidEof(BytecodeWithAllInvalidSections, out _);
    }

    [Benchmark]
    public void Bytecode_With_32_CodeSection_1536B_Each_Mixed_Parallel()
    {
        PEvmObjectFormat.IsValidEof(BytecodeWithAllMixedSections, out _);
    }

    [Benchmark]
    public void Bytecode_With_32_CodeSection_1536B_Each_Mixed_Sequential()
    {
        SEvmObjectFormat.IsValidEof(BytecodeWithAllMixedSections, out _);
    }
}