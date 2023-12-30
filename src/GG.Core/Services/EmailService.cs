using MailKit.Net.Smtp;
using MimeKit;

namespace GG.Core.Services;

public class EmailService
{
    public void Test()
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("***", "***"));
            message.To.Add(new MailboxAddress("***", "***"));
            message.Subject = "How you doin'?";

            message.Body = new TextPart("plain")
            {
                Text = @"Note to myself,

                I just wanted to let you know that the test works."
            };

            using (var client = new SmtpClient())
            {
                client.Connect("***", 465, true);

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("***", "***");

                client.Send(message);
                client.Disconnect(true);
            }
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}
