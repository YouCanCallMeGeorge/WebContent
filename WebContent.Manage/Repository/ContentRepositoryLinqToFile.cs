using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

// These two references support the utility method ContentListCopyFromDatabase
using System.Data;              
using System.Data.SqlClient;    


using WebContent.Manage.HelperClasses;
using WebContent.Manage.Interfaces;

namespace WebContent.Manage.Repository
{
    // Access to the repository via LINQ to objects, backed by a file.
    // See IContentRepository for descriptions of the methods in this class.
    public class ContentRepositoryLinqToFile : IContentRepository
    {
        // Web content in a list.
        // The list is equivalent to the Node table in the database.
        static List<ContentTransfer> contentList = null;

        static int nodeIdNext;

        // Path to file that contains the list of nodes.
        static string filePathFull;

        // dataAccess object:
        // All methods that access the static fields in this class lock this object.
        // This protects the data from corruption in the multi-threaded ASP.Net environment.
        static object dataAccess = new Object();


        //--------------------------------------
        // Constructor.
        //--------------------------------------
        public ContentRepositoryLinqToFile()
        {
            // Get path to content file.
            filePathFull = ConfigurationManager.AppSettings["ContentRepositoryFilePath"];

            // Copy from database to file.
            // Normally commented out; not for production use.
            //ContentListCopyFromDatabase();

            // Read content from file.
            ContentListRead();
        }



        //--------------------------------------
        //--------------------------------------
        public List<ContentTransfer> NodeChildrenGet(int id)
        {
            if (contentList == null)
                return null;

            lock (dataAccess)
            {
                return contentList.Where(n => n.ParentId == id).ToList<ContentTransfer>();
            }
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeCreate(ContentTransfer nodeNew)
        {
            if (contentList == null)
                return null;

            lock (dataAccess)
            {
                nodeNew.NodeId = nodeIdNext++;
                contentList.Add(nodeNew);
                ContentListSave();

                return nodeNew;
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
            if (contentList == null)
                return false;

            lock (dataAccess)
            {
                bool result = false;

                int count = contentList.Where(n => n.Path == path).Count();
                if (count == 1)
                    result = true;

                return result;
            }
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeGetById(int id)
        {
            if (contentList == null)
                return null;

            lock (dataAccess)
            {
                ContentTransfer nodeXfer = null;

                try
                {
                    nodeXfer = contentList.Where(n => n.NodeId == id).Single();
                }
                catch (Exception ex)
                {
                    // Exception is thrown if the node does not exist.
                    // Replace this with a call to the logger.
                    Debug.Print(ex.Message);
                }

                return nodeXfer;
            }
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeGetByPath(string path)
        {
            if (contentList == null)
                return null;

            lock (dataAccess)
            {
                ContentTransfer nodeXfer = null;

                try
                {
                    nodeXfer = contentList.Where(n => n.Path == path).Single();
                }
                catch (Exception ex)
                {
                    // Exception is thrown if the node does not exist.
                    // Replace this with a call to the logger.
                    Debug.Print(ex.Message);
                }

                return nodeXfer;
            }
        }



        //--------------------------------------
        //--------------------------------------
        public ContentTransfer NodeTypeMostRecentLeafGet(string nodeType)
        {
            lock (dataAccess)
            {
                return NodeTypeMostRecentNLeavesGet(nodeType, 1)[0];
            }
        }



        //--------------------------------------
        //--------------------------------------
        public List<ContentTransfer> NodeTypeMostRecentNLeavesGet(string nodeType, int num)
        {
            if (contentList == null)
                return null;

            lock (dataAccess)
            {
                List<ContentTransfer> leavesList =
                (
                    from n1 in contentList
                    where n1.Path.StartsWith(nodeType)
                    join n2 in contentList on n1.NodeId equals n2.ParentId into join1
                    from n3 in join1.DefaultIfEmpty()   // DefaultIfEmpty specifies outer join
                    where n3 == null // null n3 indicates n1.Id has no child (null n3 is a leaf)
                    orderby n1.NodeId descending
                    select n1
                    ).Take(num).ToList<ContentTransfer>();

                return leavesList;
            }
        }



        //--------------------------------------
        //--------------------------------------
        public void NodeUpdate(ContentTransfer nodeXfer)
        {
            if (contentList == null)
                return;     // no exception thrown: cannot get here if contentList is null.

            lock (dataAccess)
            {
                ContentTransfer nodeExisting = null;

                try
                {
                    nodeExisting = contentList.Where(n => n.NodeId == nodeXfer.NodeId).Single();
                }
                catch (Exception ex)
                {
                    // Exception is thrown if the node does not exist.
                    // Replace this with a call to the logger.
                    Debug.Print(ex.Message);
                }

                if (nodeExisting != null)
                {
                    nodeExisting.Title = nodeXfer.Title;
                    nodeExisting.Summary = nodeXfer.Summary;
                    nodeExisting.Content = nodeXfer.Content;
                    ContentListSave();
                }
            }
        }


        //--------------------------------------
        //--------------------------------------
        // Static methods
        //--------------------------------------
        //--------------------------------------

        //--------------------------------------
        // ContentListRead:
        // Read nodes from the repository file into the contentList.
        // Called from the constructor.
        //--------------------------------------
        private static void ContentListRead()
        {
            if (filePathFull == null)
                return;

            // Create file if it does not exist.
            if (!File.Exists(filePathFull))
            {
                using (FileStream streamCreate = File.Create(filePathFull))
                {
                    streamCreate.Close();
                }
            }

            // Deserialize.
            XmlSerializer serializer = new XmlSerializer(typeof(List<ContentTransfer>));
            using (FileStream stream = new FileStream(filePathFull, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    contentList = (List<ContentTransfer>)(serializer.Deserialize(stream));
                }
                catch (Exception ex)
                {
                    // Replace this with a call to the logger.
                    Debug.Print(ex.Message);
                    contentList = null;
                }
            }

            // If list was not created by file read, create an empty list.
            if (contentList == null)
            {
                contentList = new List<ContentTransfer>();
                nodeIdNext = 1;
            }
            else
            {
                // List has been created.
                if (contentList.Count == 0)
                    nodeIdNext = 1;
                else
                    nodeIdNext = contentList.Max(n => n.NodeId) + 1;
            }
        }



        //--------------------------------------
        //--------------------------------------
        private static void ContentListSave()
        {
            if (filePathFull == null)
                return;

            if (contentList == null)
                return;

            // Serialize.
            XmlSerializer serializer = new XmlSerializer(typeof(List<ContentTransfer>));
            using (FileStream stream = new FileStream(filePathFull, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                try
                {
                    serializer.Serialize(stream, contentList);
                }
                catch (Exception ex)
                {
                    // Replace this with a call to the logger.
                    Debug.Print(ex.Message);
                    throw;
                }
            }
        }




        //--------------------------------------
        // Remaining code is for copying from SQL Server database to the content file.
        // Not production code.
        //--------------------------------------

        // Constants copied from ContentRepositorySql for the manual operation of initializing the content list.
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
        private static void ContentListCopyFromDatabase()
        {
            string connString = ConfigurationManager.ConnectionStrings["WebContentContext"].ConnectionString;

            contentList = new List<ContentTransfer>();

            using (SqlConnection conn = new SqlConnection())
            {
                conn.ConnectionString = connString;
                conn.Open();
                using (SqlCommand cmd = new SqlCommand())
                {
                    // Select all nodes.
                    string sql = "SELECT " + COL_NAMES + " FROM Node";
                    cmd.Connection = conn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = sql;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contentList.Add(NodeRecordUnpack(reader));
                        }
                    }
                }
                conn.Close();
            }

            ContentListSave();
            contentList = null;
        }



        //--------------------------------------
        // Also copied from ContentRepositorySql
        //--------------------------------------
        private static ContentTransfer NodeRecordUnpack(SqlDataReader reader)
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
    }
}

