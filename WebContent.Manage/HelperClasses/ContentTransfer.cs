using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebContent.Manage.HelperClasses
{
    //----------------------------
    // ContentTransfer:
    //   Internal class for interface with repository.
    //   All fields in ContentNode class are in this class, publicly accessible.
    //     Exception: The Type is not included in this class because it is not stored separately in the repository.
    //
    //   On retrieval from repository:
    //     An instance of this class is populated by the repository and passed into constructor of ContentNode.
    //
    //   On update to repository:
    //     An instance of this class is produced by the ContentNode object and passed to the repository.
    //----------------------------
    public class ContentTransfer
    {
        public string Content { get; set; }
        public DateTime DateCreated { get; set; }
        public int NodeId { get; set; }
        public int ParentId { get; set; }
        public string Path { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
    }
}
