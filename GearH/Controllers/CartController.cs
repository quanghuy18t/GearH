using GearH.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Data.Entity.Infrastructure;
using System.Security.Principal;
using GearH.Models.MOMO;
using Newtonsoft.Json.Linq;

namespace GearH.Controllers
{
    public class CartController : Controller
    {
        dbGearHDataContext data = new dbGearHDataContext();
        private static double price = -1;
        private bool SendMail(string name, string subject, string content, string toMail)
        {
            bool rs = false;
            string email = ConfigurationManager.AppSettings["Email"];
            string password = ConfigurationManager.AppSettings["PasswordEmail"];
            try
            {
                MailMessage message = new MailMessage();
                var smtp = new SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com"; //host name
                    smtp.Port = 587; //port number
                    smtp.EnableSsl = true; //whether your smtp server requires SSL
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential()
                    {
                        UserName = email,
                        Password = password
                    };
                }
                MailAddress fromAddress = new MailAddress(email, name);
                message.From = fromAddress;
                message.To.Add(toMail);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = content;
                smtp.Send(message);
                rs = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                rs = false;
            }
            return rs;
        }

        private List<Cart> ViewCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        private int TotalQuantity()
        {
            int ans = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                ans = lstCart.Sum(n => n.quantity);
            }
            return ans;
        }
        private double TotalPrice()
        {
            double ans = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                ans = lstCart.Sum(n => n.Total);
            }
            return ans;
        }
        public ActionResult Add(int id, string url)
        {
            List<Cart> lstCart = ViewCart();
            Cart sp = lstCart.Find(n => n.idProduct == id);
            if (sp == null)
            {
                sp = new Cart(id);
                lstCart.Add(sp);
            }
            else
            {
                sp.quantity++;
            }
            return Redirect(url);
        }
        public ActionResult Cart()
        {
            List<Cart> lstCart = ViewCart();
            if (lstCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.TotalPrice = TotalPrice();
            return View(lstCart);
        }
        public ActionResult CartPartial()
        {
            ViewBag.TotalQuantity = TotalQuantity();
            return PartialView();
        }
        public ActionResult Update(int id, FormCollection f)
        {
            List<Cart> lstCart = ViewCart();
            Cart item = lstCart.SingleOrDefault(n => n.idProduct == id);
            if (item != null)
            {
                item.quantity = int.Parse(f["txtQuantity"].ToString());
            }
            return RedirectToAction("Cart");
        }
        public ActionResult DeleteProduct(int id)
        {
            List<Cart> lstCart = ViewCart();
            Cart item = lstCart.SingleOrDefault(n => n.idProduct == id);
            if (item != null)
            {
                lstCart.RemoveAll(n => n.idProduct == id);
            }
            return RedirectToAction("Cart");
        }
        [HttpPost] 
        public ActionResult AddCode(FormCollection f)
        {
            var code = f["fCode"];
            if (code != null)
            {
                var coupon = data.Coupons.SingleOrDefault(n => n.code == code);
                ViewBag.lstCart = ViewCart();
                if (coupon != null)
                {
                    price = TotalPrice() - (TotalPrice() * coupon.discount) / 100;
                }
            }
            return RedirectToAction("CheckOut");
        }
        [HttpGet]        
        public ActionResult CheckOut()
        {
            if (Session["Account"] == null)
            {
                return RedirectToAction("LogIn", "User");
            }
            ViewBag.lstCart = ViewCart();
            ViewBag.TotalPrice = price;
            if (ViewBag.TotalPrice == -1)
            {
                ViewBag.TotalPrice = TotalPrice();
            }
            return View(new Order());
        }
        [HttpPost]
        public ActionResult CheckOut(Order order)
        {
            if (ModelState.IsValid)
            {
                Account account = (Account)Session["Account"];
                List<Cart> lstCart = ViewCart();
                if (Session["Cart"] != null)
                {
                    order.idAccount = account.idAccount;
                    order.total = int.Parse(price.ToString());
                    if (order.total == -1)
                    {
                        order.total = int.Parse(TotalPrice().ToString());
                    }
                    order.status = 1;
                    order.order_date = DateTime.Now;
                    Random rd = new Random();
                    order.code = "DH" + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9) + rd.Next(0, 9);
                    data.Orders.Add(order);
                    data.SaveChanges();

                    foreach (var item in lstCart)
                    {
                        OrderDetail ord = new OrderDetail();
                        ord.idOrder = order.idOrder;
                        ord.idProduct = item.idProduct;
                        ord.quantity = item.quantity;
                        ord.total = (int)item.price;

                        Product pro = data.Products.SingleOrDefault(n => n.idProduct == item.idProduct);
                        pro.sold += 1;

                        data.OrderDetails.Add(ord);
                    }
                    data.SaveChanges();

                    var product = "";
                    var total = price;
                    if (total == -1)
                    {
                        total = int.Parse(TotalPrice().ToString());
                    }
                    foreach (var item in lstCart)
                    {
                        product += "<tr>";
                        product += "<td>" + item.name + "</td>";
                        product += "<td>" + item.quantity +"</td>";
                        product += "<td>" + string.Format("{0:#,##0 VNĐ}", item.Total) + "</td>";
                        product += "</tr>";
                    }
                    string contentCustomer = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/send.html"));
                    contentCustomer = contentCustomer.Replace("{{MaDon}}", order.code);
                    contentCustomer = contentCustomer.Replace("{{SanPham}}", product);
                    contentCustomer = contentCustomer.Replace("{{NgayDat}}", order.order_date.ToString("dd/MM//yyyy HH:mm:ss"));
                    contentCustomer = contentCustomer.Replace("{{TenKhachHang}}", order.name);
                    contentCustomer = contentCustomer.Replace("{{Phone}}", order.phone);
                    contentCustomer = contentCustomer.Replace("{{Email}}", order.email);
                    contentCustomer = contentCustomer.Replace("{{DiaChiNhanHang}}", order.address);
                    contentCustomer = contentCustomer.Replace("{{TongTien}}", string.Format("{0:#,##0 VNĐ}", total));
                    SendMail("GearH", "Đơn hàng #" + order.code, contentCustomer.ToString(), order.email);

                    Session["Cart"] = null;
                    if (order.payment == 1)
                    {
                        return RedirectToAction("PaymentSuccess");
                    }
                    if (order.payment == 2)
                    {
                        var url = MomoPayment(order);
                        return Redirect(url);
                    }
                    if (order.payment == 3)
                    {
                        var url = VNPayment(order);
                        return Redirect(url);
                    }
                }
            }
            return this.CheckOut();
        }
        #region CODE
        public ActionResult PaymentSuccess()
        {
            ViewBag.Message = "Đặt hàng thành công";
            return View();
        }
        #endregion
        #region VNPAY
        public string VNPayment(Order order)
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.1.0");
            pay.AddRequestData("vnp_Command", "pay");
            pay.AddRequestData("vnp_TmnCode", tmnCode);
            pay.AddRequestData("vnp_Amount", order.total.ToString());
            pay.AddRequestData("vnp_BankCode", "");
            pay.AddRequestData("vnp_CreateDate", order.order_date.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", "VND");
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress());
            pay.AddRequestData("vnp_Locale", "vn");
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
            pay.AddRequestData("vnp_OrderType", "other");
            pay.AddRequestData("vnp_ReturnUrl", returnUrl);
            pay.AddRequestData("vnp_TxnRef", order.code);

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);
            return paymentUrl;
        }
        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                string orderId = Convert.ToString(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        var order = data.Orders.FirstOrDefault(n => n.code == orderId);
                        if (order != null)
                        {
                            order.status = 2;//đã thanh toán
                            data.Orders.Attach(order);
                            data.Entry(order).State = System.Data.Entity.EntityState.Modified;
                            data.SaveChanges();
                        }
                        //Thanh toán thành công
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {                
                    ViewBag.Message = ("Có lỗi xảy ra trong quá trình xử lý, TranID={1}, ResponseCode{2}", vnpayTranId, vnp_ResponseCode);
                }
            }

            return View();
        }
        #endregion
        #region Momo
        public string MomoPayment(Order order)
        {
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMO";
            string accessKey = "F8BBA842ECF85";
            string serectkey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
            string orderInfo = "GearH";
            string returnUrl = "https://localhost:44388/Cart/MomoPaymentConfirm";
            string notifyurl = ""; //test trên ngrok

            string amount = order.total.ToString();
            string orderid = order.code;
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            string rawHash = "partnerCode=" +
                partnerCode + "&accessKey=" +
                accessKey + "&requestId=" +
                requestId + "&amount=" +
                amount + "&orderId=" +
                orderid + "&orderInfo=" +
                orderInfo + "&returnUrl=" +
                returnUrl + "&notifyUrl=" +
                notifyurl + "&extraData=" +
                extraData;

            MoMoSecurity crypto = new MoMoSecurity();
            //sign signature SHA256
            string signature = crypto.signSHA256(rawHash, serectkey);

            //build body json request
            JObject message = new JObject
            {
                { "partnerCode", partnerCode },
                { "accessKey", accessKey },
                { "requestId", requestId },
                { "amount", amount },
                { "orderId", orderid },
                { "orderInfo", orderInfo },
                { "returnUrl", returnUrl },
                { "notifyUrl", notifyurl },
                { "extraData", extraData },
                { "requestType", "captureMoMoWallet" },
                { "signature", signature }

            };

            string responseFromMomo = PaymentRequest.sendPaymentRequest(endpoint, message.ToString());

            JObject jmessage = JObject.Parse(responseFromMomo);

            return jmessage.GetValue("payUrl").ToString();
        }
        /*public ActionResult ConfirmPaymentClient(Result result)
        {
            //lấy kết quả Momo trả về và hiển thị thông báo cho người dùng (có thể lấy dữ liệu ở đây cập nhật xuống db)
            string rMessage = result.message;
            string rOrderId = result.orderId;
            string rErrorCode = result.errorCode; // = 0: thanh toán thành công
            return View();
        }*/
        [HttpPost]
        public void SavePayment()
        {

            //var order = data.Orders.FirstOrDefault(n => n.code == order.id);
        }
        #endregion
    }
}