using MyBlog.Models;

namespace ControlWork10.Entyties
{
    public class Place
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public int ReviewsCount { get; set; }
        public string Name { get; set; }
        public double AverageRating { get; set; }
        public string MainPhoto { get; set; }
        public int PhotoGalleryId { get; set; }
        public string Description { get; set; }
        public int PhotoCount { get; set; }
    }
}
