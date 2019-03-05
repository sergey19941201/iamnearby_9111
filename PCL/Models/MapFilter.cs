namespace PCL.Models
{
    public class CenterForMapFilter
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    public class MapFilter
    {
        public CenterForMapFilter center { get; set; }
        public int distanceRadius { get; set; }
        public bool hasReviews { get; set; }
    }
}