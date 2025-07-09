# Usar la imagen oficial de .NET 7.0
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Usar la imagen de SDK para compilar
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copiar archivos de proyecto
COPY ["SpaAdmin.csproj", "./"]
RUN dotnet restore "SpaAdmin.csproj"

# Copiar todo el código fuente
COPY . .
WORKDIR "/src"

# Compilar la aplicación
RUN dotnet build "SpaAdmin.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "SpaAdmin.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app

# Copiar archivos publicados
COPY --from=publish /app/publish .

# Crear usuario no-root para seguridad
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

# Variables de entorno por defecto
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost/health || exit 1

ENTRYPOINT ["dotnet", "SpaAdmin.dll"] 