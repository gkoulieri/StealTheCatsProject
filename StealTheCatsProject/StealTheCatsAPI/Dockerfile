# Use the SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the csproj and restore any dependencies
COPY *.sln ./
COPY StealTheCatsAPI/StealTheCatsAPI.csproj StealTheCatsAPI/
COPY StealTheCatsAPITests/StealTheCatsAPITests.csproj StealTheCatsAPITests/
RUN dotnet restore

# Copy the rest of the files and build the project
COPY . ./
RUN dotnet publish StealTheCatsAPI/StealTheCatsAPI.csproj -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "StealTheCatsAPI.dll"]
