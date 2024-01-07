curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh 
chmod +x dotnet-install.sh 
./dotnet-install.sh -c 8.0 --version 8.0.100 -InstallDir ./dotnet8 
./dotnet8/dotnet --version 
./dotnet8/dotnet tool install Excubo.WebCompiler --global
./dotnet8/dotnet publish ComponentViewer.Wasm/ComponentViewer.Wasm.csproj -c Release -o output
