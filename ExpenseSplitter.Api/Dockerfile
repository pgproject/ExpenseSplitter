FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

WORKDIR /app

EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["ExpenseSplitter.Api/ExpenseSplitter.Api.csproj", "ExpenseSplitter.Api/"]
COPY ["ExpenseSplitter.Application/ExpenseSplitter.Application.csproj", "ExpenseSplitter.Application/"]
COPY ["ExpenseSplitter.Domain/ExpenseSplitter.Domain.csproj", "ExpenseSplitter.Domain/"]
COPY ["ExpenseSplitter.Infrastructure/ExpenseSplitter.Infrastructure.csproj", "ExpenseSplitter.Infrastructure/"]

RUN dotnet restore "ExpenseSplitter.Api/ExpenseSplitter.Api.csproj"

COPY . .

WORKDIR /src/ExpenseSplitter.Api

RUN dotnet publish "ExpenseSplitter.Api.csproj" \
	-c Release \
	-o /app/publish \
	/p:UseAppHost=false

FROM base AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "ExpenseSplitter.Api.dll"]