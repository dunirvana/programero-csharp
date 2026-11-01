FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /repo
COPY . .
RUN dotnet publish src/RateLimitDemo -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS app
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "RateLimitDemo.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS tests
WORKDIR /repo
COPY . .
ENTRYPOINT ["dotnet", "test", "tests/RateLimitDemo.Tests"]
