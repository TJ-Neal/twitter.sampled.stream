FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src

# copy csproj and restore as distinct layers
COPY Neal.Twitter.Api/*.csproj .
RUN dotnet restore -r linux-musl-x64 /p:PublishReadyToRun=true

# copy everything else and build app
COPY Neal.Twitter.Api/. .
RUN dotnet publish -c Release -o /publish -r linux-musl-x64 --self-contained true --no-restore /p:PublishSingleFile=true

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine-amd64
WORKDIR /app
COPY --from=build /publish .
EXPOSE 443

ENV \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    LC_ALL=en_US.UTF-8 \
    LANG=en_US.UTF-8

ENTRYPOINT ["./Neal.Twitter.Api"]

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