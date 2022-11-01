# Benchmarks
a bunch of experiments and benchmarks

# Usage (In Benchmark.Brancher dir): 
```
dotnet run \
  "ConsoleApp1.sln" \
  "ConsoleApp1\ConsoleApp1.csproj" \  
  "D:\Projects\benchmarks\Benchmark.Target" \ 
  "7b5f9e3748b35c25a00560f2cc8665018c689b41" \ 
  "a7f58cc97c0052da29b3cd78c21797e5c1aa0145"
```
# Sample Output (testing) : 
```
C:\Program Files\dotnet\sdk\6.0.402\Microsoft.Common.CurrentVersion.targets(2066,5): warning : The referenced project '
..\Benchmark.Generator\Benchmarks.Generator.csproj' does not exist. [D:\Projects\benchmarks\Benchmark.Brancher\Benchmar
k.Brancher.csproj]
Running : Process with Arguments: ["ConsoleApp1.sln","ConsoleApp1\\ConsoleApp1.csproj","D:\\Projects\\benchmarks\\Benchmark.Target","7b5f9e3748b35c25a00560f2cc8665018c689b41","a7f58cc97c0052da29b3cd78c21797e5c1aa0145"]
Running : Clone with Arguments: ["D:\\Projects\\benchmarks\\Benchmark.Target","C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\CloneFolder"]
Exiting : Clone with Status: Succeeded, Completed
Running : Clone with Arguments: ["C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\CloneFolder","C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\a7f58cc97c0052da29b3cd78c21797e5c1aa0145"]
Running : Clone with Arguments: ["C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\CloneFolder","C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\7b5f9e3748b35c25a00560f2cc8665018c689b41"]
Exiting : Clone with Status: Succeeded, Completed
Running : SwitchTo with Arguments: ["C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\7b5f9e3748b35c25a00560f2cc8665018c689b41","C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\a7f58cc97c0052da29b3cd78c21797e5c1aa0145"]
Exiting : Clone with Status: Succeeded, Completed
Running : SwitchTo with Arguments: ["C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\7b5f9e3748b35c25a00560f2cc8665018c689b41","C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\7b5f9e3748b35c25a00560f2cc8665018c689b41"]
Exiting : SwitchTo with Status: Succeeded, Completed
Running : Build with Arguments: ["C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\a7f58cc97c0052da29b3cd78c21797e5c1aa0145\\ConsoleApp1.sln"]
Exiting : SwitchTo with Status: Succeeded, Completed
Running : Build with Arguments: ["C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\7b5f9e3748b35c25a00560f2cc8665018c689b41\\ConsoleApp1.sln"]
MSBuild version 17.3.2+561848881 for .NET
MSBuild version 17.3.2+561848881 for .NET
  Determining projects to restore...
  Determining projects to restore...
  Restored C:\Users\Ayman\AppData\Local\Temp\TEMP4BENCHMARK\a7f58cc97c0052da29b3cd78c21797e5c1aa0145\ConsoleApp1\Consol
  eApp1.csproj (in 88 ms).
  Restored C:\Users\Ayman\AppData\Local\Temp\TEMP4BENCHMARK\7b5f9e3748b35c25a00560f2cc8665018c689b41\ConsoleApp1\Consol
  eApp1.csproj (in 103 ms).
  ConsoleApp1 -> C:\Users\Ayman\AppData\Local\Temp\TEMP4BENCHMARK\a7f58cc97c0052da29b3cd78c21797e5c1aa0145\ConsoleApp1\
  bin\Release\net6.0\ConsoleApp1.dll
  ConsoleApp1 -> C:\Users\Ayman\AppData\Local\Temp\TEMP4BENCHMARK\7b5f9e3748b35c25a00560f2cc8665018c689b41\ConsoleApp1\
  bin\Release\net6.0\ConsoleApp1.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.83

Build succeeded.
    0 Warning(s)
    0 Error(s)

Time Elapsed 00:00:02.80
Exiting : Build with Status: Succeeded, Completed
Running : Launch with Arguments: ["C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\a7f58cc97c0052da29b3cd78c21797e5c1aa0145\\ConsoleApp1\\ConsoleApp1.csproj","net6.0",null]
Exiting : Build with Status: Succeeded, Completed
Running : Launch with Arguments: ["C:\\Users\\Ayman\\AppData\\Local\\Temp\\TEMP4BENCHMARK\\7b5f9e3748b35c25a00560f2cc8665018c689b41\\ConsoleApp1\\ConsoleApp1.csproj","net6.0",null]
        TEMP4BENCHMARK\7b5f9e3748b35c25a00560f2cc8665018c689b41\ConsoleApp1\ConsoleApp1.csproj ::>      Result : 499500
        TEMP4BENCHMARK\7b5f9e3748b35c25a00560f2cc8665018c689b41\ConsoleApp1\ConsoleApp1.csproj ::>      Logs : took 4ms
Exiting : Launch with Status: Succeeded, Completed
        TEMP4BENCHMARK\a7f58cc97c0052da29b3cd78c21797e5c1aa0145\ConsoleApp1\ConsoleApp1.csproj ::>      Result : 499500
        TEMP4BENCHMARK\a7f58cc97c0052da29b3cd78c21797e5c1aa0145\ConsoleApp1\ConsoleApp1.csproj ::>      Logs : took 6ms
Exiting : Launch with Status: Succeeded, Completed
Cleaning up...
Exiting : Process with Status: Succeeded, Completed
```
