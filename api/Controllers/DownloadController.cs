using api.Data;
using api.Dtos.Download;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DownloadController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DownloadController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DownloadDto>>> GetDownloads()
        {
            var downloads = await _context.Downloads.ToListAsync();
            return downloads.Select(d => d.ToDto()).ToList();
        }

        [HttpPost]
        public async Task<ActionResult<DownloadDto>> CreateDownload(CreateDownloadRequestDto request)
        {
            var download = request.ToDownloadFromCreateDto();
            _context.Downloads.Add(download);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDownloads), new { id = download.Id }, download.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDownload(int id, [FromBody] UpdateDownloadRequestDto request)
        {
            var download = await _context.Downloads.FindAsync(id);
            if (download == null) return NotFound("Download not found.");

            download.UserId = request.UserId;
            download.SongId = request.SongId;
            download.IsOfflineAvailable = request.IsOfflineAvailable;

            _context.Downloads.Update(download);
            await _context.SaveChangesAsync();

            return Ok(download.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDownload(int id)
        {
            var download = await _context.Downloads.FindAsync(id);
            if (download == null) return NotFound("Download not found.");

            _context.Downloads.Remove(download);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}