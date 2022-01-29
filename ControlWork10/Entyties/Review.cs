namespace ControlWork10.Entyties
{
    public class Review
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public double Rating { get; set; }
        public string TextReview { get; set; }
        public int PlaceId { get; set; }
    }
}
