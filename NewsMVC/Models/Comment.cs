using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NewsMVC.Models
{
    public class Comment
    {
        public int ID { get; set; }
        public int NewID { get; set; }
        public string Author { get; set; }
        public string Text { get; set; }
        public DateTime PublishDate { get; set; }
    }
}