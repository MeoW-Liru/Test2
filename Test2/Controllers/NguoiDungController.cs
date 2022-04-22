using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test2.Models;

namespace Test2.Controllers
{
    public class NguoiDungController : Controller
    {
        // GET: NguoiDung
        dbCoffeeDataContext data = new dbCoffeeDataContext();

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(FormCollection collection,KhachHang kh)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["Password"];
            var hovaten = collection["HoVaTen"];
            var email = collection["Email"];
            var sdt = collection["SDT"];
            var diachi = collection["DiaChi"];
            var ngaysinh = string.Format("{0:MM/dd/yyyy}",collection["NgaySinh"]);


            kh.UserName = tendn;
            kh.PassWord = matkhau;
            kh.HoVaTen = hovaten;
            kh.Email = email;
            kh.SDT = sdt;
            kh.DiaChi = diachi;
            kh.NgaySinh = DateTime.Parse(ngaysinh);
            data.KhachHangs.InsertOnSubmit(kh);
            data.SubmitChanges();
            return this.DangKy();
        }

        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection collection)
        {
            var tendn = collection["TenDN"];
            var matkhau = collection["Password"];
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.UserName == tendn && n.PassWord == matkhau);
            if(kh != null)
            {
                //ViewBag.Thongbao = "Đăng nhập thành công";
                Session["UserName"] = kh;
                return RedirectToAction("GioHang", "Giohang");
            }
            else
            {
                ViewBag.Thongbao = "Tên đăng nhập hoặc Mật khẩu không đúng";
            }
            return View();
        }
    }
}