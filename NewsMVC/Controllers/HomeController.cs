using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NewsMVC.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        private Models.NewDBContext ndb = new Models.NewDBContext();

        public ActionResult Index()
        {
            // Удаление всех просроченных токенов
            foreach(var auth in ndb.Authes)
            {
                if (DateTime.Compare(auth.LiveBefore, DateTime.Now) < 0)
                    ndb.Authes.Remove(auth);
            }
            ndb.SaveChanges();
            // Загрузка статей из бд и передача их в представление
            ViewBag.News = ndb.News.AsEnumerable();
            ViewBag.Username = GetUser();
            return View();
        }

        public ActionResult ShowNew(int id = 1)
        {
            // Получение статуса пользователя
            string uname = GetUser();
            ViewBag.Username = uname;
            // Получение комментариев к выбранной статье
            ViewBag.Comments = ndb.Comments.Where(c => (c.NewID == id));
            // Если пользователь авторизован, добавим выбранную статью к списку просмотренных.
            if (uname != "Guest")
            {
                bool b = (ndb.CheckedNews.Count() == 0);
                if (b || (ndb.CheckedNews.FirstOrDefault(d => (d.NewID == id && d.Username == uname)) == null))
                {
                    ndb.CheckedNews.Add(new Models.CheckedNew { NewID = id, Username = uname });
                    ndb.SaveChanges();
                }
            }
            // Передача выбранной статьи в представление
            ViewBag.New = ndb.News.Find(id);
            return View();
        }
        [HttpPost]
        public ActionResult AddComment(string Comment, int ID)
        {
            ndb.Comments.Add(new Models.Comment
            {
                NewID = ID, Text = Comment, PublishDate = DateTime.Now, Author = GetUser()
            });
            ndb.SaveChanges();
            return Redirect("/Home/ShowNew/" + ID);
        }
        public ActionResult NewPublish(string url)
        {
            ViewBag.ReturnURL = url;
            return View();
        }

        [HttpPost]
        public ActionResult NewPublish(Models.New m)
        {
            // Проверка состояния модели новости
            if (ModelState.IsValid && m.Text != "" && m.Title != "")
            {
                // Добавление статьи в БД
                Models.New n = new Models.New { Title = m.Title, Text = m.Text, PostDate = DateTime.Now };
                ndb.News.Add(n);
                ndb.SaveChanges();
                return Redirect("/Home/Index");
            }
            return View();
        }


        public string GetUser()
        {
            string uname = "Guest"; 
            // Получение cookie, пришедшего с запросом
            HttpCookie c = HttpContext.Request.Cookies["auth"];
            if (c != null)
            {
                // Поиск по токену имени пользователя
                Models.Auth auth = ndb.Authes.FirstOrDefault(au => au.Token == c.Value);
                if (auth != null)
                {
                    Models.User user = ndb.Users.FirstOrDefault(u => u.Username == auth.Username);
                    if (user != null)
                        uname = user.Username;
                }
                // Обновить cookie
                Response.SetCookie(c);
            } // Возвращение имени пользователя или статуса "Guest", если такой не обнаружен.
            return uname;
        }

    }
}
