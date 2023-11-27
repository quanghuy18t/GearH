using GearH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(data.Categories.ToList());
        }
        public ActionResult Detail(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var category = data.Categories.SingleOrDefault(n => n.idCategory == id);
            if (category == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(category);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(new Category());
        }
        [HttpPost]
        public ActionResult Create(Category category)
        {
            if (ModelState.IsValid)
            {
                data.Categories.Add(category);
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return this.View();
        }
        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var category = data.Categories.SingleOrDefault(n => n.idCategory == id);
            if (category == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(category);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var category = data.Categories.SingleOrDefault(n => n.idCategory == id);
            if (category == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var brand = data.Brands.Where(n => n.idCategory == id);
            if (brand.Count() > 0)
            {
                ViewBag.ThongBao = "Danh mục đang có hãng. Vui lòng xóa hãng trong danh mục trước khi xóa danh mục này";
                return View(category);
            }
            var product = data.Products.Where(n => n.idCategory == id);
            if (product.Count() > 0)
            {
                ViewBag.ThongBao = "Danh mục đang có sản phẩm. Vui lòng xóa sản phẩm trong danh mục trước khi xóa danh mục này";
                return View(category);
            }
            data.Categories.Remove(category);
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
            var category = data.Categories.SingleOrDefault(n => n.idCategory == id);
            if (category == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(category);
        }
        [HttpPost]
        public ActionResult Edit(Category category)
        {
            if (ModelState.IsValid)
            {
                var update = data.Categories.Find(category.idCategory);
                update.name = category.name;
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}