FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["LockerService.API/LockerService.API.csproj", "LockerService.API/"]
RUN dotnet restore "LockerService.API/LockerService.API.csproj"
COPY . .
WORKDIR "/src/LockerService.API"
RUN dotnet build "LockerService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "LockerService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LockerService.API.dll"]
