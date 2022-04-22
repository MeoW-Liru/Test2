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

        public ActionResult PhanLoai(string id, int ? page)
        {
            int pageSize = 8;
            int pageNum = (page ?? 1);

            var sanpham = from sp in data.SanPhams where sp.MaLoaiSP == id select sp;
           return View(sanpham.ToPagedList(pageNum,pageSize));
        }

        public ActionResult Index()
        {
            var caphe = layCaPhe(6);
            return View(caphe);

        }

        public ActionResult CafeHot()
        {
            var cafehot = layCaPhe(3);
            return View(cafehot);
        }

        public ActionResult SanPham(int ? page)
        {
            int pageSize =6;
            int pageNum = (page ?? 1);
            var caphe = layCaPhe(8);
            return View(caphe.ToPagedList(pageNum,pageSize));
        }

        public ActionResult Details(string id)
        {
            var sanpham = from sp in data.SanPhams
                          where sp.MaSP == id
                          select sp;
            return View(sanpham.Single());
        }


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
    }
}