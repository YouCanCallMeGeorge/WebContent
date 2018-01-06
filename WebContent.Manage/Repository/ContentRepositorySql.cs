using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebContent.Manage.HelperClasses;
using WebContent.Manage.Interfaces;

namespace WebContent.Manage.Repository
{
    public class ContentRepositorySql : IContentRepository
    {
        private string dbConnectionString;
        private const string COL_NAMES = "Id, Path, Summary, Title, Content, DateCreated, ParentId";
        private const int COL_ID = 0;
        private const int COL_PATH = 1;
        private const int COL_SUMMARY = 2;
        private const int COL_TITLE = 3;
        private const int COL_CONTENT = 4;
        private const int COL_DATE_CREATED = 5;
        private const int COL_PARENT_ID = 6;


        //--------------------------------------
        //--------------------------------------
        public ContentRepositorySql()
        {
            // Get connection string from Web.config.
            dbConnectionString = ConfigurationManager.ConnectionStrings["WebContentContext"].ConnectionString;
        }



        //--------------------------------------
        //--------------------------------------
        public List<ContentTransfer> NodeChildrenGet(int id)
        {
            List<ContentTransfer> children = new List<ContentTransfer>();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbConnectionString;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    string sql = "SELECT " + COL_NAMES + " FROM Node WHERE (ParentId = @pId)";
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@pId", SqlDbType.Int).Value = id;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            children.Add(NodeRecordUnpack(reader));
                        }
                    }
                }
                conn.Close();
            }

            return children;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeCreate(ContentTransfer nodeNew)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbConnectionString;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    string sql = "INSERT INTO Node(Path, Summary, Title, Content, DateCreated, ParentId)"
                                + " VALUES(@pPath, @pSummary, @pTitle, @pContent, @pDateCreated, @pParentId)";
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@pPath", SqlDbType.VarChar).Value = nodeNew.Path;
                    cmd.Parameters.Add("@pSummary", SqlDbType.VarChar).Value = nodeNew.Summary;
                    cmd.Parameters.Add("@pTitle", SqlDbType.VarChar).Value = nodeNew.Title;
                    cmd.Parameters.Add("@pContent", SqlDbType.VarChar).Value = nodeNew.Content;
                    cmd.Parameters.Add("@pDateCreated", SqlDbType.DateTime).Value = nodeNew.DateCreated.ToString("yyyy-MM-dd HH:mm:ss");
                    cmd.Parameters.Add("@pParentId", SqlDbType.Int).Value = nodeNew.ParentId;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
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

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbConnectionString;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    string sql = "(SELECT COUNT(*) FROM Node WHERE (Path = @pPath))";
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@pPath", SqlDbType.VarChar).Value = path;

                    int count = (int)cmd.ExecuteScalar();
                    if (count > 0)
                        result = true;
                }
                conn.Close();
            }

            return result;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeGetById(int id)
        {
            ContentTransfer nodeXfer = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbConnectionString;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    string sql = "SELECT " + COL_NAMES + " FROM Node WHERE (Id = @pId)";
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@pId", SqlDbType.Int).Value = id;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nodeXfer = NodeRecordUnpack(reader);
                        }
                    }
                }
                conn.Close();
            }

            return nodeXfer;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeGetByPath(string path)
        {
            ContentTransfer nodeXfer = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbConnectionString;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    string sql = "SELECT " + COL_NAMES + " FROM Node WHERE (Path = @pPath)";
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@pPath", SqlDbType.VarChar).Value = path;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            nodeXfer = NodeRecordUnpack(reader);
                        }
                    }
                }
                conn.Close();
            }

            return nodeXfer;
        }



        //--------------------------------------
        //--------------------------------------
        private ContentTransfer NodeRecordUnpack(SqlDataReader reader)
        {
            ContentTransfer nodeXfer = new ContentTransfer()
            {
                NodeId = reader.GetInt32(COL_ID),
                Path = reader.GetString(COL_PATH),
                Summary = reader.GetString(COL_SUMMARY),
                Title = reader.GetString(COL_TITLE),
                Content = reader.GetString(COL_CONTENT),
                DateCreated = reader.GetDateTime(COL_DATE_CREATED),
                ParentId = reader.GetInt32(COL_PARENT_ID)
            };

            return nodeXfer;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeTypeMostRecentLeafGet(string nodeType)
        {
            ContentTransfer nodeXfer = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbConnectionString;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    // Inner query: Select the children of the node in the outer query.
                    string sqln2 = "(SELECT * FROM Node n2 WHERE n1.Id = n2.ParentId)";

                    // Find the most recent record which is of the type and has no children.
                    string sqln1 = "SELECT MAX(n1.Id) FROM Node n1 WHERE ( (n1.Path LIKE '" + nodeType + "%') AND NOT EXISTS " + sqln2 + ")";

                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sqln1;

                    Object result = cmd.ExecuteScalar();
                    if (result != System.DBNull.Value)
                        nodeXfer = NodeGetById(Convert.ToInt32(result));
                }
                conn.Close();
            }

            return nodeXfer;
        }



        //--------------------------------------
        // NodeLeafMostRecentNLeavesGet:
        //   Returns the N most recent leaf nodes (no children) of the specified type.
        //--------------------------------------
        public List<ContentTransfer> NodeTypeMostRecentNLeavesGet(string nodeType, int num)
        {
            List<ContentTransfer> nodeList = new List<ContentTransfer>();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbConnectionString;
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
                            nodeList.Add(NodeRecordUnpack(reader));
                        }
                    }
                }
                conn.Close();
            }

            return nodeList;
        }



        //--------------------------------------
        //--------------------------------------
        public void NodeUpdate(ContentTransfer nodeXfer)
        {
            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = dbConnectionString;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    string sql = "UPDATE Node SET Content = @pContent, Summary = @pSummary, Title = @pTitle WHERE (Id = @pId)";
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@pContent", SqlDbType.VarChar).Value = nodeXfer.Content;
                    cmd.Parameters.Add("@pSummary", SqlDbType.VarChar).Value = nodeXfer.Summary;
                    cmd.Parameters.Add("@pTitle", SqlDbType.VarChar).Value = nodeXfer.Title;
                    cmd.Parameters.Add("@pId", SqlDbType.Int).Value = nodeXfer.NodeId;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
    }
}
