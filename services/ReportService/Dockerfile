# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install PostgreSQL client (pg_isready)
RUN apt-get update && apt-get install -y postgresql-client

# Install dotnet-ef tool (if needed for migrations or other operations)
RUN dotnet tool install --global dotnet-ef

# Set the working directory in the container
WORKDIR /app

# Copy the .csproj files
COPY src/ReportService.API/ReportService.API.csproj /app/src/ReportService.API/
COPY src/ReportService.Application/ReportService.Application.csproj /app/src/ReportService.Application/
COPY src/ReportService.Domain/ReportService.Domain.csproj /app/src/ReportService.Domain/
COPY src/ReportService.Infrastructure/ReportService.Infrastructure.csproj /app/src/ReportService.Infrastructure/
COPY tests/ReportService.UnitTests/ReportService.UnitTests.csproj /app/tests/ReportService.UnitTests/

# Restore dependencies for the ReportService API project
RUN dotnet restore /app/src/ReportService.API/ReportService.API.csproj

# Copy the entire codebase into the container
COPY src/ /app/src/
COPY tests/ /app/tests/

# Publish the application to the /out folder
RUN dotnet publish -c Release -o /out /app/src/ReportService.API/ReportService.API.csproj

# Create the final image using the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /out .

# Expose the port matching ocelot.json (5002 for UserService)
EXPOSE 5007

# Ensure the app listens on the correct port
ENV ASPNETCORE_URLS=http://+:5007
ENV ASPNETCORE_ENVIRONMENT=Development

# Define the entrypoint for the container to run the application
ENTRYPOINT ["dotnet", "ReportService.API.dll"]
