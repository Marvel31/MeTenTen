# API Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["MeTenTenAPI/MeTenTenAPI.csproj", "MeTenTenAPI/"]
COPY ["MeTenTenBlazor/MeTenTenBlazor.csproj", "MeTenTenBlazor/"]
RUN dotnet restore "MeTenTenAPI/MeTenTenAPI.csproj"
COPY . .
WORKDIR "/src/MeTenTenAPI"
RUN dotnet build "MeTenTenAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MeTenTenAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeTenTenAPI.dll"]
