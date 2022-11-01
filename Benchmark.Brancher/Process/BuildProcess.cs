using System.Diagnostics;

public static class DotnetProcess
{
    private static  void RunBuildWithCommands(string args) {
        var process = new Process();
        process.StartInfo.FileName = "dotnet";
        process.StartInfo.Arguments = args;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.Start();
        process.WaitForExit();
        string line;        
        while ((line = process.StandardOutput.ReadLine()) != null)
        {
            Console.WriteLine(line);
        }
        var error = process.StandardError.ReadToEnd();
        process.Close();
    }

    public static void Build(string path) {
        Console.WriteLine("Building... " + path);
        RunBuildWithCommands($"build {path} -c Release /p:DefineConstants=MONITOR");
    }
    public static void Run(string path) {
        Console.WriteLine("Running... " + path);
        RunBuildWithCommands($"run --no-build {path}");
    }
}