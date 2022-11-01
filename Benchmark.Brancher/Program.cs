// get commandLineArgs from the command line
var commandLineArgs = Environment.GetCommandLineArgs();
var (slnPath, csprojPath, repoSource, currentCommit, targetCommit)  = (commandLineArgs[1], commandLineArgs[2], commandLineArgs[3], commandLineArgs[4], commandLineArgs[5]);
await BenchProcess.Process(slnPath, csprojPath, repoSource, currentCommit, targetCommit);

