using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test2.Models;

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

        private decimal TongTien()
        {
            decimal iTongTien = 0;
            List<Giohang> lstGiohang = Session["Giohang"] as List<Giohang>;
            if (lstGiohang != null)
            {
                iTongTien = lstGiohang.Sum(n => n.dThanhtien);

            }
            return iTongTien;
        }
 
        public ActionResult Giohang()
        {
            List<Giohang> lstGiohang = Laygiohang();
            if(lstGiohang.Count==0)
            {
                return RedirectToAction("SanPham", "Home");

            }
            ViewBag.Tongsoluong = TongSoLuong();
            ViewBag.Tongtien = TongTien();
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
            ViewBag.Tongtien = TongTien();

            return View(lstGiohang);
        }

        public ActionResult DatHang(FormCollection collection)
        {
            DonHang dh = new DonHang();
            KhachHang kh = (KhachHang)Session["UserName"];
            List<Giohang> gh = Laygiohang();
            dh.MaKH = kh.MaKH;
            dh.NgayLap = DateTime.Now;
            var ngaygiao = string.Format("{0:MM/dd/yyyy}", collection["NgayGiao"]);
            dh.NgayGiao = DateTime.Parse(ngaygiao);
            dh.DiaChi = kh.DiaChi;
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
            Session["Giohang"] = null;
            return RedirectToAction("XacNhan", "Giohang");
        }

        public ActionResult XacNhan()
        {
            return View();
        }

    }
}
