FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app

COPY /publish .

EXPOSE 80

EXPOSE 443

CMD ["dotnet", "LockerService.API.dll"]