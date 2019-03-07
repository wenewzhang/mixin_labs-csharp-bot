all:
	dotnet build
clean:
	rm -rf bin/
	rm -rf obj/
	rm -rf mixin-csharp-sdk/mixin-csharp-sdk/bin/
	rm -rf mixin-csharp-sdk/mixin-csharp-sdk/obj/
run:
		dotnet bin/Debug/netcoreapp2.2/mixin_labs-csharp-bot.dll
drun:
	dotnet bin/Debug/netcoreapp2.2/mixin_labs-csharp-bot.dll
rrun:
	dotnet bin/Release/netcoreapp2.2/mixin_labs-csharp-bot.dll
