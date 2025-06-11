# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0.413 AS build
WORKDIR /src

# Copy the project file first
COPY ["Jollicow.csproj", "."]

# Restore dependencies
RUN dotnet restore "Jollicow.csproj" --no-cache

# Copy the rest of the code
COPY . .

# Build the application
RUN dotnet build "Jollicow.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Jollicow.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0.413 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "Jollicow.dll"] 