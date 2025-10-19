FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy csproj files
COPY ["src/CanadaSoftware.ApiDotNet.ServiceHost/CanadaSoftware.ApiDotNet.ServiceHost.csproj", "CanadaSoftware.ApiDotNet.ServiceHost/"]
COPY ["src/CanadaSoftware.ApiDotNet.Application/CanadaSoftware.ApiDotNet.Application.csproj", "CanadaSoftware.ApiDotNet.Application/"]
COPY ["src/CanadaSoftware.ApiDotNet.Domain/CanadaSoftware.ApiDotNet.Domain.csproj", "CanadaSoftware.ApiDotNet.Domain/"]
COPY ["src/CanadaSoftware.ApiDotNet.EntityFramework/CanadaSoftware.ApiDotNet.EntityFramework.csproj", "CanadaSoftware.ApiDotNet.EntityFramework/"]
COPY ["src/CanadaSoftware.ApiDotNet.Common/CanadaSoftware.ApiDotNet.Common.csproj", "CanadaSoftware.ApiDotNet.Common/"]
COPY ["src/CanadaSoftware.ApiDotNet.Services/CanadaSoftware.ApiDotNet.Services.csproj", "CanadaSoftware.ApiDotNet.Services/"]
COPY ["src/CanadaSoftware.ApiDotNet.HttpClient/CanadaSoftware.ApiDotNet.HttpClient.csproj", "CanadaSoftware.ApiDotNet.HttpClient/"]
COPY ["src/CanadaSoftware.ApiDotNet.Globalization/CanadaSoftware.ApiDotNet.Globalization.csproj", "CanadaSoftware.ApiDotNet.Globalization/"]
COPY ["src/CanadaSoftware.ApiDotNet.Application.Ecst/CanadaSoftware.ApiDotNet.Application.Ecst.csproj", "CanadaSoftware.ApiDotNet.Application.Ecst/"]

# Restore
RUN dotnet restore "CanadaSoftware.ApiDotNet.ServiceHost/CanadaSoftware.ApiDotNet.ServiceHost.csproj"

# Copy source
COPY src/ .

# Build
WORKDIR "/src/CanadaSoftware.ApiDotNet.ServiceHost"
RUN dotnet build "CanadaSoftware.ApiDotNet.ServiceHost.csproj" -c Release -o /app/build

# Publish
FROM build AS publish
RUN dotnet publish "CanadaSoftware.ApiDotNet.ServiceHost.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CanadaSoftware.ApiDotNet.ServiceHost.dll"]

