# -------------------------------
# Stage 1: Build
# -------------------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release

# Set working directory
WORKDIR /src

# ✅ Optimize NuGet and first-time experience
ENV NUGET_PACKAGES=/root/.nuget/packages
ENV DOTNET_NOLOGO=true
ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE=true

# Copy all project files first (csproj) to leverage Docker cache
COPY ["Hub/Hub.csproj", "Hub/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Domain/Domain.csproj", "Domain/"]

# Restore dependencies
RUN dotnet restore "Hub/Hub.csproj" \
    --ignore-failed-sources \
    --disable-parallel

# Copy all source code
COPY . .

# Build the Hub project
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

# Expose the port your app listens on
EXPOSE 8080

# Copy published output
COPY --from=publish /app/publish .

# Entry point
ENTRYPOINT ["dotnet", "Hub.dll"]
