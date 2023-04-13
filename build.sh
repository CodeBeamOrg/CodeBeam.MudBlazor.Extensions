curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh 
chmod +x dotnet-install.sh 
./dotnet-install.sh -c 7.0 -InstallDir ./dotnet7 
./dotnet7/dotnet --version 
./dotnet7/dotnet tool install Excubo.WebCompiler --global
./dotnet7/dotnet workload restore
./dotnet7/dotnet publish ComponentViewer.Wasm/ComponentViewer.Wasm.csproj -c Release -o output
