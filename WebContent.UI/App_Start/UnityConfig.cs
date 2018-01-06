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
            container.RegisterType<IContentManager, ContentManager>();

            // There are three repository classes available.
            // All three are in the WebContent.Manage.Repository namespace.
            // (In production, each would likely be in its own assembly.)
            //container.RegisterType<IContentRepository, ContentRepositorySql>();
            container.RegisterType<IContentRepository, ContentRepositoryLinqToEF>();
            //container.RegisterType<IContentRepository, ContentRepositoryLinqToFile>();

            // Lifetime management:
            // ContentRepositoryLinqToFile:
            //      Should be singleton instance across the application.
            //      The data will be locked during access.
            //
            // ContentRepositorySql, ContentRepositoryLinqToEF:
            //      These can be created at each request, as SQL Server will lock the table.
            //      (When multiple tables are touched in a series of queries, access must be within a transaction.)
            //
            // ContentManager:
            //      Can be created at each request, because both storage engines manage access to the data.
        }
    }
}