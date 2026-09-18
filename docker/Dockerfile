
# STAGE 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["DigiFikileLms.API/DigiFikileLms.API.csproj", "DigiFikileLms.API/"]
COPY ["DigiFikileLms.Application/DigiFikileLms.Application.csproj", "DigiFikileLms.Application/"]
COPY ["DigiFikileLms.Domain/DigiFikileLms.Domain.csproj", "DigiFikileLms.Domain/"]
COPY ["DigiFikileLms.Infrastructure/DigiFikileLms.Infrastructure.csproj", "DigiFikileLms.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "DigiFikileLms.API/DigiFikileLms.API.csproj"

# Copy everything and publish
COPY . .
WORKDIR "/src/DigiFikileLms.API"
RUN dotnet publish "DigiFikileLms.API.csproj" -c Release -o /app/publish /p:UseAppHost=false


# STAGE 2: Runtime

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published app
COPY --from=build /app/publish .

# Set port (Render overrides via PORT env var)
EXPOSE 8080

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080

# Run the app
ENTRYPOINT ["dotnet", "DigiFikileLms.API.dll"]