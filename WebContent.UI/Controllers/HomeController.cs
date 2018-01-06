using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebContent.Manage.HelperClasses;
using WebContent.Manage.Interfaces;
using WebContent.UI.Models.Home;

namespace WebContent.UI.Controllers
{
    public class HomeController : Controller
    {
        private IContentManager contentManager;


        //-------------------------
        //-------------------------
        public HomeController(IContentManager contentManagerArg)
        {
            contentManager = contentManagerArg;
        }



        //-------------------------
        //-------------------------
        public ViewResult Index()
        {
            HomeViewModel model = new HomeViewModel
            {
                RecentLinks = contentManager.ContentRecentNLinksGet("Blog", 5)
            };

            ViewBag.Title = "Home";
            return View("Index", model);
        }



        //-------------------------
        //-------------------------
        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            Exception exception = filterContext.Exception;

            // !!! Call the exception Logger.
            filterContext.ExceptionHandled = true;


            string linkUrlString = Url.Action(
                                    "Index",
                                    "Home",
                                    routeValues: null,
                                    protocol: Request.Url.Scheme);

            ViewBag.LinkUrl = linkUrlString;


            ViewResult Result = this.View(
                                            "Error",
                                            new HandleErrorInfo(exception,
                                                                    filterContext.RouteData.Values["controller"].ToString(),
                                                                    filterContext.RouteData.Values["action"].ToString()
                                                               )
                                         );



            filterContext.Result = Result;
        }
    }
}
