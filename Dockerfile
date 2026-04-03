FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

ENV CI=true

COPY . .

RUN dotnet restore docs/CodeBeam.MudBlazor.Extensions.Docs.Wasm/CodeBeam.MudBlazor.Extensions.Docs.Wasm.csproj

RUN dotnet publish docs/CodeBeam.MudBlazor.Extensions.Docs.Wasm/CodeBeam.MudBlazor.Extensions.Docs.Wasm.csproj -c Release -o /app/publish

FROM caddy:alpine

COPY --from=build /app/publish/wwwroot /usr/share/caddy
COPY Caddyfile /etc/caddy/Caddyfile
