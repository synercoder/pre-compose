using ComposeExample.Extensions;
using System.Text.Json.Serialization;

namespace ComposeExample.MoviesClient
{
    public class Movie
    {
        public Movie(string title)
        {
            Title = title;
        }

        public Movie()
        {
            Title = "";
        }

        public int Id { get; set; }

        [property: JsonConverter(typeof(DateOnlyConverter))]
        public DateOnly ReleaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
