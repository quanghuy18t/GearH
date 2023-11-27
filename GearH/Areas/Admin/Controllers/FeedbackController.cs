using GearH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Areas.Admin.Controllers
{
    public class FeedbackController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Index()
        {
            /*if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }*/
            return View(data.Feedbacks.ToList().OrderByDescending(n => n.create_at));
        }

        public ActionResult Detail(int id)
        {
            var feedback = data.Feedbacks.SingleOrDefault(n => n.idFeedback == id);
            if (feedback == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(feedback);
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var feedback = data.Feedbacks.SingleOrDefault(n => n.idFeedback == id);
            if (feedback == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(feedback);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var feedback = data.Feedbacks.SingleOrDefault(n => n.idFeedback == id);
            if (feedback == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Feedbacks.Remove(feedback);
            data.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}