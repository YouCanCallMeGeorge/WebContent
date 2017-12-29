using System;
using System.Collections.Generic;
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
        const string DB_CONNECTION_STRING = "server=(localdb)\\MSSQLLocalDB;Initial Catalog=WebContent;Persist Security Info=False;Integrated Security=True";

        const string COL_NAMES = "Id, Path, Summary, Title, Content, DateCreated, ParentId";
        const int COL_ID = 0;
        const int COL_PATH = 1;
        const int COL_SUMMARY = 2;
        const int COL_TITLE = 3;
        const int COL_CONTENT = 4;
        const int COL_DATE_CREATED = 5;
        const int COL_PARENT_ID = 6;


        //--------------------------------------
        //--------------------------------------
        public List<ContentTransfer> NodeChildrenGet(int id)
        {
            List<ContentTransfer> children = new List<ContentTransfer>();
            ContentTransfer node = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = DB_CONNECTION_STRING;
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
                            node = NodeRecordUnpack(reader);
                            children.Add(node);
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
                conn.ConnectionString = DB_CONNECTION_STRING;
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
                    cmd.Parameters.Add("@pDateCreated", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    cmd.Parameters.Add("@pParentId", SqlDbType.Int).Value = nodeNew.ParentId;
                    cmd.ExecuteNonQuery();
                }
                conn.Close();

                return NodeGetByPath(nodeNew.Path);
            }
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
                conn.ConnectionString = DB_CONNECTION_STRING;
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
            ContentTransfer node = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = DB_CONNECTION_STRING;
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
                            node = NodeRecordUnpack(reader);
                        }
                    }
                }
                conn.Close();
            }

            return node;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeGetByPath(string path)
        {
            ContentTransfer node = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = DB_CONNECTION_STRING;
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
                            node = NodeRecordUnpack(reader);
                        }
                    }
                }
                conn.Close();
            }

            return node;
        }



        //--------------------------------------
        //--------------------------------------
        private ContentTransfer NodeRecordUnpack(SqlDataReader reader)
        {
            ContentTransfer node = new ContentTransfer();

            node.NodeId = reader.GetInt32(COL_ID);
            node.Path = reader.GetString(COL_PATH);
            node.Summary = reader.GetString(COL_SUMMARY);
            node.Title = reader.GetString(COL_TITLE);
            node.Content = reader.GetString(COL_CONTENT);
            node.DateCreated = reader.GetDateTime(COL_DATE_CREATED);
            node.ParentId = reader.GetInt32(COL_PARENT_ID);
            return node;
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeTypeMostRecentLeafGet(string nodeType)
        {
            ContentTransfer node = null;

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = DB_CONNECTION_STRING;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
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
