# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install PostgreSQL client (pg_isready)
RUN apt-get update && apt-get install -y postgresql-client

# Install dotnet-ef tool (if needed for migrations or other operations)
RUN dotnet tool install --global dotnet-ef

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj files
COPY src/StudentService.API/StudentService.API.csproj /app/src/StudentService.API/
COPY src/StudentService.Application/StudentService.Application.csproj /app/src/StudentService.Application/
COPY src/StudentService.Domain/StudentService.Domain.csproj /app/src/StudentService.Domain/
COPY src/StudentService.Infrastructure/StudentService.Infrastructure.csproj /app/src/StudentService.Infrastructure/
COPY tests/StudentService.UnitTests/StudentService.UnitTests.csproj /app/tests/StudentService.UnitTests/

# Restore dependencies for the StudentService API project
RUN dotnet restore /app/src/StudentService.API/StudentService.API.csproj

# Copy the entire codebase into the container
COPY src/ /app/src/
COPY tests/ /app/tests/

# Publish the application to the /out folder
RUN dotnet publish -c Release -o /out /app/src/StudentService.API/StudentService.API.csproj

# Create the final image using the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /out .

# Expose the port matching ocelot.json (5002 for UserService)
EXPOSE 5004

# Ensure the app listens on the correct port
ENV ASPNETCORE_URLS=http://+:5004
ENV ASPNETCORE_ENVIRONMENT=Development

# Define the entrypoint for the container to run the application
ENTRYPOINT ["dotnet", "StudentService.API.dll"]
