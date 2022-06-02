using MoMo;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test2.Models;
using Newtonsoft.Json.Linq;

namespace Test2.Controllers
{
    public class GiohangController : Controller
    {
        dbCoffeeDataContext data = new dbCoffeeDataContext();
        // GET: Giohang
        public List<Giohang> Laygiohang()
        {
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang == null)
            {
                lstGiohang = new List<Giohang>();
                Session["Giohang"] = lstGiohang;
            }
            return lstGiohang;
        }

        public ActionResult Themgiohang (string sMaSP, string strURL)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.Find(n => n.sMaSP == sMaSP);
            if(sanpham == null)
            {
                sanpham = new Giohang(sMaSP);
                lstGiohang.Add(sanpham);
                return Redirect(strURL);
            }
            else
            {
                sanpham.iSoluong++;
                return Redirect(strURL);
            }
        }

        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if(lstGiohang!=null)
            {
                iTongSoLuong = lstGiohang.Sum(n => n.iSoluong);

            }
            return iTongSoLuong;
        }

        private decimal TongTienhang()
        {
            decimal iTongTien = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);

            }
            return iTongTien;
        }
        // Tien ship ............................

        private decimal TienShip()
        {
            decimal TienShip =25000;
           
            return TienShip;
        }

        private decimal TongTienThu()
        {
            decimal iTongTien = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);

            }
            return iTongTien + TienShip();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        private decimal TongTienDola()
        {
            decimal iTongTien = 0;
            decimal dola = 23300;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);

            }
            return iTongTien/dola;
        }

        public ActionResult Giohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if(lstGiohang.Count==0)
            {
                return RedirectToAction("SanPham", "Home");

            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTienhang();
            return View(lstGiohang);
        }


        public ActionResult XoaGiohang(string sMaSP)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.sMaSP == sMaSP);
            if(sanpham!=null)
            {
                lstGiohang.RemoveAll(n => n.sMaSP == sMaSP);
                return RedirectToAction("Giohang");
            }
            if(lstGiohang.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult CapNhatGioHang(string sMaSP,FormCollection f)
        {
            List<Giohang> lstGiohang = Laygiohang();
            Giohang sanpham = lstGiohang.SingleOrDefault(n => n.sMaSP == sMaSP);
            if(sanpham!=null)
            {
                sanpham.iSoluong = int.Parse(f["txtSoluong"].ToString());
            }
            return RedirectToAction("Giohang");
        }

        public ActionResult XoaTatcaGiohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            lstGiohang.Clear();
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["UserName"] == null || Session["UserName"].ToString()=="")
            {
                return RedirectToAction("DangNhap", "NguoiDung");
            }
            if (Session["Giohang"]==null)
            {
                return RedirectToAction("Index", "Home");
            }

            List<Giohang> lstGiohang = Laygiohang();
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtienhang = TongTienhang();
            ViewBag.TongtienThu = TongTienThu();
            ViewBag.TienShip = TienShip();

            // Select tổng hóa đơn chưa thanh toán.

            return View(lstGiohang);
        }

        public ActionResult DatHang(FormCollection collection)
        {
           // if( n<5){ cụm dưới} n: là đơn hàng giao tại nhà chưa thanh toán 
           // else{ Thông báo bạn chỉ  dc đặt thanh toán trực tuyến  }; // đây là nút đặt hàng giao sau 
            DonHang dh = new DonHang();
            KhachHang kh = (KhachHang)Session["UserName"];
            List<Giohang> gh = Laygiohang();
            dh.MaKH = kh.MaKH;
            dh.NgayLap = DateTime.Now;
          //  var ngaygiao = string.Format("0:MM/dd/yyyy}", collection["NgayGiao"]);
            dh.NgayGiao = DateTime.Now.AddDays(1);
            dh.DiaChi = collection["diachi"];
            dh.GhiChu = collection["cuthe"];
            dh.ThanhTien = TongTienhang();
            data.DonHangs.InsertOnSubmit(dh);
            data.SubmitChanges();
            foreach(var item in gh)
            {
                ChiTietDonHang ctdh = new ChiTietDonHang();
                ctdh.MaDH = dh.MaDH;
                ctdh.MaSP = item.sMaSP;
                ctdh.SoLuongSP = item.iSoluong;
                ctdh.ThanhTien = (decimal)item.dThanhtien;
                data.ChiTietDonHangs.InsertOnSubmit(ctdh);
            }
            dh.Status = false;
            data.SubmitChanges();

            String content = System.IO.File.ReadAllText(Server.MapPath("~/Content/DonHang.html"));
            content = content.Replace("{{CustomerName}}", kh.HoVaTen);
            content = content.Replace("{{Phone}}", kh.SDT);
            content = content.Replace("{{Email}}", kh.Email);
            content = content.Replace("{{Address}}", dh.DiaChi);
            content = content.Replace("{{cuthe}}", collection["cuthe"]);
            content = content.Replace("{{NgayDat}}", dh.NgayLap.ToString());
            content = content.Replace("{{NgayGiao}}", dh.NgayGiao.ToString());
            content = content.Replace("{{Total}}", TongTienThu().ToString());
            var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

            new common.MailHelper().sendMail(kh.Email, "Đơn hàng mới từ tiệm cà phê của Khoa và Quý <3", content);
            new common.MailHelper().sendMail(toEmail, "Đơn hàng mới từ tiệm cà phê của Khoa và Quý <3", content);

            Session["Giohang"] = null;
            return RedirectToAction("XacNhan", "Giohang");
        }

        public ActionResult XacNhan()
        {
            return View();
        }


        public ActionResult ThanhToanOnline()
        {
            List<Giohang> gioHang = Session["GioHang"] as List<Giohang>;
            string endpoint = "https://test-payment.momo.vn/gw_payment/transactionProcessor";
            string partnerCode = "MOMOOJOI20210710";
            string accessKey = "iPXneGmrJH0G8FOP";
            string serectKey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string orderInfo = "Đơn hàng của bạn";
            string returnUrl = "https://localhost:44332/Giohang/ReturnUrl";
            string notifyurl = "http://ba1adf48beba.ngrok.io/Giohang/NotifyUrl";

            string amount = gioHang.Sum(n => n.dThanhtien).ToString();
            string orderid = DateTime.Now.Ticks.ToString();
            string requestId = DateTime.Now.Ticks.ToString();
            string extraData = "";

            string rawHash =
                "partnerCode=" +
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
            string signature = crypto.signSHA256(rawHash, serectKey);
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

            return Redirect(jmessage.GetValue("payUrl").ToString());
        }


        public ActionResult ReturnUrl(FormCollection collection)
        {
            string param = Request.QueryString.ToString().Substring(0, Request.QueryString.ToString().IndexOf("signature") - 1);
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectkey = "sFcbSGRSJjwGxwhhcEktCHWYUuTuPNDB";
            string signature = crypto.signSHA256(param, serectkey);
            if (signature != Request["signature"].ToString())
            {
                ViewBag.message = "Thông tin Request không hợp lệ";
                return View();
            }
            if (!Request.QueryString["errorCode"].Equals("0"))
            {
                ViewBag.message = "Thanh toán thất bại";
            }
            else
            {
                DonHang dh = new DonHang();
                KhachHang kh = (KhachHang)Session["UserName"];
                List<Giohang> gh = Laygiohang();
                dh.MaKH = kh.MaKH;
                dh.NgayLap = DateTime.Now;
                var ngaygiao = string.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
                ////// lỗi ko xài đc cái chọn ngày giao nó sẽ mặc định chọn ngày hôm nay
                dh.NgayGiao = DateTime.Today.AddDays(2); // giao hàng sau 2 ngày
                ///// quý đã thử tìm cách nhưng chưa thanh
                dh.DiaChi = kh.DiaChi;
                dh.Status = true;
                dh.ThanhTien = TongTienhang();
                data.DonHangs.InsertOnSubmit(dh);
                data.SubmitChanges();
                foreach (var item in gh)
                {
                    ChiTietDonHang ctdh = new ChiTietDonHang();
                    ctdh.MaDH = dh.MaDH;
                    ctdh.MaSP = item.sMaSP;
                    ctdh.SoLuongSP = item.iSoluong;
                    ctdh.ThanhTien = (decimal)item.dThanhtien;
                    data.ChiTietDonHangs.InsertOnSubmit(ctdh);
                }
                data.SubmitChanges();
                Session["Giohang"] = null;
                return RedirectToAction("XacNhan", "Giohang");

                String content = System.IO.File.ReadAllText(Server.MapPath("~/Content/DonHang.html"));
                content = content.Replace("{{CustomerName}}", kh.HoVaTen);
                content = content.Replace("{{Phone}}", kh.SDT);
                content = content.Replace("{{Email}}", kh.Email);
                content = content.Replace("{{Address}}", kh.DiaChi);
                content = content.Replace("{{NgayDat}}", dh.NgayLap.ToString());
                content = content.Replace("{{NgayGiao}}", dh.NgayGiao.ToString());
                content = content.Replace("{{Total}}", TongTienThu().ToString());
                var toEmail = ConfigurationManager.AppSettings["ToEmailAddress"].ToString();

                new common.MailHelper().sendMail(kh.Email, "Đơn hàng mới từ tiệm cà phê của Khoa và Quý <3", content);
                new common.MailHelper().sendMail(toEmail, "Đơn hàng mới từ tiệm cà phê của Khoa và Quý <3", content);

            }
            return View();
        }
        [HttpPost]
        public JsonResult NotifyUrl()
        {
            string param = "";
            param =
                "partner_code=" + Request["partner_code"] +
                "&access_key=" + Request["access_key"] +
                "&amount=" + Request["amount"] +
                "&order_id=" + Request["order_id"] +
                "&order_info=" + Request["order_info"] +
                "&order_type=" + Request["order_type"] +
                "&transaction_id=" + Request["transaction_id"] +
                "&message=" + Request["message"] +
                "&response_time=" + Request["response_time"] +
                "&status_code=" + Request["status_code"];
            param = Server.UrlDecode(param);
            MoMoSecurity crypto = new MoMoSecurity();
            string serectkey = ConfigurationManager.AppSettings["serectkey"].ToString();
            string signature = crypto.signSHA256(param, serectkey);
            if (signature != Request["signature"].ToString())
            {

            }
            string status_code = Request["status_code"].ToString();
            if ((status_code != "0"))
            {

            }
            else
            {

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

    }
}
