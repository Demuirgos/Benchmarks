using System.Buffers.Binary;

public static class Extenstions {
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