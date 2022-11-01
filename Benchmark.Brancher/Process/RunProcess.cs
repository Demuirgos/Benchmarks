using System.Diagnostics;
using System.Reflection;
public class ProcessBuilder
{
    public static ProcessBuilder Instance => new ProcessBuilder();
    private Process process;
    public ProcessBuilder Create(string command) {
        process = new Process();
        process.StartInfo.FileName = command;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow  = true;
        return this;
    }
    public ProcessBuilder WithArguments(string args) {
        process.StartInfo.Arguments = args;
        return this;
    }
    public ProcessBuilder WithWorkingDirectory(string path) {
        if(path != null) {
            process.StartInfo.WorkingDirectory = path;
        }
        return this;
    }
    public ProcessBuilder WithRedirectStandardOutput(bool value = true, Action<string> LogsHandle = null) {
        process.StartInfo.RedirectStandardOutput = value;
        if(value && LogsHandle != null) {
            process.OutputDataReceived += (sender, e) => {
                if (e.Data != null) {
                    LogsHandle?.Invoke($"\t{e.Data}");
                }
            };
        }
        return this;
    }
    public ProcessBuilder WithStandaloneWindow(bool value = true) {
        process.StartInfo.CreateNoWindow  = !value;
        return this;
    }
    public async Task Run() {
        process.Start();
        if(process.StartInfo.RedirectStandardOutput) {
            process.BeginOutputReadLine();
        }
        await process.WaitForExitAsync();
    }
}