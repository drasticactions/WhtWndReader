ROOT=$(PWD)
ARTIFACTS_IOS_DIR=$(ROOT)/artifacts/ios
ARTIFACTS_CATALYST_DIR=$(ROOT)/artifacts/catalyst

clean:
	rm -rf $(ARTIFACTS_IOS_DIR)
	rm -rf $(ARTIFACTS_CATALYST_DIR)

build-ios:
	dotnet publish src/WhtWndReader.iOS/WhtWndReader.iOS.csproj -c Release -p:ArchiveOnBuild=true -o $(ARTIFACTS_IOS_DIR)

build-catalyst:
	dotnet publish src/WhtWndReader.Catalyst/WhtWndReader.Catalyst.csproj -c Release -o $(ARTIFACTS_CATALYST_DIR)