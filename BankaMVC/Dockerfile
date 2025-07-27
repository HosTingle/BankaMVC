FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8083
EXPOSE 8084

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BankaMVC/BankaMVC.csproj", "BankaMVC/"]
RUN dotnet restore "./BankaMVC/BankaMVC.csproj"
COPY . .
WORKDIR "/src/BankaMVC"
RUN dotnet build "./BankaMVC.csproj" -c ${BUILD_CONFIGURATION} -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BankaMVC.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BankaMVC.dll"]
