using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.AudioFile;
using api.Dtos.Song;
using api.Mappers;
using api.Models;
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
        private readonly string _imageUploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UploadedImages");
        private readonly string _audioUploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "UploadedAudioFiles");

        public SongController(ApplicationDbContext context)
        {
            _context = context;
            if (!Directory.Exists(_imageUploadDirectory))
                Directory.CreateDirectory(_imageUploadDirectory);
            if (!Directory.Exists(_audioUploadDirectory))
                Directory.CreateDirectory(_audioUploadDirectory);
        }

        // POST: api/Song/create-song
        [HttpPost("create-song")]
        public async Task<IActionResult> CreateSong([FromForm] CreateSongRequestDto request)
        {
            if (request.BPM < 40 || request.BPM > 280)
                return BadRequest("El BPM debe estar entre 40 y 280.");

            if (request.ZipFile == null || request.ZipFile.Length == 0)
                return BadRequest("No se ha enviado ningún archivo .zip.");

            if (request.CoverImage == null || request.CoverImage.Length == 0)
                return BadRequest("No se ha enviado la imagen de portada.");

            if (Path.GetExtension(request.ZipFile.FileName).ToLower() != ".zip")
                return BadRequest("El archivo debe ser un .zip.");

            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var imageExtension = Path.GetExtension(request.CoverImage.FileName).ToLower();
            if (!allowedImageExtensions.Contains(imageExtension))
                return BadRequest("El archivo de imagen debe ser .jpg, .jpeg o .png.");

            var uniqueImageFileName = Guid.NewGuid() + imageExtension;
            var imagePath = Path.Combine(_imageUploadDirectory, uniqueImageFileName);

            using (var imageStream = new FileStream(imagePath, FileMode.Create))
            {
                await request.CoverImage.CopyToAsync(imageStream);
            }

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

            var songAudioDirectory = Path.Combine(_audioUploadDirectory, song.Id.ToString());
            if (!Directory.Exists(songAudioDirectory))
                Directory.CreateDirectory(songAudioDirectory);

            using (var stream = request.ZipFile.OpenReadStream())
            using (var zip = new ZipArchive(stream, ZipArchiveMode.Read))
            {
                foreach (var entry in zip.Entries)
                {
                    var fileExtension = Path.GetExtension(entry.FullName).ToLower();
                    if (fileExtension != ".mp3" && fileExtension != ".wav")
                        continue;

                    var uniqueFileName = Guid.NewGuid() + fileExtension;
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

            var createdSong = await _context.Songs
                .Where(s => s.Id == song.Id)
                .Include(s => s.AudioFiles)
                .FirstOrDefaultAsync();

            if (createdSong == null)
                return NotFound("No se pudo recuperar la canción creada.");

            var songDto = createdSong.ToDto();
            return Ok(new { song = songDto });
        }

        // GET: api/Song/get-songs
        [HttpGet("get-songs")]
        public async Task<IActionResult> GetSongs()
        {
            var songs = await _context.Songs
                .Include(s => s.AudioFiles)
                .Select(s => s.ToDto())
                .ToListAsync();

            if (songs == null || !songs.Any())
                return NotFound("No se encontraron canciones.");

            return Ok(new { songs });
        }

        // GET: api/Song/get-song/{id}
        [HttpGet("get-song/{id}")]
        public async Task<IActionResult> GetSong(int id)
        {
            var song = await _context.Songs
                .Include(s => s.AudioFiles)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
                return NotFound("Canción no encontrada.");

            var songDto = song.ToDto();
            return Ok(new { song = songDto });
        }

        // PUT: api/Song/update-song/{id}
        [HttpPut("update-song/{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromBody] SongDto request)
        {
            if (request.BPM < 40 || request.BPM > 280)
                return BadRequest("El BPM debe estar entre 40 y 280.");

            var song = await _context.Songs
                .Include(s => s.AudioFiles)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (song == null)
                return NotFound("Canción no encontrada.");

            song.SongName = request.SongName;
            song.Artist = request.Artist;
            song.BPM = request.BPM;
            song.Tone = request.Tone;

            await _context.SaveChangesAsync();

            var songDto = song.ToDto();
            return Ok(new { song = songDto });
        }

        // DELETE: api/Song/delete-song/{id}
        [HttpDelete("delete-song/{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            try
            {
                var song = await _context.Songs
                    .Include(s => s.AudioFiles)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (song == null)
                    return NotFound("Canción no encontrada.");

                // Eliminar archivos de audio
                foreach (var audioFile in song.AudioFiles)
                {
                    var filePath = Path.Combine(_audioUploadDirectory, song.Id.ToString(), audioFile.FileName);
                    if (System.IO.File.Exists(filePath))
                        System.IO.File.Delete(filePath);
                }

                // Eliminar imagen de portada
                if (System.IO.File.Exists(song.CoverImage))
                    System.IO.File.Delete(song.CoverImage);

                // Eliminar carpeta de audios
                var songAudioDirectory = Path.Combine(_audioUploadDirectory, song.Id.ToString());
                if (Directory.Exists(songAudioDirectory))
                    Directory.Delete(songAudioDirectory, true);

                _context.AudioFiles.RemoveRange(song.AudioFiles);
                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();

                return Ok("Canción eliminada correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar la canción: {ex.Message}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}

