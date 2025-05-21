# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install PostgreSQL client (pg_isready)
RUN apt-get update && apt-get install -y postgresql-client

# Install dotnet-ef tool (if needed for migrations or other operations)
RUN dotnet tool install --global dotnet-ef

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj files
COPY src/GradeService.API/GradeService.API.csproj /app/src/GradeService.API/
COPY src/GradeService.Application/GradeService.Application.csproj /app/src/GradeService.Application/
COPY src/GradeService.Domain/GradeService.Domain.csproj /app/src/GradeService.Domain/
COPY src/GradeService.Infrastructure/GradeService.Infrastructure.csproj /app/src/GradeService.Infrastructure/
COPY tests/GradeService.UnitTests/GradeService.UnitTests.csproj /app/tests/GradeService.UnitTests/

# Restore dependencies for the GradeService API project
RUN dotnet restore /app/src/GradeService.API/GradeService.API.csproj

# Copy the entire codebase into the container
COPY src/ /app/src/
COPY tests/ /app/tests/

# Publish the application to the /out folder
RUN dotnet publish -c Release -o /out /app/src/GradeService.API/GradeService.API.csproj

# Create the final image using the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /out .

# Expose the port your API is running on
EXPOSE 8080

# Define the entrypoint for the container to run the application
ENTRYPOINT ["dotnet", "GradeService.API.dll"]
