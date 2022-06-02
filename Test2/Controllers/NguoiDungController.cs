using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Test2.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Configuration;
using System.Runtime.Serialization.Json;
using System.IO;
using GoogleRecaptcha;

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
            kh.Status = true;
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
        [AllowAnonymous]
        [ValidateAntiForgeryToken]

        public ActionResult DangNhap(FormCollection collection)
        {

            IRecaptcha<RecaptchaV2Result> recaptcha = new RecaptchaV2(
            new RecaptchaV2Data() { Secret = "6LdnoyogAAAAAMWU62Rjz46xcm6i_Nx9Ys4wfido" });
            var result = recaptcha.Verify();
            if (result.Success) // Success!!!
            {
                var makh = collection["MaKH"];
                var tendn = collection["TenDN"];
                var matkhau = collection["Password"];
                KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.UserName == tendn && n.PassWord == matkhau);
                if (kh != null)
                {
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
            else
            {
                ViewBag.Thongbao = "Bạn chưa tích xác nhận bên dưới !!!";
            }
            return View();

        }



        [HttpGet]
        public ActionResult ChinhSuaTK(String id)
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
        public ActionResult ChinhSuaTK(string id, FormCollection collection)
        {
            var khachHang = data.KhachHangs.First(a => a.MaKH == int.Parse(id));
            var maKH = collection["maKH"];
            var TenKH = collection["HoVaTen"];
            var tendn = collection["TenDN"];
            var matkhau = collection["Password"];
            var email = collection["Email"];
            var SDT = collection["SDT"];
            var NgaySinh = Convert.ToDateTime(collection["NgaySinh"]);
            var DiaChi = collection["DiaChi"];


            khachHang.MaKH = int.Parse(id);
            if (string.IsNullOrEmpty(TenKH))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                khachHang.UserName = tendn;
                khachHang.HoVaTen = TenKH;
                khachHang.PassWord = matkhau;
                khachHang.Email = email;
                khachHang.SDT = SDT;
                khachHang.NgaySinh = NgaySinh;
                khachHang.DiaChi = DiaChi;
                UpdateModel(khachHang);
                data.SubmitChanges();
                return RedirectToAction("ChinhSuaTK");
            }
            return this.ChinhSuaTK(id);

        }



        private string RandomString(int size, bool lowerCase)
        {
            StringBuilder sb = new StringBuilder();
            char c;
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                c = Convert.ToChar(Convert.ToInt32(rand.Next(65, 87)));
                sb.Append(c);
            }
            if (lowerCase)
                return sb.ToString().ToLower();
            return sb.ToString();
        }

        [HttpGet]
        public ActionResult QuenMatKhau()
        {
            return View();
        }
        [HttpPost]
        public ActionResult QuenMatKhau(FormCollection collection)
        {
            var Email = collection["Email"];
            // KhachHang khachHang = data.KhachHangs.FirstOrDefault(m => m.Email == Email);
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.Email == Email);
            if (kh != null)
            {
                String t = RandomString(8, false);
                kh.UserName = kh.UserName;
                kh.HoVaTen = kh.HoVaTen;
                kh.PassWord = t;
                kh.Email = kh.Email;
                kh.SDT = kh.SDT;
                kh.NgaySinh = kh.NgaySinh;
                kh.DiaChi = kh.DiaChi;

                String content = System.IO.File.ReadAllText(Server.MapPath("~/Content/MatKhau.html"));
                content = content.Replace("{{CustomerName}}", kh.HoVaTen);
                content = content.Replace("{{Password}}", t);


                new common.MailHelper().sendMail(Email, "Cấp mật khẩu mới từ Tiệm cafe của Anh Khoa và Quý", content);

                UpdateModel(kh);
                data.SubmitChanges();
                return RedirectToAction("EmailThongBao", "NguoiDung");
            }
            else
            {
                ViewBag.ThongBao = "Địa chỉ Email chưa đăng ký tài khoản !!! ";
            }
            return View();
        }

        public ActionResult EmailThongBao()
        {
            return View();
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

    }
}