using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebContent.Manage.HelperClasses;
using WebContent.Manage.Interfaces;
using WebContent.Manage.Repository.Context;


namespace WebContent.Manage.Repository
{
    class ContentRepositoryEF : IContentRepository
    {
        //--------------------------------------
        //--------------------------------------
        public List<ContentTransfer> NodeChildrenGet(int id)
        {
            List<ContentTransfer> children = new List<ContentTransfer>();

            using (ContentContext db = new ContentContext())
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

            using (ContentContext db = new ContentContext())
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

            using (ContentContext db = new ContentContext())
            {
                int count = db.Nodes.Where(n => n.Path == path).Count();
                if (count > 0)
                    result = true;
            }

            return result;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeGetById(int id)
        {
            ContentTransfer nodeXfer = null;

            using (ContentContext db = new ContentContext())
            {
                Node nodeDb = db.Nodes.Where(n => n.Id == id).Single();

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

            using (ContentContext db = new ContentContext())
            {
                Node nodeDb = db.Nodes.Where(n => n.Path == path).Single();

                if (nodeDb != null)
                    nodeXfer = DbNodeToContentXfer(nodeDb);
            }

            return nodeXfer;
        }
        


        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeTypeMostRecentLeafGet(string nodeType)
        {
            ContentTransfer nodeXfer = null;


            using (ContentContext db = new ContentContext())
            {
                Node nodeDb =
                    (
                    from n1 in db.Nodes.Query()
                    where (
                    ) 
                    
                    
                    
                    db.Nodes.Where( (n => n.Path.StartsWith(nodeType))                .Single();



                int childObjectIdToMatch = childObjectToMatch.ID;
                dbContext.Parents.Where(p => p.Children.Any(c => c.ID == childObjectIdToMatch));


                from m in uow.MeterReadingReadWriteRepository.Query()
                where !uow.MeterReadingReadWriteRepository.Query().Any(child => m.Id == child.LastMeterReadingId)
                select


                                    // Find the n1.Ids that have no children.
                                    string sqln2 = "((SELECT COUNT(*) FROM Node n2 WHERE n1.Id = n2.ParentId) = 0)";

                    // Find the most recent Id of the type that has no children.
                    string sqln1 = "SELECT MAX(n1.Id) FROM Node n1 WHERE ( (n1.Path LIKE '" + nodeType + "%') AND " + sqln2 + ")";

                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqln1;

                    Object result = cmd.ExecuteScalar();
                    if (result != System.DBNull.Value)
                        node = NodeGetById(Convert.ToInt32(result));
                }
                conn.Close();
            }

            return node;
        }



        //--------------------------------------
        // NodeLeafMostRecentNLeavesGet:
        //   Returns the N most recent leaf nodes (no children) of the specified type.
        //--------------------------------------
        public List<ContentTransfer> NodeTypeMostRecentNLeavesGet(string nodeType, int num)
        {
            List<ContentTransfer> nodeList = new List<ContentTransfer>();
            ContentTransfer node = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = DB_CONNECTION_STRING;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    // Find the n1.Ids that have no children.
                    string sqln2 = "((SELECT COUNT(*) FROM Node n2 WHERE n1.Id = n2.ParentId) = 0)";

                    // Find the most recent records of the type, having no children.
                    string sqln1 = "SELECT TOP(@pNum) " + COL_NAMES + " FROM Node n1";
                    sqln1 += " WHERE ( (n1.Path LIKE '" + nodeType + "%') AND " + sqln2 + ")";
                    sqln1 += " ORDER BY n1.Id DESC";

                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqln1;
                    cmd.Parameters.Add("@pNum", SqlDbType.Int).Value = num;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            node = NodeRecordUnpack(reader);
                            nodeList.Add(node);
                        }
                    }
                }
                conn.Close();
            }

            return nodeList;
        }



        //--------------------------------------
        //--------------------------------------
        public void NodeUpdate(ContentTransfer node)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = DB_CONNECTION_STRING;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    string sql = "UPDATE Node SET Content = @pContent, Summary = @pSummary, Title = @pTitle WHERE (Id = @pId)";
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@pContent", SqlDbType.VarChar).Value = node.Content;
                    cmd.Parameters.Add("@pSummary", SqlDbType.VarChar).Value = node.Summary;
                    cmd.Parameters.Add("@pTitle", SqlDbType.VarChar).Value = node.Title;
                    cmd.Parameters.Add("@pId", SqlDbType.Int).Value = node.NodeId;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
    }
}
