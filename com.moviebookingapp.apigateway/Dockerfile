#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["com.moviebookingapp.apigateway/com.moviebookingapp.apigateway.csproj", "com.moviebookingapp.apigateway/"]
RUN dotnet restore "com.moviebookingapp.apigateway/com.moviebookingapp.apigateway.csproj"
COPY . .
WORKDIR "/src/com.moviebookingapp.apigateway"
RUN dotnet build "com.moviebookingapp.apigateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "com.moviebookingapp.apigateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "com.moviebookingapp.apigateway.dll"]