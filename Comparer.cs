

using BenchmarkDotNet.Attributes;
using Nethermind.Evm.EOF;
using static EofTestsBase;

[MemoryDiagnoser]
public class MyBenchmark
{
    public byte[] ByteCodeWithEmptyCodeSection_0 => ScenarioCase.GenerateFormatScenarios(
        new byte[0][],
        new byte[0],
        FormatScenario.None
    ); 
    public byte[] ByteCodeWithEmptyCodeSection_1 => ScenarioCase.GenerateFormatScenarios(
        new byte[1][] { new byte[0] },
        new byte[0],
        FormatScenario.None
    ); 
    public byte[] ByteCodeWithEmptyCodeSection_100 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Repeat(new byte[0], 100).ToArray(),
        new byte[0],
        FormatScenario.None
    ); 
    public byte[] ByteCodeWithEmptyCodeSection_1024 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Repeat(new byte[0], 1024).ToArray(),
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithInvalidCodeSection_1 => ScenarioCase.GenerateFormatScenarios(
        new byte[1][] { ScenarioCase.CreateFromScenario(BodyScenario.UseDeprecatedOpcode)},
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithInvalidCodeSection_100 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Repeat(ScenarioCase.CreateFromScenario(BodyScenario.UseDeprecatedOpcode), 100).ToArray(),
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithInvalidCodeSection_1024 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Repeat(ScenarioCase.CreateFromScenario(BodyScenario.UseDeprecatedOpcode), 1024).ToArray(),
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithAllSectionsValid_1 => ScenarioCase.GenerateFormatScenarios(
        new byte[1][] { ScenarioCase.CreateFromScenario(BodyScenario.None)},
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithAllSectionsValid_100 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Repeat(ScenarioCase.CreateFromScenario(BodyScenario.None), 100).ToArray(),
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithAllSectionsValid_1024 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Repeat(ScenarioCase.CreateFromScenario(BodyScenario.None), 1024).ToArray(),
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithMixedSections_2 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Range(0, 2).Select(i => {
            if(i % 2 == 0)
                return ScenarioCase.CreateFromScenario(BodyScenario.None);
            else
                return ScenarioCase.CreateFromScenario(BodyScenario.UseDeprecatedOpcode);
        }).ToArray(),
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithMixedSections_100 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Range(0, 100).Select(i => {
            if(i % 2 == 0)
                return ScenarioCase.CreateFromScenario(BodyScenario.None);
            else
                return ScenarioCase.CreateFromScenario(BodyScenario.UseDeprecatedOpcode);
        }).ToArray(),
        new byte[0],
        FormatScenario.None
    );
    public byte[] ByteCodeWithMixedSections_1024 => ScenarioCase.GenerateFormatScenarios(
        Enumerable.Range(0, 1024).Select(i => {
            if(i % 2 == 0)
                return ScenarioCase.CreateFromScenario(BodyScenario.None);
            else
                return ScenarioCase.CreateFromScenario(BodyScenario.UseDeprecatedOpcode);
        }).ToArray(),
        new byte[0],
        FormatScenario.None
    );

    [Benchmark]
    public void ByteCodeWithEmptyCodeSection_0_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithEmptyCodeSection_0, out _);
    }

    [Benchmark]
    public void ByteCodeWithEmptyCodeSection_0_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithEmptyCodeSection_0, out _);
    }

    [Benchmark]
    public void ByteCodeWithEmptyCodeSection_1_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithEmptyCodeSection_1, out _);
    }

    [Benchmark]
    public void ByteCodeWithEmptyCodeSection_1_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithEmptyCodeSection_1, out _);
    }

    [Benchmark]
    [ArgumentsSource(nameof(ByteCodeWithEmptyCodeSection_100))]
    public void ByteCodeWithEmptyCodeSection_100_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithEmptyCodeSection_100, out _);
    }

    [Benchmark]
    public void ByteCodeWithEmptyCodeSection_100_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithEmptyCodeSection_100, out _);
    }

    [Benchmark]
    public void ByteCodeWithEmptyCodeSection_1024_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithEmptyCodeSection_1024, out _);
    }

    [Benchmark]
    public void ByteCodeWithEmptyCodeSection_1024_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithEmptyCodeSection_1024, out _);
    }

    [Benchmark]
    public void ByteCodeWithInvalidCodeSection_1_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithInvalidCodeSection_1, out _);
    }

    [Benchmark]
    public void ByteCodeWithInvalidCodeSection_1_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithInvalidCodeSection_1, out _);
    }

    [Benchmark]
    public void ByteCodeWithInvalidCodeSection_100_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithInvalidCodeSection_100, out _);
    }

    [Benchmark]
    public void ByteCodeWithInvalidCodeSection_100_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithInvalidCodeSection_100, out _);
    }

    [Benchmark]
    public void ByteCodeWithInvalidCodeSection_1024_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithInvalidCodeSection_1024, out _);
    }

    [Benchmark]
    public void ByteCodeWithInvalidCodeSection_1024_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithInvalidCodeSection_1024, out _);
    }

    [Benchmark]
    public void ByteCodeWithAllSectionsValid_1_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithAllSectionsValid_1, out _);
    }

    [Benchmark]
    public void ByteCodeWithAllSectionsValid_1_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithAllSectionsValid_1, out _);
    }

    [Benchmark]
    public void ByteCodeWithAllSectionsValid_100_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithAllSectionsValid_100, out _);
    }

    [Benchmark]
    public void ByteCodeWithAllSectionsValid_100_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithAllSectionsValid_100, out _);
    }

    [Benchmark]
    public void ByteCodeWithAllSectionsValid_1024_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithAllSectionsValid_1024, out _);
    }

    [Benchmark]
    public void ByteCodeWithAllSectionsValid_1024_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithAllSectionsValid_1024, out _);
    }

    [Benchmark]
    public void ByteCodeWithMixedSections_2_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithMixedSections_2, out _);
    }

    [Benchmark]
    public void ByteCodeWithMixedSections_2_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithMixedSections_2, out _);
    }

    [Benchmark]
    public void ByteCodeWithMixedSections_100_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithMixedSections_100, out _);
    }

    [Benchmark]
    public void ByteCodeWithMixedSections_100_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithMixedSections_100, out _);
    }

    [Benchmark]
    public void ByteCodeWithMixedSections_1024_Parallel()
    {
        PEvmObjectFormat.IsValidEof(ByteCodeWithMixedSections_1024, out _);
    }

    [Benchmark]
    public void ByteCodeWithMixedSections_1024_Sequential()
    {
        SEvmObjectFormat.IsValidEof(ByteCodeWithMixedSections_1024, out _);
    }
}