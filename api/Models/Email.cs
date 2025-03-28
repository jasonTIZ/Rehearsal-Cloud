namespace api.Models
{
    public class Email
    {
        public int Id { get; set; } // Esta es la clave primaria
        public string ToAddress { get; set; } // Dirección de correo electrónico del destinatario
        public string Subject { get; set; } // Asunto del correo
        public string Body { get; set; } // Cuerpo del correo
        public DateTime SentAt { get; set; } // Fecha y hora de envío
    }
}