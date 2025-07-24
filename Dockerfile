# Stage 1: Define the final, lean runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Create a non-root user and group
RUN addgroup --system appgroup && adduser --system --ingroup appgroup appuser

# ---
# Stage 2: Build the application using the SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files first to leverage Docker layer caching.
# This ensures 'dotnet restore' is only re-run when dependencies change.
COPY ["*.sln", "."]
COPY ["src/People.Api/People.Api.csproj", "src/People.Api/"]
COPY ["src/People.Data/People.Data.csproj", "src/People.Data/"]
COPY ["src/People.Tests/People.Tests.csproj", "src/People.Tests/"]

# Restore dependencies for the entire solution
RUN dotnet restore "./People.sln"

# Copy the rest of the application's source code
COPY . .

# Publish the API project, creating the final output
RUN dotnet publish "./src/People.Api/People.Api.csproj" -c Release -o /app/publish --no-restore

# ---
# Stage 3: Create the final production image from the base
FROM base AS final

# Copy the published output from the build stage
# and set ownership to the non-root user
WORKDIR /app
COPY --from=build --chown=appuser:appgroup /app/publish .

# Switch to the non-root user
USER appuser

# Set the entrypoint for the container
ENTRYPOINT ["dotnet", "People.Api.dll"]