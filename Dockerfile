#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build

RUN apt-get update \ 
    && apt-get install -y --no-install-recommends libgdiplus libc6-dev \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

COPY . /src

WORKDIR "/src/picow-cgm-server"

RUN dotnet restore "picow-cgm-server.csproj"

RUN dotnet build "picow-cgm-server.csproj" --configuration Debug


WORKDIR "/src/picow-cgm-server/bin/Debug/net5.0/"

ENTRYPOINT "dotnet" "picow-cgm-server.dll"

EXPOSE 5002