using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebContent.Manage.HelperClasses;
using WebContent.Manage.Interfaces;
using WebContent.Manage.Repository.Context;


namespace WebContent.Manage.Repository
{
    public class ContentRepositoryLinqToEF : IContentRepository
    {

        //--------------------------------------
        //--------------------------------------
        public List<ContentTransfer> NodeChildrenGet(int id)
        {
            List<ContentTransfer> children = new List<ContentTransfer>();

            using (WebContentContext db = new WebContentContext())
            {
                List<Node> childNodes = db.Nodes.Where(n => n.ParentId == id).ToList<Node>();

                foreach (Node nodeDb in childNodes)
                {
                    ContentTransfer nodeXfer = DbNodeToContentXfer(nodeDb);

                    children.Add(nodeXfer);
                }
            }

            return children;
        }



        //--------------------------------------
        //--------------------------------------
        private Node ContentXferToDbNode(ContentTransfer nodeXfer)
        {
            Node nodeDb = new Node()
            {
                Id = nodeXfer.NodeId,
                Path = nodeXfer.Path,
                Summary = nodeXfer.Summary,
                Title = nodeXfer.Title,
                Content = nodeXfer.Content,
                DateCreated = nodeXfer.DateCreated,
                ParentId = nodeXfer.ParentId
            };

            return nodeDb;
        }



        //--------------------------------------
        //--------------------------------------
        private ContentTransfer DbNodeToContentXfer(Node nodeDb)
        {
            ContentTransfer nodeXfer = new ContentTransfer()
            {
                NodeId = nodeDb.Id,
                Path = nodeDb.Path,
                Summary = nodeDb.Summary,
                Title = nodeDb.Title,
                Content = nodeDb.Content,
                DateCreated = nodeDb.DateCreated,
                ParentId = nodeDb.ParentId
            };

            return nodeXfer;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeCreate(ContentTransfer nodeNew)
        {
            Node nodeDb = ContentXferToDbNode(nodeNew);

            using (WebContentContext db = new WebContentContext())
            {
                db.Nodes.Add(nodeDb);
                db.SaveChanges();
            }

            return NodeGetByPath(nodeNew.Path);
        }



        //--------------------------------------
        //--------------------------------------
        //public void NodeDelete(int id)
        //{
        //}



        //--------------------------------------
        //--------------------------------------
        public bool NodeExistsTest(string path)
        {
            bool result = false;

            using (WebContentContext db = new WebContentContext())
            {
                int count = db.Nodes.Where(n => n.Path == path).Count();
                if (count == 1)
                    result = true;
            }

            return result;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeGetById(int id)
        {
            ContentTransfer nodeXfer = null;

            using (WebContentContext db = new WebContentContext())
            {
                Node nodeDb = null;

                try
                {
                    nodeDb = db.Nodes.Where(n => n.Id == id).Single();
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }

                if (nodeDb != null)
                    nodeXfer = DbNodeToContentXfer(nodeDb);
            }

            return nodeXfer;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeGetByPath(string path)
        {
            ContentTransfer nodeXfer = null;

            using (WebContentContext db = new WebContentContext())
            {
                Node nodeDb = null;

                try
                {
                    nodeDb = db.Nodes.Where(n => n.Path == path).Single();
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }

                if (nodeDb != null)
                    nodeXfer = DbNodeToContentXfer(nodeDb);
            }

            return nodeXfer;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeTypeMostRecentLeafGet(string nodeType)
        {
            return NodeTypeMostRecentNLeavesGet(nodeType, 1)[0];
        }



        //--------------------------------------
        //--------------------------------------
        public List<ContentTransfer> NodeTypeMostRecentNLeavesGet(string nodeType, int num)
        {
            List<ContentTransfer> nodeXferList = new List<ContentTransfer>();

            using (WebContentContext db = new WebContentContext())
            {
                List<Node> nodeList =
                    (
                    from n1 in db.Nodes
                    where n1.Path.StartsWith(nodeType)
                    join n2 in db.Nodes on n1.Id equals n2.ParentId into join1
                    from n3 in join1.DefaultIfEmpty()   // DefaultIfEmpty specifies outer join
                    where n3 == null // null n3 indicates n1.Id has no child (null n3 is a leaf)
                    orderby n1.Id descending
                    select n1
                    ).Take(num).ToList<Node>();

                foreach (Node node in nodeList)
                    nodeXferList.Add(DbNodeToContentXfer(node));
            }

            return nodeXferList;
        }



        //--------------------------------------
        //--------------------------------------
        public void NodeUpdate(ContentTransfer nodeXfer)
        {
            using (WebContentContext db = new WebContentContext())
            {
                Node nodeDb = null;

                try
                {
                    nodeDb = db.Nodes.Where(n => n.Id == nodeXfer.NodeId).Single();
                }
                catch (Exception ex)
                {
                    // Exception is thrown if the node does not exist.
                    Debug.Print(ex.Message);
                }

                if (nodeDb != null)
                {
                    nodeDb.Title = nodeXfer.Title;
                    nodeDb.Summary = nodeXfer.Summary;
                    nodeDb.Content = nodeXfer.Content;
                    db.SaveChanges();
                }
            }
        }
    }
}
