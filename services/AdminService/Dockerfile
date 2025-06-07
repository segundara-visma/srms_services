# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install PostgreSQL client (pg_isready)
RUN apt-get update && apt-get install -y postgresql-client

# Install dotnet-ef tool (if needed for migrations or other operations)
RUN dotnet tool install --global dotnet-ef

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj files
COPY src/AdminService.API/AdminService.API.csproj /app/src/AdminService.API/
COPY src/AdminService.Application/AdminService.Application.csproj /app/src/AdminService.Application/
COPY src/AdminService.Domain/AdminService.Domain.csproj /app/src/AdminService.Domain/
COPY src/AdminService.Infrastructure/AdminService.Infrastructure.csproj /app/src/AdminService.Infrastructure/
COPY tests/AdminService.UnitTests/AdminService.UnitTests.csproj /app/tests/AdminService.UnitTests/

# Restore dependencies for the AdminService API project
RUN dotnet restore /app/src/AdminService.API/AdminService.API.csproj

# Copy the entire codebase into the container
COPY src/ /app/src/
COPY tests/ /app/tests/

# Publish the application to the /out folder
RUN dotnet publish -c Release -o /out /app/src/AdminService.API/AdminService.API.csproj

# Create the final image using the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /out .

# Expose the port your API is running on
EXPOSE 5009

# Ensure the app listens on the correct port
ENV ASPNETCORE_URLS=http://+:5009
ENV ASPNETCORE_ENVIRONMENT=Development

# Define the entrypoint for the container to run the application
ENTRYPOINT ["dotnet", "AdminService.API.dll"]
