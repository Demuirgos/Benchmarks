using System.IO.MemoryMappedFiles;

public class Worker
{
    public static MemoryMappedFile mmFile;
    public static MemoryMappedViewStream mmStream;
    public static async Task Main(string[] _)
    {
        try
        {
            mmFile = MemoryMappedFile.OpenExisting("TEMPR");
            mmStream = mmFile.CreateViewStream();
            while (mmStream.CanRead)
            {
                var stageBytes = new byte[Report.StageData.MarshalledSize];
                mmStream.Read(stageBytes, 0, stageBytes.Length);
                var StageData = Engine.DeserializeStage(stageBytes);
                Console.WriteLine($"Worker running at {DateTimeOffset.Now}: CurrentStage = {StageData.Stage}, SyncMode = {StageData.SyncMode}, CurrentIndex = {StageData.Index}, VerificationPivot = {StageData.Verfication}");
                await Task.Delay(1000);
            }
        }
        catch {
            return;
        }
        finally
        {
            mmFile?.Dispose();
            mmStream?.Close();
            mmStream?.Dispose();
        }
        
    }
}
