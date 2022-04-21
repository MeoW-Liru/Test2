using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Test2.Models;
namespace Test2.Models
{
    public class Don
    {
        [Key]
        [DisplayName("Mã Đơn Hàng ")]
        [Required]
        public int MaDH { get; set; }
       
        [DisplayName("Mã Khách Hàng ")]
        [Required]
        public int MaKH { get; set; }

        [DisplayName("Tên Khách Hàng ")]
        [Required]
        public string HoVaTen { get; set; }

        [DisplayName("Thành Tiền")]
        public decimal ThanhTien { get; set; }
      
        public DateTime NgayGiao { get; set; }

        public DateTime NgayLap { get; set; }
       
        [DisplayName("Địa Chỉ ")]
        public string DiaChi { get; set; }
      
        [DisplayName("Ghi Chú ")]
        public string GhiChu { get; set; }
   
    }
}