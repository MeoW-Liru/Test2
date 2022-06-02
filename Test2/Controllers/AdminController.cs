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
                Session["UserName"] = tendn;
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
        public ActionResult ThemSP(SanPham sanpham, HttpPostedFileBase[] ProductUpload)
        {
            for (int i = 0; i < ProductUpload.Length; i++)
            {
                if (ProductUpload[i] != null && ProductUpload[i].ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ProductUpload[i].FileName);
                    var path = Path.Combine(Server.MapPath("~/images/CafeProduct"), fileName);

                    if (!System.IO.File.Exists(path))
                    {
                        ProductUpload[i].SaveAs(path);
                    }
                }
            }
            if (ProductUpload[0] != null)
                sanpham.HinhAnh = ProductUpload[0].FileName;
            if (ProductUpload[1] != null)
                sanpham.HinhAnhMoTa = ProductUpload[1].FileName;
            sanpham.Status = true;
            data.SanPhams.InsertOnSubmit(sanpham);
            data.SubmitChanges();
            return RedirectToAction("Cafe");
        }


        public ActionResult ChitietSP(string id)
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
        [HttpPost, ActionName("XoaSP")]
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
            SanPham sanpham = data.SanPhams.Where(m => m.MaSP == id).FirstOrDefault();
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




        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSP(SanPham sanpham, HttpPostedFileBase[] ProductUpload, string id)
        {
            SanPham UpdateSanPham = data.SanPhams.Where(n => n.MaSP == id).FirstOrDefault();
            UpdateSanPham.TenSP = sanpham.TenSP;
            UpdateSanPham.MoTa = sanpham.MoTa;
            UpdateSanPham.GiaTien = sanpham.GiaTien;
            UpdateSanPham.NgayDang = sanpham.NgayDang;
            UpdateSanPham.TrongLuong = sanpham.TrongLuong;
            UpdateSanPham.HSD = sanpham.HSD;
            UpdateSanPham.NSX = sanpham.NSX;
            UpdateSanPham.MaLoaiSP = sanpham.MaLoaiSP;
            UpdateSanPham.MaNCC = sanpham.MaNCC;
            UpdateSanPham.Status = sanpham.Status;
            for (int i = 0; i < ProductUpload.Length; i++)
            {
                if (ProductUpload[i] != null && ProductUpload[i].ContentLength > 0)
                {
                    var fileName = Path.GetFileName(ProductUpload[i].FileName);
                    var path = Path.Combine(Server.MapPath("~/images/CafeProduct"), fileName);
                    if (!System.IO.File.Exists(path))
                    {
                        ProductUpload[i].SaveAs(path);
                    }
                }
            }
            if (ProductUpload[0] != null)
                UpdateSanPham.HinhAnh = ProductUpload[0].FileName;
            if (ProductUpload[1] != null)
                UpdateSanPham.HinhAnhMoTa = ProductUpload[1].FileName;
            data.SubmitChanges();
            return RedirectToAction("Cafe");
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


        public ActionResult ListLoaiSP(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            return View(data.LoaiSPs.ToList().OrderBy(n => n.MaLoaiSP).ToPagedList(pageNumber, pageSize));
        }


        [HttpGet]
        public ActionResult ThemLoaiSp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ThemLoaiSP(LoaiSP loaisp, FormCollection collection)
        {
            string sMaLoai = collection["txtMaLoaiSP"];
            string sTenLoai = collection["txtTenLoai"];

            loaisp.TenLoai = sTenLoai;
            loaisp.MaLoaiSP = sMaLoai;
            loaisp.Status = true;

            data.LoaiSPs.InsertOnSubmit(loaisp);
            data.SubmitChanges();
            return RedirectToAction("ListLoaiSP");
        }

        [HttpGet]
        public ActionResult XoaLoaiSP(string id)
        {
            LoaiSP loaisp = data.LoaiSPs.SingleOrDefault(n => n.MaLoaiSP == id);
            ViewBag.MaLoaiSP = loaisp.MaLoaiSP;
            if (loaisp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(loaisp);
        }
        [HttpPost, ActionName("XoaLoaiSP")]
        public ActionResult XacNhanXoaLoaiSP(string id)
        {
            LoaiSP loaisp = data.LoaiSPs.SingleOrDefault(n => n.MaLoaiSP == id);
            ViewBag.MaLoaiSP = loaisp.MaLoaiSP;
            if (loaisp == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.LoaiSPs.DeleteOnSubmit(loaisp);
            data.SubmitChanges();
            return RedirectToAction("ListLoaiSP");
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
            data.SuKiens.InsertOnSubmit(sukien);
            data.SubmitChanges();
            return RedirectToAction("BlogList");
        }
        //Thêm Blog

        //Sửa Blog
        [HttpGet]
        public ActionResult SuaBlog(string id)
        {
            var blog = data.SuKiens.Where(n => n.MaSK == id).FirstOrDefault();
            return View(blog);
        }

        [HttpPost]
        public ActionResult SuaBlog(SuKien sukien, HttpPostedFileBase[] ImageUpload, string id)
        {
            SuKien UpdateSuKien = data.SuKiens.Where(n => n.MaSK == id).FirstOrDefault();
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
                UpdateSuKien.HinhAnh = ImageUpload[0].FileName;
            if (ImageUpload[1] != null)
                UpdateSuKien.HinhAnhChiTiet = ImageUpload[1].FileName;
            if (ImageUpload[2] != null)
                UpdateSuKien.HinhAnhChiTietThem = ImageUpload[2].FileName;
            if (ImageUpload[3] != null)
                UpdateSuKien.HinhAnhTongQuat = ImageUpload[3].FileName;
            UpdateSuKien.Status = sukien.Status;
            data.SubmitChanges();
            return RedirectToAction("BlogList");
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
        public ActionResult BlogList(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            return View(data.SuKiens.ToList().OrderBy(n => n.MaSK).ToPagedList(pageNumber, pageSize));
        }

        //Xuất sản phẩm
        public ActionResult Cafe(int? page)
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            return View(data.SanPhams.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
        }

        //Hóa đơn
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


        public ActionResult listDonhang(int? page) // chỗ này đáng lẽ sẽ là code hóa đơn hàng tháng không cần phải tìm kiếm nhưng chưa mò xong nên từ từ 
        {
            int pageNumber = (page ?? 1);
            int pageSize = 9;
            return View(data.DonHangs.ToList().OrderBy(n => n.MaDH).ToPagedList(pageNumber, pageSize));
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
                dt.Rows.Add(emp.MaDH, emp.MaKH, emp.KhachHang.HoVaTen, emp.NgayLap, emp.NgayGiao, emp.DiaChi, emp.GhiChu, emp.Status);

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

        public ActionResult QuanLyKH()
        {
            return View(data.KhachHangs.ToList());
        }

        [HttpGet]
        public ActionResult XoaKh(int id)
        {
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaKH = kh.MaKH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(kh);
        }
        [HttpPost, ActionName("XoaKh")]
        public ActionResult XacNhanXoaKh(int id)
        {
            KhachHang kh = data.KhachHangs.SingleOrDefault(n => n.MaKH == id);
            ViewBag.MaSK = kh.MaKH;
            if (kh == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.KhachHangs.DeleteOnSubmit(kh);
            data.SubmitChanges();
            return RedirectToAction("QuanLyKH");
        }




        public decimal ThongKeTongDoanhThu()
        {
            decimal TongDoanhThu = data.ChiTietDonHangs.Sum(n => n.SoLuongSP * n.ThanhTien).Value;
            return TongDoanhThu;
        }


        // cái này tạo ra 1 view cho họ nhập tháng rồi năm rồi sau đó cho nó quăng ra tháng năm tương ứng như y chan tìm sản phẩm
        // tạo sau chưa làm view 
        // cũng hóa đơn tháng nhưng cho nhập để tìm 
        public decimal ThongKeDoanhThuTheoThang(int thang, int Nam)
        {
            var listHoaDon = data.DonHangs.Where(n => n.NgayLap.Value.Month == thang &&
            n.NgayLap.Value.Year == Nam);
            decimal TongTien = 0;
            foreach (var item in listHoaDon)
            {
                TongTien += decimal.Parse(item.ChiTietDonHangs.Sum(n => n.SoLuongSP * n.ThanhTien).Value.ToString());
            }
            return TongTien;
        }
        //

        public double TongDonHang()
        {
            double slDH = data.DonHangs.Count();
            return slDH;
        }

        public double SoluongKhachHang()
        {
            double slKH = data.KhachHangs.Count();
            return slKH;
        }


        public ActionResult BangDieuKhien()
        {
            ViewBag.Online = HttpContext.Application["Online"].ToString();
            ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString();// lấy số lượng người truy cập
            ViewBag.TongDonHang = TongDonHang();
            ViewBag.SLKhachHang = SoluongKhachHang();
            ViewBag.TongDoanhThu = ThongKeTongDoanhThu();
            return View();
        }

    }
}