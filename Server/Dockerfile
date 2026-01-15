# -------------------------------
# Stage 1: Build
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# ✅ Correct way to disable fallback folders
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV DOTNET_NOLOGO=true
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true

# Copy project file and restore
COPY ["Hub/Hub.csproj", "Hub/"]
RUN dotnet restore "Hub/Hub.csproj" \
    --ignore-failed-sources \
    --disable-parallel

# Copy everything else
COPY . .

# Build
WORKDIR /src/Hub
RUN dotnet build "Hub.csproj" -c $BUILD_CONFIGURATION -o /app/build --no-restore


# -------------------------------
# Stage 2: Publish
# -------------------------------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Hub.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    --no-restore \
    /p:UseAppHost=false


# -------------------------------
# Stage 3: Runtime
# -------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

EXPOSE 8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "Hub.dll"]
