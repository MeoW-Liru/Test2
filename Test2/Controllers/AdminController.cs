using System;
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
using System.Data.Entity;

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
        [HttpGet]
        public ActionResult SuaSP(string id)
        {
            //var sanpham = data.SanPhams.First(m => m.MaSP == id);
            SanPham sanpham = data.SanPhams.FirstOrDefault(m => m.MaSP == id);
            ViewBag.MaSP = sanpham.MaSP;
            if (sanpham == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            ViewBag.MaLoaiSP = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sanpham.MaLoaiSP);
            ViewBag.MaNCC = new SelectList(data.NhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sanpham.MaNCC);
            return View(sanpham);
        }

        //Cách 1:
        //[HttpPost]
        //[ValidateInput(false)]
        //public ActionResult SuaSP(SanPham sp, HttpPostedFileBase fileupload,FormCollection collection)
        //{


        //    ViewBag.MaLoaiSP = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai", sp.MaLoaiSP);
        //    ViewBag.MaNCC = new SelectList(data.NhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC", sp.MaNCC);
        //    if (fileupload == null)
        //    {
        //        ViewBag.Thongbao = "Chọn hình ảnh vào";
        //        return View();
        //    }
        //    else
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var fileName = Path.GetFileName(fileupload.FileName);
        //            var path = Path.Combine(Server.MapPath("~/images/CafeProduct"), fileName);
        //            if (System.IO.File.Exists(path))
        //                ViewBag.Thongbao = "Hình ảnh đã tồn tại";
        //            else
        //            {
        //                fileupload.SaveAs(path);
        //            }
        //            sp.HinhAnh = fileName;
        //            UpdateModel(sp);
        //            data.SubmitChanges();
        //        }
        //        return RedirectToAction("Cafe");
        //    }
        //}

        ////Cách 2
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


        //Chi Tiết Blog
        [HttpGet]
        public ActionResult ChiTietBlog(string id)
        {
            SuKien sk = data.SuKiens.SingleOrDefault(n => n.MaSK == id);
            ViewBag.MaSK = sk.MaSK;
            if (sk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sk);
        }


        //Thêm blog
        [HttpGet]
        public ActionResult ThemBlog()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemBlog(SuKien sukien, HttpPostedFileBase[] ImageUpload)
        {
            for (int i = 0; i < ImageUpload.Length; i++)
            {
                if (ImageUpload[i] != null && ImageUpload[i].ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageUpload[i].FileName);
                    var path = Path.Combine(Server.MapPath("~/images/BlogPicture"), fileName);
                    if(!System.IO.File.Exists(path))
                    {
                        ImageUpload[i].SaveAs(path);
                    }
                }
            }
            if (ImageUpload[0] != null)
                sukien.HinhAnh = ImageUpload[0].FileName;
            if (ImageUpload[1] != null)
                sukien.HinhAnhChiTiet = ImageUpload[1].FileName;
            if (ImageUpload[2] != null)
                sukien.HinhAnhChiTietThem = ImageUpload[2].FileName;
            if (ImageUpload[3] != null)
                sukien.HinhAnhTongQuat = ImageUpload[3].FileName;
            sukien.Status = true;
            data.SuKiens.InsertOnSubmit(sukien);
            data.SubmitChanges();
            return RedirectToAction("BlogList");
        }
        //Thêm Blog


        //Sửa Blog
        [HttpPost]
        public ActionResult SuaBlog(SuKien sukien, HttpPostedFileBase[] ImageUpload,string id)
        {
            SuKien UpdateSuKien = data.SuKiens.SingleOrDefault(n => n.MaSK == id);
            UpdateSuKien.TieuDe = sukien.TieuDe;
            UpdateSuKien.MoTa = sukien.MoTa;
            UpdateSuKien.MoTaThem = sukien.MoTaChiTiet;
            UpdateSuKien.MoTaChiTiet = sukien.MoTaThem;

            for (int i = 0; i < ImageUpload.Length; i++)
            {
                if (ImageUpload[i] != null && ImageUpload[i].ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ImageUpload[i].FileName);
                    var path = Path.Combine(Server.MapPath("~/images/BlogPicture"), fileName);
                    if (!System.IO.File.Exists(path))
                    {
                        ImageUpload[i].SaveAs(path);
                    }
                }
            }
            if (ImageUpload[0] != null)
                sukien.HinhAnh = ImageUpload[0].FileName;
            if (ImageUpload[1] != null)
                sukien.HinhAnhChiTiet = ImageUpload[1].FileName;
            if (ImageUpload[2] != null)
                sukien.HinhAnhChiTietThem = ImageUpload[2].FileName;
            if (ImageUpload[3] != null)
                sukien.HinhAnhTongQuat = ImageUpload[3].FileName;
            sukien.Status = true;
            UpdateModel(sukien);
            data.SubmitChanges();
            return RedirectToAction("BlogList");
        }
        [HttpGet]
        public ActionResult SuaBlog()
        {
            return View();
        }

        //Sửa Blog


        //Xóa Blog
        [HttpGet]
        public ActionResult XoaBlog(string id)
        {
            SuKien sk = data.SuKiens.SingleOrDefault(n => n.MaSK == id);
            ViewBag.MaSK = sk.MaSK;
            if (sk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(sk);
        }
        [HttpPost, ActionName("XoaBlog")]
        public ActionResult XacNhanXoaSuKien(string id)
        {
            SuKien sk = data.SuKiens.SingleOrDefault(n => n.MaSK == id);
            ViewBag.MaSK = sk.MaSK;
            if (sk == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.SuKiens.DeleteOnSubmit(sk);
            data.SubmitChanges();
            return RedirectToAction("BlogList");
        }
        //Xóa Blog

        //Xuất Blog
        public ActionResult BlogList (int ? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            return View(data.SuKiens.ToList().OrderBy(n => n.MaSK).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Cafe(int ? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            return View(data.SanPhams.ToList().OrderBy(n=>n.MaSP).ToPagedList(pageNumber,pageSize));
        }

        public ActionResult ChiTiet(int MaDH)
        {
            using (dbCoffeeDataContext db = new dbCoffeeDataContext())
            {
                List<KhachHang> khachhang = db.KhachHangs.ToList();
                List<DonHang> donhang = db.DonHangs.ToList();
                List<SanPham> sanpham = db.SanPhams.ToList();
                List<ChiTietDonHang> ctdh = db.ChiTietDonHangs.ToList();
                var main = from h in donhang
                           join k in khachhang on h.MaKH equals k.MaKH
                           where (h.MaDH == MaDH)
                           select new ThongTinHD
                           {
                               donHang = h,
                               khachhang = k
                           };
                var sub = from c in ctdh
                          join s in sanpham on c.MaSP equals s.MaSP
                          where (c.MaDH == MaDH)
                          select new ThongTinHD
                          {
                              ctdh = c,
                              SP = s,
                              TongTien = Convert.ToDouble(c.ThanhTien * c.SoLuongSP)
                          };
                ViewBag.Main = main;
                ViewBag.Sub = sub;
                return View();
            }

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