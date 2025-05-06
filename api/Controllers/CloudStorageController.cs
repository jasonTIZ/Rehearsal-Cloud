using api.Data;
using api.Dtos.CloudStorage;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CloudStorageController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CloudStorageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CloudStorageDto>>> GetCloudStorages()
        {
            var cloudStorages = await _context.CloudStorages.ToListAsync();
            return cloudStorages.Select(cs => cs.ToDto()).ToList();
        }

        [HttpPost]
        public async Task<ActionResult<CloudStorageDto>> CreateCloudStorage(CreateCloudStorageRequestDto request)
        {
            var cloudStorage = request.ToCloudStorageFromCreateDto();
            _context.CloudStorages.Add(cloudStorage);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCloudStorages), new { id = cloudStorage.Id }, cloudStorage.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCloudStorage(int id, [FromBody] UpdateCloudStorageRequestDto request)
        {
            var cloudStorage = await _context.CloudStorages.FindAsync(id);
            if (cloudStorage == null) return NotFound("CloudStorage not found.");

            cloudStorage.FileName = request.FileName;
            cloudStorage.FileUrl = request.FileUrl;
            cloudStorage.FileSize = request.FileSize;

            _context.CloudStorages.Update(cloudStorage);
            await _context.SaveChangesAsync();

            return Ok(cloudStorage.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCloudStorage(int id)
        {
            var cloudStorage = await _context.CloudStorages.FindAsync(id);
            if (cloudStorage == null) return NotFound("CloudStorage not found.");

            _context.CloudStorages.Remove(cloudStorage);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}