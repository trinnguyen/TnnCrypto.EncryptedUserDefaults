default: build

build:
	msbuild /t:Rebuild /p:Configuration=Release

clean:
	msbuild /t:Clean

nuget-pack:
	nuget pack TnnCrypto.nuspec 

nuget-push:
	nuget push -Source https://api.nuget.org/v3/index.json
