using api.Data;
using api.Dtos.Customization;
using api.Mappers;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomizationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomizationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomizationDto>>> GetCustomizations()
        {
            var customizations = await _context.Customizations.ToListAsync();
            return customizations.Select(c => c.ToDto()).ToList();
        }

        [HttpPost]
        public async Task<ActionResult<CustomizationDto>> CreateCustomization(CreateCustomizationRequestDto request)
        {
            var customization = request.ToCustomizationFromCreateDto();
            _context.Customizations.Add(customization);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomizations), new { id = customization.Id }, customization.ToDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCustomization(int id, [FromBody] UpdateCustomizationRequestDto request)
        {
            var customization = await _context.Customizations.FindAsync(id);
            if (customization == null) return NotFound("Customization not found.");

            customization.SongId = request.SongId;
            customization.VolumeLevel = request.VolumeLevel;
            customization.EqualizationSettings = request.EqualizationSettings;
            customization.LoopEnabled = request.LoopEnabled;
            customization.FadeIn = request.FadeIn;
            customization.FadeOut = request.FadeOut;

            _context.Customizations.Update(customization);
            await _context.SaveChangesAsync();

            return Ok(customization.ToDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomization(int id)
        {
            var customization = await _context.Customizations.FindAsync(id);
            if (customization == null) return NotFound("Customization not found.");

            _context.Customizations.Remove(customization);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}