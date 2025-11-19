using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Portafolio.Interfaces;
using Portafolio.Models;

namespace Portafolio.Servicios
{
    public class ServicioEmailMailKit : IServicioEmail
    {
        private readonly IConfiguration _configuration;

        public ServicioEmailMailKit(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task Enviar(ContactoDTO contacto)
        {
            string emailEmisor = _configuration.GetValue<string>("CONFIGURACIONES_EMAIL:EMAIL");
            string emailPassword = _configuration.GetValue<string>("CONFIGURACIONES_EMAIL:PASSWORD");
            string host = _configuration.GetValue<string>("CONFIGURACIONES_EMAIL:HOST");
            int puerto = _configuration.GetValue<int>("CONFIGURACIONES_EMAIL:PORT");

            var mensaje = new MimeMessage();
            mensaje.From.Add(new MailboxAddress("Portafolio", emailEmisor));
            mensaje.To.Add(new MailboxAddress("", emailEmisor));
            mensaje.Subject = $"El cliente {contacto.Nombre} ha enviado un nuevo mensaje de contacto";

            mensaje.Body = new TextPart("plain")
            {
                Text = $"Nombre: {contacto.Nombre}\nEmail: {contacto.Email}\nMensaje: {contacto.Mensaje}"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(host, puerto, SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(emailEmisor, emailPassword);
            await client.SendAsync(mensaje);
            await client.DisconnectAsync(true);
        }
    }
}