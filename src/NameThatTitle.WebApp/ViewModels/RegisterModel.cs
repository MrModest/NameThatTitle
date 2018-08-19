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
        [Required(ErrorMessage = "UserName is required!")]
        //[Display(Name = "")]
        [JsonProperty("username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required!")]
        [EmailAddress]
        //[Display(Name = "")]
        [DataType(DataType.EmailAddress)]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        //[Display(Name = "")]
        [DataType(DataType.Password)]
        [JsonProperty("password")]
        public string Password { get; set; }

        //[Display(Name = "")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm password must be equil to password!")]
        [JsonProperty("confirm_password")]
        public string ConfirmPassword { get; set; }
    }
}
