# ────────────────────────────────
# Stage 1 – lean runtime image
# ────────────────────────────────
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS base
WORKDIR /app
EXPOSE 8080

# Create an unprivileged user (Alpine busy‑box utilities)
RUN addgroup -S appgroup \
 && adduser  -S appuser -G appgroup

# ────────────────────────────────
# Stage 2 – build & publish
# ────────────────────────────────
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# 1️⃣ Copy the solution file from *src* (matches your folder structure)
COPY src/People.sln .

# 2️⃣ Copy project files (keeps Docker layer cache effective)
COPY src/People.Api/People.Api.csproj   People.Api/
COPY src/People.Data/People.Data.csproj People.Data/
COPY src/People.Tests/People.Tests.csproj People.Tests/

RUN ls -l

# 3️⃣ Restore NuGet packages just once
RUN dotnet restore People.sln

# 4️⃣ Bring in the remaining source
COPY src/ .

# 5️⃣ Build & publish the API project
RUN dotnet publish People.Api/People.Api.csproj \
    -c Release \
    -o /app/publish

# ────────────────────────────────
# Stage 3 – final production image
# ────────────────────────────────
FROM base AS final
WORKDIR /app

# Copy published output from the build stage and set ownership
COPY --from=build --chown=appuser:appgroup /app/publish .

USER appuser
ENTRYPOINT ["dotnet", "People.Api.dll"]
