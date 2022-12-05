using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Test2
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {


            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.IgnoreRoute("{*botdetech}",
                new { botdetch = @"(.*)BotDetectCaptcha\.ashx" }
                );

            //cấu hình đường dẫn trang index khách hàng 
                routes.MapRoute(
                name: "TrangChu",
                url: "Trang-Chu",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            //cấu hình đường dẫn trang sản phẩm khách hàng 
                routes.MapRoute(
                name: "SanPham",
                url: "San-Pham",
                defaults: new { controller = "Home", action = "SanPham", id = UrlParameter.Optional }
            );


            //cấu hình đường dẫn trang Liên Hệ khách hàng 
                routes.MapRoute(
                name: "LienHe",
                url: "Lien-He",
                defaults: new { controller = "Home", action = "LienHe", id = UrlParameter.Optional }
            );

            //cấu hình đường dẫn trang Blog khách hàng 
                routes.MapRoute(
                name: "Blog",
                url: "Blog-TinTuc",
                defaults: new { controller = "Home", action = "Blog", id = UrlParameter.Optional }
            );

            //cấu hình đường dẫn xem chi tiết của sản phẩm
            routes.MapRoute(
                name: "XemChiTiet",
                url: "{tensp}-{id}",
                defaults: new { controller = "Home", action = "Details", id = UrlParameter.Optional }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
