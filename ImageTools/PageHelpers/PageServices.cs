using Microsoft.Extensions.DependencyInjection;

namespace ImageTools.PageHelpers
{
    internal static class PageServices
    {
        public static IServiceCollection AddPageServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IMatHelpers, MatHelpers>();

            return serviceCollection;
        }
    }
}
