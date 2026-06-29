FROM mcr.microsoft.com/dotnet/runtime:10.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Bookstore/Bookstore.csproj", "Bookstore/"]
RUN dotnet restore "Bookstore/Bookstore.csproj"
COPY . .
WORKDIR "/src/Bookstore"
RUN dotnet build "./Bookstore.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Bookstore.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM build AS test-deps
# Global tools commonly used in .NET unit-test and coverage workflows.
RUN dotnet tool install --global dotnet-coverage
RUN dotnet tool install --global dotnet-reportgenerator-globaltool
ENV PATH="$PATH:/root/.dotnet/tools"

FROM test-deps AS test
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN dotnet test "Bookstore.sln" -c $BUILD_CONFIGURATION --logger "trx;LogFileName=test-results.trx"

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Bookstore.dll"]
