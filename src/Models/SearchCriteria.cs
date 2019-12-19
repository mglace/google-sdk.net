namespace GooglePlaces
{
    public class SearchCriteria
    {
        public string Name { get; set; }

        public decimal Latitude { get; set; }

        public decimal Longitude { get; set; }

        public SearchCriteria(decimal latitude, decimal longitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }
    }
}