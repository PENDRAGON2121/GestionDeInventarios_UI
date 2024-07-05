using GestionDeInventarios.Model;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace GestionDeInventario.BL
{
    public class EmailService : IEmailService
    {
        public void SendEmail(EmailDTO _request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse("ppgr.gestiondeinventario.2024@gmail.com"));
            email.To.Add(MailboxAddress.Parse(_request.Para));
            email.Subject = _request.Asunto;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = _request.Contenido };

            using var client = new MailKit.Net.Smtp.SmtpClient();

            client.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

            client.Authenticate("ppgr.gestiondeinventario.2024@gmail.com", "kyecvkdtadkjqejz");
            client.Send(email);

            // Desconectarse del servidor SMTP
            client.DisconnectAsync(true);


        }
    }
}
