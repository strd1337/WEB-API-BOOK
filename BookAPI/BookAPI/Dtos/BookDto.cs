namespace BookAPI.Dtos
{
    public class BookDto
    {
        public int Id { get; set; }
        public string? Isbn { get; set; }
        public string? Title { get; set; }
        public DateTime? DatePublished { get; set; }
    }
}
