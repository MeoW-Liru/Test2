using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test2.Models;
using PagedList;
using PagedList.Mvc;


namespace Test2.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        dbCoffeeDataContext data = new dbCoffeeDataContext();


        private List<SuKien> LaySuKien(int count)
        {
            return data.SuKiens.OrderByDescending(b => b.NgayDang).Take(count).ToList();
        }

        private List<SanPham> layCaPhe(int count)
        {
            //Sắp xếp theo ngày đăng
            return data.SanPhams.OrderByDescending(a => a.NgayDang).Take(count).ToList();
        }



        public ActionResult LoaiCaPhe()
        {
            var loai = from LoaiSP in data.LoaiSPs select LoaiSP;
            return PartialView(loai);
        }

        public ActionResult PhanLoai(string id, int? page)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);

            var sanpham = from sp in data.SanPhams where sp.MaLoaiSP == id select sp;
            return View(sanpham.ToPagedList(pageNum, pageSize));
        }


        public ActionResult Index(int? page)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);
            var caphe = layCaPhe(8);
            return View(caphe.ToPagedList(pageNum, pageSize));

        }

        public ActionResult CafeHot(int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var sukien = layCaPhe(8);
            return View(sukien.ToPagedList(pageNum, pageSize));
        }

        public ActionResult SanPham(int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var caphe = layCaPhe(8);
            return View(caphe.ToPagedList(pageNum, pageSize));
        }
       
        public ActionResult Details(string id)
        {
            Session["masp"] = id;
            var sanpham = from sp in data.SanPhams
                          where sp.MaSP == id
                          select sp;

            return View(sanpham.Single());
        }
        // bình luận ở đây///////////////////////////////////////////////////////////////////////////////////////
        [HttpGet]
        public ActionResult BinhLuan(string id)
        {
            var binhluan = from bl in data.BinhLuans
                           where bl.MaSP == id
                           select bl;
            return View(binhluan.ToList());
        }

        [HttpPost]
        public ActionResult BinhLuan(FormCollection collection, BinhLuan bl, String id)
        {
            var TenNguoiBL = collection["ten"];
            var ngaybl = DateTime.Now;
            var noidung = collection["noidung"];
            var masp = collection["masp"];

            bl.MaSP = masp;
            bl.NgayLap = ngaybl;
            bl.TenNguoiBL = TenNguoiBL;
            bl.NoiDung = noidung;
            data.BinhLuans.InsertOnSubmit(bl);
            data.SubmitChanges();
            return this.BinhLuan(id);

        }
        // end bình luận ./////////////////////////////////////

        [HttpPost]
        // GET: SearchSanPham
        public ActionResult KetQuaTimKiem(int? page, FormCollection f)
        {
            string sTuKhoa = f["txtTimKiem"].ToString();
            ViewBag.TuKhoa = sTuKhoa;
            List<SanPham> listKQTK = data.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa)).ToList();
            //Phân Trang
            int pageSize = 6;
            int pageNum = (page ?? 1);
            if (listKQTK.Count == 0)
            {
                ViewBag.Thongbao = "KHÔNG TÌM THẤY SẢN PHẨM (sản phẩm thay thế)";
                return View(data.SanPhams.OrderBy(n => n.TenSP).ToPagedList(pageNum, pageSize));
            }
            ViewBag.Thongbao = "Đã Tìm THấy Sản Phẩm" + listKQTK.Count + "Kết Quả";
            return View(listKQTK.OrderBy(n => n.TenSP).ToPagedList(pageNum, pageSize));
        }

        [HttpGet]
        // GET: SearchSanPham
        public ActionResult KetQuaTimKiem(int? page, string sTuKhoa)
        {
            ViewBag.TuKhoa = sTuKhoa;
            List<SanPham> listKQTK = data.SanPhams.Where(n => n.TenSP.Contains(sTuKhoa)).ToList();
            //Phân Trang
            int pageSize = 6;
            int pageNum = (page ?? 1);
            if (listKQTK.Count == 0)
            {
                ViewBag.Thongbao = "KHÔNG TÌM THẤY SẢN PHẨM BẠN CẦN";
                return View(data.SanPhams.OrderBy(n => n.TenSP).ToPagedList(pageNum, pageSize));
            }
            ViewBag.Thongbao = "Đã Tìm THấy Sản Phẩm " + listKQTK.Count + " Kết Quả";
            return View(listKQTK.OrderBy(n => n.TenSP).ToPagedList(pageNum, pageSize));
        }

        public ActionResult LienHe()
        {
            return View();
        }

        public ActionResult Blog(int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var sukien = LaySuKien(8);
            return View(sukien.ToPagedList(pageNum, pageSize));
        }


        public ActionResult ChitietBlog(string id)
        {
            var sukien = from sk in data.SuKiens
                         where sk.MaSK == id
                         select sk;
            return View(sukien.Single());
        }

        public ActionResult BlogKT(int? page)
        {
            int pageSize = 3;
            int pageNum = (page ?? 1);
            var sukien = LaySuKien(8);
            return View(sukien.ToPagedList(pageNum, pageSize));
        }

        public ActionResult Tra(int? page)
        {
            int pageSize = 6;
            int pageNum = (page ?? 1);
            var tra = from sp in data.SanPhams where sp.MaLoaiSP == "SP7" select sp;
            return View(tra.ToPagedList(pageNum, pageSize));
        }
    }
}