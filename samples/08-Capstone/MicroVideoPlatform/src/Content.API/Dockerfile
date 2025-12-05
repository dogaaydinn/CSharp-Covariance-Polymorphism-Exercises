FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Content.API/Content.API.csproj", "Content.API/"]
RUN dotnet restore "Content.API/Content.API.csproj"
COPY src/Content.API/ Content.API/
WORKDIR "/src/Content.API"
RUN dotnet build "Content.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Content.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Content.API.dll"]
