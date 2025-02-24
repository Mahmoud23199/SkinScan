using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using SkinScan_BL.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace SkinScan_BL.Repositories
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
       
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string email, string subject, string message)
        {
           
            var apiKey = _configuration["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("MahmoudSalahAbdelzaher@gmail.com", "Skin-Scan");//Will Change
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            var response = await client.SendEmailAsync(msg);

            return response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }

        public Task SendEmailKitAsync(string toEmail, string subject, string message)
        {
            throw new NotImplementedException();
        }

        //public async Task SendEmailAsync(string email, string subject, string message)
        //{
        //    try
        //    {
        //        var apiKey = _configuration["SendGrid:ApiKey"];
        //        var client = new SendGridClient(apiKey);

        //        var from = new EmailAddress("info@get2cars.com", "kits-technology");
        //        var to = new EmailAddress(email);

        //        var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

        //        var response = await client.SendEmailAsync(msg);

        //        if (response.StatusCode == System.Net.HttpStatusCode.OK)
        //        {
        //            Console.WriteLine("Email sent successfully.");
        //        }
        //        else
        //        {
        //            var errorDetails = await response.Body.ReadAsStringAsync();
        //            Console.WriteLine($"Failed to send email. Response: {errorDetails}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"An error occurred while sending email: {ex.Message}");
        //    }
        //}

        //public async Task SendEmailKitAsync(string toEmail, string subject, string message)
        //{
        //    var emailMessage = new MimeMessage();

        //    emailMessage.From.Add(new MailboxAddress("Get2Cars", _emailFrom));
        //    emailMessage.To.Add(new MailboxAddress("", toEmail));
        //    emailMessage.Subject = subject;

        //    var bodyBuilder = new BodyBuilder
        //    {
        //        TextBody = message
        //    };

        //    emailMessage.Body = bodyBuilder.ToMessageBody();

        //    try
        //    {
        //        using (var smtpClient = new SmtpClient())
        //        {
        //            await smtpClient.ConnectAsync(_secureSmtpServer, 465, true);
        //            await smtpClient.AuthenticateAsync(_emailFrom, _emailPassword);

        //            await smtpClient.SendAsync(emailMessage);
        //            await smtpClient.DisconnectAsync(true);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Exception: {ex.Message}");
        //    }
        //}

    }
}
