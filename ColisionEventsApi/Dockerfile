# Imagem base para runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Imagem base para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copia o .csproj e restaura dependências
COPY ["ColisionEvents.Api/ColisionEventsApi.csproj", "ColisionEventsApi/"]
RUN dotnet restore "./ColisionEvents.Api/ColisionEventsApi.csproj"

# Copia o restante do código e compila
COPY . .
WORKDIR "/src/ColisionEventsApi"
RUN dotnet build "./ColisionEventsApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publica a aplicação
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ColisionEventsApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ColisionEventsApi.dll"]
