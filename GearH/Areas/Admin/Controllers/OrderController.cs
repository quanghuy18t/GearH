using GearH.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Areas.Admin.Controllers
{
    public class OrderController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(data.Orders.ToList().OrderBy(n => n.order_date));
        }
        public ActionResult Detail(int id)
        {
            ViewBag.code = data.Orders.Where(n => n.idOrder == id).Select(n => n.code).SingleOrDefault();
            var od = data.OrderDetails.Where(n => n.idOrder == id).ToList();
            return View(od);
        } 
    }
}