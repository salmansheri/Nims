FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /app

# Copy solution and project files

COPY ./Backend/Backend.csproj ./Backend/


# Restore
RUN dotnet restore ./Backend/Backend.csproj



# Copy the rest of the source code
COPY . .

# Publish
RUN dotnet publish ./Backend/Backend.csproj -c Release -o /app/publish


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app
EXPOSE 80

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Backend.dll"]