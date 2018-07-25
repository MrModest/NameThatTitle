using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NameThatTitle.WebApp.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "")]
        //[Display(Name = "")]
        [JsonProperty("username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "")]
        [EmailAddress]
        //[Display(Name = "")]
        [DataType(DataType.EmailAddress)]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "")]
        //[Display(Name = "")]
        [DataType(DataType.Password)]
        [JsonProperty("password")]
        public string Password { get; set; }

        //[Display(Name = "")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "")]
        [JsonProperty("confirm_password")]
        public string ConfirmPassword { get; set; }
    }
}
