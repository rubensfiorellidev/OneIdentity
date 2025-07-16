FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY OneID.LoginUI/OneID.LoginUI.csproj OneID.LoginUI/
RUN dotnet restore "OneID.LoginUI/OneID.LoginUI.csproj"

COPY . .
WORKDIR "/src/OneID.LoginUI"
RUN dotnet build "OneID.LoginUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OneID.LoginUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "OneID.LoginUI.dll"]
