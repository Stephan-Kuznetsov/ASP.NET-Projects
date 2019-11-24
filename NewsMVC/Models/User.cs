using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace NewsMVC.Models
{
    public class LoginModel
    {
        public int ID { get; set; }
        [Required][Display(Name = "Имя пользователя")]
        public string Username { get; set; }

        [Required][DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}