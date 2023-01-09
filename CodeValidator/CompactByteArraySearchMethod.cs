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

internal static class ByteArrayMethod
{
    public const byte VERSION = 0x01;
    internal const byte DYNAMIC_OFFSET = 0; // to mark dynamic offset needs to be added
    internal const byte TWO_BYTE_LENGTH = 2;// indicates the number of bytes to skip for immediates
    internal const byte ONE_BYTE_LENGTH = 1; // indicates the length of the count immediate of jumpv
    internal const byte MINIMUMS_ACCEPTABLE_JUMPT_JUMPTABLE_LENGTH = 1; // indicates the length of the count immediate of jumpv

    public static bool ValidateInstructions(ReadOnlySpan<byte> code, in EofHeader header)
    {
        int pos;
        byte[] codeBitmap = new byte[(code.Length / 8) + 1 + 4];
        SortedSet<int> jumpdests = new();

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

                var offset = code.Slice(pos, TWO_BYTE_LENGTH).ReadEthInt16();
                BitmapHelperOld.HandleNumbits(TWO_BYTE_LENGTH, ref codeBitmap, ref postInstructionByte);
                var rjumpdest = offset + TWO_BYTE_LENGTH + pos;
                jumpdests.Add(rjumpdest);

                if (rjumpdest < 0 || rjumpdest >= code.Length)
                {
                    return false;
                }
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
                BitmapHelperOld.HandleNumbits(immediateValueSize, ref codeBitmap, ref postInstructionByte);

                for (int j = 0; j < count; j++)
                {
                    var offset = code.Slice(pos + ONE_BYTE_LENGTH + j * TWO_BYTE_LENGTH, TWO_BYTE_LENGTH).ReadEthInt16();
                    var rjumpdest = offset + immediateValueSize + pos;
                    jumpdests.Add(rjumpdest);
                    if (rjumpdest < 0 || rjumpdest >= code.Length)
                    {
                        return false;
                    }
                }
            }

            if (opcode is >= Instruction.PUSH1 and <= Instruction.PUSH32)
            {
                int len = code[postInstructionByte - 1] - (int)Instruction.PUSH1 + 1;
                BitmapHelperOld.HandleNumbits(len, ref codeBitmap, ref postInstructionByte);
            }
            pos = postInstructionByte;
        }

        if (pos > code.Length)
        {
            return false;
        }

        foreach (int jumpdest in jumpdests)
        {
            if (BitmapHelperOld.IsCodeSegment(codeBitmap, jumpdest))
            {
                return false;
            }
        }
        return true;
    }
}
