using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WebContent.Manage.HelperClasses;

namespace WebContent.Manage.ContentClasses
{
    public class BlogEntry : ContentNode
    {

        internal BlogEntry(ContentTransfer cn) : base("Blog", cn)
        {

        }


        //--------------------------------------
        // PathMake:
        //   Returns today's date, segmented in path format.
        //   ContentManager calls this prior to creating a new BlogEntry in the repository.
        //--------------------------------------
        public static string PathMake()
        {
            return "Blog/" + DateTime.Now.ToString("yyyy/MMMM/dd");
        }
    }
}
