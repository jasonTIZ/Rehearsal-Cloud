using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Models;
using api.Dtos.Song;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        
        // Directorio base para las imágenes
        private readonly string _imageUploadDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "UploadedImages"
        );

        // Directorio base para los archivos de audio
        private readonly string _audioUploadDirectory = Path.Combine(
            Directory.GetCurrentDirectory(),
            "UploadedAudioFiles"
        );

        public SongController(ApplicationDbContext context)
        {
            _context = context;

            // Los directorios para almacenar las imágenes y archivos de audio existen o se crean si no existen
            if (!Directory.Exists(_imageUploadDirectory))
            {
                Directory.CreateDirectory(_imageUploadDirectory);
            }

            if (!Directory.Exists(_audioUploadDirectory))
            {
                Directory.CreateDirectory(_audioUploadDirectory);
            }
        }

        // POST: api/Song/create-song
        [HttpPost("create-song")]
        public async Task<IActionResult> CreateSong([FromForm] CreateSongRequestDto request)
        {
            if (request.BPM < 40 || request.BPM > 280)
            {
                return BadRequest("El BPM debe estar entre 40 y 280.");
            }

            if (request.ZipFile == null || request.ZipFile.Length == 0)
            {
                return BadRequest("No se ha enviado ningún archivo .zip.");
            }

            if (request.CoverImage == null || request.CoverImage.Length == 0)
            {
                return BadRequest("No se ha enviado la imagen de portada.");
            }

            // Validar que el archivo es un .zip
            if (Path.GetExtension(request.ZipFile.FileName).ToLower() != ".zip")
            {
                return BadRequest("El archivo debe ser un .zip.");
            }

            // Validar que el archivo de imagen sea de tipo .jpg, .jpeg, o .png
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var imageExtension = Path.GetExtension(request.CoverImage.FileName).ToLower();
            if (!allowedImageExtensions.Contains(imageExtension))
            {
                return BadRequest("El archivo de imagen debe ser .jpg, .jpeg o .png.");
            }

            // Guardar la imagen de portada
            var uniqueImageFileName = Guid.NewGuid().ToString() + imageExtension;
            var imagePath = Path.Combine(_imageUploadDirectory, uniqueImageFileName);

            using (var imageStream = new FileStream(imagePath, FileMode.Create))
            {
                await request.CoverImage.CopyToAsync(imageStream);
            }

            // Crear la canción en la base de datos
            var song = new Song
            {
                SongName = request.SongName,
                Artist = request.Artist,
                BPM = request.BPM,
                Tone = request.Tone,
                CreatedAt = DateTime.UtcNow,
                CoverImage = imagePath
            };

            _context.Songs.Add(song);
            await _context.SaveChangesAsync();

            // Crear un directorio específico para los archivos de audio de esta canción
            var songAudioDirectory = Path.Combine(_audioUploadDirectory, song.Id.ToString());
            if (!Directory.Exists(songAudioDirectory))
            {
                Directory.CreateDirectory(songAudioDirectory);
            }

            // Extraer los archivos del .zip
            using (var stream = request.ZipFile.OpenReadStream())
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    var fileExtension = Path.GetExtension(entry.FullName).ToLower();
                    if (fileExtension != ".mp3" && fileExtension != ".wav")
                    {
                        continue;
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    var filePath = Path.Combine(songAudioDirectory, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await entry.Open().CopyToAsync(fileStream);
                    }

                    var audioFileRecord = new AudioFile
                    {
                        FileName = uniqueFileName,
                        FilePath = filePath,
                        FileExtension = fileExtension,
                        FileSize = entry.Length,
                        SongId = song.Id,
                    };

                    _context.AudioFiles.Add(audioFileRecord);
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Canción creada, imagen de portada subida y archivos de audio asociados correctamente.");
        }


        // GET: api/Song/get-songs
        [HttpGet("get-songs")]
        public async Task<IActionResult> GetSongs()
        {
            try
            {
                var songs = await _context.Songs
                    .Select(s => new
                    {
                        s.Id,
                        s.SongName,
                        s.Artist,
                        s.BPM,
                        s.Tone,
                        CoverImage = Path.GetFileName(s.CoverImage), // Solo el nombre del archivo
                        s.CreatedAt
                    })
                    .ToListAsync();

                if (songs == null || !songs.Any())
                {
                    return NotFound("No se encontraron canciones.");
                }

                return Ok(songs);
            }
            catch (Exception ex)
            {
                // Log de error
                Console.WriteLine($"Error al obtener canciones: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET: api/Song/get-song/{id}
        [HttpGet("get-song/{id}")]
        public async Task<IActionResult> GetSong(int id)
        {
            try
            {
                var song = await _context.Songs
                    .Include(s => s.AudioFiles) // Incluir archivos de audio relacionados
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (song == null)
                {
                    return NotFound("Canción no encontrada.");
                }

                var songDetails = new
                {
                    song.Id,
                    song.SongName,
                    song.Artist,
                    song.BPM,
                    song.Tone,
                    CoverImage = Path.GetFileName(song.CoverImage), // Solo el nombre del archivo
                    song.CreatedAt,
                    AudioFiles = song.AudioFiles.Select(a => new
                    {
                        a.Id,
                        a.FileName,
                        a.FilePath,
                        a.FileExtension,
                        a.FileSize
                    })
                };

                return Ok(songDetails);
            }
            catch (Exception ex)
            {
                // Log de error
                Console.WriteLine($"Error al obtener la canción: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
        // DELETE: api/Song/delete-song/{id}
        [HttpDelete("delete-song/{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            try
            {
                var song = await _context.Songs
                    .Include(s => s.AudioFiles) // Incluir archivos de audio relacionados
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (song == null)
                {
                    return NotFound("Canción no encontrada.");
                }

                // Eliminar los archivos de audio del sistema de archivos
                foreach (var audioFile in song.AudioFiles)
                {
                    var filePath = Path.Combine(_audioUploadDirectory, song.Id.ToString(), audioFile.FileName);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                // Eliminar la imagen de portada del sistema de archivos
                if (System.IO.File.Exists(song.CoverImage))
                {
                    System.IO.File.Delete(song.CoverImage);
                }

                // Eliminar la carpeta que contiene los archivos de audio de esta canción
                var songAudioDirectory = Path.Combine(_audioUploadDirectory, song.Id.ToString());
                if (Directory.Exists(songAudioDirectory))
                {
                    Directory.Delete(songAudioDirectory, true); // true para eliminar también los subdirectorios y archivos
                }

                // Eliminar la canción y sus archivos de audio de la base de datos
                _context.AudioFiles.RemoveRange(song.AudioFiles);
                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();

                return Ok("Canción eliminada correctamente.");
            }
            catch (Exception ex)
            {
                // Log de error
                Console.WriteLine($"Error al eliminar la canción: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
 
/*
        // PUT: api/Song/update-song/{id}
        [HttpPut("update-song/{id}")]
        public async Task<IActionResult> UpdateSong(
            int id,
            [FromForm] string SongName,
            [FromForm] string Artist,
            [FromForm] int BPM,
            [FromForm] string Tone,
            [FromForm] IFormFile coverImage
        )
        {
            if (BPM < 40 || BPM > 280)
            {
                return BadRequest("El BPM debe estar entre 40 y 280.");
            }

            // Validación de archivos
            if (coverImage == null || coverImage.Length == 0)
            {
                return BadRequest("No se ha enviado la imagen de portada.");
            }

            // Validar que el archivo de imagen sea de tipo .jpg, .jpeg, o .png
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var imageExtension = Path.GetExtension(coverImage.FileName).ToLower();
            if (!allowedImageExtensions.Contains(imageExtension))
            {
                return BadRequest("El archivo de imagen debe ser .jpg, .jpeg o .png.");
            }

            // Buscar la canción en la base de datos
            var song = await _context.Songs.FindAsync(id);
            if (song == null)
            {
                return NotFound("Canción no encontrada.");
            }

            // Actualizar los campos de la canción
            song.SongName = SongName;
            song.Artist = Artist;
            song.BPM = BPM;
            song.Tone = Tone;

            // Guardar la imagen de portada si se proporciona una nueva
            if (coverImage != null && coverImage.Length > 0)
            {
                var uniqueImageFileName = Guid.NewGuid().ToString() + imageExtension;
                var imagePath = Path.Combine(_imageUploadDirectory, uniqueImageFileName);

                using (var imageStream = new FileStream(imagePath, FileMode.Create))
                {
                    await coverImage.CopyToAsync(imageStream);
                }

                // Eliminar la imagen anterior del sistema de archivos
                if (System.IO.File.Exists(song.CoverImage))
                {
                    System.IO.File.Delete(song.CoverImage);
                }

                song.CoverImage = imagePath; // Actualizar la ruta de la imagen en la base de datos
            }

            await _context.SaveChangesAsync();

            return Ok("Canción actualizada correctamente.");
        }*/
    }
}
