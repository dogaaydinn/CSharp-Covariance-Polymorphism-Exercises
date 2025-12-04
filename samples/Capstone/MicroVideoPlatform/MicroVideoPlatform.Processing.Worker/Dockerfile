FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroVideoPlatform.Processing.Worker/MicroVideoPlatform.Processing.Worker.csproj", "MicroVideoPlatform.Processing.Worker/"]
COPY ["MicroVideoPlatform.Shared/MicroVideoPlatform.Shared.csproj", "MicroVideoPlatform.Shared/"]
RUN dotnet restore "MicroVideoPlatform.Processing.Worker/MicroVideoPlatform.Processing.Worker.csproj"
COPY . .
WORKDIR "/src/MicroVideoPlatform.Processing.Worker"
RUN dotnet build "MicroVideoPlatform.Processing.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroVideoPlatform.Processing.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroVideoPlatform.Processing.Worker.dll"]
