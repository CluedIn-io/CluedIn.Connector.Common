using Castle.MicroKernel.Registration;
using Castle.Windsor;
using System.Reflection;

namespace Connector.Common
{
    public class CommonStaticServiceHolder
    {
        public static void InstallBaseComponents<TComponentHost>(IWindsorContainer container, Assembly assembly)
            where TComponentHost : IWindsorInstaller, new()
        {
            container.Install(new TComponentHost());
            container.Register(Types.FromAssembly(assembly).BasedOn<IConfigurationConstants>()
                .WithServiceFromInterface().If(t => !t.IsAbstract).LifestyleSingleton());
            container.Register(Component.For<ICommonServiceHolder>().ImplementedBy<CommonServiceHolder>()
                .OnlyNewServices());
        }
    }
}
