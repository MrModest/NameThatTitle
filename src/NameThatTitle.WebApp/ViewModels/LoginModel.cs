using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NameThatTitle.WebApp.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "")]
        //[Display(Name = "")]
        [JsonProperty("login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "")]
        //[Display(Name = "")]
        [DataType(DataType.Password)]
        [JsonProperty("password")]
        public string Password { get; set; }
    }
}
