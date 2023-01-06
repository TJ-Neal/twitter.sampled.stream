FROM mcr.microsoft.com/dotnet/sdk:7.0.101-alpine3.17-amd64 AS base
WORKDIR /src

FROM base AS build
COPY ["Neal.Twitter.Infrastructure.StreamReader/.", "Neal.Twitter.Infrastructure.StreamReader/"]
COPY ["Neal.Twitter.Core/.", "Neal.Twitter.Core/"]
COPY ["Neal.Twitter.Application/.", "Neal.Twitter.Application/"]
COPY ["Neal.Twitter.Simple.Client/.", "Neal.Twitter.Simple.Client/"]
COPY ["Neal.Twitter.Faster.Client/.", "Neal.Twitter.Faster.Client/"]
COPY ["Neal.Twitter.Kafka.Client/.", "Neal.Twitter.Kafka.Client/"]
COPY ["Neal.Twitter.LinqToTwitter.Client/.", "Neal.Twitter.LinqToTwitter.Client/"]
RUN dotnet publish Neal.Twitter.Infrastructure.StreamReader -c Release -o /publish -r linux-musl-x64 --self-contained true --restore /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/runtime:7.0.1-alpine3.17-amd64 AS FINAL
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["./Neal.Twitter.Infrastructure.StreamReader"]
