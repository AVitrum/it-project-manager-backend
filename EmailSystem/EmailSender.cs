using System.Net;
using System.Net.Mail;

namespace EmailSystem;

public class EmailSender : IEmailSender
{
    private const string Mail = "prms.noreply@gmail.com";
    private const string Password = "roqvjmaewgfrfeau";
    
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(Mail, Password)
        };

        var msg = new MailMessage
        {
            From = new MailAddress(Mail),
            To = { email },
            Subject = subject,
            Body = "<html>" +
                    "<body>" +
                        $"{subject}:" +
                        $"<h2>{message}</h2>" +
                        "<p>If you did not send this request," +
                        " then contact our technical support," +
                        " you may have been hacked!</p>" +
                    "</body>" +
                   "<html>",
            IsBodyHtml = true
        };
        
        return client.SendMailAsync(msg);
    }
}