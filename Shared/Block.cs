using System.Collections.Generic;
using System.Runtime.InteropServices;

public record Report
{
    public record StageData
    {
        public SyncMode SyncMode { get; set; }
        public Stage Stage { get; set; }
        public long Date { get; set; }
        public int Index { get; set; }
        public uint Verfication { get; set; }
        public static int MarshalledSize => sizeof(SyncMode) + sizeof(Stage) + sizeof(long) + sizeof(int) + sizeof(uint);
    }
    public List<StageData> StageDataList { get; set; } = new();
    public int MarshalledSize => StageData.MarshalledSize * StageDataList.Count;
}

public enum Stage : byte
{
    Prefetch,
    Disconnected,
    Waiting,
    Verifying,
    Connected,
    Presync,
    Sync,
    Postsync,
    Saving,
}
public enum SyncMode : byte
{
    OldBlock, Producing
}

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
