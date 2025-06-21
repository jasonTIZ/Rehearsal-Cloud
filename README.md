# Rehearsal Cloud

---

## Características

- **Gestión de canciones**: Crea, edita, elimina y consulta canciones con sus archivos de audio y portada.
- **Carga de archivos**: Soporte para subir imágenes y archivos de audio comprimidos en ZIP.
- **Setlists**: Organiza canciones en listas de reproducción para ensayos o presentaciones.
- **Descarga de audios**: Descarga archivos individuales o setlists completos en formato ZIP.
- **API RESTful**: Endpoints claros y documentados con Swagger.
- **Autenticación y usuarios**: (Opcional, según configuración).

---

## Estructura del Proyecto

```
api/
│
├── Controllers/         # Controladores de la API (Song, Setlist, Auth, etc.)
├── Data/                # Contexto de base de datos (Entity Framework)
├── Dtos/                # Data Transfer Objects para requests y responses
├── Mappers/             # Conversores entre modelos y DTOs
├── Models/              # Modelos de datos (Song, AudioFile, User, etc.)
├── Migrations/          # Migraciones de base de datos
├── Properties/          # launchSettings.json y otros archivos de configuración
├── UploadedAudioFiles/  # Carpeta donde se guardan los audios subidos
├── UploadedImages/      # Carpeta donde se guardan las portadas subidas
├── Program.cs           # Configuración principal de la aplicación
├── appsettings.json     # Configuración general (conexión a BD, etc.)
└── ...
```

---

## Requisitos

- [.NET 6.0 SDK o superior](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) o base de datos compatible

---

## Configuración y Ejecución

1. **Clona el repositorio y navega a la carpeta `api`:**
   ```sh
   cd c:\xampp\htdocs\Rehearsal-Cloud\api
   ```

2. **Configura la cadena de conexión en `appsettings.json`:**
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=TU_SERVIDOR;Database=RehearsalCloud;User Id=USUARIO;Password=CONTRASEÑA;"
   }
   ```

3. **Apunta la API a la IP de tu red local o a localhost (por defecto, localhost):**
   - Edita `Properties/launchSettings.json`:
     ```json
     "applicationUrl": "http://localhost:5198;https://localhost:7078"
     ```

4. **Aplica las migraciones y crea la base de datos:**
   ```sh
   dotnet ef migrations add [nombre de la migracion]
   dotnet ef database update
   ```

5. **Ejecuta la API:**
   ```sh
   dotnet build
   dotnet run
   ```
   La API estará disponible en `http://localhost:5198` y/o `https://localhost:7078`.

6. **Accede a la documentación Swagger:**
   - [http://localhost:5198/swagger](http://localhost:5198/swagger)
   - [https://localhost:7078/swagger](https://localhost:7078/swagger)

---

## Uso de la API

### Endpoints principales

- **GET /api/Song**  
  Lista todas las canciones.

- **GET /api/Song/{id}**  
  Obtiene los detalles de una canción. 

- **POST /api/Song/create-song**  
  Crea una nueva canción.  
  Parámetros:  
  - `SongName` (string, requerido)
  - `Artist` (string, requerido)
  - `BPM` (int, requerido)
  - `Tone` (string, requerido)
  - `CoverImage` (IFormFile, requerido)
  - `ZipFile` (IFormFile, requerido, archivos de audio .mp3/.wav en un .zip)

- **PUT /api/Song/{id}**  
  Actualiza una canción existente.

- **DELETE /api/Song/{id}**  
  Elimina una canción y sus archivos asociados.

- **GET /api/Song/{id}/audio/{audioId}**  
  Descarga un archivo de audio específico.

- **GET /api/Setlist/SetlistWithSongs**  
  Descarga un setlist completo con todos los audios en un ZIP.

---

## Notas para Android

- Si tu app Android consume la API por HTTP y ves el error  
  `CLEARTEXT communication to IP not permitted by network security policy`,  
  debes permitir tráfico HTTP en tu configuración de red de Android.  
  Consulta la sección de seguridad de red en la documentación de Android.

---


## Contacto

Para dudas o soporte, contacta al equipo de desarrollo o revisa la documentación interna del proyecto.

---