using System;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;

namespace ITC.Core.Utilities.Email
{
    public interface IEmailService
    {
        public void Send(string from, string to, string password, string name);
        public void SendAssign(string from, string to, string name);
    }

    public class EmailService : IEmailService
	{
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public EmailService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env = env;
        }
        public void Send(string from, string to, string password, string name)
        {
            var builder = new BodyBuilder();
            using (StreamReader SourceReader = System.IO.File.OpenText(Path.Combine(_env.ContentRootPath, "Excel", "email-inlined.html")))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
                builder.HtmlBody = builder.HtmlBody.Replace("Current Password", password);
                SourceReader.Close();
            }
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "Welcome to ITC, " + name;
            email.Body = builder.ToMessageBody();


            // send email
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_config["Emails:SmtpHost"], int.Parse(_config["Emails:SmtpPort"]), SecureSocketOptions.StartTls);
            smtp.Authenticate(_config["Emails:SmtpUser"], _config["Emails:SmtpPass"]);
            smtp.Send(email);
            smtp.Disconnect(true);

        }



        public void SendAssign(string from, string to, string name)
        {
            var builder = new BodyBuilder();
            using (StreamReader SourceReader = System.IO.File.OpenText(Path.Combine(_env.ContentRootPath, "Excel", "email-inlined-1.html")))
            {
                builder.HtmlBody = SourceReader.ReadToEnd();
                //builder.HtmlBody = builder.HtmlBody.Replace("Current Password", password);
                SourceReader.Close();
            }
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = "Welcome to ITC, " + name;
            email.Body = builder.ToMessageBody();


            // send email
            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect(_config["Emails:SmtpHost"], int.Parse(_config["Emails:SmtpPort"]), SecureSocketOptions.StartTls);
            smtp.Authenticate(_config["Emails:SmtpUser"], _config["Emails:SmtpPass"]);
            smtp.Send(email);
            smtp.Disconnect(true);

        }
    }
}

