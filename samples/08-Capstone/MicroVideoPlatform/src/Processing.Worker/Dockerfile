FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Processing.Worker/Processing.Worker.csproj", "Processing.Worker/"]
RUN dotnet restore "Processing.Worker/Processing.Worker.csproj"
COPY src/Processing.Worker/ Processing.Worker/
WORKDIR "/src/Processing.Worker"
RUN dotnet build "Processing.Worker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Processing.Worker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Processing.Worker.dll"]
