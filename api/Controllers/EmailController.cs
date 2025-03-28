using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Email;
using api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api.Controllers
{
    [ApiController]
    [Route("api/email")]
    public class EmailController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly string _smtpServer = "smtp.gmail.com"; 
        private readonly int _smtpPort = 587; 
        private readonly string _smtpUser  = "ejemplo@gmail.com"; 
        private readonly string _smtpPass = "1234"; 

        public EmailController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] CreateEmailRequestDto emailDto)
        {
            if (emailDto == null || string.IsNullOrEmpty(emailDto.ToAddress))
            {
                return BadRequest("Invalid email data.");
            }

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpUser ),
                Subject = emailDto.Subject,
                Body = emailDto.Body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(emailDto.ToAddress);

            using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
            {
                smtpClient.Credentials = new NetworkCredential(_smtpUser , _smtpPass);
                smtpClient.EnableSsl = true;

                try
                {
                    await smtpClient.SendMailAsync(mailMessage);
                    
                    var email = new Email
                    {
                        ToAddress = emailDto.ToAddress,
                        Subject = emailDto.Subject,
                        Body = emailDto.Body,
                        SentAt = DateTime.UtcNow
                    };

                    _context.Emails.Add(email);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetEmailById), new { id = email.Id }, email);
                }
                catch (Exception ex)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, $"Error sending email: {ex.Message}");
                }
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetEmailHistory()
        {
            var emails = await _context.Emails.ToListAsync();
            return Ok(emails);
        }

        [HttpGet("history/{id}")]
        public async Task<IActionResult> GetEmailById(int id)
        {
            var email = await _context.Emails.FindAsync(id);
            if (email == null)
            {
                return NotFound("Email not found.");
            }
            return Ok(email);
        }

        [HttpPut("history/{id}")]
        public async Task<IActionResult> UpdateEmail(int id, [FromBody] UpdateEmailRequestDto emailDto)
        {
            var email = await _context.Emails.FindAsync(id);
            if (email == null)
            {
                return NotFound("Email not found.");
            }

            email.Subject = emailDto.Subject;
            email.Body = emailDto.Body;

            await _context.SaveChangesAsync();
            return Ok(email);
        }

        [HttpDelete("history/{id}")]
        public async Task<IActionResult> DeleteEmail(int id)
        {
            var email = await _context.Emails.FindAsync(id);
            if (email == null)
            {
                return NotFound("Email not found.");
            }

            _context.Emails.Remove(email);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}