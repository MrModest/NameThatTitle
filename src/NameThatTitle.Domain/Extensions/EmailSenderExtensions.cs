using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NameThatTitle.Domain.Extensions
{
    public static class EmailSenderExtensions
    {
        public async static Task SendEmailAsync(this IEmailSender emailSender, SendEmailOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            await emailSender.SendEmailAsync(options.Email, options.Subject, options.Body);
        }

        public async static Task SendEmailAsync(this IEmailSender emailSender, Action<SendEmailOptions> optionsAction)
        {
            var options = new SendEmailOptions();
            optionsAction(options);

            if (String.IsNullOrWhiteSpace(options.Email))   { throw new ArgumentNullException(nameof(options.Email)); }
            if (String.IsNullOrWhiteSpace(options.Subject)) { throw new ArgumentNullException(nameof(options.Subject)); }
            if (String.IsNullOrWhiteSpace(options.Body))    { throw new ArgumentNullException(nameof(options.Body)); }

            await emailSender.SendEmailAsync(options.Email, options.Subject, options.Body);
        }
    }

    public class SendEmailOptions
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
