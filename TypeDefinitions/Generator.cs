// SPDX-FileCopyrightText: 2022 Demerzel Solutions Limited
// SPDX-License-Identifier: LGPL-3.0-only

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nethermind.Evm;

public static class EofTestsBase 
{
    [Flags]
    public enum FormatScenario
    {
        None = 0,

        OmitTypeSectionHeader = 1 << 1,
        OmitCodeSectionsHeader = 1 << 2,
        OmitdataSectionHeader = 1 << 3,

        IncompleteTypeSectionHeader = 1 << 4,
        IncompleteCodeSectionsHeader = 1 << 5,
        IncompletedataSectionHeader = 1 << 6,

        IncorrectTypeSectionHeader = 1 << 7,
        IncorrectCodeSectionsHeader = 1 << 8,
        IncorrectdataSectionHeader = 1 << 9,

        MultipleTypeSectionHeaders = 1 << 10,
        MultipleCodeSectionsHeaders = 1 << 11,
        MultipledataSectionHeaders = 1 << 12,

        MisplaceSectionHeaders = 1 << 13,

        IncompleteBody = 1 << 14,

        InvalidVersion = 1 << 15,
        MissingTerminator = 1 << 16,
        IncorrectSectionKind = 1 << 17,
        InvalidMagic = 1 << 18,

        TrailingBytes = 1 << 19,
    }

    [Flags]
    public enum BodyScenario
    {
        None = 0,
        UseUndefinedOpcode = 1 << 0,
        UseDeprecatedOpcode = 1 << 1,
        EndWithTruncatedPush = 1 << 2,
        WithEmptyCodeSection = 1 << 3,

        WithdataSection = 1 << 4,
    }

    [Flags]
    public enum DeploymentScenario
    {
        EofInitCode = 1 << 1,
        EofDeployedCode = 1 << 2,
        EofContainer = 1 << 3,

        CorruptInitCode = 1 << 4,
        CorruptDeployedCode = 1 << 5,
        CorruptContainer = 1 << 6,

        ContainerInitcodeVersionMismatch = EofInitCode | EofContainer | 1 << 7,
        InitcodeDeploycodeVersionMismatch = EofInitCode | EofDeployedCode | 1 << 8,
    }

    [Flags]
    public enum DeploymentContext
    {
        UseCreate = 1 << 1,
        UseCreate2 = 1 << 2,
        UseCreateTx = 1 << 3,
    }

    public record TestCase(int Index)
    {
        public byte[] Bytecode;
    }

    public record FunctionCase(int InputCount, int OutputCount, int MaxStack, byte[] Body);
    public record ScenarioCase(FunctionCase[] functions, byte[] data)
    {
        public static byte[] CreateFromScenario(BodyScenario scenario)
        {
            List<byte> bytecode = new();
            int opcodeCount = 0;
            if (!scenario.HasFlag(BodyScenario.WithEmptyCodeSection))
            {
                if (scenario.HasFlag(BodyScenario.UseDeprecatedOpcode))
                {
                    bytecode.Add((byte)Instruction.CALLCODE);
                    opcodeCount += 2;

                }

                if (scenario.HasFlag(BodyScenario.UseUndefinedOpcode))
                {
                    byte opcode = 0x00;
                    while (Enum.IsDefined(typeof(Instruction), opcode))
                    {
                        opcode++;
                    }
                    bytecode.Add((byte)opcode);
                    opcodeCount += 1;
                }

                if (scenario.HasFlag(BodyScenario.EndWithTruncatedPush))
                {
                    bytecode.Add((byte)Instruction.PUSH2);
                    bytecode.Add(0x01);
                }
                else
                {
                    bytecode.Add((byte)Instruction.PUSH1);
                    bytecode.Add(0x01);
                }
            }
            return bytecode.ToArray();
        }

        public static byte[] GenerateFormatScenarios(byte[][] functions, byte[] data, FormatScenario scenarios)
        {
            int eofPrefixSize =
                2 + 1 /*EF00 + version */;
            int typeSectionHeaderSize =
                1 + 2 /*Type section kind+header*/;
            int codeSectionHeaderSize =
                1 + 2 + 2 * functions.Length /*Code section kind+count+header*/;
            int dataSectionHeaderSize =
                1 + 2 /*Type section kind+header*/;
            int terminatorSectionSize =
                1 /*terminator*/;

            int bytecodeHeaderLength =
                eofPrefixSize +
                typeSectionHeaderSize +
                codeSectionHeaderSize +
                dataSectionHeaderSize +
                terminatorSectionSize;

            int bytecodeBodyLength =
                4 * functions.Length /*typesection */ +
                functions.Sum(s => s.Length) /*codesection*/ +
                data.Length /*terminator*/;

            int garbage = scenarios.HasFlag(FormatScenario.TrailingBytes) ? 1 : 0;

            var bytes = new byte[bytecodeHeaderLength + bytecodeBodyLength + garbage];

            (byte[] bytecode, int headerSize) InjectHeader(byte[] container)
            {
                (byte[], int) InjectMagicPrefix((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);

                    if (scenarios.HasFlag(FormatScenario.InvalidMagic))
                    {
                        container[i++] = 0xEF;
                        container[i++] = 0xFF; // incorrect magic
                    }
                    else
                    {
                        container[i++] = 0xEF;
                        container[i++] = 0x00; // correct magic
                    }

                    // set version
                    container[i++] = scenarios.HasFlag(FormatScenario.InvalidVersion) ? (byte)0x00 : (byte)0x01;
                    return (container, i);
                }

                (byte[], int) InjectHeaderSuffix((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);

                    if (scenarios.HasFlag(FormatScenario.IncorrectSectionKind))
                    {
                        container[i++] = 0x09;
                    }
                    else if (scenarios.HasFlag(FormatScenario.MissingTerminator))
                    {
                        return containerState;
                    }
                    else
                    {
                        container[i++] = 0x00;
                    }

                    return (container, i);
                }

                (byte[], int) InjectTypeSectionHeader((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);
                    byte[] typeSectionSize = // set typesection size
                        scenarios.HasFlag(FormatScenario.IncorrectTypeSectionHeader)
                            ? 0.ToByteArray()
                            : (functions.Length * 4).ToByteArray();

                    container[i++] = 0x01;
                    if (!scenarios.HasFlag(FormatScenario.IncompleteTypeSectionHeader))
                    {
                        container[i++] = typeSectionSize[^2];
                        container[i++] = typeSectionSize[^1];
                    }

                    return (container, i);
                }

                (byte[], int) InjectCodeSectionHeader((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);
                    byte[] functionsCount = scenarios.HasFlag(FormatScenario.IncorrectCodeSectionsHeader)
                        ? 0.ToByteArray()
                        : functions.Length.ToByteArray();

                    container[i++] = 0x02;
                    if (!scenarios.HasFlag(FormatScenario.IncompleteCodeSectionsHeader))
                    {
                        container[i++] = functionsCount[^2];
                        container[i++] = functionsCount[^1];
                        // set code sections sizes
                        for (int j = 0; j < functions.Length; j++)
                        {
                            var codeSectionCount = functions[j].Length.ToByteArray();
                            container[i++] = codeSectionCount[^2];
                            container[i++] = codeSectionCount[^1];
                        }
                    }

                    return (container, i);
                }

                (byte[], int) InjectdataSectionHeader((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);
                    byte[] dataSectionCount = scenarios.HasFlag(FormatScenario.IncorrectdataSectionHeader)
                        ? (data.Length - 1).ToByteArray()
                        : data.Length.ToByteArray();

                    container[i++] = 0x03;
                    if (!scenarios.HasFlag(FormatScenario.IncompletedataSectionHeader))
                    {
                        container[i++] = dataSectionCount[^2];
                        container[i++] = dataSectionCount[^1];
                    }

                    return (container, i);
                }

                List<Func<(byte[], int), (byte[], int)>> headerInjectionSequence = new() {
                    InjectTypeSectionHeader,
                    InjectCodeSectionHeader,
                    InjectdataSectionHeader
                };

                if (scenarios.HasFlag(FormatScenario.MisplaceSectionHeaders))
                {
                    headerInjectionSequence.Reverse();
                }
                else
                {
                    // Note(Ayman) :  Add Multiple section cases, handle dynamic bytecode size
                    if (scenarios.HasFlag(FormatScenario.OmitTypeSectionHeader))
                    {
                        headerInjectionSequence.RemoveAt(0);
                    }
                    else if (scenarios.HasFlag(FormatScenario.MultipleTypeSectionHeaders))
                    {
                        headerInjectionSequence.Insert(0, InjectTypeSectionHeader);
                        Array.Resize(ref container, container.Length + typeSectionHeaderSize);
                    }

                    if (scenarios.HasFlag(FormatScenario.OmitCodeSectionsHeader))
                    {
                        headerInjectionSequence.RemoveAt(1);
                    }
                    else if (scenarios.HasFlag(FormatScenario.MultipleCodeSectionsHeaders))
                    {
                        headerInjectionSequence.Insert(headerInjectionSequence.Count - 1, InjectCodeSectionHeader);
                        Array.Resize(ref container, container.Length + codeSectionHeaderSize);
                    }

                    if (scenarios.HasFlag(FormatScenario.OmitdataSectionHeader))
                    {
                        headerInjectionSequence.RemoveAt(headerInjectionSequence.Count - 1);
                    }
                    else if (scenarios.HasFlag(FormatScenario.MultipledataSectionHeaders))
                    {
                        headerInjectionSequence.Insert(headerInjectionSequence.Count - 1, InjectdataSectionHeader);
                        Array.Resize(ref container, container.Length + dataSectionHeaderSize);
                    }
                }

                (byte[], int) result = InjectMagicPrefix((container, 0));
                foreach (var transformation in headerInjectionSequence)
                {
                    result = transformation(result);
                }

                return InjectHeaderSuffix(result);
            }

            byte[] InjectBody(byte[] container, int headerSize)
            {
                (byte[], int) InjectTypeSection((byte[], int) containerState)
                {
                    var (container, i) = (containerState);
                    foreach (var functionDef in functions)
                    {
                        var MaxStackHeightBytes = 2.ToByteArray();
                        container[i++] = (byte)2;
                        container[i++] = (byte)0;
                        container[i++] = MaxStackHeightBytes[^2];
                        container[i++] = MaxStackHeightBytes[^1];
                    }

                    return (container, i);
                }

                (byte[], int) InjectCodeSections((byte[], int) containerState)
                {
                    var (destinationArray, i) = (containerState);
                    foreach (var section in functions)
                    {
                        Array.Copy(section, 0, destinationArray, i, section.Length);
                        i += section.Length;
                    }

                    return (destinationArray, i);
                }

                (byte[], int) InjectdataSection((byte[], int) containerState)
                {
                    var (destinationArray, i) = (containerState);
                    Array.Copy(data, 0, destinationArray, i, data.Length);
                    i += data.Length;
                    return (destinationArray, i);
                }

                (byte[], int) result = InjectTypeSection((container, headerSize));
                if (!scenarios.HasFlag(FormatScenario.IncompleteBody))
                {
                    result = InjectCodeSections(result);
                }
                (byte[] bytecode, int end) = InjectdataSection(result);
                return bytecode[..end];
            }

            (byte[] bytecode, int idx) = InjectHeader(bytes);
            bytecode = InjectBody(bytecode, idx);
            if (scenarios.HasFlag(FormatScenario.TrailingBytes))
            {
                Array.Resize(ref bytecode, bytecode.Length + 1);
            }

            return bytecode;
        }
    }
}
