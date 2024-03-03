using System.Net;
using System.Net.Mail;

namespace EmailSystem;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var mail = "prms.noreply@gmail.com";
        var pw = "roqvjmaewgfrfeau";

        var client = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            EnableSsl = true,
            Credentials = new NetworkCredential(mail, pw)
        };

        var msg = new MailMessage
        {
            From = new MailAddress(mail),
            To = { email },
            Subject = subject,
            Body = $"<html><body> {message} </body><html>",
            IsBodyHtml = true
        };
        
        return client.SendMailAsync(msg);
    }
}