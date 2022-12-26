#FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine AS base
#WORKDIR /
#
## copy csproj and restore as distinct layers
#FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
#WORKDIR /
#COPY ["Neal.Twitter.Infrastructure.StreamReader/Neal.Twitter.Infrastructure.StreamReader.csproj", "Neal.Twitter.Infrastructure.StreamReader/"]
#COPY ["Neal.Twitter.Application/Neal.Twitter.Application.csproj", "Neal.Twitter.Application/"]
#COPY ["Neal.Twitter.Core/Neal.Twitter.Core.csproj", "Neal.Twitter.Core/"]
#RUN dotnet restore Neal.Twitter.Infrastructure.StreamReader.csproj -r linux-musl-x64 /p:PublishReadyToRun=true
#COPY . .
#WORKDIR /src/Neal.Twitter.Infrastructure.StreamReader
#RUN dotnet publish -c Release -o /publish -r linux-musl-x64 --self-contained true --no-restore /p:PublishTrimmed=true /p:PublishReadyToRun=true /p:PublishSingleFile=true
#
## final stage/image
#FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine-amd64
#WORKDIR /app
#COPY --from=build /publish .
#ENTRYPOINT ["dotnet", "Neal.Twitter.Infrastructure.StreamReader.dll]
#

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS base
WORKDIR /src

# copy csproj and restore as distinct layers
FROM base AS build
COPY Neal.Twitter.Infrastructure.StreamReader/*.csproj .
RUN dotnet restore -r linux-musl-x64 /p:PublishReadyToRun=true

# copy everything else and build app
FROM build as publish
COPY Neal.Twitter.Infrastructure.StreamReader/. .
RUN dotnet publish -c Release -o /publish -r linux-musl-x64 --self-contained true --no-restore /p:PublishSingleFile=true

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine-amd64 AS FINAL
WORKDIR /app
COPY --from=publish /publish .
ENTRYPOINT ["./Neal.Twitter.Infrastructure.StreamReader"]
