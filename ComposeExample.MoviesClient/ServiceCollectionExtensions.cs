using ComposeExample.MoviesClient;
using Refit;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IHttpClientBuilder AddMoviesApiClient(this IServiceCollection services, string url)
        {
            return services.AddRefitClient<IMovieClient>()
                .ConfigureHttpClient(c => c.BaseAddress = new Uri(url));
        }
    }
}
