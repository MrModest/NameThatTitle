using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NameThatTitle.Domain.Interfaces.Services
{
    public interface IEmailBodyBuilder
    {
        Task<string> BuildAsync(string template, params object[] args);
    }
}
