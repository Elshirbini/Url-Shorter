FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

COPY ["Url-Shorter.csproj", "./"]
RUN dotnet restore

COPY . .

RUN dotnet publish \
    -c Release \
    --no-restore \
    -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 5000

ENTRYPOINT ["dotnet", "Url-Shorter.dll"]