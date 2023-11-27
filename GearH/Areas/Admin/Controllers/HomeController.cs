using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GearH.Models;

namespace GearH.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetStatistical(string fromDate, string toDate)
        {
            var query = from o in data.Orders
                        join od in data.OrderDetails
                        on o.idOrder equals od.idOrder
                        join p in data.Products
                        on od.idProduct equals p.idProduct
                        select new
                        {
                            createDate = o.order_date,
                            quantity = od.quantity,
                            price = od.total,
                            OriginalPrice = p.OriginalPrice
                        };
            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.createDate >= startDate);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.createDate < endDate);
            }
            var result = query.GroupBy(x => DbFunctions.TruncateTime(x.createDate)).Select(x => new
            {
                Date = x.Key.Value,
                TotalBuy = x.Sum(y => y.quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.quantity * y.price)
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            });
            return Json(new { Data = result }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(FormCollection f)
        {
            var email = f["Email"];
            var password = f["Password"];

            if (ModelState.IsValid)
            {
                var sEmail = f["Email"];
                var sPassword = f["Password"];

                if (String.IsNullOrEmpty(sEmail) || String.IsNullOrEmpty(sPassword))
                {
                    if (String.IsNullOrEmpty(sEmail))
                    {
                        ViewData["err1"] = "Email không được để trống";
                    }
                    if (String.IsNullOrEmpty(sPassword))
                    {
                        ViewData["err2"] = "Password không được để trống";
                    }
                }
                else
                {
                    Account admin = data.Accounts.SingleOrDefault(n => n.email == sEmail && n.password == sPassword && n.idRole == 1);
                    if (admin != null)
                    {
                        Session["Admin"] = admin;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ViewBag.ThongBao = "Tên đăng nhập hoặc mật khẩu không đúng";
                    }
                }
            }

            return View();
        }
    }
}