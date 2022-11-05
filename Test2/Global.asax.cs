using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Test2.App_Start;

namespace Test2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            Application["SoNguoiTruyCap"] = 0;
            Application["Online"] = 0;
           
        }

        protected void Session_Start()
        {
            Application.Lock();
            Application["SoNguoiTruyCap"] = (int)Application["SoNguoiTruyCap"] + 1;
            Application["Online"] = (int)Application["Online"] + 1;
            Application.UnLock();
        }

        protected void Session_End()
        {
            Application.Lock();
            Application["Online"] = (int)Application["Online"]-1;
            Application.UnLock();
        }

    }
}
