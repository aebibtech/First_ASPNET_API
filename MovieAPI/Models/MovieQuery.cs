namespace MovieAPI.Models
{
    public class MovieQuery
    {
        public int Id { get; set; }
        public string? Genre { get; set; }
        public string? Title { get; set; }

        public int PageSize { get; set; }
    }
}
