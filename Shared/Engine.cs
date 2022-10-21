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
    public static byte[] SerializeStage(Report.StageData data)
    {
        byte[] bytes = new byte[18];
        bytes[0] = (byte)data.SyncMode;
        bytes[1] = (byte)data.Stage;
        int i = 2;
        var dateBytes = BitConverter.GetBytes(data.Date);
        Array.Copy(dateBytes, 0, bytes, i, dateBytes.Length);
        i += dateBytes.Length;
        var indexBytes = BitConverter.GetBytes(data.Index);
        Array.Copy(indexBytes, 0, bytes, i, indexBytes.Length);
        i += indexBytes.Length;
        var verificationBytes = BitConverter.GetBytes(data.Verfication);
        Array.Copy(verificationBytes, 0, bytes, i, verificationBytes.Length);
        return bytes;
    }
    public static byte[] SerializeReport(Report report)
    {
        return report.StageDataList
            .Select(SerializeStage)
            .Aggregate(new List<byte>(), (acc, reportBytes) => { acc.AddRange(reportBytes); return acc; })
            .ToArray();
    }

    public static Report.StageData DeserializeStage(byte[] data)
    {
        return new Report.StageData
        {
            SyncMode = (SyncMode)data[0],
            Stage = (Stage)data[1],
            Date = BitConverter.ToInt64(data[2..10]),
            Index = BitConverter.ToInt32(data[10..14]),
            Verfication = BitConverter.ToUInt32(data[14..18]),
        };
    }
    public static Report DeserializeReport(byte[] data)
    {
        return new Report
        {
            StageDataList = data.Chunk(18)
                    .Select(DeserializeStage)
                    .ToList()
        };
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

    public static bool IsSyncing(this Stage stage) => stage == Stage.Verifying || stage == Stage.Sync || stage == Stage.Waiting;
}