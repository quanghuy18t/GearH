using GearH.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;

namespace GearH.Controllers
{
    public class HomeController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        private List<Product> NewProduct(int count)
        {
            return data.Products.OrderByDescending(a => a.create_at).Take(count).ToList();
        }
        private List<Product> BestSell(int count)
        {
            return data.Products.OrderByDescending(a => a.sold).Take(count).ToList();
        }
        public ActionResult Index()
        {
            var lstNewProduct = NewProduct(6);
            return View(lstNewProduct);
        }
        public ActionResult ProductPartial()
        {
            var lstBrand = from br in data.Brands select br;
            ViewBag.brand = lstBrand;
            var lstCategory = from ct in data.Categories select ct;
            return PartialView(lstCategory);
        }
        public ActionResult CategoryPartial()
        {
            var lstBrand = from br in data.Brands select br;
            ViewBag.brand = lstBrand;
            var lstCategory = from ct in data.Categories select ct;
            return PartialView(lstCategory);
        }
        public ActionResult BestSellPartial()
        {
            var lstSpecialProduct = BestSell(5);
            return PartialView(lstSpecialProduct);
        }
        public ActionResult Search(string searchString, int? page)
        {
            int iSize = 3;
            int iPageNum = (page ?? 1);
            ViewBag.SearchString = searchString;
            var products = from s in data.Products select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(n => n.name.Contains(searchString)).OrderByDescending(n => n.sold);
            }
            if (products.Count() == 0)
            {
                ViewBag.ThongBao = "Không tìm thấy sản phẩm với từ khóa " + searchString;
            }
            return View(products.ToPagedList(iPageNum, iSize));
        }

        public ActionResult FooterPartial()
        {
            return PartialView();
        }
    }
}