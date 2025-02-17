#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Cars/Cars.csproj", "Cars/"]
RUN dotnet restore "Cars/Cars.csproj"
COPY . .

WORKDIR "/src/Cars"
RUN dotnet build "Cars.csproj" -c $BUILD_CONFIGURATION -o /app/build 

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Cars.csproj" -c $BUILD_CONFIGURATION -o /app/publish --property:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cars.dll"]
