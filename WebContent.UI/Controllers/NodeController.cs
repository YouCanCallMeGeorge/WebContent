using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using WebContent.Manage.ContentClasses;
using WebContent.Manage.HelperClasses;
using WebContent.Manage.Interfaces;
using WebContent.UI.Models.Node;

namespace WebContent.UI.Controllers
{
    [RoutePrefix("Node")]
    [Route("{action}")]
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
        //  See WebContent.ContentManager for more information.
        //
        //  All HTML content is placed in the bootstrap grid.
        //  See _Layout.cshtml.
        //
        //  The column size specification is col-md, putting the stacking change threshold at 992 pixels.
        //  See the "Media Queries" heading at getbootstrap.com/docs/3.3/css/.
        //-------------------------

        //-------------------------
        // Index:
        // Show the most recent blog entry.
        //-------------------------
        [Route]
        [Route("Index")]
        public ViewResult Index()
        {
            return BlogMostRecent();
        }



        //-------------------------
        // BlogMostRecent:
        // Show the most recent blog entry.
        //-------------------------
        public ViewResult BlogMostRecent()
        {
            ContentNode node = contentManager.BlogEntryMostRecentGet();
            NodeDisplayViewModel model = NodeDisplayPrep(node, "Recent Blog Entry");
            return View("NodeDisplay", model);
        }



        //-------------------------
        // Display:
        // Show content of a node, with links to navigate to other entries.
        //-------------------------
        [Route("Display/{*nodePath?}")]
        public ActionResult Display(string nodePath)
        {
            if (nodePath == null)
                return RedirectToAction("Index");

            ContentNode node = contentManager.ContentGetByPath(nodePath);
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

            // Get the list of links to children of this node.
            List<ContentLinkInfo> childLinks = contentManager.ContentChildLinksGet(node);

            // If the node has no children, the list of links to its siblings will be displayed.
            if (childLinks != null)
            {
                if (childLinks.Count == 0)
                    childLinks = contentManager.ContentSiblingLinksGet(node);
            }


            NodeDisplayViewModel model = new NodeDisplayViewModel
            {
                BlogTodayExists = contentManager.BlogEntryTodayExistsTest(),
                ChildLinks = childLinks,
                PathLinks = contentManager.ContentPathLinksGet(node),
                Content = content,
                Id = id,
                Title = title
            };

            return model;
        }



        //-------------------------
        // Begin methods related to the form in the NodeDisplay view.
        //-------------------------


        //-------------------------
        // Process the form in the NodeDisplay view.
        //-------------------------
        [HttpPost]
        public ActionResult NodeDisplayFormProcess(string button, int Id)
        {
            if (button == "BlogEntryCreate")
            {
                return BlogEntryCreate();
            }

            if (button == "NodeEdit")
            {
                return NodeEditBegin(Id);
            }

            // Should never get here, as this method is only called if one of the buttons is pressed.
            return RedirectToAction("Index");
        }



        //-------------------------
        // Create a new blog entry.
        // Called by NodeDisplayFormProcess.
        //-------------------------
        private ActionResult BlogEntryCreate()
        {
            if (contentManager.BlogEntryTodayExistsTest())
                return RedirectToAction("Index");

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
        // Present the displayed node for editing.
        // Called by NodeDisplayFormProcess.
        //-------------------------
        private ActionResult NodeEditBegin(int id)
        {
            if (id == 0)
                return RedirectToAction("/Display");

            ContentNode node = contentManager.ContentGetById(id);

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
        // End methods related to the form in the NodeDisplay view.
        //-------------------------



        //-------------------------
        // Process the form in the NodeEdit view.
        // Save the edited node or cancel the edit.
        // Redirect to show the [possibly] edited node.
        //-------------------------
        [HttpPost]
        [ValidateInput(false)]  // Allow HTML markup in content. ContentManager will encode the markup prior to storage in the repository.
        public RedirectToRouteResult EditEnd(string button, bool Create, string Content, int Id, string Path, string Summary, string Title)
        {
            // On cancellation of a new node, go to the Index action of this controller.
            if (Create && (button == "Cancel"))
            {
                return RedirectToAction("Index");
            }


            if (button == "Save")
            {
                if (Create)
                {
                    // Create the new node.
                    contentManager.BlogEntryCreate(Title, Summary, Content);
                }
                else
                {
                    // Update the existing node.
                    contentManager.ContentUpdate(Id, Title, Summary, Content);
                }
            }


            // Show the node, whether saved or cancelled.
            string url = "/Display/" + Path;
            return RedirectToAction(url);
        }



        //-------------------------
        // Trap exceptions and report to user.
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