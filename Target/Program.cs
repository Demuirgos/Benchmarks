using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Runtime.InteropServices;

public class Eater {
    
    public enum Stage {
        Disconnected,
        Connected,
        Presync,
        Sync,
        Postsync,
    }
    enum SyncMode
    {
        OldBlock, NewBlock, Producing
    }

    public static Stage Current { get; set; } = Stage.Disconnected;

    private static List<Block> chain = new List<Block>();
    private static long VerificationBlock = 0;
    public static bool Verify(long i) {
        foreach (var block in chain)
        {
            if(block.Number == 0) continue;
            if(block.Number == VerificationBlock) 
                return true;
            var prevBlock = chain[(int)block.Number - 1];
            if(prevBlock.Hash != block.ParentHash
                || prevBlock.Difficulty + 1 != block.Difficulty
                || block.Hash != Engine.GetBytesOfHash(Engine.GenerateHash((block with { Hash = null }).GetHashCode()))) {
                return false;
            }
        }
        VerificationBlock = i;
        return true;
    }

    public static async Task Sync(NamedPipeClientStream accessor) {
        var random = new System.Random(23);
        int i = 0;
        Current = Stage.Presync;
        while (accessor.CanRead)
        {
            if(!accessor.IsConnected)
            {
                throw new Exception("Connection Ended Ubruptly");
            }
            Current = Stage.Sync;
            var blockBytes = new byte[Block.MarshalledSize];
            accessor.Read(blockBytes, 0, Block.MarshalledSize);
            var block = Engine.Deserialize(blockBytes);
            if(block.Number == 0)
            {
                continue;
            }
            Console.WriteLine($"\tReceiving : {block.Number} {block.Hash.Select(b => (char)b).Aggregate(String.Empty, (acc, c) => $"{acc}{c}")}");
            await Task.Delay(20);
            chain.Add(block);
            if(random.Next(0, 100) > 50)
                {
                    Verify(block.Number);
            } 
        }
        Current = Stage.Postsync;
    }

    public static async Task Run() {
        try
        {
            using NamedPipeClientStream pipeClient = new NamedPipeClientStream(".", "CHAIN_PIPE");
            Console.WriteLine("Connecting ...");
            pipeClient.Connect();
            Console.WriteLine("Connected .");
            Console.WriteLine("Hooking ...");
            pipeClient.WriteByte((byte)chain.Count);
            Console.WriteLine("Hooked .");
            Console.WriteLine("Syncing ...");
            await Sync(pipeClient);
            Console.WriteLine("Completed .");
        } catch(Exception e)
        {
            Console.WriteLine(e.Message);
            Current = Stage.Disconnected;
        }
        await Run();
    }

    private static async Task Save()
    {
        Console.WriteLine("Saving...");
        using var file = File.Create("temp", chain.Count * Block.MarshalledSize);
        file.Position = 0;
        foreach(var block in chain)
        {
            var sizeOfBlock = Engine.Serialize(block);
            await file.WriteAsync(sizeOfBlock, 0, sizeOfBlock.Length);
            Console.WriteLine($"\tStored : {block.Number} {block.Hash.Select(b => (char)b).Aggregate(String.Empty, (acc, c) => $"{acc}{c}")}");
        }
        Console.WriteLine("Saved .");
    }
    private static async Task Load()
    {
        Console.WriteLine("Loading...");
        try
        {
            using var file = File.OpenRead("temp");
            while (file.CanRead && file.Length > file.Position)
            {
                var blockBytes = new byte[Block.MarshalledSize];
                await file.ReadAsync(blockBytes);
                var block = Engine.Deserialize(blockBytes);
                chain.Add(block);
                Console.WriteLine($"\tRestored : {block.Number} {block.Hash.Select(b => (char)b).Aggregate(String.Empty, (acc, c) => $"{acc}{c}")}");
                await Task.Delay(20);
            }
        } catch(FileNotFoundException)
        {
            Console.WriteLine("Loaded .");
            return;
        } catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        Console.WriteLine("Loaded .");
    }

    public static async Task Main(string[] args)
    {
        Console.CancelKeyPress += async (src, args) => {
            args.Cancel = true; 
            await Save();
            args.Cancel = false; 
        };
        await Load();
        await Run();
    }
}