using Castle.MicroKernel.Registration;
using CluedIn.Core;
using CluedIn.Core.Server;
using ComponentHost;
using Microsoft.Extensions.Logging;

namespace Connector.Common
{
    public abstract class ComponentBase<DIComponentsPackage> : ServiceApplicationComponent<IServer>
        where DIComponentsPackage : IWindsorInstaller, new()
    {
        public ComponentBase(ComponentInfo componentInfo) : base(componentInfo)
        {
        }

        /// <summary>Starts this instance.</summary>
        public override void Start()
        {
            Container.Install(new DIComponentsPackage());
            Log.LogInformation($"{ComponentName} Registered");
            State = ServiceState.Started;
        }

        /// <summary>Stops this instance.</summary>
        public override void Stop()
        {
            if (State == ServiceState.Stopped)
                return;

            State = ServiceState.Stopped;
        }

        protected virtual string ComponentName => GetType().Name;
    }
}
