using Facebook;
using GearH.Models;
using GoogleAuthentication.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Hosting;
using System.Web.Mvc;
using System.Xml.Linq;

namespace GearH.Controllers
{
    public class UserController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();

        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("FacebookCallBack");
                return uriBuilder.Uri;
            }
        }
        private string RandomPassword()
        {
            string pass = "";
            Random rd = new Random();
            int num = rd.Next(0, 26);
            char a = (char)('A' + num);
            pass = pass + a + '@' + (rd.Next(100000, 1000000).ToString());
            return pass;
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            return View(new Account());
        }
        [HttpPost]
        public ActionResult SignIn(Account user, FormCollection f)
        {
            var iPasswordAgain = f["iPasswordAgain"];
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(iPasswordAgain))
                {
                    ViewData["err1"] = "Vui lòng nhập lại mật khẩu";
                }
                else if (iPasswordAgain != user.password)
                {
                    ViewData["err1"] = "Vui lòng nhập đúng mật khẩu";
                }
                else
                {
                    user.idRole = 2;
                    user.create_at = DateTime.Now;
                    user.update_at = DateTime.Now;

                    data.Accounts.Add(user);
                    data.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
            }
            return this.SignIn();
        }

        [HttpGet]
        public ActionResult LogIn()
        {
            return View();
        }
        [HttpPost]
        public ActionResult LogIn(FormCollection f)
        {
            var iEmail = f["iEmail"];
            var iPassword = f["iPassword"];

            if (String.IsNullOrEmpty(iEmail) || String.IsNullOrEmpty(iPassword))
            {
                if (String.IsNullOrEmpty(iEmail))
                {
                    ViewData["err1"] = "Email không được để trống";
                }
                if (String.IsNullOrEmpty(iPassword))
                {
                    ViewData["err2"] = "Mật khẩu không được để trống";
                }
                
            }
            else
            {
                Account user = data.Accounts.SingleOrDefault(n => n.email == iEmail && n.password == iPassword);
                if (user != null)
                {
                    Session["Account"] = user;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.ThongBao = "Email hoặc mật khẩu không đúng";
                }
            }
            return View();
        }
        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(FormCollection f)
        {
            var email = f["iEmail"];
            var user = data.Accounts.FirstOrDefault(n => n.email == email);
            if (user != null)
            {
                
                string mail = ConfigurationManager.AppSettings["Email"];
                string password = ConfigurationManager.AppSettings["PasswordEmail"];
                var smtp = new SmtpClient("smtp.gmail.com", 587)
                {
                    Credentials = new NetworkCredential(mail, password),
                    EnableSsl = true,
                };
                var message = new MailMessage();
                message.From = new MailAddress(mail);
                message.ReplyToList.Add(mail);
                message.To.Add(new MailAddress(user.email));
                message.Subject = "Thông báo về việc thay đổi mật khẩu của GearH";
                string mk = RandomPassword();
                message.Body = "Mật khẩu của bạn được reset thành " + mk;
                user.password = mk;
                data.Accounts.AddOrUpdate(user);
                data.SaveChanges();
                smtp.Send(message);
                ViewBag.ThongBao = "Đã gửi email thành công!!! Vui lòng kiểm tra email";
                return View("LogIn");
            }
            else
            {
                ViewBag.ThongBao = "Địa chỉ email không chính xác";
            }
            return this.View();
        }

        #region LoginFacbook
        private int InsertForFacebook(Account entity)
        {
            var user = data.Accounts.SingleOrDefault(x => x.email == entity.email);
            if (user == null)
            {
                data.Accounts.Add(entity);
                data.SaveChanges();
                return entity.idAccount;
            }
            else
            {
                return entity.idAccount;
            }
        }
        public ActionResult LoginFacebook()
        {
            var fb = new FacebookClient();
                var loginUrl = fb.GetLoginUrl(new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                response_type = "code",
                scope = "email",
            });
            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallBack(string code)
        {
            var fb = new FacebookClient();
            dynamic result = fb.Post("oauth/access_token", new
            {
                client_id = ConfigurationManager.AppSettings["FbAppId"],
                client_secret = ConfigurationManager.AppSettings["FbAppSecret"],
                redirect_uri = RedirectUri.AbsoluteUri,
                code = code
            });

            var accessToken = result.access_token;
            if (!string.IsNullOrEmpty(accessToken))
            {
                fb.AccessToken = accessToken;
                dynamic me = fb.Get("me?fields=name,email");
                string email = me.email;
                string userName = me.email;
                string name = me.name;

                var user = new Account();
                user.email = email;
                user.password = "ABC@1234";
                user.phone = "0123456789";
                user.name = name;
                user.address = "Facebook";
                user.idRole = 2;
                user.create_at = DateTime.Now;
                user.update_at = DateTime.Now;

                var resultInsert = InsertForFacebook(user);
                if (resultInsert > 0)
                {
                    Session["Account"] = user;

                }
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region LoginGoogle
        private int InsertForGoogle(Account entity)
        {
            var user = data.Accounts.SingleOrDefault(x => x.email == entity.email);
            if (user == null)
            {
                data.Accounts.Add(entity);
                data.SaveChanges();
                return entity.idAccount;
            }
            else
            {
                return entity.idAccount;
            }
        }
        public ActionResult LoginGoogle()
        {
            var clientId = ConfigurationManager.AppSettings["clientID"];
            var url = "https://localhost:44388/User/GoogleCallBack";
            var response = GoogleAuth.GetAuthUrl(clientId, url);
            return Redirect(response) ;
        }
        public async Task<ActionResult> GoogleCallBack(string code)
        {
            var clientId = ConfigurationManager.AppSettings["clientID"];
            var url = "https://localhost:44388/User/GoogleCallBack"; 
            var clientSecret = ConfigurationManager.AppSettings["clientSecret"];
            var token = await GoogleAuth.GetAuthAccessToken(code, clientId, clientSecret, url);
            var userProfile = await GoogleAuth.GetProfileResponseAsync(token.AccessToken);
            var googleUser = JsonConvert.DeserializeObject<Account>(userProfile);
            if (googleUser != null)
            {
                var user = new Account();
                user.name = googleUser.name;
                user.email = googleUser.email;
                user.password = "ABC@1234";
                user.phone = "0123456789";
                user.address = "Google";
                user.idRole = 2;
                user.create_at = DateTime.Now;
                user.update_at = DateTime.Now;

                var resultInsert = InsertForGoogle(user);
                if (resultInsert > 0)
                {
                    Session["Account"] = user;

                }
            }
            return RedirectToAction("Index", "Home");
        }
        #endregion

        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult LoginLogout()
        {
            return PartialView("LoginLogoutPartial");
        }
    }
}