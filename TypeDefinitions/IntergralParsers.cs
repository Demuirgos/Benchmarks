using System.Buffers.Binary;
using System.Collections;
using System.Numerics;

public static class Extenstions {
    public static void SetBits(this BitArray thisArr, bool value, params int[] positions)
    {
        foreach (int pos in positions)
        {
            thisArr.Set(pos, value);
        }
    }

    public static void SetBits(this BitArray thisArr, bool value, Range range)
    {
        int rangeMasked = (1 << range.End.Value) - (1 << range.Start.Value);
        byte[] rangeMaskedArr = rangeMasked.ToBigEndianByteArray();
        Array.Resize(ref rangeMaskedArr, thisArr.Count / 8);
        BitArray arr = new BitArray(rangeMaskedArr);
        thisArr.Or(arr);
    }
    public static bool Includes(this Range @this, int value)
            => value >= @this.Start.Value && value <= @this.End.Value;

    public static bool Includes(this Range @this, int value, int len)
    {
        var (offset, length) = @this.GetOffsetAndLength(len);
        return value >= offset && value < length + offset;
    }
    public static string ToHexString(this byte[] ba)
    {
    return BitConverter.ToString(ba).Replace("-","");
    }
    public static short ReadEthInt16(this Span<byte> bytes)
    {
        return ReadEthInt16((ReadOnlySpan<byte>)bytes);
    }

    public static short ReadEthInt16(this ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length > 2)
        {
            bytes = bytes.Slice(bytes.Length - 2, 2);
        }

        return bytes.Length switch
        {
            2 => BinaryPrimitives.ReadInt16BigEndian(bytes),
            1 => bytes[0],
            _ => 0
        };
    }
    public static byte[] ToByteArray(this int value)
    {
        byte[] bytes = new byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(bytes, value);
        return bytes;
    }

    public static byte[] ToBigEndianByteArray(this int value)
    {
        byte[] bytes = BitConverter.GetBytes(value);
        if (BitConverter.IsLittleEndian)
        {
            Array.Reverse(bytes);
        }

        return bytes;
    }
    public static ushort ReadEthUInt16(this Span<byte> bytes)
    {
        return ReadEthUInt16((ReadOnlySpan<byte>)bytes);
    }
    public static ushort ReadEthUInt16(this ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length > 2)
        {
            bytes = bytes.Slice(bytes.Length - 2, 2);
        }

        return bytes.Length switch
        {
            2 => BinaryPrimitives.ReadUInt16BigEndian(bytes),
            1 => bytes[0],
            _ => 0
        };
        }
}