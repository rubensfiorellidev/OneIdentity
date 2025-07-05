# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Define o fuso horário para São Paulo (ou outro fuso desejado)
RUN ln -sf /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OneID.Api/OneID.Api.csproj", "OneID.Api/"]
COPY ["OneID.Application/OneID.Application.csproj", "OneID.Application/"]
COPY ["OneID.Domain/OneID.Domain.csproj", "OneID.Domain/"]
COPY ["OneID.Data/OneID.Data.csproj", "OneID.Data/"]
COPY ["OneID.Shared/OneID.Shared.csproj", "OneID.Shared/"]
COPY ["OneID.Messaging/OneID.Messaging.csproj", "OneID.Messaging/"]
RUN dotnet restore "./OneID.Api/OneID.Api.csproj"
COPY . .
WORKDIR "/src/OneID.Api"
RUN dotnet build "./OneID.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./OneID.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "OneID.Api.dll"]