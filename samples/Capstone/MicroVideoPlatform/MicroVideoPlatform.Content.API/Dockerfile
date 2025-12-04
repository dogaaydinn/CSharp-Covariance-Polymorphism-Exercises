FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MicroVideoPlatform.Content.API/MicroVideoPlatform.Content.API.csproj", "MicroVideoPlatform.Content.API/"]
COPY ["MicroVideoPlatform.Shared/MicroVideoPlatform.Shared.csproj", "MicroVideoPlatform.Shared/"]
RUN dotnet restore "MicroVideoPlatform.Content.API/MicroVideoPlatform.Content.API.csproj"
COPY . .
WORKDIR "/src/MicroVideoPlatform.Content.API"
RUN dotnet build "MicroVideoPlatform.Content.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MicroVideoPlatform.Content.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MicroVideoPlatform.Content.API.dll"]
