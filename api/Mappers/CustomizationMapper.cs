using api.Dtos.Customization;
using api.Models;

namespace api.Mappers
{
    public static class CustomizationMapper
    {
        public static CustomizationDto ToDto(this Customization customization)
        {
            return new CustomizationDto
            {
                Id = customization.Id,
                SongId = customization.SongId,
                VolumeLevel = customization.VolumeLevel,
                EqualizationSettings = customization.EqualizationSettings,
                LoopEnabled = customization.LoopEnabled,
                FadeIn = customization.FadeIn,
                FadeOut = customization.FadeOut
            };
        }

        public static Customization ToCustomizationFromCreateDto(this CreateCustomizationRequestDto request)
        {
            return new Customization
            {
                SongId = request.SongId,
                VolumeLevel = request.VolumeLevel,
                EqualizationSettings = request.EqualizationSettings,
                LoopEnabled = request.LoopEnabled,
                FadeIn = request.FadeIn,
                FadeOut = request.FadeOut
            };
        }
    }
}