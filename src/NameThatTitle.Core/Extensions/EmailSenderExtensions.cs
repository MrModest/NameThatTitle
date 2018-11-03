using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Core.Interfaces.Services;

namespace NameThatTitle.Core.Extensions
{
    public static class EmailSenderExtensions //ToDo: replace it to separate project: 'NameThatTitle.Commons'
    {
        public static async Task SendEmailAsync(this IEmailSender emailSender, SendEmailOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            //ToDo: change validation method
            if (string.IsNullOrWhiteSpace(options.Email))   { throw new ArgumentNullException(nameof(options.Email)); }
            if (string.IsNullOrWhiteSpace(options.Subject)) { throw new ArgumentNullException(nameof(options.Subject)); }
            if (string.IsNullOrWhiteSpace(options.Body))    { throw new ArgumentNullException(nameof(options.Body)); }

            await emailSender.SendEmailAsync(options.Email, options.Subject, options.Body);
        }

        public static async Task SendEmailAsync(this IEmailSender emailSender, Action<SendEmailOptions> optionsAction)
        {
            var options = new SendEmailOptions();
            optionsAction(options);

            await emailSender.SendEmailAsync(options);
        }
    }

    public class SendEmailOptions
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
