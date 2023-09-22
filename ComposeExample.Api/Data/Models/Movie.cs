namespace ComposeExample.Api.Data.Models
{
    public class Movie
    {
        public Movie(string title)
        {
            Title = title;
        }   

        public int Id { get; set; }
        public DateOnly ReleaseDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public string Title { get; set; }
        public string? Description { get; set; }
    }
}
