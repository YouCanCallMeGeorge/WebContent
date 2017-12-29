using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebContent.Manage.HelperClasses;

namespace WebContent.Manage.ContentClasses
{
    public class ContentNode
    {
        //---------------------------------------------------------
        // A ContentNode is content at an addressable location in a tree structure.
        // The address is a path, which has the familiar form of a file system path.
        //   Example: /Blog/2017/12/20.
        //
        // The content consists of three strings:
        //    Title.
        //    Summary.
        //      The summary is shown in listings of content, with a link to view the full article.
        //    Content.
        //      The content is assumed to include HTML markup.
        //
        // The content is of a certain type, indicated by the first segment in the path.
        //    Example: The content at /Blog/2017/12/20 is of type Blog.
        //
        // This class, ContentNode, is the base content class.
        //
        // For each type, there exists a subclass of ContentNode to manage the type.
        //    Example: The type manager class for Blog type is BlogEntry.
        //
        //  A ContentNode is always created (added to the repository) through a type manager class.
        //    This rule ensures that the type manager has full control over the type. 
        // 
        //
        //  Notes on security:
        //   1. Node paths are vetted in the set method of the Path property.
        //      Permissible characters are a-z, A-Z, '/', digits.
        //      A path containing other characters is rejected with an exception.
        //
        //   2. It is assumed that write access to the repository is restricted to trusted users.
        //-------------------------------------------------
        public const string pathDividerStr = "/";
        public const char pathDividerChar = '/';
        public const int contentLengthMax = 5000;
        public const int pathLengthMax = 500;
        public const int summaryLengthMax = 250;
        public const int titleLengthMax = 150;

        // Private fields.
        // Type manager classes access these fields using the public properties.
        private DateTime dateCreated;
        private int nodeId;
        private int parentId;
        private string path;
        private string type;

        // Automatic Properties.
        public string Content { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }

        // Public Properties.
        // Read access to all protected fields.
        public DateTime DateCreated { get { return dateCreated; } }
        public int NodeId { get { return nodeId; } }
        public int ParentId { get { return parentId; } }
        public string Path { get { return path; } }
        public string Type { get { return type; } }


        // Internal constructor, called by constructors in type manager classes.
        internal ContentNode(string typeArg, ContentTransfer ct)
        {
            type = typeArg;
            if (ct == null)
                throw new Exception("Cannot create ContentNode from null ContentTransfer");

            Content = ct.Content;
            dateCreated = ct.DateCreated;
            nodeId = ct.NodeId;
            parentId = ct.ParentId;
            path = ct.Path;
            Summary = ct.Summary;
            Title = ct.Title;
        }


        //----------------------------
        // PathVerify:
        //   Test for type match and valid characters.
        //   Invalid path throws an exception.
        //----------------------------
        protected void PathVerify()
        {
            if (string.IsNullOrEmpty(path))
                throw new Exception(String.Format("Invalid node path: {0}", path));

            if (path.Length > pathLengthMax)
                throw new Exception(String.Format("Path length exceeds {0} characters: {1}...", pathLengthMax, path.Substring(0, pathLengthMax)));

            // Verify path characters: a-z, A-Z, digit, path separator.
            if (!(path.ToCharArray().All(c => Char.IsLetter(c) || Char.IsNumber(c) || c == pathDividerChar)))
                throw new Exception(String.Format("Invalid characters in path: {0}", path));

            // Verify type match.
            if (!path.StartsWith(pathDividerStr + type))
                throw new Exception(String.Format("Type {0} does not match path: {1}", type, path));
        }



        //----------------------------
        // Populate ContentTransfer object.
        //----------------------------
        internal ContentTransfer ContentTransferGet()
        {
            return new ContentTransfer
            {
                Content = Content,
                DateCreated = DateCreated,
                NodeId = NodeId,
                ParentId = ParentId,
                Path = Path,
                Summary = Summary,
                Title = Title
            };
        }
    }
}
