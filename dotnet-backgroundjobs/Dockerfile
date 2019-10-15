FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY dotnet-backgroundjobs.csproj ./dotnet-backgroundjobs/
RUN dotnet restore ./dotnet-backgroundjobs/dotnet-backgroundjobs.csproj

# copy everything else and build app
COPY . ./dotnet-backgroundjobs/
WORKDIR /app/dotnet-backgroundjobs
RUN dotnet publish dotnet-backgroundjobs.csproj -c Release -o out


FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/dotnet-backgroundjobs/out ./
ENTRYPOINT ["dotnet", "dotnet-backgroundjobs.dll"]