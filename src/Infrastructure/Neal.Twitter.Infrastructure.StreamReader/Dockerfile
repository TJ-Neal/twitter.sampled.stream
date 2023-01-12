ARG project_name="Neal.Twitter.Infrastructure.StreamReader"

FROM mcr.microsoft.com/dotnet/runtime:7.0.1-alpine3.16-amd64 AS base
WORKDIR /app
ENV TMPDIR=/app/tmp                       
ENV DOTNET_ENVIRONMENT=Docker
ENV COMPlus_EnableDiagnostics=0

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
ARG project_name
COPY ["Infrastructure/$project_name/*.csproj", "$project_name/"]
RUN dotnet restore $project_name/$project_name.csproj
COPY . .
WORKDIR /src/Infrastructure/$project_name
RUN dotnet build $project_name.csproj -c Docker -o /app/build

FROM build AS publish
RUN dotnet publish $project_name.csproj -c Docker -o /app/publish

FROM base AS FINAL
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neal.Twitter.Infrastructure.StreamReader.dll"]