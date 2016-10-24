using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using TcbInternetSolutions.Vulcan.Core;

namespace Vulcan.Examples.Customizations
{
    /// <summary>
    /// Register custom settings with Structuremap
    /// </summary>
    [ModuleDependency(typeof(ServiceContainerInitialization))]
    public class RegisterCustomizations : IConfigurableModule
    {

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Container.Configure(x =>
            {
                x.For<IVulcanClientConnectionSettings>().Singleton().Use<CustomClientConnectionSettings>();
            });
        }

        public void Initialize(InitializationEngine context)
        {

        }

        public void Uninitialize(InitializationEngine context)
        {

        }
    }
}
