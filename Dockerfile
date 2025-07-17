# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0.413 AS build
WORKDIR /app

# Copy source code
COPY . ./

# Tạo thư mục keys để Data Protection lưu key ring
RUN mkdir -p /app/keys

# Restore dependencies
RUN dotnet restore

# Publish app
RUN dotnet publish -c Release -o out

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0.13
WORKDIR /app

# Copy published app and keys
COPY --from=build /app/out ./
COPY --from=build /app/keys ./keys

# Expose port (Railway sẽ tự map port này)
EXPOSE 3000

# Start app
ENTRYPOINT ["dotnet", "Jollicow.dll"] 