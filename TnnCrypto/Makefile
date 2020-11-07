default: build

build:
	msbuild /t:Rebuild /p:Configuration=Release

clean:
	msbuild /t:Clean

nuget-pack: build
	nuget pack TnnCrypto.nuspec 

nuget-push:
	echo "nuget push -Source https://api.nuget.org/v3/index.json"
