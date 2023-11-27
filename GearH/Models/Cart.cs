using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GearH.Models
{
    public class Cart
    {
        dbGearHDataContext data = new dbGearHDataContext();
        public int idProduct { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }

        public double Total
        {
            get { return quantity * price; }
        }
        public Cart(int id)
        {
            Product item = data.Products.Single(n => n.idProduct == id);
            idProduct = id;
            name = item.name;
            image = item.image;
            price = double.Parse(item.price.ToString());
            quantity = 1;
        }
    }
}