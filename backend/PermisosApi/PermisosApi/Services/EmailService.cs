using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace PermisosApi.Services
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task EnviarCorreoAsync(string destino, string asunto, string cuerpo)
        {
            var smtpConfig = _config.GetSection("Smtp");

            using var cliente = new SmtpClient(smtpConfig["Host"], int.Parse(smtpConfig["Port"]))
            {
                Credentials = new NetworkCredential(smtpConfig["User"], smtpConfig["Password"]),
                EnableSsl = bool.Parse(smtpConfig["EnableSsl"])
            };

            var mensaje = new MailMessage(smtpConfig["User"], destino, asunto, cuerpo);
            await cliente.SendMailAsync(mensaje);
        }
    }
}