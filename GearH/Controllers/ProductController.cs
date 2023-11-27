using GearH.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GearH.Controllers
{
    public class ProductController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public ActionResult Detail(int id)
        {
            var product = from s in data.Products where s.idProduct == id select s;
            return View(product.Single());
        }
        public ActionResult ProductFromCategory(int idCategory, int idBrand, int? page)
        {
            ViewBag.idCategory = idCategory;
            ViewBag.idBrand = idBrand;
            int iSize = 3;
            int iPageNum = (page ?? 1);
            var product = from s in data.Products where s.idCategory == idCategory && s.idBrand == idBrand select s;
            product = product.OrderByDescending(n => n.sold);
            return View(product.ToPagedList(iPageNum, iSize));
        }
    }
}