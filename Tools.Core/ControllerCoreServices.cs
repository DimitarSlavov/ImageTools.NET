using Microsoft.Extensions.DependencyInjection;

namespace Tools.Core
{
    public static class ControllerCoreServices
    {
        public static IServiceCollection AddControllerCoreServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDrawingsHelper, DrawingsHelper>();

            return serviceCollection;
        }
    }
}
