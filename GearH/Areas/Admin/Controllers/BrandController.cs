using GearH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Areas.Admin.Controllers
{
    public class BrandController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(data.Brands.ToList().OrderBy(n => n.idCategory));
        }
        public ActionResult Detail(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var brand = data.Brands.SingleOrDefault(n => n.idBrand == id);
            if (brand == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(brand);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.idCategory = new SelectList(data.Categories.ToList(), "idCategory", "name");
            return View(new Brand());
        }
        [HttpPost]
        public ActionResult Create(Brand brand)
        {
            ViewBag.idCategory = new SelectList(data.Categories.ToList(), "idCategory", "name");
            if (ModelState.IsValid)
            {
                data.Brands.Add(brand);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return this.Create();
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var brand = data.Brands.SingleOrDefault(n => n.idBrand == id);
            if (brand == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(brand);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var brand = data.Brands.SingleOrDefault(n => n.idBrand == id);
            if (brand == null)
            {
                Response.StatusCode = 404;
                return null;
            }           
            var product = data.Products.Where(n => n.idBrand == id);
            if (product.Count() > 0)
            {
                ViewBag.ThongBao = "Hãng đang có sản phẩm. Vui lòng xóa sản phẩm của hãng trước khi xóa hãng này";
                return View(brand);
            }
            data.Brands.Remove(brand);
            data.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.idCategory = new SelectList(data.Categories.ToList(), "idCategory", "name");
            var brand = data.Brands.SingleOrDefault(n => n.idBrand == id);
            if (brand == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(brand);
        }
        [HttpPost]
        public ActionResult Edit(Brand brand)
        {
            ViewBag.idCategory = new SelectList(data.Categories.ToList(), "idCategory", "name");
            if (ModelState.IsValid)
            {
                var update = data.Brands.Find(brand.idBrand);
                update.name = brand.name;
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}