# mixin_labs-csharp-bot
Installation

Visual Studio 2017 Community Edition:
https://visualstudio.microsoft.com/downloads/

.Net Core
https://dotnet.microsoft.com/download


wenewzha:mixin_labs-csharp-bot wenewzhang$ dotnet --version
2.2.104

### Create mixin_labs-csharp-bot project directory
```bash
wenewzha:mixin_labs-csharp-bot wenewzhang$ dotnet new console
The template "Console Application" was created successfully.

Processing post-creation actions...
Running 'dotnet restore' on /Users/wenewzhang/Projects/mixin_labs-csharp-bot/mixin_labs-csharp-bot.csproj...
  Restoring packages for /Users/wenewzhang/Projects/mixin_labs-csharp-bot/mixin_labs-csharp-bot.csproj...
  Generating MSBuild file /Users/wenewzhang/Projects/mixin_labs-csharp-bot/obj/mixin_labs-csharp-bot.csproj.nuget.g.props.
  Generating MSBuild file /Users/wenewzhang/Projects/mixin_labs-csharp-bot/obj/mixin_labs-csharp-bot.csproj.nuget.g.targets.
  Restore completed in 458.08 ms for /Users/wenewzhang/Projects/mixin_labs-csharp-bot/mixin_labs-csharp-bot.csproj.

Restore succeeded.

wenewzha:mixin_labs-csharp-bot wenewzhang$ ls
Program.cs			README.md			mixin_labs-csharp-bot.csproj	obj
```
### Installation location
/usr/local/share/dotnet/

### Build and run
```bash
dotnet build
dotnet bin/Debug/netcoreapp2.2/mixin_labs-csharp-bot.dll
```
