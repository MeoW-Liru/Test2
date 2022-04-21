﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test2.Models;
using PagedList;
using PagedList.Mvc;
using System.IO;
using System.Data;
using ClosedXML.Excel;

namespace Test2.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        dbCoffeeDataContext data = new dbCoffeeDataContext();
        
        [HttpGet]
        public ActionResult DangNhapAdmin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhapAdmin(FormCollection collection)
        {
            var tendn = collection["UserName"];
            var matkhau = collection["PassWord"];

            Admin ad = data.Admins.SingleOrDefault(n => n.UserName == tendn && n.PassWord == matkhau);
            if (ad != null)
            {
                //ViewBag.Thongbao = "Đăng nhập thành công";
                Session["UserName"] = ad;
                return RedirectToAction("Cafe", "Admin");
            }
            else
            {
                ViewBag.Thongbao = "Tên đăng nhập hoặc Mật khẩu không đúng";
                //return View();
            }
            return View();
        }
        

        [HttpGet]
        public ActionResult ThemSP()
        {
            ViewBag.MaLoaiSP = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai");
            ViewBag.MaNCC = new SelectList(data.NhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSP(SanPham sp,HttpPostedFileBase fileupload)
        {
            ViewBag.MaLoaiSP = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai");
            ViewBag.MaNCC = new SelectList(data.NhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");


            if(fileupload == null)
            {
                ViewBag.Thongbao = "Vui lòng chọn ảnh bìa";
                    return View();
            }
            else
            {
                if(ModelState.IsValid)
                {
                    var filename = Path.GetFileName(fileupload.FileName);

                    var path = Path.Combine(Server.MapPath("~/images/CafeProduct"), filename);

                    if (System.IO.File.Exists(path))
                        ViewBag.Thongbao = "Hình ảnh đã tồn tại";
                    else
                    {
                        fileupload.SaveAs(path);
                    }
                    sp.HinhAnh = filename;
                    data.SanPhams.InsertOnSubmit(sp);
                    data.SubmitChanges();
                }
                return RedirectToAction("Cafe");
            }
        }

        public ActionResult ChitietSP(string id)
        {
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if(sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }

        [HttpGet]
        public ActionResult XoaSP(string id)
        {
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sp);
        }
        [HttpPost,ActionName("XoaSP")]
        public ActionResult XacNhanXoaSP(string id)
        {
            SanPham sp = data.SanPhams.SingleOrDefault(n => n.MaSP == id);
            ViewBag.MaSP = sp.MaSP;
            if (sp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SanPhams.DeleteOnSubmit(sp);
            data.SubmitChanges();
            return RedirectToAction("Cafe");
        }

        public ActionResult SuaSP(string id)
        {
            var sanpham = data.SanPhams.First(m => m.MaSP == id);
            ViewBag.MaLoaiSP = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sanpham.MaLoaiSP);
            ViewBag.MaNCC = new SelectList(data.NhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sanpham.MaNCC);
            return View(sanpham);
        }
        [HttpPost]
        public ActionResult SuaSP(string id, FormCollection collection)
        {
            var sanpham = data.SanPhams.First(m => m.MaSP == id);

            var maCP = collection["macp"];
            var tenSP = collection["tensp"];
            var moTa = collection["mota"];
            var giaTien = Decimal.Parse(collection["giatien"]);
            var ngayDang = Convert.ToDateTime(collection["ngaydang"]);
            var trongLuong = Double.Parse(collection["trongluong"]);
            var hanSD = Convert.ToDateTime(collection["hansd"]);
            var ngaySX = Convert.ToDateTime(collection["ngaysx"]);
            var maLoaiSP = collection["maloasp"];
            var maNCC = collection["mancc"];
            var HinhAnh = collection["HinhAnh"];
            //var trangThai = bool.Parse(collection["trangthai"]);
            sanpham.MaSP = id;
            if (string.IsNullOrEmpty(tenSP))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {
                    sanpham.MaSP = maCP;
                    sanpham.TenSP = tenSP;
                    sanpham.MoTa = moTa;
                    sanpham.GiaTien = giaTien;
                    sanpham.NgayDang = ngayDang;
                    sanpham.TrongLuong = trongLuong;
                    sanpham.HSD = hanSD;
                    sanpham.NSX = ngaySX;
                    sanpham.MaLoaiSP = maLoaiSP;
                    sanpham.MaNCC = maNCC;
                    sanpham.HinhAnh = HinhAnh;
                    //sanpham.Status = trangThai;
                    UpdateModel(sanpham);
                    data.SubmitChanges();
                    return RedirectToAction("Cafe");
            }
            return this.SuaSP(id);
        }

        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null)
            {
                return "";
            }
            file.SaveAs(Server.MapPath("~/images/CafeProduct" + file.FileName));
            return "~/images/CafeProduct" + file.FileName;
        }

        public ActionResult BangDieuKhien()
        {
            return View();
        }

        public ActionResult Form()
        {
            return View();
        }

        public ActionResult Table()
        {

            return View();
        }
        
        public ActionResult Typo()
        {
            return View();
        }
        public ActionResult Button()
        {
            return View();
        }

        public ActionResult Cafe(int ? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            return View(data.SanPhams.ToList().OrderBy(n=>n.MaSP).ToPagedList(pageNumber,pageSize));
        }

        public ActionResult ChiTiet()
        {          
            return View(data.ChiTietDonHangs.ToList());
        }

        public ActionResult DonHang()
        {
            return View(data.DonHangs.ToList());
        }

        ////In
        [HttpPost]
        public FileResult Export()
        {
            DataTable dt = new DataTable("Gird");
            dt.Columns.AddRange(new DataColumn[] {
                new DataColumn("Mã Đơn Hàng"),
                 new DataColumn("Mã Khách Hàng"),
                  new DataColumn("Tên Khách Hàng "),
                   new DataColumn ("NgayGiao"),
                   new DataColumn ("NgayLap"),
                    new DataColumn("Địa Chỉ"),
                    new DataColumn("Ghi Chú"),
                    new DataColumn("Đã Thanh Toán")});
            var emps = from emp in data.DonHangs.ToList() select emp;
            foreach (var emp in emps)
            {
                dt.Rows.Add(emp.MaDH, emp.MaKH,emp.KhachHang.HoVaTen ,emp.NgayLap, emp.NgayGiao, emp.DiaChi, emp.GhiChu, emp.Status);

            }
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Hóa-Đơn.xlsx");
                }
            }

        }
    }
}