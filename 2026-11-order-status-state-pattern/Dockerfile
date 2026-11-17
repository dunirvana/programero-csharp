FROM mcr.microsoft.com/dotnet/sdk:11.0 AS tests
WORKDIR /repo
COPY . .
RUN dotnet build tests/OrderWorkflow.Tests
ENTRYPOINT ["dotnet", "test", "--project", "tests/OrderWorkflow.Tests", "--no-build"]
