curl -sSL https://dot.net/v1/dotnet-install.sh > dotnet-install.sh 
chmod +x dotnet-install.sh 
./dotnet-install.sh -c 10.0 --version 10.0.100 -InstallDir ./dotnet10 
./dotnet10/dotnet --version
./dotnet10/dotnet publish CodeBeam.MudBlazor.Extensions.Docs.Wasm/CodeBeam.MudBlazor.Extensions.Docs.Wasm.csproj -c Release -o output
