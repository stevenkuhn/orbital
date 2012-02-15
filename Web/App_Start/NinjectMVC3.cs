[assembly: WebActivator.PreApplicationStartMethod(typeof(Web.App_Start.NinjectMVC3), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(Web.App_Start.NinjectMVC3), "Stop")]
[assembly: WebActivator.PostApplicationStartMethod(typeof(Web.App_Start.NinjectMVC3), "PostStart")]

namespace Web.App_Start
{
    using System.Reflection;
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using Ninject;
    using Ninject.Web.Mvc;
    using System.Configuration;
    using Dapper;
    using Raven.Client;
    using Raven.Client.Embedded;
    using Raven.Client.Indexes;

    public static class NinjectMVC3 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestModule));
            DynamicModuleUtility.RegisterModule(typeof(HttpApplicationInitializationModule));
            bootstrapper.Initialize(CreateKernel);
        }

        public static void PostStart()
        {
            var store = bootstrapper.Kernel.Get<IDocumentStore>();
            store.Initialize();

            IndexCreation.CreateIndexes(Assembly.GetCallingAssembly(), store);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            var connectionString = ConfigurationManager.ConnectionStrings["PerfMonDatabase"].ConnectionString;

            kernel.Bind<IDbContext>().To<SqlContext>()
                .InRequestScope()
                .WithConstructorArgument("connectionString", connectionString);

            kernel.Bind<IDocumentStore>().To<EmbeddableDocumentStore>()
                .InSingletonScope()
                .WithPropertyValue("ConnectionStringName", "RavenDB");

            kernel.Bind<IDocumentSession>()
                .ToMethod(ctx => { return kernel.Get<IDocumentStore>().OpenSession(); })
                .InRequestScope()
                .OnDeactivation((ctx, instance) =>
                {
                    if (instance != null)
                        instance.SaveChanges();
                });
        }        
    }
}
