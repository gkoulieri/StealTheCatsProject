# Step 1: Use the official Microsoft .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# Copy the project files to the container
COPY . ./

# Restore project dependencies
RUN dotnet restore

# Build the project in release mode
RUN dotnet build -c Release

# Publish the app for deployment
RUN dotnet publish -c Release -o out

# Step 2: Use the official ASP.NET Core runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy the published files from the build environment
COPY --from=build-env /app/out .

# Expose port 80 for the app to listen on
EXPOSE 80

# Set the entry point to run the app
ENTRYPOINT ["dotnet", "StealTheCatsAPI.dll"]
