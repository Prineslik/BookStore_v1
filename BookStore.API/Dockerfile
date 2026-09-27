FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["BookStore.API/BookStore.API.csproj", "BookStore.API/"]
COPY ["BookStore.Application/BookStore.Application.csproj", "BookStore.Application/"]
COPY ["BookStore.Infrastructure/BookStore.Infrastructure.csproj", "BookStore.Infrastructure/"]
COPY ["BookStore.Core/BookStore.Core.csproj", "BookStore.Core/"]

RUN dotnet restore "BookStore.API/BookStore.API.csproj"

COPY . .

WORKDIR "/src/BookStore.API"
RUN dotnet build "BookStore.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookStore.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

RUN useradd --create-home --shell /bin/bash appuser

COPY --from=publish /app/publish .

RUN chown -R appuser:appuser /app
USER appuser

EXPOSE 8080

ENTRYPOINT ["dotnet", "BookStore.API.dll"]