namespace PCL.Models
{
    public class Region
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class Country
    {
        public string id { get; set; }
        public string name { get; set; }
    }

    public class MyLocation
    {
        public City city { get; set; }
        public Region region { get; set; }
        public Country country { get; set; }
    }
}