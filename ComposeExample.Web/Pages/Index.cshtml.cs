using ComposeExample.MoviesClient;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComposeExample.Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMovieClient _client;

        public IndexModel(IMovieClient client, ILogger<IndexModel> logger)
        {
            _logger = logger;
            _client = client;
        }

        public async Task OnGet()
        {
            var moviesResponse = await _client.GetAll();

            if (moviesResponse.IsSuccessStatusCode)
                Movies = moviesResponse.Content;
        }

        public IEnumerable<Movie> Movies { get; set; } = Array.Empty<Movie>();
    }
}