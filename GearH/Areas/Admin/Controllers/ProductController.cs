using GearH.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            return View(data.Products.ToList());
        }
        public ActionResult Detail(int id)
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var product = data.Products.SingleOrDefault(n => n.idProduct == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Admin"] == null)
            {
                return RedirectToAction("Login", "Home");
            }
            ViewBag.idCategory = new SelectList(data.Categories.ToList(), "idCategory", "name");
            ViewBag.idBrand = new SelectList(data.Brands.ToList(), "idBrand", "name");
            return View(new Product());
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product product, HttpPostedFileBase image)
        {
            ViewBag.idCategory = new SelectList(data.Categories.ToList(), "idCategory", "name");
            ViewBag.idBrand = new SelectList(data.Brands.ToList(), "idBrand", "name");
            if (ModelState.IsValid)
            {
                var sFileName = Path.GetFileName(image.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images/products"), sFileName);

                if (!System.IO.File.Exists(path))
                {
                    image.SaveAs(path);
                }

                product.create_at = DateTime.Now;
                product.update_at = DateTime.Now;
                data.Products.Add(product);
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
            var product = data.Products.SingleOrDefault(n => n.idProduct == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirm(int id)
        {
            var product = data.Products.SingleOrDefault(n => n.idProduct == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            var orderDetail = data.OrderDetails.Where(n => n.idProduct == id);
            if (orderDetail.Count() > 0)
            {
                ViewBag.ThongBao = "Sản phẩm đang có trong đơn hàng. Vui lòng xóa sản phẩm trong đơn hàng trước khi xóa sản phẩm này";
                return View(product);
            }
            data.Products.Remove(product);
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
            ViewBag.idBrand = new SelectList(data.Brands.ToList(), "idBrand", "name");
            var product = data.Products.SingleOrDefault(n => n.idProduct == id);
            if (product == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(product);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(Product product, HttpPostedFileBase image)
        {
            ViewBag.idCategory = new SelectList(data.Categories.ToList(), "idCategory", "name");
            ViewBag.idBrand = new SelectList(data.Brands.ToList(), "idBrand", "name");
            if (ModelState.IsValid)
            {
                var sFileName = Path.GetFileName(image.FileName);
                var path = Path.Combine(Server.MapPath("~/Content/images/products"), sFileName);

                if (!System.IO.File.Exists(path))
                {
                    image.SaveAs(path);
                }
                var update = data.Products.Find(product.idProduct);
                update.name = product.name;
                update.idBrand = product.idBrand;
                update.idCategory = product.idCategory;
                update.image = product.image;
                update.describe = product.describe; 
                update.price = product.price;
                update.quantity = product.quantity;
                update.update_at = DateTime.Now;
                
                data.SaveChanges();
                return RedirectToAction("Index");
            }
            return this.View();
        }
    }
}