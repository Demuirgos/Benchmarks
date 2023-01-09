// SPDX-FileCopyrightText: 2022 Demerzel Solutions Limited
// SPDX-License-Identifier: LGPL-3.0-only

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Nethermind.Evm.EOF;

internal static class BitArrayMethod
{
    public const byte VERSION = 0x01;
    internal const byte DYNAMIC_OFFSET = 0; // to mark dynamic offset needs to be added
    internal const byte TWO_BYTE_LENGTH = 2;// indicates the number of bytes to skip for immediates
    internal const byte ONE_BYTE_LENGTH = 1; // indicates the length of the count immediate of jumpv
    internal const byte MINIMUMS_ACCEPTABLE_JUMPT_JUMPTABLE_LENGTH = 1; // indicates the length of the count immediate of jumpv

    public static bool ValidateInstructions(ReadOnlySpan<byte> code, in EofHeader header)
    {
        int pos;
        int nearestPowerOf8 = code.Length / 8 + 1;
        BitArray immediatesMask = new BitArray(nearestPowerOf8 * 8);
        BitArray rjumpdestsMask = new BitArray(nearestPowerOf8 * 8);

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

                var offset = code.Slice(pos, TWO_BYTE_LENGTH).ReadEthUInt16();
                immediatesMask.SetBits(true, pos, pos + 1);
                var rjumpdest = offset + TWO_BYTE_LENGTH + pos;
                rjumpdestsMask.Set(rjumpdest, true);

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
                immediatesMask.SetBits(true, pos..(pos + immediateValueSize - 1));

                for (int j = 0; j < count; j++)
                {
                    var offset = code.Slice(pos + ONE_BYTE_LENGTH + j * TWO_BYTE_LENGTH, TWO_BYTE_LENGTH).ReadEthInt16();
                    var rjumpdest = offset + immediateValueSize + pos;
                    rjumpdestsMask.Set(rjumpdest, true);
                    if (rjumpdest < 0 || rjumpdest >= code.Length)
                    {
                        return false;
                    }
                }
                postInstructionByte += immediateValueSize;
            }

            if (opcode is >= Instruction.PUSH1 and <= Instruction.PUSH32)
            {
                int len = code[postInstructionByte - 1] - (int)Instruction.PUSH1 + 1;
                immediatesMask.SetBits(true, postInstructionByte..(postInstructionByte + len - 1));
                postInstructionByte += len;
            }
            pos = postInstructionByte;
        }

        if (pos > code.Length)
        {
            return false;
        }

        BitArray result = rjumpdestsMask.Or(immediatesMask);
        for (int i = 0; i < result.Length; i++)
        {
            if (result.Get(i))
            {
                return false;
            }
        }
        return true;
    }
}
