# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj files
COPY ApiGateway.csproj ./ApiGateway/

# Restore dependencies for the ApiGateway project
RUN dotnet restore ./ApiGateway/ApiGateway.csproj

# Copy the entire codebase into the container
COPY . ./ApiGateway/

# Publish the application to the /out folder
RUN dotnet publish ./ApiGateway/ApiGateway.csproj -c Release -o /out --no-restore

# Create the final image using the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /out .

# Expose the port your API is running on
EXPOSE 5000

# Environment variables for configuration (to be overridden via docker-compose)
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Development
ENV Redis__ConnectionString=redis:6379
ENV Redis__InstanceName=SRMServiceInstance

# Define the entrypoint for the container to run the application
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
