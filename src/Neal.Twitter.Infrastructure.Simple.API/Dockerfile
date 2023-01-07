FROM mcr.microsoft.com/dotnet/sdk:7.0.101-alpine3.17-amd64 AS base
WORKDIR /src
# Create dev cert with GUID password for debugging in container
RUN dotnet dev-certs https -ep /app/ssl/private/dev-cert.pfx -p f9ea0a74-d7b3-49ae-b18b-25630bfbea10
EXPOSE 4000
EXPOSE 4001

FROM base AS build
COPY ["Neal.Twitter.Infrastructure.Simple.API/.", "Neal.Twitter.Infrastructure.Simple.API/"]
COPY ["Neal.Twitter.Infrastructure.Simple.Repository/.", "Neal.Twitter.Infrastructure.Simple.Repository/"]
COPY ["Neal.Twitter.Core/.", "Neal.Twitter.Core/"]
COPY ["Neal.Twitter.Application/.", "Neal.Twitter.Application/"]
COPY ["Neal.Twitter.Simple.Client/.", "Neal.Twitter.Simple.Client/"]
RUN dotnet publish Neal.Twitter.Infrastructure.Simple.API -c Docker -o /publish -r linux-musl-x64 --self-contained true --restore /p:PublishSingleFile=true

FROM mcr.microsoft.com/dotnet/aspnet:7.0.1-alpine3.17-amd64 AS FINAL
WORKDIR /app
COPY --from=build /publish .
COPY /ssl/certs/. /app/ssl/private/
ENTRYPOINT ["./Neal.Twitter.Infrastructure.Simple.API"]

# Add support files for Kestrel, ICU, and SSL
RUN apk update \
    && apk add --no-cache \
    icu-libs \
    krb5-libs \
    libgcc \
    libintl \
    libssl1.1 \
    libstdc++ \
    zlib \
    && apk add --no-cache --virtual .build-deps \
    curl \
    msttcorefonts-installer \
    fontconfig \
    && update-ms-fonts \
    && fc-cache -f \
    && apk del .build-deps

RUN chown -R root /app/ssl/private/