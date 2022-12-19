namespace MovieAPI.Models
{
    public class MovieUpdate
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string? Genre { get; set; }
        public decimal Price { get; set; }
        public string? Rating { get; set; }
    }
}
