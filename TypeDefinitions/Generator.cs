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
    public enum Scenario
    {
        Valid = 0,
        Invalid = 1,
    }

    public record TestCase(int Index)
    {
        public byte[] Bytecode;
    }

    public record FunctionCase(int InputCount, int OutputCount, int MaxStack, byte[] Body);
    public record ScenarioCase(FunctionCase[] functions, byte[] data)
    {
        public static byte[] CreateFromScenario(Scenario scenario)
        {
            byte[][] codeSegments = new[] {
                new byte[] {
                    (byte)Instruction.PUSH1,
                    0x01,
                },
                new byte[] {
                    (byte)Instruction.PUSH8,
                    0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01, 0x01
                },
                new byte[] {
                    (byte)Instruction.RJUMP,
                    0x00,0x00,
                },
                new byte[] {
                    (byte)Instruction.RJUMPI,
                    0x00,0x00
                },
                new byte[] {
                    (byte)Instruction.RJUMPV,
                    0x02,
                    0x00, 0x00,
                    0x00, 0x10
                }
            };
            List<byte> bytecode = new();
            int j = 0;
            for(int i = 0; i < 49000;j++) {
                int segment = i % codeSegments.Length;
                bytecode.AddRange(codeSegments[segment]);
                i += codeSegments[segment].Length;
            }
            
            if (scenario.HasFlag(Scenario.Invalid))
            {
                bytecode.Add((byte)Instruction.RJUMP);
                bytecode.Add(0x00);
                bytecode.Add(0x01);

            }
            bytecode.Add(0x00);
            return bytecode.ToArray();
        }

        public static byte[] GenerateFormatScenarios(byte[][] functions, byte[] data)
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

            var bytes = new byte[bytecodeHeaderLength + bytecodeBodyLength];

            (byte[] bytecode, int headerSize) InjectHeader(byte[] container)
            {
                (byte[], int) InjectMagicPrefix((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);

                    container[i++] = 0xEF;
                    container[i++] = 0x00; // correct magic
                    container[i++] = 0x01;
                    return (container, i);
                }

                (byte[], int) InjectHeaderSuffix((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);
                    container[i++] = 0x00;
                    return (container, i);
                }

                (byte[], int) InjectTypeSectionHeader((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);
                    byte[] typeSectionSize = (functions.Length * 4).ToByteArray();

                    container[i++] = 0x01;
                    container[i++] = typeSectionSize[^2];
                    container[i++] = typeSectionSize[^1];

                    return (container, i);
                }

                (byte[], int) InjectCodeSectionHeader((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);
                    byte[] functionsCount =  functions.Length.ToByteArray();

                    container[i++] = 0x02;
                    container[i++] = functionsCount[^2];
                    container[i++] = functionsCount[^1];
                    // set code sections sizes
                    for (int j = 0; j < functions.Length; j++)
                    {
                        var codeSectionCount = functions[j].Length.ToByteArray();
                        container[i++] = codeSectionCount[^2];
                        container[i++] = codeSectionCount[^1];
                    }
                    return (container, i);
                }

                (byte[], int) InjectdataSectionHeader((byte[], int) containerState)
                {
                    (byte[] container, int i) = (containerState);
                    byte[] dataSectionCount =  data.Length.ToByteArray();

                    container[i++] = 0x03;
                    container[i++] = dataSectionCount[^2];
                    container[i++] = dataSectionCount[^1];

                    return (container, i);
                }

                List<Func<(byte[], int), (byte[], int)>> headerInjectionSequence = new() {
                    InjectTypeSectionHeader,
                    InjectCodeSectionHeader,
                    InjectdataSectionHeader
                };

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
                        container[i++] = (byte)0;
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
                result = InjectCodeSections(result);
                (byte[] bytecode, int end) = InjectdataSection(result);
                return bytecode[..end];
            }

            (byte[] bytecode, int idx) = InjectHeader(bytes);
            bytecode = InjectBody(bytecode, idx);
            return bytecode;
        }
    }
}
