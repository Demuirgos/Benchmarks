using System.IO;
using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Runtime.InteropServices;

public class Eater {
    public class ServerDisconnectedException : Exception { }
    public class MessageCorruptedException : Exception { }

    public static MemoryMappedFile mmFile;
    public static MemoryMappedViewStream mmStream;
    public static Report metadata = new ();
    
    public static Stage current = Stage.Disconnected; 
    public static Stage Current
    {
        get => current;
        set
        {
            if (current == value) return;
            var Previous = Current;
            current = value;
            Log(Previous, Current);
        }
    }

    private static List<Block> chain = new List<Block>();
    private static uint VerificationBlock = 0;
    public static bool Verify(uint i, CancellationToken token) {
        Current = Stage.Verifying;
        bool valid = true;
        foreach (var block in chain.Reverse<Block>())
        {
            if(token.IsCancellationRequested) return false;

            if (block.Number == VerificationBlock)
                break;
            var prevBlock = chain[(int)block.Number - 1];
            if(prevBlock.Hash != block.ParentHash
                || prevBlock.Difficulty + 1 != block.Difficulty
                || block.Hash != Engine.GetBytesOfHash(Engine.GenerateHash((block with { Hash = null }).GetHashCode()))) {
                valid = false;
            }
        }
        VerificationBlock = i;
        return true;
    }

    public static void Log(Stage previous, Stage current)
    {
        var stage = new Report.StageData
        {
            Stage = current,
            SyncMode = current == Stage.Postsync ? SyncMode.Producing : SyncMode.OldBlock,
            Date = DateTime.UtcNow.Ticks,
            Index = chain.Count,
            Verfication = VerificationBlock
        };
        metadata.StageDataList.Add(stage);
        var stageBytes = Engine.SerializeStage(stage);
        mmStream.Write(stageBytes, 0, Report.StageData.MarshalledSize);
    }
    public static async Task Sync(NamedPipeClientStream accessor, CancellationToken token) {
        var random = new System.Random(23);
        int i = 0;
        Current = Stage.Presync;
        accessor.WriteByte((byte)chain.Count);
        Current = Stage.Waiting;
        while (accessor.CanRead && Current.IsSyncing())
        {
            if(token.IsCancellationRequested)
            {
                return;
            }

            if (!accessor.IsConnected)
            {
                Current = Stage.Disconnected;
                throw new ServerDisconnectedException();
            }
            Current = Stage.Sync;
            var blockBytes = new byte[Block.MarshalledSize];
            accessor.Read(blockBytes, 0, Block.MarshalledSize);
            var block = Engine.Deserialize(blockBytes);
            if(block.Number == 0)
            {
                continue;
            }
            Console.WriteLine($"Receiving : {block.Number} {block.Hash.Select(b => (char)b).Aggregate(String.Empty, (acc, c) => $"{acc}{c}")}");
            await Task.Delay(100);
            chain.Add(block);
            if(random.Next(0, 100) > 10 && !Verify(block.Number, token))
            {
                throw new MessageCorruptedException();
            } 
        }
        Current = Stage.Postsync;
    }

    public static async Task Run(CancellationToken token) {
        if (token.IsCancellationRequested)
            return;

        try
        {
            using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "CHAIN_PIPE");
            pipeClient.Connect();
            Current = Stage.Connected;
            Current = Stage.Sync;
            await Sync(pipeClient, token);
        } catch(Exception e)
        {
            Console.WriteLine(e);
            Current = Stage.Disconnected;
            await Run(token);
        }
    }

    private static async Task Save()
    {
        Current = Stage.Saving;
        using var file = File.Create("TEMP", chain.Count * Block.MarshalledSize);
        file.Position = 0;
        foreach(var block in chain)
        {
            var sizeOfBlock = Engine.Serialize(block);
            await file.WriteAsync(sizeOfBlock, 0, sizeOfBlock.Length);
            Console.WriteLine($"Stored : {block.Number} {block.Hash.Select(b => (char)b).Aggregate(String.Empty, (acc, c) => $"{acc}{c}")}");
            await Task.Delay(100);
        }
    }
    private static async Task Load()
    {
        Current = Stage.Prefetch;
        try
        {
            using var file = File.OpenRead("TEMP");
            while (file.CanRead && file.Length > file.Position)
            {
                var blockBytes = new byte[Block.MarshalledSize];
                await file.ReadAsync(blockBytes);
                var block = Engine.Deserialize(blockBytes);
                chain.Add(block);
                Console.WriteLine($"Restored : {block.Number} {block.Hash.Select(b => (char)b).Aggregate(String.Empty, (acc, c) => $"{acc}{c}")}");
                await Task.Delay(100);
            }
        } catch(FileNotFoundException)
        {
            return;
        } catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }

    public static async Task DisposeAll()
    {
        await Save();
        mmFile.Dispose();
        mmStream.Close();
        mmStream.Dispose();
    }
    public static async Task Main(string[] args)
    {
        mmFile = MemoryMappedFile.CreateOrOpen("TEMPR", 1024 * 1024 * 1024);
        mmStream = mmFile.CreateViewStream();
        var tokenSrc = new CancellationTokenSource();
        Console.CancelKeyPress += async (src, args) => {
            if (Current != Stage.Saving && Current != Stage.Prefetch)
            {
                args.Cancel = true;
                tokenSrc.Cancel();
            };
        };
        await Load();
        await Run(tokenSrc.Token);
        await DisposeAll();
    }
}