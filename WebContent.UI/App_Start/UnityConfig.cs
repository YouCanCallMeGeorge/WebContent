using System;

using Unity;

using WebContent.Manage.ContentManager;
using WebContent.Manage.Interfaces;
using WebContent.Manage.Repository;

namespace WebContent.UI
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public static class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container =
          new Lazy<IUnityContainer>(() =>
          {
              var container = new UnityContainer();
              RegisterTypes(container);
              return container;
          });

        /// <summary>
        /// Configured Unity Container.
        /// </summary>
        public static IUnityContainer Container => container.Value;
        #endregion

        /// <summary>
        /// Registers the type mappings with the Unity container.
        /// </summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>
        /// There is no need to register concrete types such as controllers or
        /// API controllers (unless you want to change the defaults), as Unity
        /// allows resolving a concrete type even if it was not previously
        /// registered.
        /// </remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below.
            // Make sure to add a Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // Interface to Class mappings.

            // There is only one class that implements IContentManager.
            // Use of the interface allows for easy substitution of a different implementation in future.
            container.RegisterType<IContentManager, ContentManager>();


            // There are three repository classes available.
            // All three are in the WebContent.Manage.Repository namespace.
            // (In production, each would likely be in its own assembly.)
            //      ContentRepositorySql            Access SQL Server database via SqlConnection object and SQL statements.
            //      ContentRepositoryLinqToEF       Access SQL Server database via LINQ query and Entity Framework.
            //      ContentRepositoryLinqToFile     A generic list holds the data in memory, backed by a file on disc, via XmlSerializer.
            //
            // At first release, there is one table in the SQL Server database.
            // That table is automatically locked by SQL Server while a command is executing.
            // The memory-resident list and its file backup are protected from corruption by a lock on a static object.
            // So there is no risk of corrupting the data in the multi-threaded ASP.Net environment,
            // regardless of which repository implementation is used, and regardless of whether there are many instances
            // or a singleton instance of the repository class.

            // ***Singleton instance.***
            // Uncomment the line for the selected repository class.
            // Comment out all lines for multiple instances, below.
            //IContentRepository cr = new ContentRepositorySql();
            //IContentRepository cr = new ContentRepositoryLinqToEF();
            IContentRepository cr = new ContentRepositoryLinqToFile();
            container.RegisterInstance(cr);

            // ***Multiple instances.***
            // Uncomment the line for the selected repository class.
            // Comment out all lines for singleton instance, above.
            //container.RegisterType<IContentRepository, ContentRepositorySql>();
            //container.RegisterType<IContentRepository, ContentRepositoryLinqToEF>();
            //container.RegisterType<IContentRepository, ContentRepositoryLinqToFile>();
        }
    }
}