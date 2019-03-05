namespace PCL.Models
{
    public class ReviewsList
    {
        public string id { get; set; }
        public string name { get; set; }
        public string avatarUrl { get; set; }
        public string rating { get; set; }
        public string online { get; set; }
        public string reviewDate { get; set; }
        public string reviewText { get; set; }
        public string userId { get; set; }
        public bool canAnswer { get; set; }
        public string reviewAnswer { get; set; }
    }
}