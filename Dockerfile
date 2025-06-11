# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0.413 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["Jollicow.csproj", "./"]
RUN dotnet restore

# Copy the rest of the code
COPY . .
RUN dotnet build "Jollicow.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Jollicow.csproj" -c Release -o /app/publish

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