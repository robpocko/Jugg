using System;
using System.Reflection;
using System.ServiceModel;
using Atlas;
using Atlas.Configuration;
using Autofac;
using Autofac.Integration.Wcf;
using NLog;

namespace MyWcfService
{
    public static class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Logger.Info("Starting TimiService");

            GlobalDiagnosticsContext.Set("applicationVersion", Assembly.GetExecutingAssembly().GetName().Version.ToString());

            try
            {
                var builder = new ContainerBuilder();
                var container = builder.Build();

                // uncomment this line to see if any of the dependency injection has failed
                //var test = container.Resolve<TIMIService>();

                //WCFServiceHost.AddWCFService(typeof(TIMIService), "DnB TIMI Service", container);

                Configuration<WCFServiceHost> configuration;
                if (args != null && args.Length > 0)
                {
                    configuration = Host.UseAppConfig<WCFServiceHost>().WithArguments(args);
                }
                else
                {
                    configuration = Host.UseAppConfig<WCFServiceHost>();
                }

                Host.Start(configuration);
            }
            catch (Exception ex)
            {
                Logger.Fatal("Exception during startup.", ex);
                Console.ReadLine();
            }

            Logger.Info("TimiService successfully started.");
        }
    }
}
