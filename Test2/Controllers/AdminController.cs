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
using System.Web.Security;
using System.Security.Cryptography;
using System.Text;

namespace Test2.Controllers
{
    [HandleError]
    public class AdminController : Controller
    {
        // GET: Admin
        dbCoffeeDataContext data = new dbCoffeeDataContext();



        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

           
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }


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

            string pass_encryp = MD5Hash(matkhau);

            Admin ad = data.Admins.SingleOrDefault(n => n.UserName == tendn && n.PassWord == pass_encryp);
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

        public ActionResult DangXuat()
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("DangNhapAdmin", "Admin");
        }



        [HttpGet]
        public ActionResult ThemSP()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                ViewBag.MaLoaiSP = new SelectList(data.LoaiSPs.ToList().OrderBy(n => n.TenLoai), "MaLoaiSP", "TenLoai");
                ViewBag.MaNCC = new SelectList(data.NhaCungCaps.ToList().OrderBy(n => n.TenNCC), "MaNCC", "TenNCC");
                ViewBag.saleID = new SelectList(data.SALEs.ToList().OrderBy(n => n.tenSK), "saleID", "TenSK");
                return View();
            }
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ThemSP(SanPham sanpham, HttpPostedFileBase[] ProductUpload, SALE sales)
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

            // làm sao để truyền đc giá trị đã sale cho sản phẩm vào database 
            if (sanpham.GiaTien <= 0)
                sanpham.GiaTien = sanpham.GiaTien * sales.mucSale;
            //

            sanpham.Status = true;
            data.SanPhams.InsertOnSubmit(sanpham);
            data.SubmitChanges();
            return RedirectToAction("Cafe");
        }


        public ActionResult ChitietSP(string id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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
        }


        [HttpGet]
        public ActionResult XoaSP(string id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult SuaSP(SanPham sanpham, HttpPostedFileBase[] ProductUpload, string id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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
        }



        public ActionResult ListLoaiSP(int? page)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                return View(data.LoaiSPs.ToList().OrderBy(n => n.MaLoaiSP).ToPagedList(pageNumber, pageSize));
            }
        }


        [HttpGet]
        public ActionResult ThemLoaiSp()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                return View();
            }
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





        //Sale Sản phẩm 
        public ActionResult ListSale(int? page)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                return View(data.SALEs.ToList().OrderBy(n => n.saleID).ToPagedList(pageNumber, pageSize));
            }
        }



        //[HttpGet]
        //public ActionResult ThemSale()
        //{
        //    return View();
        //}
        //[HttpPost]
        //public ActionResult ThemSale(SALE sale , FormCollection collection)
        //{
        //    string sMaSale = collection["txtIDSALE"];
        //    string sTenSale = collection["txtTenSale"];
        //    string sMoTa = collection["txtMoTa"];

        //    sale.saleID = sMaSale;
        //    sale.tenSK = sTenSale;
        //    sale.keySale = sMoTa;

        //    data.SALEs.InsertOnSubmit(sale);
        //    data.SubmitChanges();
        //    return RedirectToAction("ListSale");

        //}



        public ActionResult ListNCC(int? page)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                return View(data.NhaCungCaps.ToList().OrderBy(n => n.MaNCC).ToPagedList(pageNumber, pageSize));
            }
        }

        [HttpGet]
        public ActionResult ThemNCC()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                return View();
            }
        }
        [HttpPost]
        public ActionResult ThemNCC(NhaCungCap NCC, FormCollection collection)
        {
            string sMaNCC = collection["txtMaNCC"];
            string sTenNCC = collection["txtTenNCC"];
            string sDiaChi = collection["txtDiaChi"];

            NCC.MaNCC = sMaNCC;
            NCC.TenNCC = sTenNCC;
            NCC.DiaChi = sDiaChi;
            NCC.Status = true;

            data.NhaCungCaps.InsertOnSubmit(NCC);
            data.SubmitChanges();
            return RedirectToAction("ListNCC");
        }


        [HttpGet]
        public ActionResult XoaNCC(string id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                NhaCungCap NCC = data.NhaCungCaps.SingleOrDefault(n => n.MaNCC == id);
                ViewBag.MaNCC = NCC.MaNCC;
                if (NCC == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(NCC);
            }
        }
        [HttpPost, ActionName("XoaNCC")]
        public ActionResult XacNhanXoaNCC(string id)
        {
            NhaCungCap NCC = data.NhaCungCaps.SingleOrDefault(n => n.MaNCC == id);
            ViewBag.MaNCC = NCC.MaNCC;
            if (NCC == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            data.NhaCungCaps.DeleteOnSubmit(NCC);
            data.SubmitChanges();
            return RedirectToAction("ListNCC");
        }



        [HttpGet]
        public ActionResult XoaLoaiSP(string id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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
        }


        //Thêm blog
        [HttpGet]
        public ActionResult ThemBlog()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                return View();
            }
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
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                var blog = data.SuKiens.Where(n => n.MaSK == id).FirstOrDefault();
                return View(blog);
            }
        }

        [HttpPost]
        public ActionResult SuaBlog(SuKien sukien, HttpPostedFileBase[] ImageUpload, string id)
        {
            SuKien UpdateSuKien = data.SuKiens.Where(n => n.MaSK == id).FirstOrDefault();
            UpdateSuKien.TieuDe = sukien.TieuDe;
            UpdateSuKien.MoTa = sukien.MoTa;
            UpdateSuKien.MoTaThem = sukien.MoTaChiTiet;
            UpdateSuKien.MoTaChiTiet = sukien.MoTaThem;
            UpdateSuKien.Status = sukien.Status;
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
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                return View(data.SuKiens.ToList().OrderBy(n => n.MaSK).ToPagedList(pageNumber, pageSize));
            }
        }

        //Xuất sản phẩm
        public ActionResult Cafe(int? page)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin","Admin");
            }
            else
            {
                int pageNumber = (page ?? 1);
                int pageSize = 9;
                return View(data.SanPhams.ToList().OrderBy(n => n.MaSP).ToPagedList(pageNumber, pageSize));
            }
        }

        //Hóa đơn
        public ActionResult ChiTiet(int MaDH)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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
        }
        public ActionResult DonHang()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                return View(data.DonHangs.ToList());
            }
        }

        [HttpGet]
        public ActionResult fixDonHang(string id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                DonHang donhang = data.DonHangs.SingleOrDefault(n => n.MaDH == int.Parse(id));
                ViewBag.MaKH = donhang.MaDH;
                if (donhang == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(donhang);
            }
        }
        [HttpPost]
        public ActionResult fixDonHang(string id, FormCollection collection)
        {
            DonHang donhang = data.DonHangs.SingleOrDefault(n => n.MaDH == int.Parse(id));
            var MaKH = collection["MaKH"];
            var ngaylap = collection["NgayLap"];
            var ngaygiao = collection["NgayGiao"];
            var ThanhTien = collection["ThanhTien"];
            var DiaChi = collection["DiaChi"];
            var ghichu = collection["GhiChu"];
            var status = collection["Status"];
            var status2 = collection["Status2"];
            var giaohang = collection["giaohang"];
            donhang.MaDH = int.Parse(id);
            if (string.IsNullOrEmpty(MaKH))
            {
                ViewData["Error"] = "Don't empty!";
            }
            else
            {

                donhang.MaKH = int.Parse(MaKH);
                donhang.NgayLap = Convert.ToDateTime(ngaylap);
                donhang.NgayGiao = Convert.ToDateTime(ngaygiao);
                donhang.GhiChu = ghichu;
                donhang.DiaChi = DiaChi;
                donhang.Status = Convert.ToBoolean(status);
                donhang.Status2 = Convert.ToBoolean(status2);
                donhang.giaohang = giaohang;
                UpdateModel(donhang);
                data.SubmitChanges();
                return RedirectToAction("DonHang");
            }
            return View(donhang);
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
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                return View(data.KhachHangs.ToList());
            }
        }

        [HttpGet]
        public ActionResult FixKhachHang(string id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                KhachHang khachHang = data.KhachHangs.SingleOrDefault(n => n.MaKH == int.Parse(id));
                ViewBag.MaKH = khachHang.MaKH;
                if (khachHang == null)
                {
                    Response.StatusCode = 404;
                    return null;
                }
                return View(khachHang);
            }
        }
        [HttpPost]
        public ActionResult FixKhachHang(string id, FormCollection collection)
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
            var status = collection["Status"];
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
                khachHang.Status = Convert.ToBoolean(status);
                UpdateModel(khachHang);
                data.SubmitChanges();
                return RedirectToAction("QuanLyKH");
            }
            return this.FixKhachHang(id);

        }

        [HttpGet]
        public ActionResult XoaKh(int id)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
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



        public ActionResult DoanhThu(int? page)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {

                var TongDonHangNgay = data.DonHangs.ToList();
                int pageSize = 7;
                int pageNum = page ?? 1;
                return View(TongDonHangNgay.ToPagedList(pageNum, pageSize));
            }
        }







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

        public double slSanPham()
        {
            double slSanPham = data.SanPhams.Count();
            return slSanPham;
        }


        public ActionResult BangDieuKhien()
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                ViewBag.Online = HttpContext.Application["Online"].ToString();
                ViewBag.SoNguoiTruyCap = HttpContext.Application["SoNguoiTruyCap"].ToString();// lấy số lượng người truy cập
                ViewBag.TongDonHang = TongDonHang();
                ViewBag.SLKhachHang = SoluongKhachHang();
                ViewBag.TongDoanhThu = ThongKeTongDoanhThu();
                ViewBag.slSanPham = slSanPham();

                return View();
            }
        }


        [HttpGet]
        public ActionResult TimHoaDon(int? page, string sTuKhoa)
        {
            if (Session["UserName"] == null)
            {
                return RedirectToAction("DangNhapAdmin", "Admin");
            }
            else
            {
                ViewBag.TuKhoa = sTuKhoa;
                List<DonHang> listKQTK = data.DonHangs.Where(n => n.KhachHang.HoVaTen.Contains(sTuKhoa)).ToList();
                if (listKQTK.Count == 0)
                {
                    ViewBag.Thongbao = "KHÔNG TÌM THẤY HÓA ĐƠN BẠN CẦN Tham khảo";
                    return View(data.DonHangs.OrderBy(n => n.KhachHang.HoVaTen));
                }
                ViewBag.Thongbao = "Đã Tìm thấy  " + listKQTK.Count + " Kết Quả";
                return View(listKQTK.OrderBy(n => n.KhachHang.HoVaTen));
            }
        }

        [HttpPost]
        public ActionResult TimHoaDon(int? page, FormCollection f)
        {
            string sTuKhoa = f["txtTimKiem"].ToString();
            ViewBag.TuKhoa = sTuKhoa;
            List<DonHang> listKQTK = data.DonHangs.Where(n => n.KhachHang.HoVaTen.Contains(sTuKhoa)).ToList();
            int pageSize = 6;
            int pageNum = (page ?? 1);
            if (listKQTK.Count == 0)
            {
                ViewBag.Thongbao = "KHÔNG TÌM THẤY HÓA ĐƠN BẠN CẦN";
                return View(data.DonHangs.OrderBy(n => n.KhachHang.HoVaTen));
            }
            ViewBag.Thongbao = "Đã Tìm thấy  " + listKQTK.Count + " Kết Quả";
            return View(listKQTK.OrderBy(n => n.KhachHang.HoVaTen));
        }
       
        
        
        ///////////////// xử lý đơn hàng ở đây /////////////////////
        public ActionResult DonHangDTT()
        {
            List<DonHang> donhang = data.DonHangs.Where(n => n.Status==true).ToList();
           
            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }
        public ActionResult DonHangCTT()
        {
            List<DonHang> donhang = data.DonHangs.Where(n => n.Status == false).ToList();

            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }
        public ActionResult DonHangBiHuy()
        {
            List<DonHang> donhang = data.DonHangs.Where(n => n.Status2 == false).ToList();

            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }
        public ActionResult DonHangTonTai()
        {
            List<DonHang> donhang = data.DonHangs.Where(n => n.Status2 == true).ToList();

            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }



        public ActionResult DonHangDaGiao()
        {
            List<DonHang> donhang = data.DonHangs.Where(n => n.giaohang== "DGH").ToList();

            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }
        public ActionResult DonHangDangGiao()
        {
            List<DonHang> donhang = data.DonHangs.Where(n => n.giaohang == "DVC").ToList();

            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }
        public ActionResult DonHangChuaGiao()
        {
            List<DonHang> donhang = data.DonHangs.Where(n => n.giaohang== "CGH").ToList();

            if (donhang == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            return View(donhang);
        }


        public ActionResult Analyst()
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


    }
}