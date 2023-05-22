#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/windows:ltsc2019

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

COPY ["picow-cgm-server/picow-cgm-server.csproj", "picow-cgm-server/"]
COPY ["picow-cgm-core/picow-cgm-core.csproj", "picow-cgm-core/"]

RUN dotnet restore "picow-cgm-server/picow-cgm-server.csproj"
COPY . .

WORKDIR "/src/picow-cgm-server"
RUN dotnet build "picow-cgm-server.csproj" -c Debug -o /app/build

WORKDIR "/src/picow-cgm-server/bin/Debug/net7.0-windows"

ENTRYPOINT ["dotnet", "picow-cgm-server.dll"]

EXPOSE 5002