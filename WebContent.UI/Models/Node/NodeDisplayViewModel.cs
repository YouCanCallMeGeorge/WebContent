using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using WebContent.Manage.ContentClasses;
using WebContent.Manage.HelperClasses;

namespace WebContent.UI.Models.Node
{
    public class NodeDisplayViewModel
    {
        public bool BlogTodayExists { get; set; }
        public List<ContentLinkInfo> PathLinks { get; set; }
        public List<ContentLinkInfo> ChildLinks { get; set; }
        public string Content { get; set; }
        public int Id { get; set; }
        public string Title { get; set; }
    }
}