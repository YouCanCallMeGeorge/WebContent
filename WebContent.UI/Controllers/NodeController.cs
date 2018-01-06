using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebContent.Manage.ContentClasses;
using WebContent.Manage.Interfaces;
using WebContent.UI.Models.Node;

namespace WebContent.UI.Controllers
{
    public class NodeController : Controller
    {
        private IContentManager contentManager;

        //-------------------------
        //-------------------------
        public NodeController(IContentManager contentManagerArg)
        {
            contentManager = contentManagerArg;
        }



        //-------------------------
        // Notes:
        //  A Node is content at an addressable location in a tree structure.
        //  See WebContent.Manage for more information.
        //
        //  All HTML content is placed in the bootstrap grid.
        //  See _Layout.cshtml.
        //
        //  The column size specification is col-md, putting the stacking change threshold at 992 pixels.
        //  See the "Media Queries" heading at getbootstrap.com/docs/3.3/css/.
        //
        //  Node content occupies 8 of the 12 columns in the grid: col-md-8.
        //-------------------------

        //-------------------------
        // Index:
        // Show the most recent blog entry.
        //-------------------------
        public ViewResult Index()
        {
            return BlogMostRecent();
        }



        //-------------------------
        // BlogEntryCreate:
        // Create a new blog entry.
        //-------------------------
        public ViewResult BlogEntryCreate()
        {
            NodeEditViewModel model = new NodeEditViewModel
            {
                Content = "[content]",
                Create = true,
                Id = 0,
                Path = BlogEntry.PathMake(),
                Summary = "[summary]",
                Title = "Blog Entry " + DateTime.Now.ToString("yyyy/MM/dd"),
                Type = "Blog"
            };

            ViewBag.Title = "New Entry: " + model.Path;
            return View("NodeEdit", model);
        }



        //-------------------------
        // BlogMostRecent:
        // Show the most recent blog entry.
        //-------------------------
        public ViewResult BlogMostRecent()
        {
            ContentNode node = contentManager.BlogEntryMostRecentGet();
            ViewBag.Title = "Blog";
            NodeDisplayViewModel model = NodeDisplayPrep(node, "Recent Blog Entry");
            return View("NodeDisplay", model);
        }



        //-------------------------
        //-------------------------
        public ActionResult Display(string nodePath)
        {
            if (nodePath == null)
                return RedirectToAction("Index");

            ContentNode node = contentManager.ContentGetByPath(nodePath);
            ViewBag.Title = "Blog";
            NodeDisplayViewModel model = NodeDisplayPrep(node, nodePath);
            return View("NodeDisplay", model);
        }



        //-------------------------
        //-------------------------
        private NodeDisplayViewModel NodeDisplayPrep(ContentNode node, string nodePath)
        {
            // Default property values, if node is null.
            string content = "";
            int id = 0;
            string title = "Content at \"" + nodePath + "\" not found";

            if (node != null)
            {
                content = node.Content.Replace(System.Environment.NewLine, "<br/>");
                id = node.NodeId;
                title = node.Title;
            }

            NodeDisplayViewModel model = new NodeDisplayViewModel
            {
                ChildLinks = contentManager.ContentChildLinksGet(node),
                PathLinks = contentManager.ContentPathLinksGet(node),
                Content = content,
                Id = id,
                Title = title
            };

            return model;
        }



        //-------------------------
        //-------------------------
        [HttpPost]
        public ActionResult EditBegin(string EditButton, int Id)
        {
            if (Id == 0)
                return RedirectToAction("/Display/");

            ContentNode node = contentManager.ContentGetById(Id);

            NodeEditViewModel model = new NodeEditViewModel
            {
                Content = node.Content,
                Create = false,
                Id = node.NodeId,
                Path = node.Path,
                Summary = node.Summary,
                Title = node.Title,
                Type = node.Type
            };

            ViewBag.Title = "Edit: " + model.Path;
            return View("NodeEdit", model);
        }



        //-------------------------
        //-------------------------
        [HttpPost]
        [ValidateInput(false)]
        public RedirectToRouteResult EditEnd(string button, bool Create, string Content, int Id, string Path, string Summary, string Title)
        {
            if (button == "save")
            {
                if (Create)
                {
                    contentManager.BlogEntryCreate(Title, Summary, Content);
                }
                else
                {
                    contentManager.ContentUpdate(Id, Title, Summary, Content);
                }
            }

            // Show the edited node.
            return RedirectToAction("/Display/" + Path);
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
                                    "Node",
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