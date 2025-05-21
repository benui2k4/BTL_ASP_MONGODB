using Microsoft.Extensions.Options;
using System.Net.Mail;
using System.Net;
using ASP_MongoDB.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
namespace ASP_MongoDB.Areas.Admin.EmailManager
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("sondz2k4g@gmail.com", "fozxhhsubylcnakk")
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress("sondz2k4g@gmail.com","Shopper Online"),
                Subject = subject,
                Body = message,
                IsBodyHtml = true  // Thiết lập để nội dung được gửi dưới dạng HTML
            };

            mailMessage.To.Add(email);

            return client.SendMailAsync(mailMessage);
        }
    }
}
