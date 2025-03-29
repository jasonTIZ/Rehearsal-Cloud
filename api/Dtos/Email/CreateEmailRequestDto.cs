using System;

namespace api.Dtos.Email
{
    public class CreateEmailRequestDto
    {
        public string ToAddress { get; set; } // Dirección de correo electrónico del destinatario
        public string Subject { get; set; } // Asunto del correo
        public string Body { get; set; } // Cuerpo del correo
    }
}