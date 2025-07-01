# ======= BASE IMAGE: ASP.NET 8.0 ==========
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Define o fuso horário para São Paulo (ou outro fuso desejado)
RUN ln -sf /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime


#RUN apt-get update && apt-get install -y \
#    libldap-2.5-0 \
#    libsasl2-2 \
#    ca-certificates \
#    && rm -rf /var/lib/apt/lists/*

# Define o fuso horário para São Paulo (ou outro fuso desejado)
RUN ln -sf /usr/share/zoneinfo/America/Sao_Paulo /etc/localtime
 
# Instala pacotes necessários
RUN echo "deb http://deb.debian.org/debian bookworm contrib non-free" > /etc/apt/sources.list.d/contrib.list
RUN apt-get update && apt-get install -y libldap-2.5-0 libsasl2-2 libgdiplus ttf-mscorefonts-installer fontconfig && rm -rf /var/lib/apt/lists/*

RUN fc-cache -f -v

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# ======= BUILD IMAGE: .NET SDK 8.0 ==========
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/HawkId.Login.Api/HawkId.WebApi.csproj", "src/HawkId.Login.Api/"]
COPY ["src/HawkId.CrossCutting/HawkId.CrossCutting.csproj", "src/HawkId.CrossCutting/"]
COPY ["src/HawkId.Application/HawkId.Application.csproj", "src/HawkId.Application/"]
COPY ["src/HawkId.Core/HawkId.Domain.csproj", "src/HawkId.Core/"]
COPY ["src/HawkId.Data/HawkId.Data.csproj", "src/HawkId.Data/"]

RUN dotnet restore "./src/HawkId.Login.Api/HawkId.WebApi.csproj"
COPY . .
WORKDIR "/src/src/HawkId.Login.Api"
RUN dotnet build "./HawkId.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# ======= PUBLISH IMAGE ==========
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./HawkId.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# ======= FINAL IMAGE ==========
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HawkId.WebApi.dll"]
