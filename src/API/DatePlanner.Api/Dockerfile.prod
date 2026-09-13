# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/API/DatePlanner.Api/DatePlanner.Api.csproj", "src/API/DatePlanner.Api/"]
COPY ["src/API/DatePlanner.Application/DatePlanner.Application.csproj", "src/API/DatePlanner.Application/"]
COPY ["src/API/DatePlanner.Domain/DatePlanner.Domain.csproj", "src/API/DatePlanner.Domain/"]
COPY ["src/API/DatePlanner.Infrastructure/DatePlanner.Infrastructure.csproj", "src/API/DatePlanner.Infrastructure/"]
RUN dotnet restore "src/API/DatePlanner.Api/DatePlanner.Api.csproj"

COPY src/API/ src/API/
RUN dotnet publish "src/API/DatePlanner.Api/DatePlanner.Api.csproj" \
    --configuration "$BUILD_CONFIGURATION" \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

USER root
RUN apt-get update \
    && apt-get install --yes --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0

COPY --from=build /app/publish .

USER $APP_UID
EXPOSE 8080

HEALTHCHECK --interval=15s --timeout=5s --start-period=30s --retries=5 \
    CMD curl --fail --silent --show-error http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "DatePlanner.Api.dll"]
