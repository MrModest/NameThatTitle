using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Core.Interfaces.Services;

namespace NameThatTitle.Services.Implementations
{
    public class EmailSenderMock : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            await Task.Delay(10);
        }
    }
}
