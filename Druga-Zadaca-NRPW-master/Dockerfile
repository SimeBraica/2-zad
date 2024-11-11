# Stage 1: Build the Blazor UI (Frontend)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS blazor-build
WORKDIR /app

# Copy the Blazor project file and restore dependencies
COPY ./UI/UI.csproj ./UI/
RUN dotnet restore ./UI/UI.csproj

# Copy the rest of the Blazor files
COPY ./UI/ ./UI/

# Build the Blazor WebAssembly app for production
RUN dotnet publish ./UI/UI.csproj -c Release -o /app/UI/dist

# Stage 2: Build the .NET Core API (Backend)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build
WORKDIR /app

# Copy the .NET Core API project file and restore dependencies
COPY ./API/API.csproj ./API/
RUN dotnet restore ./API/API.csproj

# Copy the rest of the API files
COPY ./API/ ./API/

# Publish the .NET Core API
RUN dotnet publish ./API/API.csproj -c Release -o /out

# Stage 3: Runtime environment (for serving both the API and Blazor UI)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published .NET Core API from the build stage
COPY --from=dotnet-build /out .

# Copy the published Blazor UI from the blazor-build stage to the wwwroot folder
COPY --from=blazor-build /app/UI/dist ./wwwroot

# Expose port 80 for the web app (you can use any port, but 80 is standard)
EXPOSE 80

# Start the .NET Core API when the container starts
ENTRYPOINT ["dotnet", "API.dll"]
