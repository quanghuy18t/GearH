using GearH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Areas.Admin.Controllers
{
    public class AccountController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(data.Accounts.Where(n => n.idRole == 2).ToList());
        }
        public ActionResult Detail(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var acc = data.Accounts.SingleOrDefault(n => n.idAccount == id);
            if (acc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(acc);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var acc = data.Accounts.SingleOrDefault(n => n.idAccount == id);
            if (acc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(acc);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var acc = data.Accounts.SingleOrDefault(n => n.idAccount == id);
            if (acc == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var fb = data.Feedbacks.SingleOrDefault(n => n.idAccount == id);
            if (fb != null)
            {
                data.Feedbacks.Remove(fb);
            }
            
            var or = data.Orders.SingleOrDefault(n => n.idAccount == id);
            if (or != null)
            {
                data.Orders.Remove(or);
            }
    
            data.Accounts.Remove(acc);
            data.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}