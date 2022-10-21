using System.Collections.Generic;
using System.Runtime.InteropServices;
public record Block
{
    public uint Number { get; set; }
    public long Timestamp { get; set; }
    public uint Difficulty { get; set; }

    public byte[] Hash { get; set; }
    public byte[] ParentHash { get; set; }
    public byte[][] Transactions { get; set; }

    public static int MarshalledSize => sizeof(uint) + sizeof(long) + sizeof(int) + 16 + 16 + 16 * 10;
}

public struct Transaction
{
    public string Hash;
    public byte[] GetBytes() => System.Text.Encoding.UTF8.GetBytes(Hash).Take(16).ToArray();
    public Transaction(string hash)
    {
        Hash = hash;
    }
}
