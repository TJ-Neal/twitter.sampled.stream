#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Neal.Twitter.Infrastructure.StreamReader/Neal.Twitter.Infrastructure.StreamReader.csproj", "Neal.Twitter.Infrastructure.StreamReader/"]
COPY ["Neal.Twitter.Core/Neal.Twitter.Core.csproj", "Neal.Twitter.Core/"]
COPY ["Neal.Twitter.Faster.Client/Neal.Twitter.Faster.Client.csproj", "Neal.Twitter.Faster.Client/"]
COPY ["Neal.Twitter.Application/Neal.Twitter.Application.csproj", "Neal.Twitter.Application/"]
COPY ["Neal.Twitter.Kafka.Client/Neal.Twitter.Kafka.Client.csproj", "Neal.Twitter.Kafka.Client/"]
COPY ["Neal.Twitter.LinqToTwitter.Client/Neal.Twitter.LinqToTwitter.Client.csproj", "Neal.Twitter.LinqToTwitter.Client/"]
COPY ["Neal.Twitter.Simple.Client/Neal.Twitter.Simple.Client.csproj", "Neal.Twitter.Simple.Client/"]
RUN dotnet restore "Neal.Twitter.Infrastructure.StreamReader/Neal.Twitter.Infrastructure.StreamReader.csproj"
COPY . .
WORKDIR "/src/Neal.Twitter.Infrastructure.StreamReader"
RUN dotnet build "Neal.Twitter.Infrastructure.StreamReader.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Neal.Twitter.Infrastructure.StreamReader.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Neal.Twitter.Infrastructure.StreamReader.dll"]