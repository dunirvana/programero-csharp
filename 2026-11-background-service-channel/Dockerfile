FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /repo
COPY . .
RUN dotnet publish src/JobQueueDemo -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS app
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "JobQueueDemo.dll"]

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS tests
WORKDIR /repo
COPY . .
ENTRYPOINT ["dotnet", "test", "tests/JobQueueDemo.Tests"]
