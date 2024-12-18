#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["CRUDing/CRUDing.API.csproj", "CRUDing/"]
COPY ["CRUDing.DAL/CRUDing.DAL.csproj", "CRUDing.DAL/"]
COPY ["CRUDing.Core/CRUDing.Core.csproj", "CRUDing.Core/"]
COPY ["CRUDing.Domain/CRUDing.Domain.csproj", "CRUDing.Domain/"]
RUN dotnet restore "CRUDing/CRUDing.API.csproj"
COPY . .
WORKDIR "/src/CRUDing"
RUN dotnet build "CRUDing.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CRUDing.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CRUDing.API.dll"]