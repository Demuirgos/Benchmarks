// SPDX-FileCopyrightText: 2022 Demerzel Solutions Limited
// SPDX-License-Identifier: LGPL-3.0-only

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Nethermind.Evm.EOF;

internal static class NaiveSearchMethod
{
    public const byte VERSION = 0x01;
    internal const byte DYNAMIC_OFFSET = 0; // to mark dynamic offset needs to be added
    internal const byte TWO_BYTE_LENGTH = 2;// indicates the number of bytes to skip for immediates
    internal const byte ONE_BYTE_LENGTH = 1; // indicates the length of the count immediate of jumpv
    internal const byte MINIMUMS_ACCEPTABLE_JUMPT_JUMPTABLE_LENGTH = 1; // indicates the length of the count immediate of jumpv

    public static bool ValidateInstructions(ReadOnlySpan<byte> code, in EofHeader header)
    {
        int pos;
        List<Range> immediates = new();
        SortedSet<int> rjumpdests = new();

        for (pos = 0; pos < code.Length; pos++)
        {
            Instruction opcode = (Instruction)code[pos];
            int postInstructionByte = pos + 1;

            if (!opcode.IsValid(IsEofContext: true))
            {
                return false;
            }

            if (opcode is Instruction.RJUMP or Instruction.RJUMPI)
            {
                if (postInstructionByte + TWO_BYTE_LENGTH > code.Length)
                {
                    return false;
                }

                var offset = code.Slice(postInstructionByte, TWO_BYTE_LENGTH).ReadEthInt16();
                immediates.Add(new Range(postInstructionByte, postInstructionByte + 1));
                var rjumpdest = offset + TWO_BYTE_LENGTH + postInstructionByte;
                rjumpdests.Add(rjumpdest);
                if (rjumpdest < 0 || rjumpdest >= code.Length)
                {
                    return false;
                }
                postInstructionByte += TWO_BYTE_LENGTH;
            }

            if (opcode is Instruction.RJUMPV)
            {
                if (postInstructionByte + TWO_BYTE_LENGTH > code.Length)
                {
                    return false;
                }

                byte count = code[postInstructionByte];
                if (count < MINIMUMS_ACCEPTABLE_JUMPT_JUMPTABLE_LENGTH)
                {
                    return false;
                }

                if (postInstructionByte + ONE_BYTE_LENGTH + count * TWO_BYTE_LENGTH > code.Length)
                {
                    return false;
                }

                var immediateValueSize = ONE_BYTE_LENGTH + count * TWO_BYTE_LENGTH;
                immediates.Add(new Range(postInstructionByte, postInstructionByte + immediateValueSize - 1));
                for (int j = 0; j < count; j++)
                {
                    var offset = code.Slice(postInstructionByte + ONE_BYTE_LENGTH + j * TWO_BYTE_LENGTH, TWO_BYTE_LENGTH).ReadEthInt16();
                    var rjumpdest = offset + immediateValueSize + postInstructionByte;
                    rjumpdests.Add(rjumpdest);

                    if (rjumpdest < 0 || rjumpdest >= code.Length)
                    {
                        return false;
                    }
                }
                postInstructionByte += immediateValueSize;
            }

            if (opcode is >= Instruction.PUSH1 and <= Instruction.PUSH32)
            {
                int len = opcode - Instruction.PUSH1 + 1;
                immediates.Add(new Range(postInstructionByte, postInstructionByte + len - 1));
                postInstructionByte += len;
            }
            pos = postInstructionByte;
        }

        if (pos > code.Length)
        {
            return false;
        }

        foreach (int rjumpdest in rjumpdests)
        {
            foreach (var range in immediates)
            {
                if (range.Includes(rjumpdest))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
