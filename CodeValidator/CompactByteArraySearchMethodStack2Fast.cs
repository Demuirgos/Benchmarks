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

internal static class TwoByteArrayStackMethodFast
{
    public const byte VERSION = 0x01;
    internal const byte DYNAMIC_OFFSET = 0; // to mark dynamic offset needs to be added
    internal const byte TWO_BYTE_LENGTH = 2;// indicates the number of bytes to skip for immediates
    internal const byte ONE_BYTE_LENGTH = 1; // indicates the length of the count immediate of jumpv
    internal const byte MINIMUMS_ACCEPTABLE_JUMPT_JUMPTABLE_LENGTH = 1; // indicates the length of the count immediate of jumpv

    public static bool ValidateInstructions(ReadOnlySpan<byte> code, in EofHeader header)
    {
        int pos;
        Span<byte> codeBitmap = stackalloc byte[(code.Length / 8) + 1 + 4];
        Span<byte> jumpBitmap = stackalloc byte[(code.Length / 8) + 1 + 4];

        for (pos = 0; pos < code.Length; )
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
                var rjumpdest = offset + TWO_BYTE_LENGTH + postInstructionByte;
                if (rjumpdest < 0 || rjumpdest >= code.Length)
                {
                    return false;
                }
                BitmapHelper.HandleNumbits(TWO_BYTE_LENGTH, ref codeBitmap, ref postInstructionByte);
                BitmapHelper.HandleNumbits(ONE_BYTE_LENGTH, ref jumpBitmap, ref rjumpdest);
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

                for (int j = 0; j < count; j++)
                {
                    var offset = code.Slice(postInstructionByte + ONE_BYTE_LENGTH + j * TWO_BYTE_LENGTH, TWO_BYTE_LENGTH).ReadEthInt16();
                    var rjumpdest = offset + immediateValueSize + postInstructionByte;
                    if (rjumpdest < 0 || rjumpdest >= code.Length)
                    {
                        return false;
                    }
                    BitmapHelper.HandleNumbits(ONE_BYTE_LENGTH, ref jumpBitmap, ref rjumpdest);
                }
                BitmapHelper.HandleNumbits(immediateValueSize, ref codeBitmap, ref postInstructionByte);
            }

            if (opcode is >= Instruction.PUSH1 and <= Instruction.PUSH32)
            {
                int len = opcode - Instruction.PUSH1 + 1;
                BitmapHelper.HandleNumbits(len, ref codeBitmap, ref postInstructionByte);
            }
            pos = postInstructionByte;
        }

        if (pos > code.Length)
        {
            return false;
        }

        if(codeBitmap.CheckCollisionFast(jumpBitmap)) {
                return false;
        }
        return true;
    }
}
