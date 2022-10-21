using System.Buffers.Text;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
public static class Engine
{
    private static readonly int seed = 23;
    private static readonly System.Random random = new System.Random(seed);
    public static string GenerateHash(int i) => System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(i.ToString()));
    public static Transaction GetTransaction(int idx)
        =>  new Transaction(GenerateHash(random.Next() + idx));
    public static byte[] GetBytesOfHash(string hash)
        => Encoding.UTF8.GetBytes(hash).Take(16).ToArray();

    public static Block GetBlock(uint i, byte[] parentHash, uint prevDifficulty)
    {
        var transactions = Enumerable.Range(0,10)
            .Select(GetTransaction)
            .Select(tr => tr.GetBytes())
            .ToArray();

        var TransactionsRootBytes = GetBytesOfHash(GenerateHash(transactions.GetHashCode()));

        var block = new Block
        {
            Number = i,
            ParentHash = parentHash,
            Difficulty = prevDifficulty + 1,
            Transactions = transactions,
            Timestamp = DateTime.Now.Ticks
        };
        block.Hash = GetBytesOfHash(GenerateHash(block.GetHashCode()));
        return block;
    }

    public static byte[] Serialize(Block block)
    {
        byte[] bytes = new byte[Block.MarshalledSize];
        var numberBytes = BitConverter.GetBytes(block.Number);
        var difficultyBytes = BitConverter.GetBytes(block.Difficulty);
        var timestampBytes = BitConverter.GetBytes(block.Timestamp);
        var hashBytes = block.Hash;
        var parentHashBytes = block.ParentHash;
        var transactionsBytes = block.ParentHash;
        int i = 0;
        Array.Copy(numberBytes, 0, bytes, i, numberBytes.Length);
        i += numberBytes.Length;
        Array.Copy(difficultyBytes, 0, bytes, i, difficultyBytes.Length);
        i += difficultyBytes.Length;
        Array.Copy(timestampBytes, 0, bytes, i, timestampBytes.Length);
        i += timestampBytes.Length;
        Array.Copy(hashBytes, 0, bytes, i, hashBytes.Length);
        i += hashBytes.Length;
        Array.Copy(parentHashBytes, 0, bytes, i, parentHashBytes.Length);
        i += parentHashBytes.Length;
        Array.Copy(transactionsBytes, 0, bytes, i, transactionsBytes.Length);
        return bytes;
    }

    public static Block? Deserialize(byte[] bytes)
    {
        int i = 0;
        var number = BitConverter.ToUInt32(bytes[i..(i + 4)]);
        i += 4;
        var difficulty = BitConverter.ToUInt32(bytes[i..(i + 4)]);
        i += 4;
        var timestamp = BitConverter.ToInt64(bytes[i..(i + 8)]);
        i += 8;
        var hash = bytes[i..(i + 16)];
        i += 16;
        var parentHash = bytes[i..(i + 16)];
        i += 16;
        byte[][] transactions = new byte[10][];
        for(int j = 0; j < 10; j++, i += 16)
        {
            transactions[j] = bytes[i..(i + 16)];
        }
        return new Block
        {
            Number = number,
            Difficulty = difficulty,
            Timestamp = timestamp,
            Hash = hash,
            ParentHash = parentHash,
            Transactions = transactions
        };
    }
}