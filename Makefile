all:
	dotnet build -c Release
	cp ZGenesis/bin/Release/net4.8/ZGenesis.dll ../../0.2.0/
clean:
	rm ZGenesis/bin/Release/net4.8/*
