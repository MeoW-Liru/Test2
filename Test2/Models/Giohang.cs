using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Test2.Models;

namespace Test2.Models
{
    public class Giohang
    {
        dbCoffeeDataContext data = new dbCoffeeDataContext();
        public string sMaSP { set; get; }
        public string sTenSP { set; get; }
        public string sHinhAnh { set; get; }

        public decimal dGiatien { set; get; }

        public int iSoluong { set; get; }

        public decimal dThanhtien
        {
            get { return iSoluong * dGiatien; }
        }

        public Giohang (string MaSP)
        {
            sMaSP = MaSP;
            SanPham sp = data.SanPhams.Single(n => n.MaSP == sMaSP);
            sTenSP = sp.TenSP;
            sHinhAnh = sp.HinhAnh;
            dGiatien = decimal.Parse(sp.GiaTien.ToString());
            iSoluong = 1;

        }
    }
}