#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:5.0-nanoserver-1903 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:5.0-nanoserver-1903 AS build
WORKDIR /src
COPY ["ArchiCAD.DB.WebAPI/ArchiCAD.DB.WebAPI.csproj", "ArchiCAD.DB.WebAPI/"]
RUN dotnet restore "ArchiCAD.DB.WebAPI/ArchiCAD.DB.WebAPI.csproj"
COPY . .
WORKDIR "/src/ArchiCAD.DB.WebAPI"
RUN dotnet build "ArchiCAD.DB.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ArchiCAD.DB.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArchiCAD.DB.WebAPI.dll"]