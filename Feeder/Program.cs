using System.IO.MemoryMappedFiles;
using System.IO.Pipes;
using System.Runtime.InteropServices;

public class Feeder {
    public static uint BlockSyncIndex = 0;
    public static uint BlockSyncDepth = 0;
   
    public static IEnumerable<Block> Generate(uint start = 0, uint depth = 0) {
        var prevBlock = Engine.GetBlock(start, null, 0);
        while(depth == 0 || (depth-- > 0))
        {
            prevBlock = Engine.GetBlock(prevBlock.Number + 1, prevBlock.Hash, prevBlock.Difficulty);
            yield return prevBlock;

            if (prevBlock.Number > 254) break; 
        }
    }

    public static async Task BroadcastBlock(Stream writer, Block block) {
        var sizeOfBlock = Engine.Serialize(block);
        await Task.Run(() => writer.Write(sizeOfBlock, 0, sizeOfBlock.Length));
        BlockSyncIndex++;
    }

    public static async Task Relay(Stream writer) {
        foreach (var block in Generate(BlockSyncIndex, BlockSyncDepth))
        {
            Console.WriteLine($"\tBroadcasting : {block.Number} {block.Hash.Select(b => (char)b).Aggregate(String.Empty ,(acc, c) => $"{acc}{c}")}");
            await BroadcastBlock(writer, block);
            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }
    public static async Task Run() {
        try
        {
            using NamedPipeServerStream pipeServer = new NamedPipeServerStream("CHAIN_PIPE", PipeDirection.InOut, 1);
            Console.WriteLine("Connecting ...");
            pipeServer.WaitForConnection();
            Console.WriteLine("Connected .");
            Console.WriteLine("Hooking ...");
            BlockSyncIndex = (uint)pipeServer.ReadByte();
            Console.WriteLine("Hooked .");
            Console.WriteLine("Broadcasting ...");
            await Relay(pipeServer);
            Console.WriteLine("Completed .");
        } catch
        {
        }
        await Run();
    }
    public static async Task Main(string[] args) => await Run();
}