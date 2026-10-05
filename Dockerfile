# API de Buscador interno (.NET 10). Construir desde la raíz del repositorio:
#   docker build -t buscador-api .
# Configuración por variables de entorno (ver DESPLIEGUE.md); nunca va dentro de la imagen.

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Primero los .csproj: así la restauración de paquetes se cachea mientras no cambien las dependencias
COPY internal-search.Domain/internal-search-backend.Domain.csproj internal-search.Domain/
COPY internal-search-backend.Business/internal-search-backend.Business.csproj internal-search-backend.Business/
COPY internal-search-backend.Infraestructure/internal-search-backend.Infraestructure.csproj internal-search-backend.Infraestructure/
COPY internal-search-backend/internal-search-backend.Api.csproj internal-search-backend/
RUN dotnet restore internal-search-backend/internal-search-backend.Api.csproj

COPY . .
RUN dotnet publish internal-search-backend/internal-search-backend.Api.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .

# Archivos del historial de descargas. La API corre sin privilegios y no puede escribir en /app, así que
# usan /data/historial, que pertenece a ese usuario. docker-compose.yml monta ahí un volumen para que
# los archivos no se pierdan al recrear el contenedor.
RUN mkdir -p /data/historial && chown -R $APP_UID /data

# Detrás de un túnel o proxy HTTPS; el contenedor solo habla HTTP internamente
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    Historial__Carpeta=/data/historial
EXPOSE 8080

# Corre sin privilegios
USER $APP_UID
ENTRYPOINT ["dotnet", "internal-search-backend.Api.dll"]
