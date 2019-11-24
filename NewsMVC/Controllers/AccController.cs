using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewsMVC.Models;
using System.Security.Cryptography;

namespace NewsMVC.Controllers
{
    public class AccController : Controller
    {
        private NewDBContext ndb = new NewDBContext();
        
        public ActionResult Login(string returnURL)
        {
            ViewBag.ReturnUrl = returnURL;
            return View();
        }
        [HttpPost]
        public ActionResult Login(LoginModel m)
        {
            if (ModelState.IsValid) // Проверка состояния модели авторизации
            {
                bool b = false;
                try
                {
                    // Проверка наличия введенных данных в базе
                    b = (ndb.Users.Single(u => (u.Username == m.Username) && (u.Password == m.Password)) != null);
                }
                catch (Exception e) { }
                if (b)
                {
                    // Генерация токена и добавление данных об авторизации в БД
                    // Время жизни токена - 2 часа
                    string t = GenerateToken();
                    ndb.Authes.Add(new Auth { Username = m.Username, 
                        Token = t, LiveBefore = DateTime.Now.AddHours(2) });
                    ndb.SaveChanges();
                    Response.SetCookie(new HttpCookie("auth", t));
                    return Redirect("/Home/Index");
                }
            }
            return View(m);
        }

        [HttpPost]
        public ActionResult Register(LoginModel m)
        {
            // Проверка состояния модели авторизации
            if (ModelState.IsValid)
            {
                // Проверка наличия в базе введенного имени пользователя
                bool b = false;
                try
                {
                    b = (ndb.Users.Single(u => (u.Username == m.Username)) != null);
                }
                catch (Exception e) { }
                // Если такой пользователь еще не зарегистрирован и не используется имя "Guest"
                if (!b && m.Username != "Guest") 
                {
                    HttpCookie c = Request.Cookies["auth"];
                    // Если получен cookie с тэгом 
                    if (c != null)
                    {
                        foreach (Auth a in ndb.Authes)
                        {
                            if (a.Token == c.Value)
                            {
                                ndb.Authes.Remove(a);
                                break;
                            }
                        }
                    }
                    // Генерация токена и регистрация пользователя в системе
                    string t = GenerateToken();
                    ndb.Users.Add(new User { Username = m.Username, Password = m.Password });
                    ndb.SaveChanges();
                    ndb.Authes.Add(new Auth
                    {
                        Username = m.Username,
                        Token = t,
                        LiveBefore = DateTime.Now.AddHours(2)
                    });
                    ndb.SaveChanges();
                    // Передача cookie с заголовком авторизации браузеру и перенаправление на главную страницу
                    Response.SetCookie(new HttpCookie("auth", t));
                    return Redirect("/Home/Index");
                }
            }
            return View(m);
        }
        public ActionResult Register(string returnURL)
        {
            ViewBag.ReturnUrl = returnURL;
            return View();
        }
        public ActionResult UserPage(string n)
        {
            if (n == null)
            {
                return Redirect("/Shared/Error");
            }
            // Получение просмотренные пользователем статьи из таблицы БД CheckedNews
            List<New> Chn = new List<New>();
            var Checked = ndb.CheckedNews.Where(c => c.Username == n);
            foreach(var cn in Checked)
            {
                Chn.Add(ndb.News.Find(cn.NewID));
            }
            // Передача полученных данных в представление
            ViewBag.Checked = Chn;
            ViewBag.Username = n;
            return View();
        }

        public ActionResult LogOut(string n)
        {
            // Удаление из БД всех данных, связанных с авторизацией пользователя
            foreach(Auth auth in ndb.Authes)
            {
                if (auth.Username == n) ndb.Authes.Remove(auth);
            }
            ndb.SaveChanges();
            return Redirect("/Home/Index");
        }

        public string GenerateToken()
        {
            // Генерация уникального токена
            string token = "";
            do
            {
                RandomNumberGenerator rng = new RNGCryptoServiceProvider();
                byte[] tokenData = new byte[32];
                rng.GetBytes(tokenData);
                token = Convert.ToBase64String(tokenData);
            } // Проверка на совпадение сгенерированного токена с имеющимися в базе
            while (ndb.Authes.FirstOrDefault(auth => auth.Token == token) != null);
            return token;
        }

    }
}
