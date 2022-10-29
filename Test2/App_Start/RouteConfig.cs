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
