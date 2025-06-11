# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0.413 AS build
WORKDIR /src

# Copy the project file first
COPY ["Jollicow.csproj", "."]

# Create a new user with appropriate permissions
RUN adduser --disabled-password --gecos "" appuser && \
    chown -R appuser:appuser /src

# Switch to the new user
USER appuser

# Restore dependencies with explicit project file
RUN dotnet restore "Jollicow.csproj" --no-cache --force

# Copy the rest of the code
COPY --chown=appuser:appuser . .

# Build the application
RUN dotnet build "Jollicow.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Jollicow.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0.413 AS final
WORKDIR /app

# Create a new user in the final stage
RUN adduser --disabled-password --gecos "" appuser && \
    chown -R appuser:appuser /app

# Copy published files
COPY --from=publish --chown=appuser:appuser /app/publish .

# Switch to the non-root user
USER appuser

# Set environment variables
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port 80
EXPOSE 80

# Start the application
ENTRYPOINT ["dotnet", "Jollicow.dll"] 