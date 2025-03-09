# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

# Install dotnet-ef tool
RUN dotnet tool install --global dotnet-ef

# Set the working directory in the container
WORKDIR /app

# Create necessary directories for the .csproj files
RUN rm -rf /app/src/AuthService.API && mkdir -p /app/src/AuthService.API
RUN rm -rf /app/src/AuthService.Application && mkdir -p /app/src/AuthService.Application
RUN rm -rf /app/src/AuthService.Infrastructure && mkdir -p /app/src/AuthService.Infrastructure
RUN rm -rf /app/src/AuthService.Domain && mkdir -p /app/src/AuthService.Domain

# Copy the .csproj files
COPY src/AuthService.API/AuthService.API.csproj /app/src/AuthService.API/
COPY src/AuthService.Application/AuthService.Application.csproj /app/src/AuthService.Application/
COPY src/AuthService.Domain/AuthService.Domain.csproj /app/src/AuthService.Domain/
COPY src/AuthService.Infrastructure/AuthService.Infrastructure.csproj /app/src/AuthService.Infrastructure/

COPY tests/AuthService.UnitTests/AuthService.UnitTests.csproj /app/tests/AuthService.UnitTests/

# Now restore dependencies for each project
RUN dotnet restore /app/src/AuthService.API/AuthService.API.csproj
RUN dotnet restore /app/src/AuthService.Application/AuthService.Application.csproj
RUN dotnet restore /app/src/AuthService.Domain/AuthService.Domain.csproj
RUN dotnet restore /app/src/AuthService.Infrastructure/AuthService.Infrastructure.csproj

# Copy the entire codebase into the container
COPY src/ /app/src/

COPY tests/ /app/tests/

# Publish the application to the /out folder
RUN dotnet publish -c Release -o /out /app/src/AuthService.API/AuthService.API.csproj

# Create the final image using the official .NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

# Set the working directory in the container
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /out .

# Expose the port your API is running on
EXPOSE 8080

# Define the entrypoint for the container to run the application
ENTRYPOINT ["dotnet", "AuthService.API.dll"]
