using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace WebContent.UI.Models.Node
{
    public class NodeEditViewModel
    {
        [AllowHtml]
        public string Content { get; set; }
        public bool Create { get; set; }
        public int Id { get; set; }
        public string Path { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }
}