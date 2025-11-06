FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/Taskify.Api/Taskify.Api.csproj", "src/Taskify.Api/"]
COPY ["src/Taskify.Application/Taskify.Application.csproj", "src/Taskify.Application/"]
COPY ["src/Taskify.Core/Taskify.Core.csproj", "src/Taskify.Core/"]
COPY ["src/Taskify.Infrastructure/Taskify.Infrastructure.csproj", "src/Taskify.Infrastructure/"]
RUN dotnet restore "src/Taskify.Api/Taskify.Api.csproj"
COPY . .
WORKDIR "/src/src/Taskify.Api"
RUN dotnet build "Taskify.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Taskify.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Taskify.Api.dll"]
