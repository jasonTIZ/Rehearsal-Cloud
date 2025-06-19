using api.Data;
using api.Models;
using api.Mappers;
using api.Dtos.Setlist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetlistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public SetlistController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: api/Setlist
        [HttpGet]
        public async Task<IActionResult> GetSetlistsWithoutSongs()
        {
            var setlists = await _context.Setlists.ToListAsync();

            var setlistDtos = setlists.Select(s => s.ToWithoutSongsDto()).ToList();

            return Ok(setlistDtos);
        }

        // GET: api/Setlist/SetlistWithSongs
        [HttpGet("SetlistWithSongs")]
        public async Task<IActionResult> GetSetlistsWithSongs()
        {
            // Cargar setlists con canciones y archivos de audio
            var setlists = await _context.Setlists
                .Include(s => s.SetlistSongs)
                    .ThenInclude(ss => ss.Song)
                        .ThenInclude(s => s.AudioFiles)
                .ToListAsync();

            // Crear DTOs para las setlists
            var setlistDtos = setlists.Select(s => s.ToDto()).ToList();

            // Generar archivo .zip con los archivos de audio
            var zipStream = new MemoryStream();
            using (var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var setlist in setlists)
                {
                    foreach (var setlistSong in setlist.SetlistSongs)
                    {
                        var song = setlistSong.Song;
                        if (song?.AudioFiles != null)
                        {
                            foreach (var audioFile in song.AudioFiles)
                            {
                                // Crear una entrada en el .zip con la estructura: Song_{SongId}/{FileName}
                                var entryName = $"Song_{song.Id}/{audioFile.FileName}";
                                var entry = zipArchive.CreateEntry(entryName);

                                // Leer el archivo de audio desde el sistema de archivos
                                var filePath = Path.Combine(_environment.ContentRootPath, audioFile.FilePath);
                                if (System.IO.File.Exists(filePath))
                                {
                                    using (var fileStream = new System.IO.FileStream(filePath, FileMode.Open, FileAccess.Read))
                                    using (var entryStream = entry.Open())
                                    {
                                        await fileStream.CopyToAsync(entryStream);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Preparar la respuesta
            zipStream.Seek(0, SeekOrigin.Begin);

            return base.File(
                zipStream.ToArray(),
                "application/zip",
                "SetlistAudioFiles.zip",
                enableRangeProcessing: true
            );
        }

        // POST: api/Setlist
        [HttpPost]
        public async Task<IActionResult> CreateSetlist([FromBody] CreateSetlistRequestDto createSetlistDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var setlist = new Setlist
            {
                Name = createSetlistDto.Name,
                Date = createSetlistDto.Date,
                SetlistSongs = new List<SetlistSong>() // Vacío al principio
            };

            _context.Setlists.Add(setlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSetlistsWithoutSongs), new { id = setlist.Id }, setlist.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSetlist(int id, [FromBody] UpdateSetlistRequestDto updateSetlistDto)
        {
            var setlist = await _context.Setlists
                .Include(s => s.SetlistSongs)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (setlist == null) return NotFound();

            setlist.Name = updateSetlistDto.Name;
            setlist.Date = updateSetlistDto.Date;

            if (updateSetlistDto.SetlistSongs != null)
            {
                // Primero, eliminar las canciones que no están en la lista nueva
                var toRemove = setlist.SetlistSongs
                    .Where(ss => !updateSetlistDto.SetlistSongs.Contains(ss.SongId))
                    .ToList();

                foreach (var rem in toRemove)
                {
                    setlist.SetlistSongs.Remove(rem);
                }

                // Luego, agregar las canciones nuevas que no existen aún
                var existingSongIds = setlist.SetlistSongs.Select(ss => ss.SongId).ToHashSet();

                foreach (var songId in updateSetlistDto.SetlistSongs)
                {
                    if (!existingSongIds.Contains(songId))
                    {
                        setlist.SetlistSongs.Add(new SetlistSong
                        {
                            SetlistId = id,
                            SongId = songId
                        });
                    }
                }
            }
            // Si SetlistSongs es null, no modificamos la relación canciones

            await _context.SaveChangesAsync();

            // Aquí recargamos el setlist incluyendo las relaciones necesarias para el DTO completo
            var updatedSetlist = await _context.Setlists
                .Include(s => s.SetlistSongs)
                    .ThenInclude(ss => ss.Song)
                        .ThenInclude(song => song.AudioFiles)
                .FirstOrDefaultAsync(s => s.Id == id);

            return Ok(updatedSetlist.ToDto());
        }

        // GET: api/Setlist/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSetlistById(int id)
        {
            var setlist = await _context.Setlists
                .Include(s => s.SetlistSongs)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (setlist == null)
            {
                return NotFound();
            }

            return Ok(setlist.ToDto());
        }

        // DELETE: api/Setlist/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSetlist(int id)
        {
            var setlist = await _context.Setlists
                .Include(s => s.SetlistSongs)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (setlist == null) return NotFound();

            _context.Setlists.Remove(setlist);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{setlistId}/AddSongs")]
        public async Task<IActionResult> AddSongsToSetlist(int setlistId, [FromBody] List<int> songIds)
        {
            var setlist = await _context.Setlists
                .Include(s => s.SetlistSongs)
                .FirstOrDefaultAsync(s => s.Id == setlistId);

            if (setlist == null)
                return NotFound();

            foreach (var songId in songIds)
            {
                if (!setlist.SetlistSongs.Any(ss => ss.SongId == songId))
                {
                    setlist.SetlistSongs.Add(new SetlistSong
                    {
                        SetlistId = setlistId,
                        SongId = songId
                    });
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
