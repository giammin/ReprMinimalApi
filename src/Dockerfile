FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build-env
WORKDIR /App

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore ReprMinimalApi/ReprMinimalApi.csproj
# Build and publish a release
RUN dotnet publish ReprMinimalApi/ReprMinimalApi.csproj -c Release -o out --no-restore  /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine

#timezone
RUN apk add --no-cache tzdata icu-libs icu-data-full
ENV TZ=Europe/Rome
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

EXPOSE 5000

ENV Kestrel__Endpoints__Http__Url=http://0.0.0.0:5000
# enviroment meglio impostartlo a runtime del container
# ENV ASPNETCORE_ENVIRONMENT=Staging
WORKDIR /App
COPY --from=build-env /App/out .
HEALTHCHECK CMD wget --no-verbose --tries=1 --spider http://localhost:5000/ || exit 1
ENTRYPOINT ["dotnet", "ReprMinimalApi.dll"]