using api.Data;
using api.Models;
using api.Mappers;
using api.Dtos.Setlist;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SetlistController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SetlistController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Setlist
        [HttpGet]
        public async Task<IActionResult> GetSetlists()
        {
            var setlists = await _context.Setlists
                .Include(s => s.SetlistSongs)
                .ToListAsync();

            var setlistDtos = setlists.Select(s => s.ToDto()).ToList();
            return Ok(setlistDtos);
        }

        // GET: api/Setlist/SetlistWithSongs
        [HttpGet("SetlistWithSongs")]
        public async Task<IActionResult> GetSetlistsWithSongs()
        {
            var setlists = await _context.Setlists
                .Include(s => s.SetlistSongs)
                .ToListAsync();

            var setlistDtos = setlists.Select(s => s.ToDto()).ToList();
            return Ok(setlistDtos);
        }

        // POST: api/Setlist
        [HttpPost]
        public async Task<IActionResult> CreateSetlist([FromBody] CreateSetlistRequestDto createSetlistDto)
        {
            var setlist = new Setlist
            {
                Name = createSetlistDto.Name,
                Date = createSetlistDto.Date,
                SetlistSongs = (createSetlistDto.SetlistSongs ?? new List<int>())
                    .Select((songId, index) => new SetlistSong
                    {
                        SongId = songId,
                        Order = index
                    })
                    .ToList()
            };

            _context.Setlists.Add(setlist);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSetlists), new { id = setlist.Id }, setlist.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSetlist(int id, [FromBody] CreateSetlistRequestDto updateSetlistDto)
        {
            var setlist = await _context.Setlists
                .Include(s => s.SetlistSongs)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (setlist == null) return NotFound();

            setlist.Name = updateSetlistDto.Name;
            setlist.Date = updateSetlistDto.Date;

            // Clear current songs
            setlist.SetlistSongs.Clear();

            // ✅ Only add new ones if the list is not null or empty
            if (updateSetlistDto.SetlistSongs != null && updateSetlistDto.SetlistSongs.Any())
            {
                foreach (var songId in updateSetlistDto.SetlistSongs)
                {
                    setlist.SetlistSongs.Add(new SetlistSong
                    {
                        SongId = songId,
                        Order = updateSetlistDto.SetlistSongs.IndexOf(songId)
                    });
                }
            }

            await _context.SaveChangesAsync();
            return Ok(setlist.ToDto());
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
    }
}
