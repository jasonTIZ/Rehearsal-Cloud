using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using api.Models;
using api.Dtos.Song;
using api.Mappers;
using System.IO.Compression;
using System.Threading.Tasks;
using api.Data;

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

        // GET: api/Song
        [HttpGet]
        public async Task<IActionResult> GetSongs()
        {
            var songs = await _context.Songs.Include(s => s.AudioFiles).ToListAsync();

            var songDtos = songs.Select(s => s.ToDto()).ToList(); // Ahora sí incluye los audios
            return Ok(songDtos);
        }

        // GET: api/Song/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSong(int id)
        {
            var song = await _context.Songs
                .Include(s => s.AudioFiles)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
                return NotFound();
            return Ok(song.ToDto());
        }

        // GET: api/Song/5/audio/1
        [HttpGet("{id}/audio/{audioId}")]
        public async Task<IActionResult> GetAudioFile(int id, int audioId)
        {
            var audioFile = await _context.AudioFiles
                .FirstOrDefaultAsync(af => af.Id == audioId && af.SongId == id);
            if (audioFile == null)
                return NotFound();

            if (!System.IO.File.Exists(audioFile.FilePath))
                return NotFound();

            var fileStream = System.IO.File.OpenRead(audioFile.FilePath);
            return File(fileStream, $"audio/{Path.GetExtension(audioFile.FileName).TrimStart('.')}", audioFile.FileName);
        }

        // POST: api/Song/create-song
        [HttpPost("create-song")]
        public async Task<IActionResult> CreateSong([FromForm] CreateSongRequestDto request)
        {
            if (request.BPM < 40 || request.BPM > 280)
                return BadRequest("BPM must be between 40 and 280.");

            if (request.ZipFile == null || request.ZipFile.Length == 0)
                return BadRequest("No .zip file provided.");

            if (request.CoverImage == null || request.CoverImage.Length == 0)
                return BadRequest("No cover image provided.");

            if (Path.GetExtension(request.ZipFile.FileName).ToLower() != ".zip")
                return BadRequest("File must be a .zip.");

            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var imageExtension = Path.GetExtension(request.CoverImage.FileName).ToLower();
            if (!allowedImageExtensions.Contains(imageExtension))
                return BadRequest("Image must be .jpg, .jpeg, or .png.");

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
                CoverImage = $"/UploadedImages/{uniqueImageFileName}"
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
                        SongId = song.Id
                    };

                    _context.AudioFiles.Add(audioFileRecord);
                }
            }

            await _context.SaveChangesAsync();

            var createdSong = await _context.Songs
                .Include(s => s.AudioFiles)
                .FirstOrDefaultAsync(s => s.Id == song.Id);

            if (createdSong == null)
                return NotFound("Could not retrieve created song.");

            return Ok(createdSong.ToDto());
        }

        // PUT: api/Song/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSong(int id, [FromForm] UpdateSongRequestDto request)
        {
            var song = await _context.Songs
                .Include(s => s.AudioFiles)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
                return NotFound();

            if (request.BPM < 40 || request.BPM > 280)
                return BadRequest("BPM must be between 40 and 280.");

            song.SongName = request.SongName;
            song.Artist = request.Artist;
            song.BPM = request.BPM;
            song.Tone = request.Tone;

            if (request.CoverImage != null && request.CoverImage.Length > 0)
            {
                var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var imageExtension = Path.GetExtension(request.CoverImage.FileName).ToLower();
                if (!allowedImageExtensions.Contains(imageExtension))
                    return BadRequest("Image must be .jpg, .jpeg, or .png.");

                var uniqueImageFileName = Guid.NewGuid() + imageExtension;
                var imagePath = Path.Combine(_imageUploadDirectory, uniqueImageFileName);

                using (var imageStream = new FileStream(imagePath, FileMode.Create))
                {
                    await request.CoverImage.CopyToAsync(imageStream);
                }

                // Delete old cover image if exists
                if (!string.IsNullOrEmpty(song.CoverImage) && System.IO.File.Exists(song.CoverImage))
                    System.IO.File.Delete(song.CoverImage);

                song.CoverImage = $"/UploadedImages/{uniqueImageFileName}";
            }

            if (request.ZipFile != null && request.ZipFile.Length > 0)
            {
                if (Path.GetExtension(request.ZipFile.FileName).ToLower() != ".zip")
                    return BadRequest("File must be a .zip.");

                var songAudioDirectory = Path.Combine(_audioUploadDirectory, song.Id.ToString());
                if (!Directory.Exists(songAudioDirectory))
                    Directory.CreateDirectory(songAudioDirectory);

                // Delete existing audio files
                foreach (var audioFile in song.AudioFiles)
                {
                    if (System.IO.File.Exists(audioFile.FilePath))
                        System.IO.File.Delete(audioFile.FilePath);
                }
                _context.AudioFiles.RemoveRange(song.AudioFiles);

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
                            SongId = song.Id
                        };

                        _context.AudioFiles.Add(audioFileRecord);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok(song.ToDto());
        }

        // DELETE: api/Song/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSong(int id)
        {
            var song = await _context.Songs
                .Include(s => s.AudioFiles)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (song == null)
                return NotFound();

            // Delete audio files
            foreach (var audioFile in song.AudioFiles)
            {
                if (System.IO.File.Exists(audioFile.FilePath))
                    System.IO.File.Delete(audioFile.FilePath);
            }
            var songAudioDirectory = Path.Combine(_audioUploadDirectory, song.Id.ToString());
            if (Directory.Exists(songAudioDirectory))
                Directory.Delete(songAudioDirectory);

            // Delete cover image
            if (!string.IsNullOrEmpty(song.CoverImage) && System.IO.File.Exists(song.CoverImage))
                System.IO.File.Delete(song.CoverImage);

            _context.Songs.Remove(song);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}