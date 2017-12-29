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
