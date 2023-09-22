using Refit;

namespace ComposeExample.MoviesClient
{
    public interface IMovieClient
    {
        [Get("/api/movies")]
        Task<ApiResponse<IEnumerable<Movie>>> GetAll();

        [Get("/api/movies/{id}")]
        Task<ApiResponse<Movie>> Get(int id);

        [Put("/api/movies/{movie.Id}")]
        Task<IApiResponse> Update([Body] Movie movie);

        [Post("/api/movies")]
        Task<ApiResponse<Movie>> Insert([Body] Movie movie);

        [Delete("/api/movies/{id}")]
        Task<IApiResponse> Delete(int id);
    }
}
