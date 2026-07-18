# =========================
# Build Stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY Url-Shorter.csproj .
RUN dotnet restore

COPY . .

RUN dotnet publish \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# =========================
# Runtime Stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:5000

EXPOSE 5000

ENTRYPOINT ["dotnet", "Url-Shorter.dll"]