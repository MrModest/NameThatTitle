using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NameThatTitle.Core.Interfaces.Services;

namespace NameThatTitle.Services.Implementations
{
    public class EmailBodyBuilderMock : IEmailBodyBuilder
    {
        public async Task<string> BuildAsync(string template, params object[] args)
        {
            return string.Empty;
        }
    }
}
