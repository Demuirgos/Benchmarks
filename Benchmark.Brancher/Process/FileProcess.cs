using System.IO;
using static BenchProcess;
public static class FileProcess
{
    public static (string ClonePath, string CurrentPath, string TargetPath) CreateTestContainer(string repoUrl, string currentCommit, string targetCommit) {
        // check if the folder exists and delete it
        if (Directory.Exists(TempFolder)) {
            Directory.Delete(TempFolder, true);
        }
            
        var hostFolder = Directory.CreateDirectory(TempFolder);
        // create sub folder
        var cloneTarget = hostFolder.CreateSubdirectory("CloneFolder");
        var currentTarget = hostFolder.CreateSubdirectory(currentCommit);
        var targetTarget = hostFolder.CreateSubdirectory(targetCommit);
        return (cloneTarget.FullName, currentTarget.FullName, targetTarget.FullName);
    } 

    public static void CleanUp() {
        if (Directory.Exists(TempFolder)) {
            Directory.Delete(TempFolder, true);
        }
    }
}