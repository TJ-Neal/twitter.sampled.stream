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