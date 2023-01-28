FROM mcr.microsoft.com/dotnet/runtime:latest
COPY kataviv/bin/Release/net7.0/publish /App
WORKDIR /App
ENTRYPOINT ["dotnet", "kataviv.dll"]