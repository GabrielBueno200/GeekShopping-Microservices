# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
RUN mkdir /app
WORKDIR /app

# copy csproj and restore as distinct layers
COPY GeekShopping.Web.csproj .
RUN dotnet restore

# copy everything else and build app
COPY . .
RUN dotnet publish -c release -o /dist --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /dist
COPY --from=build /dist ./
ENTRYPOINT ["dotnet", "GeekShopping.Web.dll"]