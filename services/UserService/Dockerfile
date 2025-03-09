# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install PostgreSQL client (pg_isready)
RUN apt-get update && apt-get install -y postgresql-client

# Install dotnet-ef tool (if needed for migrations or other operations)
RUN dotnet tool install --global dotnet-ef

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj files
COPY src/UserService.API/UserService.API.csproj /app/src/UserService.API/
COPY src/UserService.Application/UserService.Application.csproj /app/src/UserService.Application/
COPY src/UserService.Domain/UserService.Domain.csproj /app/src/UserService.Domain/
COPY src/UserService.Infrastructure/UserService.Infrastructure.csproj /app/src/UserService.Infrastructure/
COPY tests/UserService.UnitTests/UserService.UnitTests.csproj /app/tests/UserService.UnitTests/

# Restore dependencies for the UserService API project
RUN dotnet restore /app/src/UserService.API/UserService.API.csproj

# Copy the entire codebase into the container
COPY src/ /app/src/
COPY tests/ /app/tests/

# Publish the application to the /out folder
RUN dotnet publish -c Release -o /out /app/src/UserService.API/UserService.API.csproj

# Create the final image using the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /out .

# Expose the port your API is running on
EXPOSE 8080

# Define the entrypoint for the container to run the application
ENTRYPOINT ["dotnet", "UserService.API.dll"]
