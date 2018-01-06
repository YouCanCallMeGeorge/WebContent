using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebContent.Manage.ContentClasses;
using WebContent.Manage.HelperClasses;

namespace WebContent.Manage.Interfaces
{
    public interface IContentManager
    {
        //-------------------------------------
        //  NOTE:
        //  Every method that retrieves an existing object (or list of objects)
        //  will return null if the requested object (or list) was not found.
        //  The caller must trap the null return and take appropriate action.
        //
        //  (The above, by definition, does not include methods that create new records in the repository.)
        //
        //  Any other failure to complete a method will result in an exception.
        //  At first release, the base Exception object is thrown.
        //  Custom exception subclasses can be defined in later releases, as desired.
        //--------------------------------------


        //--------------------------------------
        //--------------------------------------
        BlogEntry BlogEntryCreate(string title, string summary, string content);


        //--------------------------------------
        //--------------------------------------
        BlogEntry BlogEntryMostRecentGet();


        //--------------------------------------
        //--------------------------------------
        List<ContentLinkInfo> ContentChildLinksGet(ContentNode node);


        //--------------------------------------
        //--------------------------------------
        ContentNode ContentGetById(int id);


        //--------------------------------------
        //--------------------------------------
        ContentNode ContentGetByPath(string path);


        //--------------------------------------
        //--------------------------------------
        List<ContentLinkInfo> ContentPathLinksGet(ContentNode node);


        //--------------------------------------
        //--------------------------------------
        List<ContentLinkInfo> ContentRecentNLinksGet(string type, int num);


        //--------------------------------------
        //--------------------------------------
        void ContentUpdate(int id, string title, string summary, string content);
    }
}
