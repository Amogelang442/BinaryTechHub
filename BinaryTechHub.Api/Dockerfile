# Step 1: Build environment using .NET 9.0 SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the backend files and publish
COPY . ./
RUN dotnet publish -c Release -o /out

# Step 2: Runtime environment using .NET 9.0 ASP.NET
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /out ./

# Configure the port Render listens on
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "BinaryTechHub.Api.dll"]