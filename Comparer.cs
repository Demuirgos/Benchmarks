

using BenchmarkDotNet.Attributes;
using Nethermind.Evm.EOF;
using static EofTestsBase;

[MemoryDiagnoser]
public class MyBenchmark
{
    public byte[] InvalidBytecode = ScenarioCase.CreateFromScenario(Scenario.Invalid);
    public byte[] ValidBytecode = ScenarioCase.CreateFromScenario(Scenario.Valid);
    EofHeader dummy = new EofHeader();

    [Benchmark]
    public bool Bitarray_Valid_Bytecode()
    {
        return BitArrayMethod.ValidateInstructions(ValidBytecode, dummy);
    }

    [Benchmark]
    public bool BinarySearch_Valid_Bytecode()
    {
        return BinarySearchMethod.ValidateInstructions(ValidBytecode, dummy);
    }
    
    [Benchmark]
    public bool  Bitmap_Valid_Bytecode()
    {
        return ByteArrayMethod.ValidateInstructions(ValidBytecode, dummy);
    }

    
    [Benchmark]
    public bool  NaiveWay_Valid_Bytecode()
    {
        return NaiveSearchMethod.ValidateInstructions(ValidBytecode, dummy);
    }

    [Benchmark]
    public bool  Bitarray_Invalid_Bytecode()
    {
        return BitArrayMethod.ValidateInstructions(InvalidBytecode, dummy);
    }


    [Benchmark]
    public bool BinarySearch_Invalid_Bytecode()
    {
        return BinarySearchMethod.ValidateInstructions(InvalidBytecode, dummy);
    }

    
    [Benchmark]
    public bool Bitmap_Invalid_Bytecode()
    {
        return ByteArrayMethod.ValidateInstructions(InvalidBytecode, dummy);
    }


    [Benchmark]
    public bool NaiveWay_Invalid_Bytecode()
    {
        return NaiveSearchMethod.ValidateInstructions(InvalidBytecode, dummy);
    }

}