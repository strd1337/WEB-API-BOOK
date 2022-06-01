namespace BookAPI.Dtos
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public string? Headline { get; set; }
        public string? ReviewText { get; set; }
        public int Rating { get; set; }
    }
}
