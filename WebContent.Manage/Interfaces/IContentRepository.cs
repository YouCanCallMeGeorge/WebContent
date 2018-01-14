using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebContent.Manage.HelperClasses;

namespace WebContent.Manage.Interfaces
{
    public interface IContentRepository
    {
        //--------------------------------------
        //--------------------------------------
        List<ContentTransfer> NodeChildrenGet(int id);


        //--------------------------------
        // NodeCreate:
        //  nodeNew: Prior to calling this method, populate Content, DateCreated, Path, Summary, Title.
        //  Classes implementing this interface must set NodeId and ParentId.
        //  NodeId must be unique in the data store.
        //--------------------------------
        ContentTransfer NodeCreate(ContentTransfer nodeNew);


        //--------------------------------------
        //--------------------------------------
        bool NodeExistsTest(string path);


        //--------------------------------------
        //--------------------------------------
        //void NodeDelete(int id);


        //--------------------------------------
        //--------------------------------------
        ContentTransfer NodeGetById(int id);


        //--------------------------------------
        //--------------------------------------
        ContentTransfer NodeGetByPath(string path);


        //--------------------------------------
        // NodeTypeMostRecentLeafGet:
        //   Returns the most recent leaf node (no children) of the specified type.
        //--------------------------------------
        ContentTransfer NodeTypeMostRecentLeafGet(string nodeType);


        //--------------------------------------
        // NodeTypeMostRecentNLeavesGet:
        //   Returns the N most recent leaf nodes (no children) of the specified type.
        //--------------------------------------
        List<ContentTransfer> NodeTypeMostRecentNLeavesGet(string nodeType, int num);


        //--------------------------------------
        //--------------------------------------
        void NodeUpdate(ContentTransfer node);
    }
}
