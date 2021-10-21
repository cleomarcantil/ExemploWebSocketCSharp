#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ExemploWebSocketCSharp/ExemploWebSocketCSharp.csproj", "ExemploWebSocketCSharp/"]
COPY ["HelpersExtensions/HelpersExtensions.WebSocketConnection/HelpersExtensions.WebSocketConnection.csproj", "HelpersExtensions/HelpersExtensions.WebSocketConnection/"]
RUN dotnet restore "ExemploWebSocketCSharp/ExemploWebSocketCSharp.csproj"
COPY . .
WORKDIR "/src/ExemploWebSocketCSharp"
RUN dotnet build "ExemploWebSocketCSharp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ExemploWebSocketCSharp.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExemploWebSocketCSharp.dll"]