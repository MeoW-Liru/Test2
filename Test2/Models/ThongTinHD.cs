using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace Test2.Models
{
    public class ThongTinHD
    {
        public KhachHang khachhang { get; set; }
        public ChiTietDonHang ctdh { get; set; }
        public DonHang donHang { get; set; }
        public LoaiSP loaiSP { get; set; }
        public SanPham SP { get; set; }
        [DisplayFormat (DataFormatString ="{0:0.##0}")]
        public double TongTien { get; set; }

    }
}