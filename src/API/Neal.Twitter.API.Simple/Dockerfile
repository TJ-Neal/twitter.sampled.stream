ARG project_name="Neal.Twitter.API.Simple"

FROM mcr.microsoft.com/dotnet/aspnet:7.0.1-alpine3.16-amd64 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443
ENV TMPDIR=/app/tmp                       
ENV ASPNETCORE_ENVIRONMENT=Docker
ENV ASPNETCORE_URLS=https://+:443;http://+:80
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=f9ea0a74-d7b3-49ae-b18b-25630bfbea10
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/https/aspnetapp.pfx
ENV COMPlus_EnableDiagnostics=0

# Add support files for Kestrel, ICU, and SSL
ONBUILD RUN apk update \
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

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
ARG project_name
COPY ["API/$project_name/*.csproj", "$project_name/"]
RUN dotnet restore $project_name/$project_name.csproj
COPY . .
WORKDIR /src/API/$project_name
RUN dotnet build $project_name.csproj -c Docker -o /app/build

FROM build AS publish
RUN dotnet publish $project_name.csproj -c Docker -o /app/publish
# Create dev cert with GUID password for debugging in container
RUN dotnet dev-certs https -ep /app/https/aspnetapp.pfx -p f9ea0a74-d7b3-49ae-b18b-25630bfbea10

FROM base AS FINAL
WORKDIR /app
COPY --from=publish /app/https/aspnetapp.pfx ./https/aspnetapp.pfx
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neal.Twitter.API.Simple.dll"]