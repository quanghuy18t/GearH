using GearH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Areas.Admin.Controllers
{
    public class CouponController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Index()
        {
            return View(data.Coupons.ToList());
        }
        public ActionResult Detail(int id)
        {
            var coupon = data.Coupons.SingleOrDefault(n => n.idCoupon == id);
            if (coupon == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(coupon);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View(new Coupon());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                data.Coupons.Add(coupon);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(new Coupon());
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var coupon = data.Coupons.SingleOrDefault(n => n.idCoupon == id);
            if (coupon == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(coupon);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var coupon = data.Coupons.SingleOrDefault(n => n.idCoupon == id);
            if (coupon == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.Coupons.Remove(coupon);
            data.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var coupon = data.Coupons.SingleOrDefault(n => n.idCoupon == id);
            if (coupon == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(coupon);
        }
        [HttpPost]
        public ActionResult Edit(Coupon coupon)
        {
            if (ModelState.IsValid)
            {
                var update = data.Coupons.Find(coupon.idCoupon);
                update.code = coupon.code;
                update.discount = coupon.discount;
                update.condition = coupon.condition;
                update.quantity = coupon.quantity;
                update.date_start = update.date_start;
                update.date_end = coupon.date_end;
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(coupon);
        }
    }
}