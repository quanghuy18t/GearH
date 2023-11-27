using GearH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Controllers
{
    public class FeedBackController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();

        [HttpGet]
        public ActionResult Index()
        {
            return View(new Feedback());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                if (Session["Account"] == null)
                {
                    return RedirectToAction("LogIn", "User");
                }
                Account ac = (Account)Session["Account"];
                feedback.idAccount = ac.idAccount;
                feedback.create_at = DateTime.Now;
                data.Feedbacks.Add(feedback);
                data.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return this.Index();
        }
    }
}