using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsMVC.Models
{
    public class Auth
    {
        public int ID { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public DateTime LiveBefore { get; set; }
    }
}