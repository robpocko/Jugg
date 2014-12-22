using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Atlas;
using Autofac;
using Autofac.Integration.Wcf;
using NLog;

namespace MyWcfService
{
    public class WCFServiceHost : IAmAHostedProcess
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public class WCFService
        {
            private static readonly Logger logger = LogManager.GetCurrentClassLogger();

            public Type WCFServiceType { get; private set; }
            public string WCFServiceDesc { get; private set; }
            public IContainer Container { get; private set; }
            private ServiceHost Host { get; set; }

            public WCFService(Type wcfServiceType, string wcfServiceDesc, IContainer container)
            {
                WCFServiceType = wcfServiceType;
                WCFServiceDesc = wcfServiceDesc;
                Container = container;
            }

            public void StartServiceHost()
            {
                Host = new ServiceHost(WCFServiceType);
                Host.AddDependencyInjectionBehavior(WCFServiceType, Container);
                logger.Info("{0} - created", WCFServiceDesc);
                logger.Info(() => WCFServiceDesc + " - endpoint: \n\r" + Host.Description.Endpoints.Select(e => e.ListenUri.ToString()).Aggregate((e, ne) => e + "\n\r" + ne));

                Host.Open();

                logger.Info("{0} - started", WCFServiceDesc);
            }

            public void StopServiceHost()
            {
                logger.Info("{0} - Stopping", WCFServiceDesc);

                Host.Close();
                Host = null;

                logger.Info("{0} - Stopped", WCFServiceDesc);
            }
        }

        private static List<WCFService> Services = new List<WCFService>();

        public static void AddWCFService(Type wcfServiceType, string wcfServiceDesc, IContainer container)
        {
            Services.Add(new WCFService(wcfServiceType, wcfServiceDesc, container));
        }

        public void Pause()
        {
            Stop();
        }

        public void Resume()
        {
            Start();
        }

        public void Start()
        {
            try
            {
                foreach (WCFService service in Services)
                {
                    service.StartServiceHost();
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("Error starting Service", ex);
            }
        }

        public void Stop()
        {
            try
            {
                foreach (WCFService service in Services)
                {
                    service.StopServiceHost();
                }
            }
            catch (Exception ex)
            {
                logger.Fatal("Error stopping Service", ex);
            }
        }
    }
}
