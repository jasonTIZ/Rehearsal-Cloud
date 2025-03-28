using System;

namespace api.Dtos.Email
{
    public class UpdateEmailRequestDto
    {
        public string Subject { get; set; } // Asunto del correo
        public string Body { get; set; } // Cuerpo del correo
    }
}