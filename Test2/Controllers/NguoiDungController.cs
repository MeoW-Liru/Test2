using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test2.Models;
using System.Data.Entity;

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
            var makh = collection["MaKH"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Password"];

            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.UserName == tendn && n.PassWord == matkhau);
            if(kh != null)
            {
                //ViewBag.Thongbao = "Đăng nhập thành công";
                Session["UserName1"] = tendn;
                Session["UserName"] = kh;
                Session["MaKH"] = kh.MaKH;
                return RedirectToAction("GioHang", "Giohang");
            }
            else
            {
                ViewBag.Thongbao = "Tên đăng nhập hoặc Mật khẩu không đúng";
            }
            return View();
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index","Home");
        }

        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////////\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\//////\\\\\\.\.\.\/\/\/\/\/\
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChinhSuaTK(String id )
        {
            
            KhachHang khachHang = data.KhachHangs.FirstOrDefault(m => m.MaKH == int.Parse(id));
            ViewBag.MaKH = khachHang.MaKH;
            if (khachHang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(khachHang);
        }
        [HttpPost]
        public ActionResult ChinhSuaTK(String id, FormCollection collection)
        {
            var khachHang = data.KhachHangs.First(m => m.MaKH == int.Parse(id));
            var maKH = collection["maKH"];
            var TenKH = collection["HoVaTen"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Password"];
            var Email = collection["Email"];
            var SDT = collection["SDT"];
            var NgaySinh = Convert.ToDateTime(collection["NgaySinh"]);
            var DiaChi = collection["DiaChi"];
            //  var trangThai = bool.Parse(collection["Status"]);


            khachHang.MaKH = int.Parse(id);
            if (string.IsNullOrEmpty
                (TenKH))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                khachHang.UserName = tendn;
                khachHang.HoVaTen = TenKH;
                khachHang.PassWord = matkhau;
                khachHang.Email = Email;
                khachHang.SDT = SDT;
                khachHang.NgaySinh = NgaySinh;
                khachHang.DiaChi = DiaChi;
            ///    khachHang.Status = trangThai;
               
                UpdateModel(khachHang);
                data.SubmitChanges();
                return RedirectToAction("ChinhSuaTK");
            }
            return this.ChinhSuaTK(id);
        }

    }
}