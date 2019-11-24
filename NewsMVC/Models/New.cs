using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsMVC.Models
{
    public class New
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public DateTime PostDate { get; set; }
        public string Text { get; set; }
    }
    public class NewDBContext : System.Data.Entity.DbContext
    {
        public System.Data.Entity.DbSet<CheckedNew> CheckedNews { get; set; }
        public System.Data.Entity.DbSet<Comment> Comments { get; set; }
        public System.Data.Entity.DbSet<New> News { get; set; }

        public System.Data.Entity.DbSet<User> Users { get; set; }

        public System.Data.Entity.DbSet<Auth> Authes { get; set; }

    }
    public class CheckedNew
    {
        public int ID { get; set; }
        public int NewID { get; set; }
        public string Username { get; set; }
    }
}